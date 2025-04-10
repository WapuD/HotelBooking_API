using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBooking_WEB.Pages
{
    public class RoomsModel : PageModel
    {
        private readonly ILogger<RoomsModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public IEnumerable<Room> Rooms { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckInDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckOutDate { get; set; }

        public RoomsModel(ILogger<RoomsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet(DateTime? checkInDate = null, DateTime? checkOutDate = null)
        {
            var hotelId = int.Parse(HttpContext.Session.GetString("HotelId"));

            CheckInDate = checkInDate ?? DateTime.Today;
            CheckOutDate = checkOutDate;

            var allRooms = await _apiClient.GetRoomByHotelId(hotelId);

            if (CheckInDate.HasValue && CheckOutDate.HasValue)
            {
                allRooms = await FilterAvailableRooms(allRooms, CheckInDate.Value, CheckOutDate.Value);
            }

            Rooms = allRooms.ToList();
        }

        private async Task<IEnumerable<Room>> FilterAvailableRooms(IEnumerable<Room> rooms, DateTime checkInDate, DateTime checkOutDate)
        {
            var availableRooms = new List<Room>();

            foreach (var room in rooms)
            {
                var bookings = await _apiClient.GetBookingsByRoomId(room.Id);
                bool isAvailable = true;

                foreach (var booking in bookings)
                {
                    if ((checkInDate < booking.CheckOutDate) && (checkOutDate > booking.CheckInDate))
                    {
                        isAvailable = false;
                        break;
                    }
                }

                if (isAvailable)
                {
                    availableRooms.Add(room);
                }
            }

            return availableRooms;
        }
    }
}
