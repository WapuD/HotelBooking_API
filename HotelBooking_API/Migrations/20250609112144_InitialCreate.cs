using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBooking_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amenity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TaxId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LegalAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hotel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Rating = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotel_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    RoomName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PricePerNight = table.Column<decimal>(type: "numeric", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Hotel_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    HotelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Hotel_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    CheckInDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CheckOutDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalPrice = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomAmenity",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    AmenityId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAmenity", x => new { x.RoomId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_RoomAmenity_Amenity_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "Amenity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomAmenity_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomImages_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Amenity",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Бесплатный интернет" },
                    { 2, "Парковка" },
                    { 3, "Подходит для детей" },
                    { 4, "Кондиционер" },
                    { 5, "Разрешено с домашними животными" },
                    { 6, "Круглосуточная стойка регистрации" },
                    { 7, "Места для курения" },
                    { 8, "Отопление" },
                    { 9, "Стиральная машина" },
                    { 10, "Терраса" },
                    { 11, "Ранняя регистрация заезда" },
                    { 12, "Поздняя регистрация выезда" },
                    { 13, "Индивидуальная регистрация заезда и отъезда" },
                    { 14, "Люкс для новобрачных" },
                    { 15, "Номера для некурящих" },
                    { 16, "Холодильник" },
                    { 17, "Семейные номера" },
                    { 18, "Кабельное телевидение" },
                    { 19, "Гладильные принадлежности" },
                    { 20, "Фен (по запросу)" },
                    { 21, "Кухня" },
                    { 22, "Микроволновая печь" },
                    { 23, "Бесплатный Wi-Fi" },
                    { 24, "Бесплатная парковка" },
                    { 25, "Размещение подходит для семей/детей" },
                    { 26, "Размещение с домашними животными" },
                    { 27, "Оплачивается отдельно" },
                    { 28, "Температурный контроль для персонала" },
                    { 29, "Индивидуальные средства защиты для персонала" },
                    { 30, "Усиленные меры дезинфекции" },
                    { 31, "Температурный контроль для гостей" }
                });

            migrationBuilder.InsertData(
                table: "Company",
                columns: new[] { "Id", "Description", "Email", "LegalAddress", "LogoUrl", "Name", "Phone", "TaxId", "Website" },
                values: new object[,]
                {
                    { 1, "Сервис для бронирования отелей", "HotelBooking@mail.ru", "Тверская ул., 6с, Москва, 125009, Россия", "https://www.HotelBooking/Logo.svg.png", "Hotel Booking", "+79645873664", "US-517351059", "https://www.HotelBooking.ru" },
                    { 2, "Международная сеть отелей класса люкс, основанная в 1919 году", "corporate@hilton.com", "7930 Jones Branch Dr, McLean, VA 22102, США", "https://upload.wikimedia.org/wikipedia/commons/thumb/2/24/Hilton_Logo_2019.svg/1200px-Hilton_Logo_2019.svg.png", "Hilton Worldwide", "+1 800 445 8667", "US-123456789", "https://www.hilton.com" },
                    { 3, "Современный отель в центре Уфы с комфортабельными номерами", "ufa@azimuthotels.com", "Проспект Октября, 81, Уфа, Башкортостан, Россия", "https://azimuthotels.com/local/templates/azimuth2016/img/logo_new_azimuth.svg", "Отель Азимут Уфа", "+7 (347) 295-90-00", "RU-987654321", "https://azimuthotels.com/Russia/ufa" },
                    { 4, "Премиальный отель в Уфе с широким спектром услуг и удобств", "info.ufa@crowneplaza.com", "ул. Цюрупы, 7, Уфа, Башкортостан, Россия", "https://www.ihg.com/content/dam/ihg/logos/brands/cp_rgb.png", "Crowne Plaza Ufa", "+7 (347) 286-50-00", "RU-112233445", "https://ufa.crowneplaza.com/" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CompanyId", "Email", "FirstName", "LastName", "PasswordHash", "Phone", "SecondName" },
                values: new object[,]
                {
                    { 1, null, "Sergei@mail.ru", "Сергей", "Александрович", new byte[] { 97, 115, 100, 97, 115, 100 }, "+79172227890", "Иванов" },
                    { 2, null, "Ann@mail.ru", "Смирнова", "Сергеевна", new byte[] { 97, 115, 100, 97, 115, 100 }, "+79177222780", "Анна" },
                    { 3, null, "ivan@mail.ru", "Иван", "Иванович", new byte[] { 97, 115, 100, 97, 115, 100 }, "+71234567890", "Иванов" },
                    { 4, null, "maria@mail.ru", "Мария", "Николаевна", new byte[] { 97, 115, 100, 97, 115, 100 }, "+79876543210", "Петрова" },
                    { 5, null, "Miha@gmail.com", "Андрей", "Евгеньевич", new byte[] { 97, 115, 100, 97, 115, 100 }, "+79876543210", "Михайлов" }
                });

            migrationBuilder.InsertData(
                table: "Hotel",
                columns: new[] { "Id", "Address", "City", "CompanyId", "Description", "ImageUrl", "Name", "Rating" },
                values: new object[,]
                {
                    { 1, "ул. Тверская, 15", "Москва", 2, "Grand Royal Hotel - это идеальное место для тех, кто ценит комфорт и высокий уровень сервиса. Расположенный в самом сердце \r\n                                    Москвы, отель предлагает просторные номера с современным дизайном и всеми необходимыми удобствами. Гости могут насладиться \r\n                                    панорамным видом на город из ресторанов на крыше и расслабиться в спа-центре с бассейном и сауной. Для деловых \r\n                                    путешественников доступны конференц-залы с современным оборудованием. Отель также предлагает фитнес-зал, круглосуточную \r\n                                    службу консьержа и бесплатный Wi-Fi по всей территории. Рядом находятся главные достопримечательности Москвы, включая \r\n                                    Красную площадь и Большой театр, что делает Grand Royal отличным выбором как для туристов, так и для бизнесменов.", "https://cf.bstatic.com/xdata/images/hotel/max1024x768/483452095.jpg?k=6bcbc9f9509821add5c51d951a4e6d837eab8d326154142bedb0f20fb4a5c333&o=", "Grand Royal Hotel", 0m },
                    { 2, "Уфа, ул. Ленина, 45", "Уфа", 2, "Comfort Inn Ufa - уютный и доступный отель, расположенный в живописном районе Уфы. Отель предлагает чистые и светлые номера\r\n                                    с необходимым набором удобств для комфортного проживания. Завтрак включён в стоимость и подаётся в просторном зале с \r\n                                    панорамными окнами. Гости могут воспользоваться бесплатной парковкой и круглосуточной рецепцией. Рядом с отелем находится \r\n                                    несколько кафе и магазинов, а до центра города легко добраться на общественном транспорте. Comfort Inn Ufa - отличный \r\n                                    выбор для тех, кто ищет спокойствие и удобство по разумной цене.", "https://cdn.worldota.net/t/640x400/extranet/9e/51/9e51944fa5956df322cc10fce8156e0bcd940280.jpeg", "Comfort Inn Ufa", 0m },
                    { 3, "Черниковская улица, д.51", "Уфа", 2, "Общая кухня оборудована для самостоятельного приготовления пищи. На территории работает бесплатный Wi-Fi. \r\n                                    Уточняйте информацию сразу при заезде. Специально для автопутешественников организована бесплатная парковка. \r\n                                    Дополнительно: гладильные услуги. Персонал отеля говорит на русском.", "https://cdn.worldota.net/t/640x400/extranet/ef/a2/efa2fd8ad78697669e4dc12d2d47f052b377f6b8.jpeg", "Малый отель на Черниковской", 0m },
                    { 4, "пр-т Октября, д. 81", "Уфа", 3, "В ресторане отеля сервируется завтрак в формате «шведский стол». В течение дня гости могут заказать различные блюда и напитки \r\n                                    по меню a la carte, также доступна услуга обслуживания номеров. В лобби-баре, расположенном на 1 этаже отеля гости \r\n                                    могут приобрести лёгкие закуски, снеки и напитки. Лобби-бар работает круглосуточно.Персонал круглосуточной стойки \r\n                                    регистрации поможет с заказом такси и трансфером.На всей территории отеля доступен бесплатный Wi-Fi.К услугам гостей \r\n                                    бесплатная парковка и услуги прачечной за дополнительную плату.", "https://cdn.worldota.net/t/640x400/extranet/11/d0/11d0ed32b250489fd79a224237bf2662c3d7cf3c.JPEG", "AZIMUT Сити Отель Уфа", 0m },
                    { 5, "Карла Маркса, д.25", "Уфа", 3, "Скоротать вечер или приятно провести время перед сном в уютной атмосфере можно в баре. Для гостей работает ресторан. Попробуйте кофе в кафе — вдруг именно он станет лучшим в городе? На территории работает бесплатный Wi-Fi. Уточняйте информацию сразу при заезде.\r\n                                    Специально для автопутешественников организована парковка. Любителям спорта подготовили фитнес-центр и тренажёрный зал. Для участников деловых встреч предусмотрен бизнес-центр. Если планируете экскурсии, обратите внимание на экскурсионное бюро отеля.\r\n                                    Сотрудники отеля по запросу организуют гостям трансфер. Удобно для гостей с ограниченными возможностями: на верхние этажи гостей поднимает лифт. Гостям доступны и другие услуги. Например, прачечная, химчистка, банк, банкомат, индивидуальная регистрация заезда и отъезда, гладильные услуги, пресса, сейф и консьерж. Сотрудники отеля поддержат беседу на английском и русском.", "https://cdn.worldota.net/t/640x400/extranet/20/03/2003356e1fefe82372bff783f6d2d550b12b9ccd.jpeg", "Отель Уфа-Астория", 0m },
                    { 6, "Камышлинская 61д", "Уфа", 3, "Место, куда приятно возвращаться после долгих прогулок. Гостевой дом «Маракеш» располагается в Уфе. Этот гостевой дом находится в 5 км от центра города. Рядом с гостевым домом можно прогуляться. Неподалёку: Парк им. Ивана Якутова, Памятник Салавату Юлаеву и Национальный музей Башкортостана.", "https://cdn.worldota.net/t/640x400/extranet/ee/b0/eeb0e2ad0ad14a84a82a3b586d6e78484c1f18ce.jpeg", "Отель Маракеш", 0m },
                    { 7, "Цюрупы, д.7", "Уфа", 4, "Скоротать вечер или приятно провести время перед сном в уютной атмосфере можно в баре. Время вспомнить о хлебе насущном! Для гостей работает ресторан. На территории работает бесплатный Wi-Fi. Уточняйте информацию сразу при заезде. Для путешественников на машине организована платная парковка.\r\n                                    Гостям также доступны следующие услуги: сауна, спа-центр и баня. Спортивные гости оценят фитнес-центр и тренажёрный зал. Для участников деловых встреч предусмотрен бизнес-центр. Доступная среда: работает лифт.\r\n                                    Дополнительно: прачечная, банкомат, гладильные услуги, пресса и консьерж.", "https://cdn.worldota.net/t/640x400/extranet/ca/1f/ca1fa04c7aa3ced83a8dd7fab0517845867c8706.JPEG", "Отель SheratonPlaza Ufa Congress Hotel", 0m },
                    { 8, "Белоозёрская, д.74", "Уфа", 4, "Меняем стандартную обстановку отеля на домашний уют! Мини-отель «Мини-Отель Европа» находится в Уфе. Этот мини-отель располагается в 15 км от центра города. Рядом с мини-отелем — Парк Первомайский, Парк Кашкадан и Парк Победы.", "https://cdn.worldota.net/t/640x400/extranet/f6/d3/f6d3dc84d101242660b0667f63fe00edcac73461.jpeg", "Мини-Отель Европа", 0m }
                });

            migrationBuilder.InsertData(
                table: "Comment",
                columns: new[] { "Id", "CreatedDate", "HotelId", "Rating", "Text", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 5, "Отличный номер, очень уютно и чисто!", 1 },
                    { 2, new DateTimeOffset(new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 4, "Прекрасный вид из окна, но немного шумно ночью.", 2 },
                    { 3, new DateTimeOffset(new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 5, "Персонал очень вежливый, номер полностью соответствует описанию.", 3 },
                    { 4, new DateTimeOffset(new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 3, "Неплохой номер, но хотелось бы обновить мебель.", 4 },
                    { 5, new DateTimeOffset(new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 4, "Хорошее соотношение цены и качества.", 5 },
                    { 6, new DateTimeOffset(new DateTime(2025, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 5, "Очень комфортный номер, рекомендую для деловых поездок.", 1 },
                    { 7, new DateTimeOffset(new DateTime(2025, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 4, "Хорошее расположение и чистота.", 2 },
                    { 8, new DateTimeOffset(new DateTime(2025, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 3, "Номер небольшой, но уютный.", 3 },
                    { 9, new DateTimeOffset(new DateTime(2025, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 5, "Отличный сервис и удобства.", 4 },
                    { 10, new DateTimeOffset(new DateTime(2025, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 4, "Приятный номер, но хотелось бы больше освещения.", 5 },
                    { 11, new DateTimeOffset(new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 4, "Хороший номер, всё необходимое под рукой.", 1 },
                    { 12, new DateTimeOffset(new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 3, "Номер простой, но чистый.", 2 },
                    { 13, new DateTimeOffset(new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 5, "Очень понравился дизайн номера и удобства.", 3 },
                    { 14, new DateTimeOffset(new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 4, "Хороший номер, но немного шумно с улицы.", 4 },
                    { 15, new DateTimeOffset(new DateTime(2025, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 5, "Отличное соотношение цена-качество.", 5 },
                    { 16, new DateTimeOffset(new DateTime(2025, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 4, "Уютный номер, рекомендую.", 1 }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Capacity", "Count", "Description", "HotelId", "PricePerNight", "RoomName" },
                values: new object[,]
                {
                    { 1, 2, 3, "Превосходный выбор для двоих: здесь есть всё для отдыха и работы.", 1, 4800m, "Двухместный Полулюкс «Комфорт»" },
                    { 2, 1, 2, "Уютное гнёздышко для тех, кто ценит функциональность и спокойствие.", 1, 3200m, "Одноместный Стандарт «Эконом»" },
                    { 3, 4, 1, "Идеальное решение для семьи: много места и приятные детали для каждого.", 1, 7500m, "Семейный Люкс «Гармония»" },
                    { 4, 2, 4, "Надёжный выбор для тех, кто предпочитает проверенные решения и комфорт.", 1, 4000m, "Двухместный Стандарт «Классика»" },
                    { 5, 2, 1, "Погрузитесь в атмосферу роскоши и забудьте обо всём, глядя на бескрайние волны.", 1, 9000m, "Люкс с видом на море «Романтика»" },
                    { 6, 2, 2, "Насладитесь утренним кофе на свежем воздухе, не покидая номер.", 1, 4500m, "Стандарт с балконом «Свежесть»" },
                    { 7, 1, 2, "Творческое пространство для вдохновения и отдыха.", 2, 3500m, "Студия «Арт»" },
                    { 8, 2, 1, "Элегантный люкс для самых взыскательных гостей.", 3, 6000m, "Двухместный Люкс «Премиум»" },
                    { 9, 4, 1, "Много места для всей семьи — комфорт гарантирован.", 4, 7000m, "Семейный номер «XL»" },
                    { 10, 2, 1, "Всё необходимое для продуктивной работы и отдыха.", 5, 4000m, "Стандартный «Бизнес»" },
                    { 11, 2, 1, "Погрузитесь в релакс: джакузи и премиальный сервис.", 6, 8500m, "Люкс с джакузи «SPA»" },
                    { 12, 1, 1, "Компактный номер для тех, кто ценит мобильность.", 7, 3000m, "Одноместный «Мини»" },
                    { 13, 2, 1, "Больше пространства и удобств для вашего отдыха.", 8, 4200m, "Двухместный «Комфорт+»" }
                });

            migrationBuilder.InsertData(
                table: "Booking",
                columns: new[] { "Id", "CheckInDate", "CheckOutDate", "RoomId", "Status", "TotalPrice", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, "Подтверждено", 10000, 1 },
                    { 2, new DateTimeOffset(new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, "Отменено", 20000, 2 },
                    { 3, new DateTimeOffset(new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, "Завершено", 30000, 3 },
                    { 4, new DateTimeOffset(new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, "Активно", 25000, 4 },
                    { 5, new DateTimeOffset(new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, "Ожидание", 28000, 5 },
                    { 6, new DateTimeOffset(new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, "Завершено", 22000, 1 },
                    { 7, new DateTimeOffset(new DateTime(2025, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, "Активно", 18000, 2 },
                    { 8, new DateTimeOffset(new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, "Ожидание", 21000, 3 },
                    { 9, new DateTimeOffset(new DateTime(2025, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, "Отменено", 17000, 4 },
                    { 10, new DateTimeOffset(new DateTime(2025, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 10, "Активно", 24000, 5 }
                });

            migrationBuilder.InsertData(
                table: "RoomAmenity",
                columns: new[] { "AmenityId", "RoomId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 4, 1 },
                    { 10, 1 },
                    { 15, 1 },
                    { 23, 1 },
                    { 1, 2 },
                    { 8, 2 },
                    { 9, 2 },
                    { 9, 3 },
                    { 12, 3 },
                    { 21, 3 },
                    { 25, 3 },
                    { 27, 3 },
                    { 30, 3 },
                    { 1, 4 },
                    { 2, 4 },
                    { 7, 4 },
                    { 16, 4 },
                    { 5, 5 },
                    { 14, 5 },
                    { 28, 5 },
                    { 29, 5 },
                    { 30, 5 },
                    { 10, 6 },
                    { 11, 6 },
                    { 12, 6 },
                    { 13, 6 },
                    { 26, 6 },
                    { 1, 7 },
                    { 7, 7 },
                    { 4, 8 },
                    { 20, 8 },
                    { 23, 8 },
                    { 21, 9 },
                    { 25, 9 },
                    { 8, 10 },
                    { 16, 10 },
                    { 20, 11 },
                    { 30, 11 },
                    { 1, 12 },
                    { 27, 12 },
                    { 4, 13 },
                    { 24, 13 }
                });

            migrationBuilder.InsertData(
                table: "RoomImages",
                columns: new[] { "Id", "ImageUrl", "RoomId" },
                values: new object[,]
                {
                    { 1, "RoomImg/test1.png", 1 },
                    { 2, "RoomImg/test2.png", 2 },
                    { 3, "RoomImg/test3.png", 3 },
                    { 4, "RoomImg/test4.png", 4 },
                    { 5, "RoomImg/test5.png", 5 },
                    { 6, "RoomImg/test6.png", 6 },
                    { 7, "RoomImg/test7.png", 7 },
                    { 8, "RoomImg/test8.png", 8 },
                    { 9, "RoomImg/test9.png", 9 },
                    { 10, "RoomImg/test10.png", 10 },
                    { 11, "RoomImg/test11.png", 11 },
                    { 12, "RoomImg/test12.png", 12 },
                    { 13, "RoomImg/test13.png", 13 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_RoomId",
                table: "Booking",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_UserId",
                table: "Booking",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_HotelId",
                table: "Comment",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotel_CompanyId",
                table: "Hotel",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotel_Name",
                table: "Hotel",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Room_HotelId",
                table: "Room",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomName",
                table: "Room",
                column: "RoomName");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAmenity_AmenityId",
                table: "RoomAmenity",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomImages_RoomId",
                table: "RoomImages",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CompanyId",
                table: "User",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "RoomAmenity");

            migrationBuilder.DropTable(
                name: "RoomImages");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Amenity");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Hotel");

            migrationBuilder.DropTable(
                name: "Company");
        }
    }
}
