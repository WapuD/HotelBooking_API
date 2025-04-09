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
        public IEnumerable<Booking> BookingsRedact { get; set; }
        public IEnumerable<Booking> BookingsList { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public double AveragePrice { get; set; }
        public int MaxPrice { get; set; }
        public int MinPrice { get; set; }
        public Dictionary<string, int> StatusDistribution { get; set; }
        public double AverageStayDuration { get; set; }
        public int CancelledBookingsCount { get; set; }
        public double OccupancyRate { get; set; }

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

                // Фильтрация бронирований
                BookingsRedact = Bookings.Where(b => b.Status == "Ожидание")
                                         .OrderBy(b => b.CheckInDate)
                                         .ToList();
                BookingsList = Bookings.Where(b => b.Status == "Подтверждено" || b.Status == "Отменено")
                                       .OrderBy(b => b.CheckInDate)
                                       .ToList();

                // Вычисление статистических данных
                TotalBookings = Bookings.Count();
                ActiveBookings = BookingsRedact.Count();
                AveragePrice = Bookings.Average(b => b.TotalPrice);
                MaxPrice = Bookings.Max(b => b.TotalPrice);
                MinPrice = Bookings.Min(b => b.TotalPrice);

                StatusDistribution = Bookings.GroupBy(b => b.Status)
                                            .ToDictionary(g => g.Key, g => g.Count());

                AverageStayDuration = Bookings.Average(b => (b.CheckOutDate - b.CheckInDate).TotalDays);

                CancelledBookingsCount = Bookings.Count(b => b.Status == "Отменено");

                // Пример вычисления процента загрузки отеля (может потребовать дополнительной информации)
                OccupancyRate = (double)ActiveBookings / TotalBookings * 100;
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
