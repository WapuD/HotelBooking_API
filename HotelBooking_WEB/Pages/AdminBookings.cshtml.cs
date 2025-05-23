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

        [BindProperty(SupportsGet = true)]
        public string UserFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        public Dictionary<string, List<Booking>> BookingsGroupedByHotel =>
    BookingsRedact?.GroupBy(b => b.Room.Hotel.Name)
                   .ToDictionary(g => g.Key, g => g.ToList())
    ?? new Dictionary<string, List<Booking>>();


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

                // --- Фильтрация по пользователю ---
                if (!string.IsNullOrWhiteSpace(UserFilter))
                {
                    Bookings = Bookings.Where(b =>
                        (b.User.FirstName + " " + b.User.SecondName + " " + (b.User.LastName ?? "")).Contains(UserFilter, StringComparison.OrdinalIgnoreCase)
                        || b.User.Email.Contains(UserFilter, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // --- Фильтрация по статусу ---
                if (!string.IsNullOrWhiteSpace(StatusFilter))
                {
                    Bookings = Bookings.Where(b => b.Status == StatusFilter).ToList();
                }

                // --- Сортировка ---
                Bookings = SortOrder switch
                {
                    "date_desc" => Bookings.OrderByDescending(b => b.CheckInDate).ToList(),
                    "price_asc" => Bookings.OrderBy(b => b.TotalPrice).ToList(),
                    "price_desc" => Bookings.OrderByDescending(b => b.TotalPrice).ToList(),
                    _ => Bookings.OrderBy(b => b.CheckInDate).ToList(), // date_asc по умолчанию
                };

                // Разделение на активные и завершённые
                BookingsRedact = Bookings.Where(b => b.Status == "Ожидание").ToList();
                BookingsList = Bookings.Where(b => b.Status == "Подтверждено" || b.Status == "Отменено").ToList();

                // Статистика
                TotalBookings = Bookings.Count();
                ActiveBookings = BookingsRedact.Count();
                AveragePrice = Bookings.Any() ? Bookings.Average(b => b.TotalPrice) : 0;
                MaxPrice = Bookings.Any() ? Bookings.Max(b => b.TotalPrice) : 0;
                MinPrice = Bookings.Any() ? Bookings.Min(b => b.TotalPrice) : 0;

                StatusDistribution = Bookings.GroupBy(b => b.Status)
                                            .ToDictionary(g => g.Key, g => g.Count());

                AverageStayDuration = Bookings.Any() ? Bookings.Average(b => (b.CheckOutDate - b.CheckInDate).TotalDays) : 0;

                CancelledBookingsCount = Bookings.Count(b => b.Status == "Отменено");

                OccupancyRate = TotalBookings > 0 ? (double)ActiveBookings / TotalBookings * 100 : 0;
            }
            catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Bookings = new List<Booking>();
                BookingsRedact = new List<Booking>();
                BookingsList = new List<Booking>();
                TotalBookings = ActiveBookings = CancelledBookingsCount = 0;
                AveragePrice = MaxPrice = MinPrice = 0;
                AverageStayDuration = OccupancyRate = 0;
                StatusDistribution = new Dictionary<string, int>();
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
