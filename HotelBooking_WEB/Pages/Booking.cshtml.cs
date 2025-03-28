using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public BookingModel(ILogger<BookingModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet(int roomId)
        {
            Room = await _apiClient.GetRoomById(roomId);
        }
    }
}
