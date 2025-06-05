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
                    Name = "Hotel Booking",
                    Description = "Сервис для бронирования отелей",
                    Email = "HotelBooking@mail.ru",
                    Phone = "+79645873664",
                    Website = "https://www.HotelBooking.ru",
                    LogoUrl = "https://www.HotelBooking/Logo.svg.png",
                    TaxId = "US-517351059",
                    LegalAddress = "Тверская ул., 6с, Москва, 125009, Россия"
                },
                new Company
                {
                    Id = 2,
                    Name = "Hilton Worldwide",
                    Description = "Международная сеть отелей класса люкс, основанная в 1919 году",
                    Email = "corporate@hilton.com",
                    Phone = "+1 800 445 8667",
                    Website = "https://www.hilton.com",
                    LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Hilton_Logo_2019.svg/1200px-Hilton_Logo_2019.svg.png",
                    TaxId = "US-123456789",
                    LegalAddress = "7930 Jones Branch Dr, McLean, VA 22102, США"
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
                    UserId = 3,
                    HotelId = 1,
                    Rating = 3,
                    Text = "Всё хорошо, но дорого.",
                    CreatedDate = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 4,
                    UserId = 1,
                    HotelId = 1,
                    Rating = 5,
                    Text = "Номер так и просит ремонта",
                    CreatedDate = new DateTime(2025, 4, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 5,
                    UserId = 1,
                    HotelId = 1,
                    Rating = 1,
                    Text = "Достаточно просто, но и цена соответствующая",
                    CreatedDate = new DateTime(2025, 4, 20, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Сергей",
                    SecondName = "Иванов",
                    LastName = "Александрович",
                    Email = "Sergei@mail.ru",
                    Phone = "+79172227890",
                    CompanyId = null,
                    PasswordHash = Encoding.UTF8.GetBytes("asdasd")
                },
                new User
                {
                    Id = 2,
                    FirstName = "Смирнова",
                    SecondName = "Анна",
                    LastName = "Сергеевна",
                    Email = "Ann@mail.ru",
                    Phone = "+79177222780",
                    CompanyId = 1,
                    PasswordHash = Encoding.UTF8.GetBytes("asdasd")
                },
                new User
                {
                    Id = 3,
                    FirstName = "Иван",
                    SecondName = "Иванов",
                    LastName = "Иванович",
                    Email = "ivan@mail.ru",
                    Phone = "+71234567890",
                    CompanyId = 2,
                    PasswordHash = Encoding.UTF8.GetBytes("asdasd")
                },
                new User
                {
                    Id = 4,
                    FirstName = "Мария",
                    SecondName = "Петрова",
                    LastName = "Николаевна",
                    Email = "maria@mail.ru",
                    Phone = "+79876543210",
                    CompanyId = 2,
                    PasswordHash = Encoding.UTF8.GetBytes("asdasd")
                },
                new User
                {
                    Id = 5,
                    FirstName = "Андрей",
                    SecondName = "Михайлов",
                    LastName = "Евгеньевич",
                    Email = "Miha@gmail.com",
                    Phone = "+79876543210",
                    CompanyId = 2,
                    PasswordHash = Encoding.UTF8.GetBytes("asdasd")
                }
            );

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Grand Royal Hotel",
                    CompanyId = 1,
                    Description = @"Grand Royal Hotel - это идеальное место для тех, кто ценит комфорт и высокий уровень сервиса. Расположенный в самом сердце Москвы, отель предлагает просторные номера с современным дизайном и всеми необходимыми удобствами. Гости могут насладиться панорамным видом на город из ресторанов на крыше и расслабиться в спа-центре с бассейном и сауной. Для деловых путешественников доступны конференц-залы с современным оборудованием. Отель также предлагает фитнес-зал, круглосуточную службу консьержа и бесплатный Wi-Fi по всей территории. Рядом находятся главные достопримечательности Москвы, включая Красную площадь и Большой театр, что делает Grand Royal отличным выбором как для туристов, так и для бизнесменов.",
                    Address = "ул. Тверская, 15",
                    City = "Москва",
                    ImageUrl = "https://cf.bstatic.com/xdata/images/hotel/max1024x768/483452095.jpg?k=6bcbc9f9509821add5c51d951a4e6d837eab8d326154142bedb0f20fb4a5c333&o=",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Comfort Inn Ufa",
                    CompanyId = 1,
                    Description = @"Comfort Inn Ufa - уютный и доступный отель, расположенный в живописном районе Уфы. Отель предлагает чистые и светлые номера с необходимым набором удобств для комфортного проживания. Завтрак включён в стоимость и подаётся в просторном зале с панорамными окнами. Гости могут воспользоваться бесплатной парковкой и круглосуточной рецепцией. Рядом с отелем находится несколько кафе и магазинов, а до центра города легко добраться на общественном транспорте. Comfort Inn Ufa - отличный выбор для тех, кто ищет спокойствие и удобство по разумной цене.",
                    Address = "Уфа, ул. Ленина, 45",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/9e/51/9e51944fa5956df322cc10fce8156e0bcd940280.jpeg",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Малый отель на Черниковской",
                    CompanyId = 1,
                    Description = @"Общая кухня оборудована для самостоятельного приготовления пищи. На территории работает бесплатный Wi-Fi. Уточняйте информацию сразу при заезде. Специально для автопутешественников организована бесплатная парковка. Дополнительно: гладильные услуги. Персонал отеля говорит на русском.",
                    Address = "Черниковская улица, д.51",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/ef/a2/efa2fd8ad78697669e4dc12d2d47f052b377f6b8.jpeg",
                    Rating = 0
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
                    Name = "Современный 5G интернет (Wi-Fi)"
                },
                new Amenity
                {
                    Id = 2,
                    Name = "Завтрак - Личная кухня и первоклассные повара"
                },
                new Amenity
                {
                    Id = 3,
                    Name = "Прекрасная парковка на 2 машины"
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
