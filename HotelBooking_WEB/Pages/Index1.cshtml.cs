using HotelBooking_API.Data.Models;
using HotelBooking_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace HotelBooking_WEB.Pages
{
    public class CommentsModel : PageModel
    {
        private readonly ICommentsService _commentsService;
        private readonly IApiClient _apiClient;

        public CommentsModel(ICommentsService commentsService, IApiClient apiClient)
        {
            _commentsService = commentsService;
            _apiClient = apiClient;
        }

        [BindProperty(SupportsGet = true)]
        public int EntityId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EntityType { get; set; } // "Hotel" или "Room"

        public List<Data.Models.Comments> Comments { get; set; }

        [BindProperty]
        public NewCommentDto NewComment { get; set; } = new NewCommentDto();

        public async Task OnGetAsync(int entityId, string entityType)
        {
            EntityId = entityId;
            EntityType = entityType;

            Comments = await _apiClient.GetCommentsByHotelId(hotelId);
        }


        /*public async Task<IActionResult> OnPostBookHotel(int hotelId)
        {
            HttpContext.Session.SetString("HotelId", hotelId.ToString());
            return RedirectToPage("/Rooms");
        }*/

        public async Task<IActionResult> OnPostAddCommentAsync()
        {
            if (!ModelState.IsValid)
            {
                Comments = await _commentsService.GetCommentsByEntityAsync(EntityType, EntityId);
                return Page();
            }

            NewComment.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
            // Устанавливаем правильное поле в зависимости от EntityType
            if (EntityType == "Hotel")
            {
                NewComment.HotelId = EntityId;
                NewComment.RoomId = null;
            }
            else if (EntityType == "Room")
            {
                NewComment.RoomId = EntityId;
                NewComment.HotelId = null;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Неверный тип сущности.");
                Comments = await _commentsService.GetCommentsByEntityAsync(EntityType, EntityId);
                return Page();
            }

            await _commentsService.AddCommentAsync(NewComment);

            return RedirectToPage("/Comments", new { entityType = EntityType, entityId = EntityId });
        }
    }

    public class NewCommentDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Рейтинг должен быть от 1 до 5.")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; }

        public int? HotelId { get; set; }

        public int? RoomId { get; set; }
    }

    public interface ICommentsService
    {
        Task<List<Data.Models.Comments>> GetCommentsByEntityAsync(string entityType, int entityId);
        Task AddCommentAsync(NewCommentDto comment);
    }
}
