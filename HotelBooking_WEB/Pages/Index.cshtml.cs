using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static NuGet.Packaging.PackagingConstants;

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
        public IngexModel(ILogger<IngexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            if (email != null && password != null)
            {
                if (email == "HotelBooking@mail.ru")
                    return RedirectToPage("/AdminBookings");

                var verificationUser = await _apiClient.GetVerification(email, password);
                if (verificationUser != null)
                {
                    HttpContext.Session.SetString("UserId", verificationUser.Id.ToString());
                    return RedirectToPage("/Hotels");
                }
                else
                {
                    return Page();
                }
            }

            return Page();
        }
    }
}
