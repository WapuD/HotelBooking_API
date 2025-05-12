using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [MaxLength(100)]
        public string RoomName { get; set; }
        
        [Required]
        public decimal PricePerNight { get; set; }

        [Range(1, 10)] // Ограничение для вместимости
        public int Capacity { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public int Count { get; set; }

        // Навигационные свойства
        public Hotel? Hotel { get; set; }

        [JsonIgnore]
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<RoomAmenity>? RoomAmenities { get; set; }
        public ICollection<RoomImages>? RoomImages { get; set; }
    }

}