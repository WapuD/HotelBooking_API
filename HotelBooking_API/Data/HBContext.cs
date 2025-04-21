using Microsoft.EntityFrameworkCore;
using HotelBooking_API.Data.Models;

namespace HotelBooking_API.Data
{
    public class HBContext : DbContext
    {
        // Конструктор с параметрами для настройки контекста
        public HBContext(DbContextOptions<HBContext> options)
            : base(options)
        {
        }

        // DbSet для каждой сущности
        public DbSet<User> User { get; set; }
        public DbSet<Amenity> Amenity { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<RoomAmenity> RoomAmenity { get; set; }
        public DbSet<RoomImages> RoomImages { get; set; }

        // Настройка моделей и отношений между сущностями
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи между User и Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление бронирований при удалении пользователя

            // Настройка связи между Hotel и Room
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление комнат при удалении отеля

            // Настройка связи между Room и RoomAmenity
            modelBuilder.Entity<RoomAmenity>()
                .HasKey(ra => new { ra.RoomId, ra.AmenityId }); // Составной ключ

            modelBuilder.Entity<RoomAmenity>()
                .HasOne(ra => ra.Room)
                .WithMany(r => r.RoomAmenities)
                .HasForeignKey(ra => ra.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomAmenity>()
                .HasOne(ra => ra.Amenity)
                .WithMany(a => a.RoomAmenities)
                .HasForeignKey(ra => ra.AmenityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи между Room и RoomImages
            modelBuilder.Entity<RoomImages>()
                .HasOne(ri => ri.Room)
                .WithMany(r => r.RoomImages)
                .HasForeignKey(ri => ri.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка индексов для улучшения производительности
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Уникальный индекс для Email

            modelBuilder.Entity<Hotel>()
                .HasIndex(h => h.Name);

            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomNumber);

            // Seed data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Иван",
                    SecondName = "Иванов",
                    LastName = "Иванович",
                    Email = "ivan@example.com",
                    Phone = "+7 123 456 7890",
                    PasswordHash = new byte[16] // Needs to be correct length
                },
                new User
                {
                    Id = 2,
                    FirstName = "Мария",
                    SecondName = "Петрова",
                    LastName = "Николаевна",
                    Email = "maria@example.com",
                    Phone = "+7 987 654 3210",
                    PasswordHash = new byte[16] // Needs to be correct length
                }
            );

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Отель Премиум",
                    Description = "Отель Премиум",
                    Address = "Москва, ул. Ленина, 10",
                    City = "Москва",
                    ImageUrl = "Premium.png",
                    Rating = 4.5M
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Отель Эконом",
                    Description = "Отель Эконом",
                    Address = "Санкт-Петербург, ул. Пушкина, 5",
                    City = "Санкт-Петербург",
                    ImageUrl = "Ekonom.png",
                    Rating = 3.8M
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Отель Бизнес",
                    Description = "Отель для деловых поездок с конференц-залом",
                    Address = "Москва, ул. Ленина, 10",
                    City = "Москва",
                    ImageUrl = "Business.png",
                    Rating = 4.2M
                },
                new Hotel
                {
                    Id = 4,
                    Name = "Гранд Отель",
                    Description = "Роскошный отель с видом на город",
                    Address = "Санкт-Петербург, Невский проспект, 20",
                    City = "Санкт-Петербург",
                    ImageUrl = "Grand.png",
                    Rating = 4.8M
                },
                new Hotel
                {
                    Id = 5,
                    Name = "Отель на набережной",
                    Description = "Отель с видом на реку, идеален для романтических поездок",
                    Address = "Казань, ул. Речная, 15",
                    City = "Казань",
                    ImageUrl = "RiverView.png",
                    Rating = 4.5M
                },
                new Hotel
                {
                    Id = 6,
                    Name = "Отель для семьи",
                    Description = "Отель с детскими площадками и развлекательными программами",
                    Address = "Сочи, ул. Морская, 30",
                    City = "Сочи",
                    ImageUrl = "Family.png",
                    Rating = 4.1M
                },
                new Hotel
                {
                    Id = 7,
                    Name = "Отель в центре города",
                    Description = "Удобное расположение для туристов",
                    Address = "Екатеринбург, ул. Ленина, 25",
                    City = "Екатеринбург",
                    ImageUrl = "CityCenter.png",
                    Rating = 4.0M
                },
                new Hotel
                {
                    Id = 8,
                    Name = "Отель у горы",
                    Description = "Отель для любителей активного отдыха",
                    Address = "Красная Поляна, ул. Горная, 10",
                    City = "Красная Поляна",
                    ImageUrl = "Mountain.png",
                    Rating = 4.3M
                },
                new Hotel
                {
                    Id = 9,
                    Name = "Отель на пляже",
                    Description = "Отель с прямым выходом на пляж",
                    Address = "Анапа, ул. Пляжная, 5",
                    City = "Анапа",
                    ImageUrl = "Beach.png",
                    Rating = 4.6M
                },
                new Hotel
                {
                    Id = 10,
                    Name = "Отель в историческом центре",
                    Description = "Отель в историческом здании с уникальной атмосферой",
                    Address = "Ростов-на-Дону, ул. Старая, 20",
                    City = "Ростов-на-Дону",
                    ImageUrl = "Historic.png",
                    Rating = 4.4M
                }
            );

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    HotelId = 1, // Связь с отелем "Отель Премиум"
                    RoomNumber = "101",
                    RoomName = "Стандарт",
                    PricePerNight = 5000,
                    Capacity = 2,
                    Description = "Обычный номер, предоставляющий всё необходимое",
                    Count = 10
                },
                new Room
                {
                    Id = 2,
                    HotelId = 1, // Связь с отелем "Отель Премиум"
                    RoomNumber = "102",
                    RoomName = "Люкс",
                    PricePerNight = 10000,
                    Capacity = 2,
                    Description = "Для самых требовательных гостей",
                    Count = 10
                }
            );

            modelBuilder.Entity<Amenity>().HasData(
                new Amenity
                {
                    Id = 1,
                    Name = "Wi-Fi",
                    Description = "Современный 5G интернет"
                },
                new Amenity
                {
                    Id = 2,
                    Name = "Завтрак",
                    Description = "Личная кухня и первоклассные повара"
                },
                new Amenity
                {
                    Id = 3,
                    Name = "Парковка",
                    Description = "Прекрасная парковка на 2 машины"
                }
            );

            modelBuilder.Entity<RoomAmenity>().HasData(
                new RoomAmenity
                {
                    RoomId = 1, // Комната 101
                    AmenityId = 1 // Wi-Fi
                },
                new RoomAmenity
                {
                    RoomId = 1, // Комната 101
                    AmenityId = 2 // Завтрак
                },
                new RoomAmenity
                {
                    RoomId = 2, // Комната 102
                    AmenityId = 1 // Wi-Fi
                },
                new RoomAmenity
                {
                    RoomId = 2, // Комната 102
                    AmenityId = 3 // Парковка
                }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    UserId = 1, // Иван Иванов
                    RoomId = 1, // Комната 101
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2023, 1, 3), DateTimeKind.Utc),
                    TotalPrice = 10000,
                    Status = "Confirmed"
                },
                new Booking
                {
                    Id = 2,
                    UserId = 2, // Мария Петрова
                    RoomId = 2, // Комната 102
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2023, 1, 10), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2023, 1, 15), DateTimeKind.Utc),
                    TotalPrice = 20000,
                    Status = "Pending"
                }
            );

            modelBuilder.Entity<RoomImages>().HasData(
                new RoomImages
                {
                    Id = 1,
                    ImageUrl = "test1.png",
                    RoomId = 1
                },
                new RoomImages
                {
                    Id = 2,
                    ImageUrl = "test2.png",
                    RoomId = 1
                },
                new RoomImages
                {
                    Id = 3,
                    ImageUrl = "test3.png",
                    RoomId = 2
                },
                new RoomImages
                {
                    Id = 4,
                    ImageUrl = "test4.png",
                    RoomId = 2
                }
            );
        }
    }
}
