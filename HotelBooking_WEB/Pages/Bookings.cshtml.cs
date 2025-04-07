using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking_WEB.Pages
{
    public class BookingsModel : PageModel
    {
        private readonly ILogger<BookingsModel> _logger;
        private readonly IApiClient _apiClient;

        public IEnumerable<Booking> Bookings { get; set; }

        public BookingsModel(ILogger<BookingsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            // Получаем ID текущего пользователя
            var userId = 1;// Получите ID текущего пользователя из сессии или другого источника
            Bookings = await _apiClient.GetUserBookings(userId);
        }
    }
}
