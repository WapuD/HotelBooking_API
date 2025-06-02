using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace HotelBooking_WEB.Pages
{
    public class RoomsModel : PageModel
    {
        private readonly ILogger<RoomsModel> _logger;
        private readonly IApiClient _apiClient;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        [BindProperty]
        public IEnumerable<RoomDto> Rooms { get; set; }  // Используем RoomDto

        [BindProperty]
        public IEnumerable<Comment> Comments { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckInDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? CheckOutDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterCapacity { get; set; }

        [BindProperty(SupportsGet = true)]
        public string RoomNameFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DescriptionFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortCommentsBy { get; set; } = "date";

        [BindProperty(SupportsGet = true)]
        public bool SortDescending { get; set; } = true; // true — по убыванию, false — по возрастанию

        public Hotel Hotel { get; set; }

        public RoomsModel(ILogger<RoomsModel> logger, IApiClient apiClient,
                          ICompositeViewEngine viewEngine,
                          ITempDataProvider tempDataProvider,
                          IServiceProvider serviceProvider)
        {
            _logger = logger;
            _apiClient = apiClient;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> OnGet(DateTime? checkInDate = null, DateTime? checkOutDate = null)
        {
            var hotelIdStr = HttpContext.Session.GetString("HotelId");
            if (string.IsNullOrEmpty(hotelIdStr))
            {
                return RedirectToPage("/Hotels");
            }

            var hotelId = int.Parse(hotelIdStr);

            CheckInDate = checkInDate ?? DateTime.Today;
            CheckOutDate = checkOutDate;

            var allRooms = await _apiClient.GetRoomByHotelId(hotelId); // Возвращает IEnumerable<RoomDto>
            Hotel = await _apiClient.GetHotelById(hotelId);

            if (CheckInDate.HasValue && CheckOutDate.HasValue)
            {
                if (CheckOutDate < CheckInDate)
                {
                    ModelState.AddModelError(string.Empty, "Дата выезда не может быть раньше даты заезда.");
                    Rooms = Enumerable.Empty<RoomDto>();
                    return Page();
                }

                allRooms = await FilterAvailableRooms(allRooms, CheckInDate.Value, CheckOutDate.Value);

                HttpContext.Session.SetString("CheckInDate", CheckInDate.Value.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString("CheckOutDate", CheckOutDate.Value.ToString("yyyy-MM-dd"));
            }

            // Фильтрация по цене, вместимости, названию и описанию (если нужно)
            if (MinPrice.HasValue)
                allRooms = allRooms.Where(r => r.PricePerNight >= MinPrice.Value);
            if (MaxPrice.HasValue)
                allRooms = allRooms.Where(r => r.PricePerNight <= MaxPrice.Value);
            if (FilterCapacity.HasValue)
                allRooms = allRooms.Where(r => r.Capacity >= FilterCapacity.Value);
            if (!string.IsNullOrWhiteSpace(RoomNameFilter))
                allRooms = allRooms.Where(r => r.RoomName.Contains(RoomNameFilter, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(DescriptionFilter))
                allRooms = allRooms.Where(r => r.Description.Contains(DescriptionFilter, StringComparison.OrdinalIgnoreCase));

            Rooms = allRooms.ToList();

            // Загрузка и сортировка отзывов с учётом направления сортировки
            var comments = (await _apiClient.GetCommentsByHotelId(hotelId)).ToList();

            Comments = (SortCommentsBy?.ToLower(), SortDescending) switch
            {
                ("rating", true) => comments.OrderByDescending(c => c.Rating ?? 0).ToList(),
                ("rating", false) => comments.OrderBy(c => c.Rating ?? 0).ToList(),
                ("date", true) => comments.OrderByDescending(c => c.CreatedDate).ToList(),
                ("date", false) => comments.OrderBy(c => c.CreatedDate).ToList(),
                _ => comments.OrderByDescending(c => c.CreatedDate).ToList(),
            };

            return Page();
        }

        private async Task<IEnumerable<RoomDto>> FilterAvailableRooms(IEnumerable<RoomDto> rooms, DateTime checkInDate, DateTime checkOutDate)
        {
            var utcCheckIn = new DateTime(checkInDate.Year, checkInDate.Month, checkInDate.Day, 5, 0, 0).ToUniversalTime();
            var utcCheckOut = new DateTime(checkOutDate.Year, checkOutDate.Month, checkOutDate.Day, 5, 0, 0).ToUniversalTime();
            var checkInStr = utcCheckIn.ToString("yyyy-MM-dd");
            var checkOutStr = utcCheckOut.ToString("yyyy-MM-dd");

            var availableRooms = new List<RoomDto>();
            foreach (var room in rooms)
            {
                var roomCount = await _apiClient.GetAvailableRoomCount(room.Id, checkInStr, checkOutStr);
                if (roomCount != 0)
                    availableRooms.Add(room);
            }

            return availableRooms;
        }

        public async Task<IActionResult> OnGetCommentsPartialAsync(string sortCommentsBy, bool sortDescending = true)
        {
            var hotelIdStr = HttpContext.Session.GetString("HotelId");
            if (string.IsNullOrEmpty(hotelIdStr))
            {
                return Content("");
            }

            var hotelId = int.Parse(hotelIdStr);
            var comments = (await _apiClient.GetCommentsByHotelId(hotelId)).ToList();

            var sortedComments = (sortCommentsBy?.ToLower(), sortDescending) switch
            {
                ("rating", true) => comments.OrderByDescending(c => c.Rating ?? 0).ToList(),
                ("rating", false) => comments.OrderBy(c => c.Rating ?? 0).ToList(),
                ("date", true) => comments.OrderByDescending(c => c.CreatedDate).ToList(),
                ("date", false) => comments.OrderBy(c => c.CreatedDate).ToList(),
                _ => comments.OrderByDescending(c => c.CreatedDate).ToList(),
            };

            var html = await RenderPartialViewToStringAsync("_CommentsPartial", sortedComments);
            return Content(html, "text/html");
        }


        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
                throw new ArgumentNullException($"View {viewName} не найден.");

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(httpContext, _tempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                tempData,
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }

    }
}
