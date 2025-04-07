using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking_WEB.Pages
{
    public class AdminBookingsModel : PageModel
    {
        private readonly ILogger<BookingsModel> _logger;
        private readonly IApiClient _apiClient;

        public IEnumerable<Booking> Bookings { get; set; }

        public AdminBookingsModel(ILogger<BookingsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            try
            {
                var response = await _apiClient.GetAllBookings();
                Bookings = response;
            }
            catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Bookings = new List<Booking>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw;
            }
        }

        public async Task<IActionResult> OnPostUpdateStatus(int bookingId, string newStatus)
        {
            await _apiClient.UpdateBookingStatus(bookingId, newStatus);
            return RedirectToPage("/AdminBookings");
        }
    }

}
