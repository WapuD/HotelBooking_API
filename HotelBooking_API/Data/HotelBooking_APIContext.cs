using Microsoft.EntityFrameworkCore;
using HotelBooking_API.Data.Models;
using HotelBooking_API.Data.Other;

namespace HotelBooking_API.Data
{
    public class HotelBooking_APIContext : DbContext
    {
        // Конструктор с параметрами для настройки контекста
        public HotelBooking_APIContext(DbContextOptions<HotelBooking_APIContext> options)
            : base(options)
        {
        }

        // DbSet для каждой сущности
        public DbSet<User> User { get; set; }
        public DbSet<Amenity> Amenity { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<RoomAmenity> RoomAmenity { get; set; }

        // Настройка моделей и отношений между сущностями
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Пример настройки отношений и индексов:

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
                .HasForeignKey(ra => ra.RoomId);

            modelBuilder.Entity<RoomAmenity>()
                .HasOne(ra => ra.Amenity)
                .WithMany(a => a.RoomAmenities)
                .HasForeignKey(ra => ra.AmenityId);

            // Настройка индексов для улучшения производительности
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Уникальный индекс для Email

            modelBuilder.Entity<Hotel>()
                .HasIndex(h => h.Name)
                .IsUnique(); // Уникальный индекс для названия отеля

            // Настройка каскадного удаления для Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking) // У Payment есть одно бронирование
                .WithOne(b => b.Payment) // У Booking есть один Payment
                .HasForeignKey<Payment>(p => p.BookingId) // Внешний ключ в Payment
                .OnDelete(DeleteBehavior.Cascade); // Удаление платежа при удалении бронирования

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Иван",
                    LastName = "Иванов",
                    Email = "ivan@example.com",
                    Phone = "+7 123 456 7890",
                    PasswordHash = new byte[byte.MaxValue],
                },
                new User
                {
                    Id = 2,
                    FirstName = "Мария",
                    LastName = "Петрова",
                    Email = "maria@example.com",
                    Phone = "+7 987 654 3210",
                    PasswordHash = new byte[byte.MinValue],
                }
            );
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Отель Премиум",
                    Address = "Москва, ул. Ленина, 10",
                    City = "Уфа",
                    Rating = 4.5M
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Отель Эконом",
                    Address = "Санкт-Петербург, ул. Пушкина, 5",
                    City = "Уфа",
                    Rating = 3.8M
                }
            );
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    HotelId = 1, // Связь с отелем "Отель Премиум"
                    RoomNumber = "101",
                    RoomType = "Стандарт",
                    PricePerNight = 5000,
                    Capacity = 2,
                    Description = "Обычный номер, предоставляющий всё необходимое",
                    IsAvailable = true
                },
                new Room
                {
                    Id = 2,
                    HotelId = 1, // Связь с отелем "Отель Премиум"
                    RoomNumber = "102",
                    RoomType = "Люкс",
                    PricePerNight = 10000,
                    Capacity = 2,
                    Description = "Для самых требовательных гостей",
                    IsAvailable = true
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
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    Id = 1,
                    BookingId = 1, // Бронирование 1
                    Amount = 10000,
                    PaymentDate = DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc),
                    PaymentMethod = "CreditCard"
                },
                new Payment
                {
                    Id = 2,
                    BookingId = 2, // Бронирование 2
                    Amount = 20000,
                    PaymentDate = DateTime.SpecifyKind(new DateTime(2023, 1, 10), DateTimeKind.Utc),
                    PaymentMethod = "PayPal"
                }
            );
        }
    }
}