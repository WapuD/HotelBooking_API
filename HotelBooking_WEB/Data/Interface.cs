namespace HotelBooking_WEB.Data
{
    using Refit;
    using HotelBooking_API.Data.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IApiClient
    {
        // --- Отели ---
        [Get("/Hotels")]
        Task<IEnumerable<Hotel>> GetHotelsAsync();

        [Get("/Hotels/{hotelId}")]
        Task<Hotel> GetHotelById(int hotelId);

        [Get("/Hotels/Company/{companyId}")]
        Task<IEnumerable<Hotel>> GetHotelsByCompanyId(int companyId);

        [Post("/Hotels")]
        Task<bool> CreateHotel([Body] HotelDtoCreate hotel);

        [Put("/Hotels/Update/{id}")]
        Task<bool> UpdateHotel(int id, [Body] HotelDtoCreate hotel);

        [Delete("/Hotels/{id}")]
        Task DeleteHotel(int id);

        [Post("/Hotels/UpdateRatings")]
        Task UpdateHotelRatings();


        // --- Компании ---
        [Get("/Companies")]
        Task<IEnumerable<Company>> GetCompaniesAsync();

        [Get("/Companies/{id}")]
        Task<Company> GetCompany(int id);

        [Post("/Companies")]
        Task<bool> CreateCompany([Body] CompanyCreateDto company);

        [Put("/Companies/{id}")]
        Task<bool> UpdateCompany(int id, [Body] CompanyCreateDto company);

        [Delete("/Companies/{id}")]
        Task DeleteCompany(int id);


        // --- Номера ---
        [Get("/Rooms/Hotel/{hotelId}")]
        Task<IEnumerable<RoomDto>> GetRoomByHotelId(int hotelId);

        [Get("/Rooms/{roomId}")]
        Task<RoomDto> GetRoomById(int roomId);

        [Get("/Rooms/{roomId}/{checkIn}_{checkOut}")]
        Task<int> GetAvailableRoomCount(int roomId, string checkIn, string checkOut);

        [Post("/Rooms")]
        Task<bool> CreateRoom([Body] Room room);

        [Post("/RoomImages")]
        Task<RoomImages> PostRoomImage([Body] RoomImages roomImage);

        [Delete("/{id}")]
        Task<IActionResult> DeleteRoom(int id);

        [Put("/Rooms/{id}")]
        Task UpdateRoom(int id, [Body] RoomDto roomDto);

        [Get("/Rooms/HotelIdByRoom/{roomId}")]
        Task<int?> GetHotelIdByRoomIdAsync(int roomId);


        // --- Комментарии ---
        [Get("/Comments/Hotel/{hotelId}")]
        Task<IEnumerable<Comment>> GetCommentsByHotelId(int hotelId);

        [Post("/Comments")]
        Task<Comment> PostComment([Body] Comment comment);


        // --- Пользователи ---
        [Get("/Users/Verification")]
        Task<User> GetVerification(string email, string password);

        [Get("/Users/{id}")]
        Task<User> GetUser(int id);

        [Post("/Users/CreateUser")]
        Task<bool> CreateUser(CreateUserDto newUser);

        [Put("/Users/update")]
        Task UpdateUser(UpdateUserDto updateUserDto);

        [Put("/Users/updatePassword")]
        Task<bool> UpdateUserPassword([FromBody] UpdateUserPasswordDto dto);


        // --- Бронирования ---
        [Get("/Bookings/user/{userId}")]
        Task<IEnumerable<Booking>> GetUserBookings(int userId);

        [Get("/Bookings")]
        Task<IEnumerable<Booking>> GetAllBookings(int companyId);

        [Put("/Bookings/update")]
        Task UpdateBookingStatus(int id, string newStatus);

        [Post("/Bookings")]
        Task CreateBooking([Body] BookingDTO bookingDTO);

        [Get("/Bookings/Room/{roomId}")]
        Task<IEnumerable<Booking>> GetBookingsByRoomId(int roomId);

        [Get("/Bookings/{id}")]
        Task<Booking> GetBookingById(int id);
    }
}
