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

        [ForeignKey("Hotel")]
        public int HotelId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Название комнаты")]
        public string RoomName { get; set; }
        
        [Required]
        [Display(Name = "Цена за ночь")]
        public decimal PricePerNight { get; set; }

        [Range(1, 10)]
        [Display(Name = "Вместимость")]
        public int Capacity { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Количество")]
        public int Count { get; set; }

        public Hotel? Hotel { get; set; }

        [JsonIgnore]
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<RoomAmenity>? RoomAmenities { get; set; }
        public ICollection<RoomImages>? RoomImages { get; set; }
    }

    public class RoomDto
    {
        public int Id { get; set; }

        [Display(Name = "Название комнаты")]
        public string RoomName { get; set; }

        [Display(Name = "Цена за ночь")]
        public decimal PricePerNight { get; set; }

        [Display(Name = "Вместимость")]
        public int Capacity { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Количество")]
        public int Count { get; set; }

        public List<AmenityDto> Amenities { get; set; } = new();
        public List<RoomImageDto> RoomImages { get; set; } = new();
    }
}