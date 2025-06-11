using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking_WEB.Pages
{
    public class AddRoomModel : PageModel
    {
        private readonly IApiClient _apiClient;
        private readonly IWebHostEnvironment _environment;

        public AddRoomModel(IApiClient apiClient, IWebHostEnvironment environment)
        {
            _apiClient = apiClient;
            _environment = environment;
        }

        [BindProperty]
        public Room Room { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public IEnumerable<Hotel> Hotels { get; set; }

        public async Task OnGet()
        {
            Hotels = await _apiClient.GetHotelsAsync();
            var companyId = int.Parse(HttpContext.Session.GetString("CompanyId"));
            Hotels = Hotels.Where(h => h.CompanyId == companyId).ToList();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                Hotels = await _apiClient.GetHotelsAsync();
                return Page();
            }

            try
            {
                // Сохраняем комнату без изображения
                var createRoomResult = await _apiClient.CreateRoom(Room);
                if (!createRoomResult)
                {
                    ModelState.AddModelError("", "Ошибка при добавлении комнаты.");
                    Hotels = await _apiClient.GetHotelsAsync();
                    return Page();
                }

                // Предполагается, что Room.Id заполнен после создания комнаты
                // Если нет, нужно получить Id другим способом

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var imagesFolder = Path.Combine(_environment.WebRootPath, "RoomImg");

                    // Проверяем, существует ли папка, если нет - создаём
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    //var baseName = "HotelPhoto"; // Ошибка
                    var baseName = "RoomPhoto";
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

                    // Сохраняем ссылку в базе
                    var roomImage = new RoomImageCreateDto
                    {
                        RoomId = Room.Id,
                        ImageUrl = "/RoomImg/" + fileName.Replace("\\", "/")
                    };

                    await _apiClient.PostRoomImage(roomImage);
                    TempData["SuccessMessage"] = true;
                    TempData["SuccessMessage"] = "Комната успешно добавлена.";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка: {ex.Message}");
                Hotels = await _apiClient.GetHotelsAsync();
                return RedirectToPage("/Rooms");
            }

            return RedirectToPage("/Rooms");
        }
    }
}
