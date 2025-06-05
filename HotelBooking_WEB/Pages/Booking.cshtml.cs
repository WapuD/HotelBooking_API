using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using System.Globalization;

namespace HotelBooking_WEB.Pages
{
    public class BookingModel : PageModel
    {
        private readonly ILogger<BookingModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public RoomDto Room { get; set; }  // Используем RoomDto

        [BindProperty]
        public DateTime CheckInDate { get; set; }

        [BindProperty]
        public DateTime CheckOutDate { get; set; }

        [BindProperty]
        public int TotalPrice { get; set; }

        [BindProperty]
        public int RoomId { get; set; }

        [BindProperty]
        public int UserId { get; set; }

        public List<string> BookedDates { get; set; } = new List<string>();

        public BookingModel(ILogger<BookingModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet(int roomId)
        {
            CheckInDate = DateTime.Parse(HttpContext.Session.GetString("CheckInDate"));
            CheckOutDate = DateTime.Parse(HttpContext.Session.GetString("CheckOutDate"));

            if (roomId == 0)
                roomId = Convert.ToInt32(HttpContext.Session.GetString("RoomId"));
            HttpContext.Session.SetString("RoomId", roomId.ToString());

            Room = await _apiClient.GetRoomById(roomId);
            RoomId = roomId;

            var bookings = await _apiClient.GetBookingsByRoomId(roomId);

            BookedDates = bookings.SelectMany(b => Enumerable.Range(0, (b.CheckOutDate - b.CheckInDate).Days)
                                                             .Select(i => b.CheckInDate.AddDays(i).ToString("yyyy-MM-dd"))).ToList();

            int nights = (CheckOutDate - CheckInDate).Days;
            decimal pricePerNight = Room?.PricePerNight ?? 0;
            TotalPrice = (int)(pricePerNight * nights);
        }

        public async Task<IActionResult> OnPost()
        {
            UserId = int.Parse(HttpContext.Session.GetString("UserId"));

            if (RoomId <= 0 || UserId <= 0 || CheckInDate == default || CheckOutDate == default || TotalPrice <= 0)
            {
                ModelState.AddModelError(string.Empty, "Некорректные данные для бронирования.");
                return RedirectToPage("/Rooms");
            }

            // Приводим даты к UTC с обнулением времени (полночь)
            var utcCheckIn = new DateTimeOffset(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, 0, 0, 0, TimeSpan.Zero);
            var utcCheckOut = new DateTimeOffset(CheckOutDate.Year, CheckOutDate.Month, CheckOutDate.Day, 0, 0, 0, TimeSpan.Zero);
            var checkInStr = utcCheckIn.ToString("yyyy-MM-dd");
            var checkOutStr = utcCheckOut.ToString("yyyy-MM-dd");

            // Проверяем доступность комнат через API
            int availableRoomCount;
            try
            {
                availableRoomCount = await _apiClient.GetAvailableRoomCount(RoomId, checkInStr, checkOutStr);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ошибка при проверке доступности: {ex.Message}");

                await OnGet(RoomId);
                return Page();
            }

            if (availableRoomCount <= 0)
            {
                // Нет свободных комнат — сбрасываем даты и показываем ошибку
                ModelState.AddModelError(string.Empty, "На выбранные даты нет доступных комнат. Пожалуйста, выберите другие даты.");

                // Сброс полей даты (если у вас есть свойства для привязки)
                CheckInDate = default;
                CheckOutDate = default;

                HttpContext.Session.SetString("RoomId", RoomId.ToString());

                await OnGet(RoomId);
                return Page();
            }

            var bookingDTO = new BookingDTO
            {
                UserId = UserId,
                RoomId = RoomId,
                CheckInDate = utcCheckIn,
                CheckOutDate = utcCheckOut,
                TotalPrice = TotalPrice,
                Status = "Ожидание"
            };

            try
            {
                await _apiClient.CreateBooking(bookingDTO);
                return RedirectToPage("/Bookings");
            }
            catch (ValidationApiException ex)
            {
                _logger.LogError($"Ошибка API: {ex.Content}");
                ModelState.AddModelError(string.Empty, $"Ошибка сервера: {ex.Content}");
                return Page();
            }
        }
    }
}
