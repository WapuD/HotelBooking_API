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
                var fileName = "HotelPhoto"; // Используем имя файла, которое выбрал пользователь
                var filePath = Path.Combine(_environment.WebRootPath, fileName);

                // Проверка на существование файла с таким же именем
                if (System.IO.File.Exists(filePath))
                {
                    // Если файл существует, добавляем номер к имени
                    var baseName = Path.GetFileNameWithoutExtension(fileName);
                    var extension = Path.GetExtension(fileName);
                    var counter = 1;
                    while (System.IO.File.Exists(filePath))
                    {
                        fileName = $"{baseName}_{counter}{extension}.png";
                        filePath = Path.Combine(_environment.WebRootPath, fileName);
                        counter++;
                    }
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                Hotel.ImageUrl = fileName; // Сохраняем имя файла
            }

            try
            {
                await _apiClient.CreateHotel(Hotel);
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
