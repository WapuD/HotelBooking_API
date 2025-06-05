using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelBooking_API.Data.Models
{
    public class Hotel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Range(0, 5)] // Ограничение для рейтинга (от 0 до 5)
        public decimal? Rating { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        [MaxLength(1000)]
        public string? ImageUrl { get; set; }

        // Навигационное свойство для номеров
        [JsonIgnore]
        public ICollection<Room>? Rooms { get; set; }

        [ForeignKey("User")]
        public int CompanyId { get; set; }

        public Company? Company { get; set; }

        [JsonIgnore]
        public ICollection<Comment>? Comments { get; set; }
    }
    public class HotelDtoCreate
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Range(0, 5)]
        public decimal? Rating { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string ImageUrl { get; set; }
        public int CompanyId { get; set; }
    }
    public class HotelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public decimal Rating { get; set; }
        public List<RoomDto> Rooms { get; set; } = new();
        public string ImageUrl { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }

}