using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking_WEB.Pages
{
    public class AdminHotelsModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public AdminHotelsModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty(SupportsGet = true)]
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
        public List<Hotel> Hotels { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var company = await _apiClient.GetCompany(CompanyId);
            if (company == null)
            {
                return RedirectToPage("/CompaniesList");
            }
            CompanyName = company.Name;
            Hotels = (await _apiClient.GetHotelsByCompanyId(CompanyId)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _apiClient.DeleteHotel(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при удалении отеля: " + ex.Message);
                Hotels = (await _apiClient.GetHotelsByCompanyId(CompanyId)).ToList();
                return Page();
            }

            return RedirectToPage();
        }

    }
}
