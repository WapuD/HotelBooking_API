using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking_WEB.Pages
{
    public class RoomsModel : PageModel
    {
        private readonly ILogger<RoomsModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public IEnumerable<Room> Rooms { get; set; }

        public RoomsModel(ILogger<RoomsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet(int hotelId)
        {
            Rooms = await _apiClient.GetRoomByHotelId(hotelId);
        }
    }
}
