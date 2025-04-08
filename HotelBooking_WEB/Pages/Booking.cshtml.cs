using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;

namespace HotelBooking_WEB.Pages
{
    public class BookingModel : PageModel
    {
        private readonly ILogger<BookingModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public Room Room { get; set; }

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

        public BookingModel(ILogger<BookingModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet(int roomId)
        {
            Room = await _apiClient.GetRoomById(roomId);
            RoomId = roomId;
        }

        public async Task<IActionResult> OnPost()
        {
            UserId = int.Parse(HttpContext.Session.GetString("UserId"));

            if (RoomId <= 0 || UserId <= 0 || CheckInDate == default || CheckOutDate == default || TotalPrice <= 0)
            {
                ModelState.AddModelError(string.Empty, "Некорректные данные для бронирования.");
                return Page();
            }

            var bookingDTO = new BookingDTO
            {
                UserId = UserId,
                RoomId = RoomId,
                CheckInDate = new DateTimeOffset(CheckInDate).ToUniversalTime(),
                CheckOutDate = new DateTimeOffset(CheckOutDate).ToUniversalTime(),
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
                Console.WriteLine($"Ошибка API: {ex.Content}");
                ModelState.AddModelError(string.Empty, $"Ошибка сервера: {ex.Content}");
                return Page();
            }
        }
    }
}
