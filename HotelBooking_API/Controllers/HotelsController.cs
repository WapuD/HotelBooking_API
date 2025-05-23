using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking_API.Data;
using HotelBooking_API.Data.Models;

namespace HotelBooking_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly HBContext _context;

        public HotelsController(HBContext context)
        {
            _context = context;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotel()
        {
            return await _context.Hotel.Include(h => h.Rooms)
                                       .ToListAsync();
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<Hotel> GetHotel(int id)
        {
            var hotel = await _context.Hotel.FindAsync(id);

            return hotel;
        }

        // GET: api/Hotels/Company/5
        [HttpGet("Company/{companyId}")]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotelsByCompanyId(int companyId)
        {
            var hotels = await _context.Hotel
                .Where(h => h.CompanyId == companyId)
                .Include(h => h.Rooms)
                .ToListAsync();

            return hotels;
        }

        // PUT: api/Hotels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, Hotel hotel)
        {
            if (id != hotel.Id)
            {
                return BadRequest();
            }

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateHotel(int id, HotelDtoCreate hotelDto)
        {
            if (hotelDto.Rating < 0 || hotelDto.Rating > 5)
                return BadRequest("Рейтинг должен быть от 0 до 5");

            if (string.IsNullOrWhiteSpace(hotelDto.Name))
                return BadRequest("Название отеля обязательно");

            var companyExists = await _context.Company.AnyAsync(c => c.Id == hotelDto.CompanyId);
            if (!companyExists)
                return BadRequest("Компания с таким Id не найдена");

            var hotel = await _context.Hotel.FindAsync(id);
            if (hotel == null)
                return NotFound();

            hotel.Name = hotelDto.Name;
            hotel.Address = hotelDto.Address;
            hotel.City = hotelDto.City;
            hotel.Description = hotelDto.Description;
            hotel.ImageUrl = hotelDto.ImageUrl;
            hotel.Rating = hotelDto.Rating;
            hotel.CompanyId = hotelDto.CompanyId;

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!HotelExists(id))
                    return NotFound();

                Console.Error.WriteLine(ex);
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }

            return Ok(true);
        }


        // POST: api/Hotels
        [HttpPost]
        public async Task<bool> PostHotel(HotelDtoCreate hotel)
        {
            var newHotel = new Hotel
            {
                Address = hotel.Address,
                ImageUrl = hotel.ImageUrl,
                Name = hotel.Name,
                Description = hotel.Description,
                City = hotel.City,
                Rating = hotel.Rating,
                Rooms = null,
                Company = await _context.Company.FindAsync(hotel.CompanyId),
                CompanyId = hotel.CompanyId
            };

            try
            {
                _context.Hotel.Add(newHotel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _context.Hotel.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            _context.Hotel.Remove(hotel);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool HotelExists(int id)
        {
            return _context.Hotel.Any(e => e.Id == id);
        }

        [HttpPost("UpdateRatings")]
        public async Task<IActionResult> UpdateRatings()
        {
            var hotels = await _context.Hotel.Include(h => h.Comments).ToListAsync();
            foreach (var hotel in hotels)
            {
                if (hotel.Comments != null && hotel.Comments.Any())
                {
                    hotel.Rating = (decimal)hotel.Comments.Average(c => c.Rating);
                }
                else
                {
                    hotel.Rating = 0;
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
