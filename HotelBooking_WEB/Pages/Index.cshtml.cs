using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;

namespace HotelBooking_WEB.Pages
{
    public class IngexModel : PageModel
    {
        private readonly ILogger<IngexModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public string email { get; set; }
        [BindProperty]
        public string password { get; set; }
        public string ErrorMessage { get; set; }

        public IngexModel(ILogger<IngexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<IActionResult> OnPost()
        {
            if (email != null && password != null)
            {
                if (email == "HotelBooking@mail.ru" && password == "qweqwe")
                    return RedirectToPage("/AdminBookings");

                try
                {
                    var verificationUser = await _apiClient.GetVerification(email, password);
                    if (verificationUser != null)
                    {
                        HttpContext.Session.SetString("UserId", verificationUser.Id.ToString());
                        return RedirectToPage("/Hotels");
                    }
                    else
                    {
                        ErrorMessage = "Пользователь с такими данными не найден.";
                    }
                }
                catch (ApiException ex)
                {
                    ErrorMessage = "Неверные данные пользователя.";
                }
            }

            return Page();
        }
    }
}
