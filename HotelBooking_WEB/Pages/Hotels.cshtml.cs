using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using static NuGet.Packaging.PackagingConstants;

namespace HotelBooking_WEB.Pages
{
    public class HotelsModel : PageModel
    {
        private readonly ILogger<HotelsModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public IEnumerable<Hotel> Hotels { get; set; }


        [BindProperty(SupportsGet = true)]
        public string SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchCity { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinRating { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }



        public HotelsModel(ILogger<HotelsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["MinRating"]))
            {
                var ratingValue = HttpContext.Request.Query["MinRating"].ToString()
                    .Replace(",", ".", StringComparison.Ordinal);

                if (decimal.TryParse(ratingValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedRating))
                {
                    MinRating = parsedRating;
                }
            }

            var hotels = await _apiClient.GetHotelsAsync();

            foreach (var hotel in hotels)
            {
                var rooms = await _apiClient.GetRoomByHotelId(hotel.Id);
                hotel.Rooms = rooms.ToList();
            }

            if (!string.IsNullOrWhiteSpace(SearchName))
                hotels = hotels.Where(h => h.Name.Contains(SearchName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(SearchCity))
                hotels = hotels.Where(h => h.City.Contains(SearchCity, StringComparison.OrdinalIgnoreCase));

            if (MinRating.HasValue)
                hotels = hotels.Where(h => h.Rating >= MinRating.Value);

            if (MinPrice.HasValue)
            {
                hotels = hotels.Where(h =>
                    h.Rooms.Any(r => r.PricePerNight >= MinPrice.Value)
                );
            }

            if (MaxPrice.HasValue)
            {
                hotels = hotels.Where(h =>
                    h.Rooms.Any(r => r.PricePerNight <= MaxPrice.Value) 
                );
            }

            if (MinPrice.HasValue && MaxPrice.HasValue)
            {
                hotels = hotels.Where(h =>
                    h.Rooms.Any(r => r.PricePerNight >= MinPrice.Value && r.PricePerNight <= MaxPrice.Value)
                );
            }

            Hotels = hotels.ToList();
        }


        public async Task<IActionResult> OnPostBookHotel(int hotelId)
        {
            HttpContext.Session.SetString("HotelId", hotelId.ToString());
            return RedirectToPage("/Rooms");
        }

    }
}
