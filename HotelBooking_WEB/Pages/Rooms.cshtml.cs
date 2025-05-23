using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HotelBooking_WEB.Pages
{
    public class RoomsModel : PageModel
    {
        private readonly ILogger<RoomsModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public IEnumerable<Room> Rooms { get; set; }
        [BindProperty]
        public IEnumerable<Comment> Comments { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckInDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckOutDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterCapacity { get; set; }

        [BindProperty(SupportsGet = true)]
        public string RoomNameFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DescriptionFilter { get; set; }
        public Hotel Hotel { get; set; }


        public RoomsModel(ILogger<RoomsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet(DateTime? checkInDate = null, DateTime? checkOutDate = null)
        {
            var hotelIdStr = HttpContext.Session.GetString("HotelId");
            if (string.IsNullOrEmpty(hotelIdStr))
            {
                // HotelId отсутствует в сессии, например, перенаправить на страницу выбора отеля
                RedirectToPage("/Hotels"); // Или другая страница выбора отеля
                return;
            }

            var hotelId = int.Parse(hotelIdStr);
            //HttpContext.Session.SetString("HotelId", hotelId.ToString()); //это лишнее

            CheckInDate = checkInDate ?? DateTime.Today;
            CheckOutDate = checkOutDate;

            Comments = await _apiClient.GetCommentsByHotelId(hotelId);
            var allRooms = await _apiClient.GetRoomByHotelId(hotelId);
            Hotel = await _apiClient.GetHotelById(hotelId);

            if (CheckInDate.HasValue && CheckOutDate.HasValue)
            {
                if (CheckOutDate < CheckInDate)
                {
                    ModelState.AddModelError(string.Empty, "Дата выезда не может быть раньше даты заезда.");
                    // Можно вернуть пустой список комнат или обработать ошибку по-другому
                    allRooms = Enumerable.Empty<Room>();
                    return;
                }

                allRooms = await FilterAvailableRooms(allRooms, CheckInDate.Value, CheckOutDate.Value);

                HttpContext.Session.SetString("CheckInDate", CheckInDate.Value.ToString());
                HttpContext.Session.SetString("CheckOutDate", CheckOutDate.Value.ToString());
            }


            // Фильтрация по минимальной цене
            if (MinPrice.HasValue)
            {
                allRooms = allRooms.Where(r => r.PricePerNight >= MinPrice.Value);
            }

            // Фильтрация по максимальной цене
            if (MaxPrice.HasValue)
            {
                allRooms = allRooms.Where(r => r.PricePerNight <= MaxPrice.Value);
            }

            // Фильтрация по минимальной вместимости
            if (FilterCapacity.HasValue)
            {
                allRooms = allRooms.Where(r => r.Capacity >= FilterCapacity.Value);
            }

            Rooms = allRooms.ToList();
        }


        private async Task<IEnumerable<Room>> FilterAvailableRooms(IEnumerable<Room> rooms, DateTime checkInDate, DateTime checkOutDate)
        {
            var utcCheckIn = new DateTime(checkInDate.Year, checkInDate.Month, checkInDate.Day, 5, 0, 0).ToUniversalTime();
            var utcCheckOut = new DateTime(checkOutDate.Year, checkOutDate.Month, checkOutDate.Day, 5, 0, 0).ToUniversalTime();
            var checkInStr = utcCheckIn.ToString("yyyy-MM-dd");
            var checkOutStr = utcCheckOut.ToString("yyyy-MM-dd");

            var availableRooms = new List<Room>();
            foreach (var room in rooms)
            {
                var roomCount = await _apiClient.GetAvailableRoomCount(room.Id, checkInStr, checkOutStr);
                if (roomCount != 0)
                    availableRooms.Add(room);
            }

            return availableRooms;
        }
    }
}
