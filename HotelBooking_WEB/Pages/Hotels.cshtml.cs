using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static NuGet.Packaging.PackagingConstants;

namespace HotelBooking_WEB.Pages
{
    public class HotelsModel : PageModel
    {
        private readonly ILogger<HotelsModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public IEnumerable<Hotel> Hotels { get; set; }
        public HotelsModel(ILogger<HotelsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            Hotels = await _apiClient.GetHotelsAsync();
            var z = 0;
        }
    }
}
