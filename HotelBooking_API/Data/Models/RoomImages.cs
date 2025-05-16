using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelBooking_API.Data.Models
{
    public class RoomImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Room")] // Внешний ключ
        public int RoomId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ImageUrl { get; set; }

        // Навигационные свойства
        [JsonIgnore]
        public Room? Room { get; set; }
    }

}