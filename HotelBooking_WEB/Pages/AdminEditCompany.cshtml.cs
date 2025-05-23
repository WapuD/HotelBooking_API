using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking_WEB.Pages
{
    public class AdminEditCompanyModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public AdminEditCompanyModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public Company Company { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Company = await _apiClient.GetCompany(id);
            if (Company == null)
            {
                return RedirectToPage("/AdminCompanies");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var dto = new CompanyCreateDto
            {
                Name = Company.Name,
                Description = Company.Description,
                Email = Company.Email,
                Phone = Company.Phone,
                Website = Company.Website,
                LogoUrl = Company.LogoUrl,
                TaxId = Company.TaxId,
                LegalAddress = Company.LegalAddress
            };

            var success = await _apiClient.UpdateCompany(Company.Id, dto);

            if (!success)
            {
                ModelState.AddModelError("", "Ошибка при обновлении компании");
                return Page();
            }

            return RedirectToPage("/AdminCompanies");
        }
    }
}
