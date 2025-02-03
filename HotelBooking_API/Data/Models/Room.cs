using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking_API.Data.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Hotel")] // Внешний ключ
        public int HotelId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoomNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoomType { get; set; }

        public decimal PricePerNight { get; set; }

        [Range(1, 10)] // Ограничение для вместимости
        public int Capacity { get; set; }

        public string Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Навигационные свойства
        public Hotel Hotel { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<RoomAmenity> RoomAmenities { get; set; }
    }

}