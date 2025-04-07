using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelBooking_API.Data.Models
{
    public class User
    {
        [Key] // Указываем, что это первичный ключ
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
        public int Id { get; set; }

        [Required] // Обязательное поле
        [MaxLength(50)] // Ограничение длины
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string SecondName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress] // Проверка формата email
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [Phone] // Проверка формата телефона
        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        // Навигационное свойство для бронирований
        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; }
    }
    public class CreateUserDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string SecondName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}