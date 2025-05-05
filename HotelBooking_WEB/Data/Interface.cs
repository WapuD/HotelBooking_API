namespace HotelBooking_WEB.Data
{
    using Refit;
    using HotelBooking_API.Data.Models;
    using Microsoft.AspNetCore.Mvc;

    public interface IApiClient
    {
        [Get("/Hotels")]
        Task<IEnumerable<Hotel>> GetHotelsAsync();
        [Get("/Hotels/{hotelId}")]
        Task<Hotel> GetHotelById(int hotelId);

        [Post("/Hotels")]
        Task<bool> CreateHotel(HotelDtoCreate hotel);

        [Post("/Hotels/UpdateRatings")]
        Task UpdateHotelRatings();



        [Get("/Rooms/Hotel/{hotelId}")]
        Task<IEnumerable<Room>> GetRoomByHotelId(int hotelId);

        [Get("/Rooms/{roomId}")]
        Task<Room> GetRoomById(int roomId);

        [Get("/Rooms/{roomId}/{checkIn}_{checkOut}")]
        Task<int> GetAvailableRoomCount(int roomId, string checkIn, string checkOut);


        [Get("/Users/Verification")]
        Task<User> GetVerification(string email, string password);

        [Get("/Users/{id}")]
        Task<User> GetUser(int id);

        [Post("/Users/CreateUser")]
        Task<bool> CreateUser(CreateUserDto newUser);

        [Put("/Users/update")]
        Task UpdateUser(UpdateUserDto updateUserDto);


        [Get("/Bookings/user/{userId}")]
        Task<IEnumerable<Booking>> GetUserBookings(int userId);

        [Get("/Bookings")]
        Task<IEnumerable<Booking>> GetAllBookings();

        [Put("/Bookings/update")]
        Task UpdateBookingStatus(int id, string newStatus);

        [Post("/Bookings")]
        Task CreateBooking([Body] BookingDTO bookingDTO);

        [Get("/Bookings/Room/{roomId}")]
        Task<IEnumerable<Booking>> GetBookingsByRoomId(int roomId);

        [Get("/Bookings/{id}")]
        Task<Booking> GetBookingById(int id);


        [Get("/Companies")]
        Task<IEnumerable<Company>> GetCompaniesAsync();
        [Post("/Companies")]
        Task<bool> CreateCompany([Body] CompanyCreateDto company);
    }
}
