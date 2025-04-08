using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelBooking_API.Data.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }

        [Required]
        public DateTimeOffset CheckInDate { get; set; }

        [Required]
        public DateTimeOffset CheckOutDate { get; set; }

        [Required]
        public int TotalPrice { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        // Навигационные свойства
        public User? User { get; set; }
        public Room? Room { get; set; }
    }
    public class BookingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTimeOffset CheckInDate { get; set; }
        public DateTimeOffset CheckOutDate { get; set; }
        public int TotalPrice { get; set; }
        public string Status { get; set; }
    }

}