using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
                Rooms = null
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
