using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using HotelBooking_WEB.Data.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace HotelBooking_WEB.Pages
{
    public class BookingsModel : PageModel
    {
        private readonly ILogger<BookingsModel> _logger;
        private readonly IApiClient _apiClient;
        private readonly IEmailService _emailService;

        public IEnumerable<Booking> Bookings { get; set; }

        public BookingsModel(ILogger<BookingsModel> logger, IApiClient apiClient, IEmailService emailService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _emailService = emailService;
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

        public async Task<IActionResult> OnPostCancelBooking(int bookingId)
        {
            await _apiClient.UpdateBookingStatus(bookingId, "Отменено");

            var booking = await _apiClient.GetBookingById(bookingId);

            await _emailService.SendEmailAsync(
                booking.User.Email,
                "Изменение статуса бронирования в HotelBooking",
                $"Уважаемый(ая) {booking.User.SecondName + " " + booking.User.FirstName},<br/><br/>" +
                $"Ваше бронирование №{booking.Id} было отменено.<br/>" +
                $"Дата заезда: {booking.CheckInDate:dd MMMM yyyy}<br/>" +
                $"Дата выезда: {booking.CheckOutDate:dd MMMM yyyy}<br/>" +
                $"Общая стоимость: {booking.TotalPrice:C}<br/><br/>" +
                "Спасибо, что выбрали HotelBooking.<br/><br/>" +
                "С уважением,<br/>Команда HotelBooking");

            return RedirectToPage("/Bookings");
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int hotelId, int bookingId, int rating, string text)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdString))
                {
                    return RedirectToPage("/Login");
                }

                var userId = int.Parse(userIdString);

                var comment = new Comment
                {
                    HotelId = hotelId,
                    UserId = userId,
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
                await OnGet();
                return Page();
            }
        }
    }
}
