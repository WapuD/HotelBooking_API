using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking_API.Data.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; }
        public int? HotelId { get; set; }
        public int? RoomId { get; set; }

        public Hotel? Hotel { get; set; }
        public Room? Room { get; set; }
    }
    public class NewCommentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; }
        public int? HotelId { get; set; }
        public int? RoomId { get; set; }
    }
}
