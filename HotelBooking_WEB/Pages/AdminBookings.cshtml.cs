using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using HotelBooking_WEB.Data.Service;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HotelBooking_WEB.Pages
{
    public class AdminBookingsModel : PageModel
    {
        private readonly ILogger<BookingsModel> _logger;
        private readonly IApiClient _apiClient;
        private readonly IEmailService _emailService;

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


        public AdminBookingsModel(ILogger<BookingsModel> logger, IApiClient apiClient, IEmailService emailService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _emailService = emailService;
        }

        public async Task OnGet()
        {
            try
            {
                var companyId = Convert.ToInt32(HttpContext.Session.GetString("CompanyId"));

                var response = await _apiClient.GetAllBookings(companyId);
                Bookings = response;

                // --- ���������� �� ������������ ---
                if (!string.IsNullOrWhiteSpace(UserFilter))
                {
                    Bookings = Bookings.Where(b =>
                        (b.User.FirstName + " " + b.User.SecondName + " " + (b.User.LastName ?? "")).Contains(UserFilter, StringComparison.OrdinalIgnoreCase)
                        || b.User.Email.Contains(UserFilter, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // --- ���������� �� ������� ---
                if (!string.IsNullOrWhiteSpace(StatusFilter))
                {
                    Bookings = Bookings.Where(b => b.Status == StatusFilter).ToList();
                }

                // --- ���������� ---
                Bookings = SortOrder switch
                {
                    "date_desc" => Bookings.OrderByDescending(b => b.CheckInDate).ToList(),
                    "price_asc" => Bookings.OrderBy(b => b.TotalPrice).ToList(),
                    "price_desc" => Bookings.OrderByDescending(b => b.TotalPrice).ToList(),
                    _ => Bookings.OrderBy(b => b.Room.Hotel.Name).ToList(),
                };

                // ���������� �� �������� � �����������
                BookingsRedact = Bookings.Where(b => b.Status == "��������" || b.Status == "�������").ToList();
                BookingsList = Bookings.Where(b => b.Status == "������������" || b.Status == "��������").ToList();

                // ����������
                TotalBookings = Bookings.Count();
                ActiveBookings = BookingsRedact.Count();
                AveragePrice = Bookings.Any() ? Bookings.Average(b => b.TotalPrice) : 0;
                MaxPrice = Bookings.Any() ? Bookings.Max(b => b.TotalPrice) : 0;
                MinPrice = Bookings.Any() ? Bookings.Min(b => b.TotalPrice) : 0;

                StatusDistribution = Bookings.GroupBy(b => b.Status)
                                            .ToDictionary(g => g.Key, g => g.Count());

                AverageStayDuration = Bookings.Any() ? Bookings.Average(b => (b.CheckOutDate - b.CheckInDate).TotalDays) : 0;

                CancelledBookingsCount = Bookings.Count(b => b.Status == "��������");

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
                Console.WriteLine($"������: {ex.Message}");
                throw;
            }
        }
        public async Task<IActionResult> OnPostUpdateStatus(int bookingId, string newStatus)
        {
            var booking = await _apiClient.GetBookingById(bookingId);

            if (booking == null)
            {
                ErrorMessage = "������������ �� �������.";
                return RedirectToPage("/AdminBookings");
            }

            TempData["SuccessMessage"] = true;
            TempData["SuccessMessage"] = "������ ������������ ������� �������.";
            await _apiClient.UpdateBookingStatus(bookingId, newStatus);


            try
            {
                await _emailService.SendEmailAsync(
                    booking.User.Email,
                    "��������� ������� ������������ � HotelBooking",
                    $"���������(��) {booking.User.SecondName + " " + booking.User.FirstName},<br/><br/>" +
                    $"���� ������������ �{booking.Id} {newStatus}.<br/>" +
                    $"���� ������: {booking.CheckInDate:dd MMMM yyyy}<br/>" +
                    $"���� ������: {booking.CheckOutDate:dd MMMM yyyy}<br/>" +
                    $"����� ���������: {booking.TotalPrice:C}<br/><br/>" +
                    "�������, ��� ������� HotelBooking.<br/><br/>" +
                    "� ���������,<br/>������� HotelBooking");
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError($"������ SMTP: {ex.Message}");
                return RedirectToPage("/AdminBookings");
            }
            catch (Exception ex)
            {
                _logger.LogError($"������ �������� ������: {ex.Message}");
                return RedirectToPage("/AdminBookings");
            }

            return RedirectToPage("/AdminBookings");
        }
    }

}
