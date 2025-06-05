using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using System.Threading.Tasks;

namespace HotelBooking_WEB.Pages
{
    public class AdminEditRoomModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public AdminEditRoomModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public RoomDto Room { get; set; }
        [BindProperty]
        public int? HotelId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Room = await _apiClient.GetRoomById(id);
            if (Room == null)
            {
                return RedirectToPage("/AdminRooms");
            }
            HotelId = await _apiClient.GetHotelIdByRoomIdAsync(id);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _apiClient.UpdateRoom(Room.Id, Room);
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError("", "Ошибка при обновлении номера: " + ex.Message);
                return Page();
            }

            return RedirectToPage("/AdminRooms", new { hotelId = HotelId });
        }
    }
}