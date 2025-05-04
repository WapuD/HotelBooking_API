using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelBooking_WEB.Pages
{
    public class AdminRegistrationModel : PageModel
    {
        private readonly ILogger<AdminRegistrationModel> _logger;
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

        [BindProperty]
        public int? CompanyId { get; set; }

        public List<Company> Companies { get; set; }
        public List<SelectListItem> CompanySelectList { get; set; }

        public AdminRegistrationModel(ILogger<AdminRegistrationModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            Companies = (await _apiClient.GetCompaniesAsync()).ToList();
            CompanySelectList = Companies
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
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
                    CompanyId = CompanyId
                };
                bool itog = await _apiClient.CreateUser(newUser);
                if (itog)
                    return RedirectToPage("/AdminBookings");
            }
            Companies = (await _apiClient.GetCompaniesAsync()).ToList();
            return Page();
        }
    }
}

