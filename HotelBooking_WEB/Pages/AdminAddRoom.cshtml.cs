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
                // ��������� ������� ��� �����������
                var createRoomResult = await _apiClient.CreateRoom(Room);
                if (!createRoomResult)
                {
                    ModelState.AddModelError("", "������ ��� ���������� �������.");
                    Hotels = await _apiClient.GetHotelsAsync();
                    return Page();
                }

                // ��������������, ��� Room.Id �������� ����� �������� �������
                // ���� ���, ����� �������� Id ������ ��������

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var imagesFolder = Path.Combine(_environment.WebRootPath, "RoomImg");

                    // ���������, ���������� �� �����, ���� ��� - ������
                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    //var baseName = "HotelPhoto"; // ������
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

                    // ��������� ������ � ����
                    var roomImage = new RoomImageCreateDto
                    {
                        RoomId = Room.Id,
                        ImageUrl = "/RoomImg/" + fileName.Replace("\\", "/")
                    };

                    await _apiClient.PostRoomImage(roomImage);
                    TempData["SuccessMessage"] = true;
                    TempData["SuccessMessage"] = "������� ������� ���������.";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"������: {ex.Message}");
                Hotels = await _apiClient.GetHotelsAsync();
                return RedirectToPage("/Rooms");
            }

            return RedirectToPage("/Rooms");
        }
    }
}
