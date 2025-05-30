using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking_WEB.Pages
{
    public class AddHotelModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public Hotel Hotel { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }
        public List<Company> Companies { get; set; }

        public AddHotelModel(IWebHostEnvironment environment, IApiClient apiClient)
        {
            _environment = environment;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            Companies = (await _apiClient.GetCompaniesAsync()).ToList();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Если есть Companies для выбора, нужно их заново загрузить
                Companies = (await _apiClient.GetCompaniesAsync()).ToList();
                return Page();
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var imagesFolder = Path.Combine(_environment.WebRootPath, "HotelImg");

                // Проверяем, существует ли папка, если нет - создаём
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                var baseName = "HotelPhoto";
                var extension = Path.GetExtension(ImageFile.FileName);
                var fileName = $"{baseName}{extension}";
                var filePath = Path.Combine(imagesFolder, fileName);

                var counter = 1;
                while (System.IO.File.Exists(filePath))
                {
                    fileName = $"{baseName}_{counter}{extension}";
                    filePath = Path.Combine(imagesFolder, fileName);
                    counter++;
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                Hotel.ImageUrl = "/HotelImg/" + fileName; // Относительный путь для отображения

            }

            try
            {
                var companyId = Convert.ToInt32(HttpContext.Session.GetString("CompanyId"));

                var newHotel = new HotelDtoCreate
                {
                    Name = Hotel.Name,
                    Address = Hotel.Address,
                    City = Hotel.City,
                    Description = Hotel.Description,
                    ImageUrl = Hotel.ImageUrl,
                    Rating = 0,
                    CompanyId = companyId
                };

                var result = await _apiClient.CreateHotel(newHotel);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Ошибка при создании отеля");
                    Companies = (await _apiClient.GetCompaniesAsync()).ToList();
                    return Page();
                }
            }
            catch (Refit.ApiException ex)
            {
                ModelState.AddModelError(string.Empty, $"Ошибка API: {ex.StatusCode}");
                Companies = (await _apiClient.GetCompaniesAsync()).ToList();
                return Page();
            }

            return RedirectToPage("/AdminBookings");
        }
    }
}
