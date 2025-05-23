using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking_WEB.Pages
{
    public class AdminCompaniesModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public AdminCompaniesModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<Company> Companies { get; set; } = new();

        public async Task OnGet()
        {
            Companies = (await _apiClient.GetCompaniesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _apiClient.DeleteCompany(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при удалении компании: " + ex.Message);
                Companies = (await _apiClient.GetCompaniesAsync()).ToList();
                return Page();
            }

            return RedirectToPage();
        }
    }
}
