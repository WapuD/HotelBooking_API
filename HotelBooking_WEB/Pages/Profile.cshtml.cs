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
        public UpdateUserDto User { get; set; }

        public ProfileModel(ILogger<ProfileModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = await _apiClient.GetUser(userId);
            User = new UpdateUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                LastName = user.LastName,
                Phone = user.Phone
            };
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _apiClient.UpdateUser(User);
                TempData["SuccessMessage"] = "Данные пользователя успешно обновлены.";
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ошибка: {ex.Message}");
                return Page();
            }
        }
    }
}
