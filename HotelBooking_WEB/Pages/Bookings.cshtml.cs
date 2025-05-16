using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace HotelBooking_WEB.Pages
{
    public class BookingsModel : PageModel
    {
        private readonly ILogger<BookingsModel> _logger;
        private readonly IApiClient _apiClient;

        public IEnumerable<Booking> Bookings { get; set; }
        public Dictionary<int, Comment> UserCommentsByHotel { get; set; } = new();


        public BookingsModel(ILogger<BookingsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    Bookings = null;
                    return;
                }

                var userId = int.Parse(userIdString);
                var bookings = await _apiClient.GetUserBookings(userId);
                Bookings = bookings.OrderBy(b => b.CheckInDate)
                                   .ToList();
            }
            catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Bookings = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw;
            }
        }
        public async Task<IActionResult> OnPost(int bookingId)
        {
            await _apiClient.UpdateBookingStatus(bookingId, "Отменено");
            return RedirectToPage("/Bookings");
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int hotelId, int bookingId, int rating, string text)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                if (userId == null)
                {
                    return RedirectToPage("/Login");
                }

                var comment = new Comment
                {
                    HotelId = hotelId,
                    Hotel = null,
                    UserId = userId,
                    User = null,
                    Rating = rating,
                    Text = text,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                await _apiClient.PostComment(comment);

                return RedirectToPage("/Bookings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении отзыва");
                ModelState.AddModelError(string.Empty, "Ошибка при добавлении отзыва");
                // Перезагрузить страницу с ошибкой
                await OnGet();
                return Page();
            }
        }
    }
}
