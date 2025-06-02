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
        private readonly IEmailService _emailService;

        [BindProperty]
        public string email { get; set; }
        [BindProperty]
        public string password { get; set; }
        public string ErrorMessage { get; set; }

        public IngexModel(ILogger<IngexModel> logger, IApiClient apiClient, IEmailService emailService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _emailService = emailService;
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
                        try
                        {
                            await _emailService.SendEmailAsync(email.ToString(), "Регистрация", "Поздравляем вас с регистрацией в сервисе HotelBooking");
                        }
                        catch (SmtpCommandException ex)
                        {
                            _logger.LogError($"Ошибка SMTP: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Ошибка отправки письма: {ex.Message}");
                        }

                        HttpContext.Session.SetString("UserId", verificationUser.Id.ToString());
                        HttpContext.Session.SetString("UserEmail", verificationUser.Email.ToString());

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
