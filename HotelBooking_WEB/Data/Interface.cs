namespace HotelBooking_WEB.Data
{
    using Refit;
    using HotelBooking_API.Data.Models;
    using Microsoft.AspNetCore.Mvc;

    public interface IApiClient
    {
        [Get("/Hotels")]
        Task<IEnumerable<Hotel>> GetHotelsAsync();

        [Get("/Rooms/Hotel/{hotelId}")]
        Task<IEnumerable<Room>> GetRoomByHotelId(int hotelId);

        [Get("/Rooms/{roomId}")]
        Task<Room> GetRoomById(int roomId);

        [Get("/Users/{email},{password}")]
        Task<bool> GetVerification(string email, string password);



        /*[Get("/Partners")]
        Task<IEnumerable<Partner>> GetPartnerAsync();
        [Get("/Partners/{id}")]
        Task<Partner> GetPartnerAsync(int id);
        [Put("/Partners/{partner}")]
        Task<IActionResult> PutPartnerAsync(Partner partner);
        [Post("/Partners")]
        Task<IActionResult> AddPartnerAsync(Partner partner);
        [Get("/Partners/Discount/{id}")]
        Task<int> GetPartnerDiscountAsync(int id);*/
    }
}
