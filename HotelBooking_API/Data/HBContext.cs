using Microsoft.EntityFrameworkCore;
using HotelBooking_API.Data.Models;
using System.Text;

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
        public DbSet<Company> Company { get; set; }
        public DbSet<Comment> Comment { get; set; }

        // Настройка моделей и отношений между сущностями
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Hotel)
                .WithMany(h => h.Comments)
                .HasForeignKey(c => c.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь User и Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Hotel и Room
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Room и RoomAmenity
            modelBuilder.Entity<RoomAmenity>()
                .HasKey(ra => new { ra.RoomId, ra.AmenityId });

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

            // Связь Room и RoomImages
            modelBuilder.Entity<RoomImages>()
                .HasOne(ri => ri.Room)
                .WithMany(r => r.RoomImages)
                .HasForeignKey(ri => ri.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Company и Hotel
            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Company)
                .WithMany(c => c.Hotels)
                .HasForeignKey(h => h.CompanyId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete

            // Индексы
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Hotel>()
                .HasIndex(h => h.Name);

            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomName);


            // Seed data
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "Hilton Worldwide",
                    Description = "Международная сеть отелей класса люкс, основанная в 1919 году",
                    Email = "corporate@hilton.com",
                    Phone = "+1 800 445 8667",
                    Website = "https://www.hilton.com",
                    LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Hilton_Logo_2019.svg/1200px-Hilton_Logo_2019.svg.png",
                    TaxId = "US-123456789",
                    LegalAddress = "7930 Jones Branch Dr, McLean, VA 22102, США"
                },
                new Company
                {
                    Id = 2,
                    Name = "Marriott International",
                    Description = "Крупнейшая гостиничная сеть мира, управляющая более чем 8000 объектами",
                    Email = "info@marriott.com",
                    Phone = "+1 301 380 3000",
                    Website = "https://www.marriott.com",
                    LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f9/Marriott_International_logo_2019.svg/1280px-Marriott_International_logo_2019.svg.png",
                    TaxId = "US-987654321",
                    LegalAddress = "10400 Fernwood Rd, Bethesda, MD 20817, США"
                },
                new Company
                {
                    Id = 3,
                    Name = "Accor Group",
                    Description = "Французская гостиничная группа, управляющая брендами Sofitel, Novotel, Ibis",
                    Email = "contact@accor.com",
                    Phone = "+33 1 45 38 86 00",
                    Website = "https://group.accor.com",
                    LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2e/Accor_logo_2022.svg/1280px-Accor_logo_2022.svg.png",
                    TaxId = "FR-789123456",
                    LegalAddress = "82 rue Henri Farman, 92130 Issy-les-Moulineaux, Франция"
                },
                new Company
                {
                    Id = 4,
                    Name = "Азимут Отели Россия",
                    Description = "Крупнейшая российская гостиничная сеть, основанная в 2010 году",
                    Email = "info@azimuthotels.com",
                    Phone = "+7 495 225 25 25",
                    Website = "https://www.azimuthotels.com",
                    LogoUrl = "https://www.azimuthotels.com/local/templates/azimuth_main/img/logo.svg",
                    TaxId = "RU-1234567890",
                    LegalAddress = "125040, Москва, Ленинградский проспект, 36"
                },
                new Company
                {
                    Id = 5,
                    Name = "Cosmos Hotel Group",
                    Description = "Российская гостиничная управляющая компания",
                    Email = "booking@cosmos-hotel.com",
                    Phone = "+7 495 785 45 45",
                    Website = "https://cosmos-hotel.com",
                    LogoUrl = "https://cosmos-hotel.com/local/templates/cosmos/img/logo.svg",
                    TaxId = "RU-0987654321",
                    LegalAddress = "150040, Ярославль, ул. Комсомольская, 2"
                }
            );


            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    Id = 1,
                    UserId = 1,
                    HotelId = 1,
                    Rating = 5,
                    Text = "Отличный отель! Всем рекомендую.",
                    CreatedDate = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 2,
                    UserId = 2,
                    HotelId = 1,
                    Rating = 4,
                    Text = "Хороший сервис, но дорогой мини-бар.",
                    CreatedDate = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 3,
                    UserId = 1,
                    HotelId = 2,
                    Rating = 3,
                    Text = "Усталый номер, требует ремонта.",
                    CreatedDate = new DateTime(2025, 4, 20, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Иван",
                    SecondName = "Иванов",
                    LastName = "Иванович",
                    Email = "ivan@example.com",
                    Phone = "+7 123 456 7890",
                    CompanyId = 1,
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
                    CompanyId = 2,
                    PasswordHash = new byte[16] // Needs to be correct length
                }
            );

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Отель Премиум",
                    CompanyId = 1,
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
                    CompanyId = 2,
                    Description = "Отель Эконом",
                    Address = "Уфа, ул. Пушкина, 5",
                    City = "Уфа",
                    ImageUrl = "Ekonom.png",
                    Rating = 3.8M
                }
            );

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    HotelId = 1,
                    RoomName = "Стандарт",
                    PricePerNight = 5000,
                    Capacity = 2,
                    Description = "Обычный номер, предоставляющий всё необходимое",
                    Count = 3
                },
                new Room
                {
                    Id = 2,
                    HotelId = 1,
                    RoomName = "Люкс",
                    PricePerNight = 10000,
                    Capacity = 4,
                    Description = "Для самых требовательных гостей",
                    Count = 1
                },
                new Room
                {
                    Id = 3,
                    HotelId = 2,
                    RoomName = "Эконом",
                    PricePerNight = 2000,
                    Capacity = 1,
                    Description = "Выбор для самых экономных граждан",
                    Count = 4
                },
                new Room
                {
                    Id = 4,
                    HotelId = 2,
                    RoomName = "Классика",
                    PricePerNight = 3000,
                    Capacity = 2,
                    Description = "Классический номер, оформленный в стиле сети отелей",
                    Count = 2
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
