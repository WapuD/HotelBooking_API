using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelBooking_API.Data.Models
{
    public class Booking
    {
        [Key] // Первичный ключ
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
        public int Id { get; set; }

        [ForeignKey("User")] // Внешний ключ для пользователя
        public int UserId { get; set; }

        [ForeignKey("Room")] // Внешний ключ для номера
        public int RoomId { get; set; }

        [Required] // Обязательное поле
        public DateTimeOffset CheckInDate { get; set; }

        [Required]
        public DateTimeOffset CheckOutDate { get; set; }

        [Required]
        public int TotalPrice { get; set; }

        [Required]
        [MaxLength(20)] // Ограничение длины для статуса
        public string Status { get; set; } = "Pending"; // Например, "Pending", "Confirmed", "Cancelled"

        // Навигационные свойства
        public User User { get; set; }
        public Room Room { get; set; }
        public Payment Payment { get; set; } // Один Payment на одно бронирование
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