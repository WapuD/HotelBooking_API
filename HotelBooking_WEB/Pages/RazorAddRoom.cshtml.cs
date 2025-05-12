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
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "RoomImg");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // ��������� ������� ��� ����� �� �������� �������, ������� ������� � ����������� �������
                    string safeRoomName = string.Concat(Room.RoomName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)))
                                            .Replace(" ", "_");

                    string extension = Path.GetExtension(ImageFile.FileName);
                    if (string.IsNullOrEmpty(extension))
                        extension = ".png"; // �� ���������

                    string fileName;
                    string filePath;
                    int counter = 1;

                    do
                    {
                        fileName = $"{safeRoomName}_{counter}{extension}";
                        filePath = Path.Combine(uploadsFolder, fileName);
                        counter++;
                    } while (System.IO.File.Exists(filePath));

                    // ��������� ����
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // ��������� ������ � ����
                    var roomImage = new RoomImages
                    {
                        RoomId = Room.Id,
                        ImageUrl = "/RoomImg/" + fileName.Replace("\\", "/")
                    };

                    await _apiClient.PostRoomImage(roomImage);
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
