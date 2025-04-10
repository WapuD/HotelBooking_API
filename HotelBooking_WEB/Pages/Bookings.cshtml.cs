﻿using HotelBooking_API.Data.Models;
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

    }
}
