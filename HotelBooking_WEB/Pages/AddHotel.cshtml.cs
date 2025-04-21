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

        public AddHotelModel(IWebHostEnvironment environment, IApiClient apiClient)
        {
            _environment = environment;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var baseName = "HotelPhoto";
                var extension = ".png";
                var fileName = $"{baseName}{extension}";
                var filePath = Path.Combine(_environment.WebRootPath, fileName);

                // Проверка на существование файла с таким же именем
                var counter = 1;
                while (System.IO.File.Exists(filePath))
                {
                    fileName = $"{baseName}_{counter}{extension}";
                    filePath = Path.Combine(_environment.WebRootPath, fileName);
                    counter++;
                }

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    Hotel.ImageUrl = fileName;
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
                    throw;
                }
            }

            try
            {
                var newHotel = new HotelDtoCreate
                {
                    Name = Hotel.Name,
                    Address = Hotel.Address,
                    City = Hotel.City,
                    Description = Hotel.Description,
                    ImageUrl = Hotel.ImageUrl,
                    Rating = 0
                };
                var result = await _apiClient.CreateHotel(newHotel);
            }
            catch (Refit.ApiException ex)
            {
                Console.WriteLine($"Ошибка API: {ex.StatusCode}, Content: {ex.Content}");
                throw;
            }
            return RedirectToPage("/Hotels");
        }
    }

}
