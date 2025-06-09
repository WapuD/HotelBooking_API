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
                },
                new Company
                {
                    Id = 3,
                    Name = "Отель Азимут Уфа",
                    Description = "Современный отель в центре Уфы с комфортабельными номерами",
                    Email = "ufa@azimuthotels.com",
                    Phone = "+7 (347) 295-90-00",
                    Website = "https://azimuthotels.com/Russia/ufa",
                    LogoUrl = "https://azimuthotels.com/local/templates/azimuth2016/img/logo_new_azimuth.svg",
                    TaxId = "RU-987654321",
                    LegalAddress = "Проспект Октября, 81, Уфа, Башкортостан, Россия"
                },
                new Company
                {
                    Id = 4,
                    Name = "Crowne Plaza Ufa",
                    Description = "Премиальный отель в Уфе с широким спектром услуг и удобств",
                    Email = "info.ufa@crowneplaza.com",
                    Phone = "+7 (347) 286-50-00",
                    Website = "https://ufa.crowneplaza.com/",
                    LogoUrl = "https://www.ihg.com/content/dam/ihg/logos/brands/cp_rgb.png",
                    TaxId = "RU-112233445",
                    LegalAddress = "ул. Цюрупы, 7, Уфа, Башкортостан, Россия"
                }
            );

            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    Id = 1,
                    UserId = 1,
                    HotelId = 1,
                    Rating = 5,
                    Text = "Отличный номер, очень уютно и чисто!",
                    CreatedDate = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 2,
                    UserId = 2,
                    HotelId = 1,
                    Rating = 4,
                    Text = "Прекрасный вид из окна, но немного шумно ночью.",
                    CreatedDate = new DateTime(2025, 4, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 3,
                    UserId = 3,
                    HotelId = 1,
                    Rating = 5,
                    Text = "Персонал очень вежливый, номер полностью соответствует описанию.",
                    CreatedDate = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 4,
                    UserId = 4,
                    HotelId = 1,
                    Rating = 3,
                    Text = "Неплохой номер, но хотелось бы обновить мебель.",
                    CreatedDate = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 5,
                    UserId = 5,
                    HotelId = 1,
                    Rating = 4,
                    Text = "Хорошее соотношение цены и качества.",
                    CreatedDate = new DateTime(2025, 4, 20, 0, 0, 0, DateTimeKind.Utc)
                },

                new Comment
                {
                    Id = 6,
                    UserId = 1,
                    HotelId = 2,
                    Rating = 5,
                    Text = "Очень комфортный номер, рекомендую для деловых поездок.",
                    CreatedDate = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 7,
                    UserId = 2,
                    HotelId = 2,
                    Rating = 4,
                    Text = "Хорошее расположение и чистота.",
                    CreatedDate = new DateTime(2025, 4, 8, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 8,
                    UserId = 3,
                    HotelId = 2,
                    Rating = 3,
                    Text = "Номер небольшой, но уютный.",
                    CreatedDate = new DateTime(2025, 4, 12, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 9,
                    UserId = 4,
                    HotelId = 2,
                    Rating = 5,
                    Text = "Отличный сервис и удобства.",
                    CreatedDate = new DateTime(2025, 4, 18, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 10,
                    UserId = 5,
                    HotelId = 2,
                    Rating = 4,
                    Text = "Приятный номер, но хотелось бы больше освещения.",
                    CreatedDate = new DateTime(2025, 4, 22, 0, 0, 0, DateTimeKind.Utc)
                },

                new Comment
                {
                    Id = 11,
                    UserId = 1,
                    HotelId = 3,
                    Rating = 4,
                    Text = "Хороший номер, всё необходимое под рукой.",
                    CreatedDate = new DateTime(2025, 4, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 12,
                    UserId = 2,
                    HotelId = 4,
                    Rating = 3,
                    Text = "Номер простой, но чистый.",
                    CreatedDate = new DateTime(2025, 4, 7, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 13,
                    UserId = 3,
                    HotelId = 5,
                    Rating = 5,
                    Text = "Очень понравился дизайн номера и удобства.",
                    CreatedDate = new DateTime(2025, 4, 10, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 14,
                    UserId = 4,
                    HotelId = 6,
                    Rating = 4,
                    Text = "Хороший номер, но немного шумно с улицы.",
                    CreatedDate = new DateTime(2025, 4, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 15,
                    UserId = 5,
                    HotelId = 7,
                    Rating = 5,
                    Text = "Отличное соотношение цена-качество.",
                    CreatedDate = new DateTime(2025, 4, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id = 16,
                    UserId = 1,
                    HotelId = 8,
                    Rating = 4,
                    Text = "Уютный номер, рекомендую.",
                    CreatedDate = new DateTime(2025, 4, 22, 0, 0, 0, DateTimeKind.Utc)
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
                    CompanyId = null,
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
                    CompanyId = null,
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
                    CompanyId = null,
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
                    CompanyId = null,
                    PasswordHash = Encoding.UTF8.GetBytes("asdasd")
                }
            );

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Grand Royal Hotel",
                    CompanyId = 2,
                    Description = @"Grand Royal Hotel - это идеальное место для тех, кто ценит комфорт и высокий уровень сервиса. Расположенный в самом сердце 
                                    Москвы, отель предлагает просторные номера с современным дизайном и всеми необходимыми удобствами. Гости могут насладиться 
                                    панорамным видом на город из ресторанов на крыше и расслабиться в спа-центре с бассейном и сауной. Для деловых 
                                    путешественников доступны конференц-залы с современным оборудованием. Отель также предлагает фитнес-зал, круглосуточную 
                                    службу консьержа и бесплатный Wi-Fi по всей территории. Рядом находятся главные достопримечательности Москвы, включая 
                                    Красную площадь и Большой театр, что делает Grand Royal отличным выбором как для туристов, так и для бизнесменов.",
                    Address = "ул. Тверская, 15",
                    City = "Москва",
                    ImageUrl = "https://cf.bstatic.com/xdata/images/hotel/max1024x768/483452095.jpg?k=6bcbc9f9509821add5c51d951a4e6d837eab8d326154142bedb0f20fb4a5c333&o=",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Comfort Inn Ufa",
                    CompanyId = 2,
                    Description = @"Comfort Inn Ufa - уютный и доступный отель, расположенный в живописном районе Уфы. Отель предлагает чистые и светлые номера
                                    с необходимым набором удобств для комфортного проживания. Завтрак включён в стоимость и подаётся в просторном зале с 
                                    панорамными окнами. Гости могут воспользоваться бесплатной парковкой и круглосуточной рецепцией. Рядом с отелем находится 
                                    несколько кафе и магазинов, а до центра города легко добраться на общественном транспорте. Comfort Inn Ufa - отличный 
                                    выбор для тех, кто ищет спокойствие и удобство по разумной цене.",
                    Address = "Уфа, ул. Ленина, 45",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/9e/51/9e51944fa5956df322cc10fce8156e0bcd940280.jpeg",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Малый отель на Черниковской",
                    CompanyId = 2,
                    Description = @"Общая кухня оборудована для самостоятельного приготовления пищи. На территории работает бесплатный Wi-Fi. 
                                    Уточняйте информацию сразу при заезде. Специально для автопутешественников организована бесплатная парковка. 
                                    Дополнительно: гладильные услуги. Персонал отеля говорит на русском.",
                    Address = "Черниковская улица, д.51",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/ef/a2/efa2fd8ad78697669e4dc12d2d47f052b377f6b8.jpeg",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 4,
                    Name = "AZIMUT Сити Отель Уфа",
                    CompanyId = 3,
                    Description = @"В ресторане отеля сервируется завтрак в формате «шведский стол». В течение дня гости могут заказать различные блюда и напитки 
                                    по меню a la carte, также доступна услуга обслуживания номеров. В лобби-баре, расположенном на 1 этаже отеля гости 
                                    могут приобрести лёгкие закуски, снеки и напитки. Лобби-бар работает круглосуточно.Персонал круглосуточной стойки 
                                    регистрации поможет с заказом такси и трансфером.На всей территории отеля доступен бесплатный Wi-Fi.К услугам гостей 
                                    бесплатная парковка и услуги прачечной за дополнительную плату.",
                    Address = "пр-т Октября, д. 81",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/11/d0/11d0ed32b250489fd79a224237bf2662c3d7cf3c.JPEG",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 5,
                    Name = "Отель Уфа-Астория",
                    CompanyId = 3,
                    Description = @"Скоротать вечер или приятно провести время перед сном в уютной атмосфере можно в баре. Для гостей работает ресторан. Попробуйте кофе в кафе — вдруг именно он станет лучшим в городе? На территории работает бесплатный Wi-Fi. Уточняйте информацию сразу при заезде.
                                    Специально для автопутешественников организована парковка. Любителям спорта подготовили фитнес-центр и тренажёрный зал. Для участников деловых встреч предусмотрен бизнес-центр. Если планируете экскурсии, обратите внимание на экскурсионное бюро отеля.
                                    Сотрудники отеля по запросу организуют гостям трансфер. Удобно для гостей с ограниченными возможностями: на верхние этажи гостей поднимает лифт. Гостям доступны и другие услуги. Например, прачечная, химчистка, банк, банкомат, индивидуальная регистрация заезда и отъезда, гладильные услуги, пресса, сейф и консьерж. Сотрудники отеля поддержат беседу на английском и русском.",
                    Address = "Карла Маркса, д.25",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/20/03/2003356e1fefe82372bff783f6d2d550b12b9ccd.jpeg",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 6,
                    Name = "Отель Маракеш",
                    CompanyId = 3,
                    Description = @"Место, куда приятно возвращаться после долгих прогулок. Гостевой дом «Маракеш» располагается в Уфе. Этот гостевой дом находится в 5 км от центра города. Рядом с гостевым домом можно прогуляться. Неподалёку: Парк им. Ивана Якутова, Памятник Салавату Юлаеву и Национальный музей Башкортостана.",
                    Address = "Камышлинская 61д",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/ee/b0/eeb0e2ad0ad14a84a82a3b586d6e78484c1f18ce.jpeg",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 7,
                    Name = "Отель SheratonPlaza Ufa Congress Hotel",
                    CompanyId = 4,
                    Description = @"Скоротать вечер или приятно провести время перед сном в уютной атмосфере можно в баре. Время вспомнить о хлебе насущном! Для гостей работает ресторан. На территории работает бесплатный Wi-Fi. Уточняйте информацию сразу при заезде. Для путешественников на машине организована платная парковка.
                                    Гостям также доступны следующие услуги: сауна, спа-центр и баня. Спортивные гости оценят фитнес-центр и тренажёрный зал. Для участников деловых встреч предусмотрен бизнес-центр. Доступная среда: работает лифт.
                                    Дополнительно: прачечная, банкомат, гладильные услуги, пресса и консьерж.",
                    Address = "Цюрупы, д.7",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/ca/1f/ca1fa04c7aa3ced83a8dd7fab0517845867c8706.JPEG",
                    Rating = 0
                },
                new Hotel
                {
                    Id = 8,
                    Name = "Мини-Отель Европа",
                    CompanyId = 4,
                    Description = @"Меняем стандартную обстановку отеля на домашний уют! Мини-отель «Мини-Отель Европа» находится в Уфе. Этот мини-отель располагается в 15 км от центра города. Рядом с мини-отелем — Парк Первомайский, Парк Кашкадан и Парк Победы.",
                    Address = "Белоозёрская, д.74",
                    City = "Уфа",
                    ImageUrl = "https://cdn.worldota.net/t/640x400/extranet/f6/d3/f6d3dc84d101242660b0667f63fe00edcac73461.jpeg",
                    Rating = 0
                }
            );

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    HotelId = 1,
                    RoomName = "Двухместный Полулюкс «Комфорт»",
                    PricePerNight = 4800,
                    Capacity = 2,
                    Description = "Превосходный выбор для двоих: здесь есть всё для отдыха и работы.",
                    Count = 3
                },
                new Room
                {
                    Id = 2,
                    HotelId = 1,
                    RoomName = "Одноместный Стандарт «Эконом»",
                    PricePerNight = 3200,
                    Capacity = 1,
                    Description = "Уютное гнёздышко для тех, кто ценит функциональность и спокойствие.",
                    Count = 2
                },
                new Room
                {
                    Id = 3,
                    HotelId = 1,
                    RoomName = "Семейный Люкс «Гармония»",
                    PricePerNight = 7500,
                    Capacity = 4,
                    Description = "Идеальное решение для семьи: много места и приятные детали для каждого.",
                    Count = 1
                },
                new Room
                {
                    Id = 4,
                    HotelId = 1,
                    RoomName = "Двухместный Стандарт «Классика»",
                    PricePerNight = 4000,
                    Capacity = 2,
                    Description = "Надёжный выбор для тех, кто предпочитает проверенные решения и комфорт.",
                    Count = 4
                },
                new Room
                {
                    Id = 5,
                    HotelId = 1,
                    RoomName = "Люкс с видом на море «Романтика»",
                    PricePerNight = 9000,
                    Capacity = 2,
                    Description = "Погрузитесь в атмосферу роскоши и забудьте обо всём, глядя на бескрайние волны.",
                    Count = 1
                },
                new Room
                {
                    Id = 6,
                    HotelId = 1,
                    RoomName = "Стандарт с балконом «Свежесть»",
                    PricePerNight = 4500,
                    Capacity = 2,
                    Description = "Насладитесь утренним кофе на свежем воздухе, не покидая номер.",
                    Count = 2
                },
                new Room
                {
                    Id = 7,
                    HotelId = 2,
                    RoomName = "Студия «Арт»",
                    PricePerNight = 3500,
                    Capacity = 1,
                    Description = "Творческое пространство для вдохновения и отдыха.",
                    Count = 2
                },
                new Room
                {
                    Id = 8,
                    HotelId = 3,
                    RoomName = "Двухместный Люкс «Премиум»",
                    PricePerNight = 6000,
                    Capacity = 2,
                    Description = "Элегантный люкс для самых взыскательных гостей.",
                    Count = 1
                },
                new Room
                {
                    Id = 9,
                    HotelId = 4,
                    RoomName = "Семейный номер «XL»",
                    PricePerNight = 7000,
                    Capacity = 4,
                    Description = "Много места для всей семьи — комфорт гарантирован.",
                    Count = 1
                },
                new Room
                {
                    Id = 10,
                    HotelId = 5,
                    RoomName = "Стандартный «Бизнес»",
                    PricePerNight = 4000,
                    Capacity = 2,
                    Description = "Всё необходимое для продуктивной работы и отдыха.",
                    Count = 1
                },
                new Room
                {
                    Id = 11,
                    HotelId = 6,
                    RoomName = "Люкс с джакузи «SPA»",
                    PricePerNight = 8500,
                    Capacity = 2,
                    Description = "Погрузитесь в релакс: джакузи и премиальный сервис.",
                    Count = 1
                },
                new Room
                {
                    Id = 12,
                    HotelId = 7,
                    RoomName = "Одноместный «Мини»",
                    PricePerNight = 3000,
                    Capacity = 1,
                    Description = "Компактный номер для тех, кто ценит мобильность.",
                    Count = 1
                },
                new Room
                {
                    Id = 13,
                    HotelId = 8,
                    RoomName = "Двухместный «Комфорт+»",
                    PricePerNight = 4200,
                    Capacity = 2,
                    Description = "Больше пространства и удобств для вашего отдыха.",
                    Count = 1
                }
            );

            modelBuilder.Entity<Amenity>().HasData(
                new Amenity { Id = 1, Name = "Бесплатный интернет" },
                new Amenity { Id = 2, Name = "Парковка" },
                new Amenity { Id = 3, Name = "Подходит для детей" },
                new Amenity { Id = 4, Name = "Кондиционер" },
                new Amenity { Id = 5, Name = "Разрешено с домашними животными" },
                new Amenity { Id = 6, Name = "Круглосуточная стойка регистрации" },
                new Amenity { Id = 7, Name = "Места для курения" },
                new Amenity { Id = 8, Name = "Отопление" },
                new Amenity { Id = 9, Name = "Стиральная машина" },
                new Amenity { Id = 10, Name = "Терраса" },
                new Amenity { Id = 11, Name = "Ранняя регистрация заезда" },
                new Amenity { Id = 12, Name = "Поздняя регистрация выезда" },
                new Amenity { Id = 13, Name = "Индивидуальная регистрация заезда и отъезда" },
                new Amenity { Id = 14, Name = "Люкс для новобрачных" },
                new Amenity { Id = 15, Name = "Номера для некурящих" },
                new Amenity { Id = 16, Name = "Холодильник" },
                new Amenity { Id = 17, Name = "Семейные номера" },
                new Amenity { Id = 18, Name = "Кабельное телевидение" },
                new Amenity { Id = 19, Name = "Гладильные принадлежности" },
                new Amenity { Id = 20, Name = "Фен (по запросу)" },
                new Amenity { Id = 21, Name = "Кухня" },
                new Amenity { Id = 22, Name = "Микроволновая печь" },
                new Amenity { Id = 23, Name = "Бесплатный Wi-Fi" },
                new Amenity { Id = 24, Name = "Бесплатная парковка" },
                new Amenity { Id = 25, Name = "Размещение подходит для семей/детей" },
                new Amenity { Id = 26, Name = "Размещение с домашними животными" },
                new Amenity { Id = 27, Name = "Оплачивается отдельно" },
                new Amenity { Id = 28, Name = "Температурный контроль для персонала" },
                new Amenity { Id = 29, Name = "Индивидуальные средства защиты для персонала" },
                new Amenity { Id = 30, Name = "Усиленные меры дезинфекции" },
                new Amenity { Id = 31, Name = "Температурный контроль для гостей" }
            );

            modelBuilder.Entity<RoomAmenity>().HasData(
                new RoomAmenity { RoomId = 1, AmenityId = 1 },
                new RoomAmenity { RoomId = 1, AmenityId = 4 },
                new RoomAmenity { RoomId = 1, AmenityId = 15 },
                new RoomAmenity { RoomId = 1, AmenityId = 23 },
                new RoomAmenity { RoomId = 1, AmenityId = 10 },

                new RoomAmenity { RoomId = 2, AmenityId = 1 },
                new RoomAmenity { RoomId = 2, AmenityId = 8 },
                new RoomAmenity { RoomId = 2, AmenityId = 9 },

                new RoomAmenity { RoomId = 3, AmenityId = 25 },
                new RoomAmenity { RoomId = 3, AmenityId = 21 },
                new RoomAmenity { RoomId = 3, AmenityId = 9 },
                new RoomAmenity { RoomId = 3, AmenityId = 12 },
                new RoomAmenity { RoomId = 3, AmenityId = 27 },
                new RoomAmenity { RoomId = 3, AmenityId = 30 },

                new RoomAmenity { RoomId = 4, AmenityId = 1 },
                new RoomAmenity { RoomId = 4, AmenityId = 2 },
                new RoomAmenity { RoomId = 4, AmenityId = 16 },
                new RoomAmenity { RoomId = 4, AmenityId = 7 },

                new RoomAmenity { RoomId = 5, AmenityId = 5 },
                new RoomAmenity { RoomId = 5, AmenityId = 14 },
                new RoomAmenity { RoomId = 5, AmenityId = 30 },
                new RoomAmenity { RoomId = 5, AmenityId = 28 },
                new RoomAmenity { RoomId = 5, AmenityId = 29 },

                new RoomAmenity { RoomId = 6, AmenityId = 10 },
                new RoomAmenity { RoomId = 6, AmenityId = 11 },
                new RoomAmenity { RoomId = 6, AmenityId = 12 },
                new RoomAmenity { RoomId = 6, AmenityId = 13 },
                new RoomAmenity { RoomId = 6, AmenityId = 26 },

                new RoomAmenity { RoomId = 7, AmenityId = 1 },
                new RoomAmenity { RoomId = 7, AmenityId = 7 },

                new RoomAmenity { RoomId = 8, AmenityId = 4 },
                new RoomAmenity { RoomId = 8, AmenityId = 23 },
                new RoomAmenity { RoomId = 8, AmenityId = 20 },

                new RoomAmenity { RoomId = 9, AmenityId = 25 },
                new RoomAmenity { RoomId = 9, AmenityId = 21 },

                new RoomAmenity { RoomId = 10, AmenityId = 8 },
                new RoomAmenity { RoomId = 10, AmenityId = 16 },

                new RoomAmenity { RoomId = 11, AmenityId = 20 },
                new RoomAmenity { RoomId = 11, AmenityId = 30 },

                new RoomAmenity { RoomId = 12, AmenityId = 1 },
                new RoomAmenity { RoomId = 12, AmenityId = 27 },

                new RoomAmenity { RoomId = 13, AmenityId = 4 },
                new RoomAmenity { RoomId = 13, AmenityId = 24 }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    UserId = 1,
                    RoomId = 1,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 1, 3), DateTimeKind.Utc),
                    TotalPrice = 10000,
                    Status = "Подтверждено"
                },
                new Booking
                {
                    Id = 2,
                    UserId = 2,
                    RoomId = 2,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 1, 10), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 1, 15), DateTimeKind.Utc),
                    TotalPrice = 20000,
                    Status = "Отменено"
                },
                new Booking
                {
                    Id = 3,
                    UserId = 3,
                    RoomId = 3,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 5, 20), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 5, 25), DateTimeKind.Utc),
                    TotalPrice = 30000,
                    Status = "Завершено"
                },
                new Booking
                {
                    Id = 4,
                    UserId = 4,
                    RoomId = 4,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 6, 5), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 6, 10), DateTimeKind.Utc),
                    TotalPrice = 25000,
                    Status = "Активно"
                },
                new Booking
                {
                    Id = 5,
                    UserId = 5,
                    RoomId = 5,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 6, 15), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 6, 20), DateTimeKind.Utc),
                    TotalPrice = 28000,
                    Status = "Ожидание"
                },
                new Booking
                {
                    Id = 6,
                    UserId = 1,
                    RoomId = 6,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 4, 10), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 4, 15), DateTimeKind.Utc),
                    TotalPrice = 22000,
                    Status = "Завершено"
                },
                new Booking
                {
                    Id = 7,
                    UserId = 2,
                    RoomId = 7,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 6, 8), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 6, 12), DateTimeKind.Utc),
                    TotalPrice = 18000,
                    Status = "Активно"
                },
                new Booking
                {
                    Id = 8,
                    UserId = 3,
                    RoomId = 8,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 7, 1), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 7, 5), DateTimeKind.Utc),
                    TotalPrice = 21000,
                    Status = "Ожидание"
                },
                new Booking
                {
                    Id = 9,
                    UserId = 4,
                    RoomId = 9,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 3, 20), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 3, 25), DateTimeKind.Utc),
                    TotalPrice = 17000,
                    Status = "Отменено"
                },
                new Booking
                {
                    Id = 10,
                    UserId = 5,
                    RoomId = 10,
                    CheckInDate = DateTime.SpecifyKind(new DateTime(2025, 6, 9), DateTimeKind.Utc),
                    CheckOutDate = DateTime.SpecifyKind(new DateTime(2025, 6, 14), DateTimeKind.Utc),
                    TotalPrice = 24000,
                    Status = "Активно"
                }
            );

            modelBuilder.Entity<RoomImages>().HasData(
                new RoomImages { Id = 1, ImageUrl = "RoomImg/test1.png", RoomId = 1 },
                new RoomImages { Id = 2, ImageUrl = "RoomImg/test2.png", RoomId = 2 },
                new RoomImages { Id = 3, ImageUrl = "RoomImg/test3.png", RoomId = 3 },
                new RoomImages { Id = 4, ImageUrl = "RoomImg/test4.png", RoomId = 4 },
                new RoomImages { Id = 5, ImageUrl = "RoomImg/test5.png", RoomId = 5 },
                new RoomImages { Id = 6, ImageUrl = "RoomImg/test6.png", RoomId = 6 },
                new RoomImages { Id = 7, ImageUrl = "RoomImg/test7.png", RoomId = 7 },
                new RoomImages { Id = 8, ImageUrl = "RoomImg/test8.png", RoomId = 8 },
                new RoomImages { Id = 9, ImageUrl = "RoomImg/test9.png", RoomId = 9 },
                new RoomImages { Id = 10, ImageUrl = "RoomImg/test10.png", RoomId = 10 },
                new RoomImages { Id = 11, ImageUrl = "RoomImg/test11.png", RoomId = 11 },
                new RoomImages { Id = 12, ImageUrl = "RoomImg/test12.png", RoomId = 12 },
                new RoomImages { Id = 13, ImageUrl = "RoomImg/test13.png", RoomId = 13 }
            );
        }
    }
}
