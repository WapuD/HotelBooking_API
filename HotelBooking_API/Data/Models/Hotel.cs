using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking_API.Data.Models
{
    public class Hotel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Range(0, 5)] // Ограничение для рейтинга (от 0 до 5)
        public decimal Rating { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string ImageUrl { get; set; }

        // Навигационное свойство для номеров
        public ICollection<Room> Rooms { get; set; }
    }
}