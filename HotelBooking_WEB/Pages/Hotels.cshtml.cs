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

        [BindProperty(SupportsGet = true)]
        public string SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchCity { get; set; }
        public List<string> Cities { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public decimal? MinRating { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty]
        public List<HotelViewModel> Hotels { get; set; }

        public HotelsModel(ILogger<HotelsModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGet()
        {
            await _apiClient.UpdateHotelRatings();

            if (!string.IsNullOrEmpty(HttpContext.Request.Query["MinRating"]))
            {
                var ratingValue = HttpContext.Request.Query["MinRating"].ToString()
                    .Replace(",", ".", StringComparison.Ordinal);

                if (decimal.TryParse(ratingValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedRating))
                {
                    MinRating = parsedRating;
                }
            }

            var apiHotels = await _apiClient.GetHotelsAsync();

            Cities = apiHotels.Select(h => h.City?.Trim())
                              .Where(c => !string.IsNullOrEmpty(c))
                              .Distinct(StringComparer.OrdinalIgnoreCase)
                              .OrderBy(c => c)
                              .ToList();

            var hotelViewModels = new List<HotelViewModel>();

            foreach (var hotel in apiHotels)
            {
                var rooms = await _apiClient.GetRoomByHotelId(hotel.Id); // Возвращает List<RoomDto>

                var hotelVm = new HotelViewModel
                {
                    Id = hotel.Id,
                    Name = hotel.Name,
                    City = hotel.City,
                    Rating = (decimal)hotel.Rating,
                    ImageUrl = hotel.ImageUrl,
                    Address = hotel.Address,
                    Description = hotel.Description,
                    Rooms = rooms.ToList()
                };

                hotelViewModels.Add(hotelVm);
            }

            // Фильтрация

            var filteredHotels = hotelViewModels.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchName))
                filteredHotels = filteredHotels.Where(h => h.Name.Contains(SearchName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(SearchCity))
                filteredHotels = filteredHotels.Where(h => h.City.Equals(SearchCity, StringComparison.OrdinalIgnoreCase));

            if (MinRating.HasValue)
                filteredHotels = filteredHotels.Where(h => h.Rating >= MinRating.Value);

            if (MinPrice.HasValue)
            {
                filteredHotels = filteredHotels.Where(h =>
                    h.Rooms.Any(r => r.PricePerNight >= MinPrice.Value)
                );
            }

            if (MaxPrice.HasValue)
            {
                filteredHotels = filteredHotels.Where(h =>
                    h.Rooms.Any(r => r.PricePerNight <= MaxPrice.Value)
                );
            }

            if (MinPrice.HasValue && MaxPrice.HasValue)
            {
                filteredHotels = filteredHotels.Where(h =>
                    h.Rooms.Any(r => r.PricePerNight >= MinPrice.Value && r.PricePerNight <= MaxPrice.Value)
                );
            }

            Hotels = filteredHotels.ToList();
        }


        public async Task<IActionResult> OnPostBookHotel(int hotelId)
        {
            HttpContext.Session.SetString("HotelId", hotelId.ToString());
            return RedirectToPage("/Rooms");
        }

    }
}
