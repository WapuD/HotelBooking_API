using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking_API.Data.Models
{
    public class RoomAmenity
    {
        [Key] // Указываем, что это первичный ключ
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
        public int Id { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }

        [ForeignKey("Amenity")]
        public int AmenityId { get; set; }

        // Навигационные свойства
        public Room Room { get; set; }
        public Amenity Amenity { get; set; }
    }

}