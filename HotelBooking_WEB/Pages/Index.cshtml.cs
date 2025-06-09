using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using HotelBooking_WEB.Data.Service;
using MailKit.Net.Smtp;
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
                try
                {
                    var verificationUser = await _apiClient.GetVerification(email, password);

                    if (verificationUser != null)
                    {
                        if (verificationUser.CompanyId != null)
                        {
                            HttpContext.Session.SetString("CompanyId", verificationUser.CompanyId.ToString());
                        }

                        HttpContext.Session.SetString("UserId", verificationUser.Id.ToString());
                        HttpContext.Session.SetString("UserEmail", verificationUser.Email.ToString());

                        return RedirectToPage("/Hotels");
                    }
                    else
                    {
                        ErrorMessage = "������������ � ������ ������� �� ������.";
                    }
                }
                catch (ApiException ex)
                {
                    ErrorMessage = "�������� ������ ������������.";
                }
            }

            return Page();
        }
    }
}
