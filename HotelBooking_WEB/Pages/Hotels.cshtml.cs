using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static NuGet.Packaging.PackagingConstants;

namespace HotelBooking_WEB.Pages
{
    public class HotelsModel : PageModel
    {
        private readonly ILogger<HotelsModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public IEnumerable<Hotel> Hotels { get; set; }
        public HotelsModel(ILogger<HotelsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            var hotels = await _apiClient.GetHotelsAsync();
            foreach (var hotel in hotels) { 
                var rooms = await _apiClient.GetRoomByHotelId(hotel.Id);
                ICollection<Room> roomList = rooms.ToList();
                hotel.Rooms = roomList;
            }
            Hotels = hotels;
        }
        public async Task<IActionResult> OnPostBookHotel(int hotelId)
        {
            HttpContext.Session.SetString("HotelId", hotelId.ToString());
            return RedirectToPage("/Rooms");
        }

    }
}
