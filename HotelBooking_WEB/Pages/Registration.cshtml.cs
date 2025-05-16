using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
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

        public RegistrationModel(ILogger<RegistrationModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
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

    }
}

