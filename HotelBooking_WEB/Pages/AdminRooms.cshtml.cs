using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking_WEB.Pages
{
    public class RoomsListModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public RoomsListModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty(SupportsGet = true)]
        public int HotelId { get; set; }

        public string HotelName { get; set; }
        public int CompanyId { get; set; }
        public List<RoomDto> Rooms { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var hotel = await _apiClient.GetHotelById(HotelId);
            if (hotel == null)
            {
                return RedirectToPage("/AdminHotels");
            }
            HotelName = hotel.Name;
            CompanyId = hotel.CompanyId;
            HttpContext.Session.SetString("CompanyId", CompanyId.ToString());
            Rooms = (await _apiClient.GetRoomByHotelId(HotelId)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _apiClient.DeleteRoom(id);
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError("", "Ошибка при удалении комнаты: " + ex.Message);
                Rooms = (await _apiClient.GetRoomByHotelId(HotelId)).ToList();
                return Page();
            }

            return RedirectToPage();
        }
    }
}
