﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking_API.Data.Models
{
public class Amenity
    {
        [Key] // Первичный ключ
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент
        public int Id { get; set; }

        [Required]
        [MaxLength(100)] // Ограничение длины для названия
        public string Name { get; set; }

        // Навигационное свойство для связи с номерами
        public ICollection<RoomAmenity> RoomAmenities { get; set; }
    }
    public class AmenityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}