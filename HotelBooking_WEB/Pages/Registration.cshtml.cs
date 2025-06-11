using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using HotelBooking_WEB.Data.Service;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Refit;
using System.Net;
using System.Threading.Tasks;

namespace HotelBooking_WEB.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly ILogger<RegistrationModel> _logger;
        private readonly IApiClient _apiClient;
        private readonly IEmailService _emailService;

        [BindProperty]
        public string email { get; set; }

        [BindProperty]
        public string phoneNumber { get; set; }

        [BindProperty]
        public string firstName { get; set; }

        [BindProperty]
        public string secondName { get; set; }

        [BindProperty]
        public string lastName { get; set; }

        [BindProperty]
        public string password { get; set; }

        public RegistrationModel(ILogger<RegistrationModel> logger, IApiClient apiClient, IEmailService emailService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _emailService = emailService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newUser = new CreateUserDto
                    {
                        FirstName = firstName,
                        SecondName = secondName,
                        LastName = lastName,
                        Email = email,
                        Phone = phoneNumber,
                        Password = password,
                        CompanyId = null
                    };

                    try
                    {
                        bool itog = await _apiClient.CreateUser(newUser);


                        try
                        {
                            await _emailService.SendEmailAsync(email.ToString(), 
                                "Регистрация в HotelBooking",
                                "Уважаемый(ая) пользователь,<br/><br/>" +
                                "Благодарим вас за регистрацию в сервисе HotelBooking.<br/><br/>" +
                                "Мы рады приветствовать вас и надеемся, что наш сервис поможет вам удобно и быстро бронировать отели.<br/><br/>" +
                                "Если у вас возникнут вопросы или потребуется помощь, пожалуйста, свяжитесь с нашей службой поддержки.<br/><br/>" +
                                "С уважением,<br/><br/>" +
                                "Команда HotelBooking");
                        }
                        catch (SmtpCommandException ex)
                        {
                            _logger.LogError($"Ошибка SMTP: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Ошибка отправки письма: {ex.Message}");
                        }

                        if (itog)
                            return RedirectToPage("/Index");
                    }
                    catch (ApiException ex) // Используйте нужный тип исключения для вашего клиента
                    {
                        // Например, если вернулся BadRequest
                        if (ex.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, "Пользователь с таким email уже существует.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Ошибка при создании пользователя.");
                        }
                    }
                }

                return Page();
            }
            catch
            {
                return Page();
            }
        }

    }
}

