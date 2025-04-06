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
        public string TypeComent { get; set; }
    }
}
