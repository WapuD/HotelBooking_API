using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking_WEB.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly ILogger<ProfileModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public User User { get; set; }

        public ProfileModel(ILogger<ProfileModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            User = await _apiClient.GetUser(userId);
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                //await _apiClient.UpdateUser(User);
                return RedirectToPage("/Profile");
            }
            return Page();
        }
    }
}
