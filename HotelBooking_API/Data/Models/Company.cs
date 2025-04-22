using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelBooking_API.Data.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string ContactPerson { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Website { get; set; }

        [MaxLength(200)]
        public string LogoUrl { get; set; }

        [MaxLength(20)]
        public string TaxId { get; set; }

        [MaxLength(100)]
        public string LegalAddress { get; set; }


        [JsonIgnore]
        public ICollection<Hotel> Hotels { get; set; }
    }
}