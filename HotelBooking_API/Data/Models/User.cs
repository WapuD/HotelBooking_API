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

        [MaxLength(50)]
        public string? LastName { get; set; } = null;

        [Required]
        [EmailAddress] // Проверка формата email
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Введите корректный номер телефона.")]
        public string Phone { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

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

        public string? LastName { get; set; } = null;

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Введите корректный номер телефона.")]
        public string Phone { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public int? CompanyId { get; set; } = null;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? LastName { get; set; }
        public string Phone { get; set; }
        public int? CompanyId { get; set; }
    }
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Введите старый пароль")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Повторите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class UpdateUserPasswordDto
    {
        public int UserId { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}