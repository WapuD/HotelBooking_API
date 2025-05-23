using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace HotelBooking_WEB.Pages
{
    public class AdminEditHotelModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public AdminEditHotelModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public Hotel Hotel { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Hotel = await _apiClient.GetHotelById(id);
            if (Hotel == null)
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

            var dto = new HotelDtoCreate
            {
                Name = Hotel.Name,
                City = Hotel.City,
                Address = Hotel.Address,
                Description = Hotel.Description,
                ImageUrl = Hotel.ImageUrl,
                Rating = Hotel.Rating,
                CompanyId = Hotel.CompanyId
            };

            var success = await _apiClient.UpdateHotel(Hotel.Id, dto);

            if (!success)
            {
                ModelState.AddModelError("", "Ошибка при обновлении отеля");
                return Page();
            }

            return RedirectToPage("/AdminHotels", new { companyId = Hotel.CompanyId });
        }
    }
}
