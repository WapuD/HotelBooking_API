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
    public class BookingsController : ControllerBase
    {
        private readonly HBContext _context;

        public BookingsController(HBContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
        {
            var bookings = await _context.Booking.ToListAsync();
            return bookings;
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }


        // GET: api/Bookings/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetUserBookings(int userId)
        {
            var bookings = await _context.Booking
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            if (!bookings.Any())
            {
                return NotFound();
            }

            return bookings;
        }

        // PUT: api/Bookings/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateBookingStatus(int id, string newStatus)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            booking.Status = newStatus;
            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound(new { message = "Booking not found during update" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Booking status updated successfully" });
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
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

        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking([FromBody] BookingDTO bookingDTO)
        {
            if (bookingDTO == null ||
                bookingDTO.RoomId <= 0 ||
                bookingDTO.UserId <= 0 ||
                bookingDTO.CheckInDate == default ||
                bookingDTO.CheckOutDate == default ||
                bookingDTO.TotalPrice <= 0)
            {
                return BadRequest("Некорректные данные бронирования");
            }

            var booking = new Booking
            {
                RoomId = bookingDTO.RoomId,
                UserId = bookingDTO.UserId,
                CheckInDate = bookingDTO.CheckInDate.ToUniversalTime(),
                CheckOutDate = bookingDTO.CheckOutDate.ToUniversalTime(),
                TotalPrice = bookingDTO.TotalPrice,
                Status = bookingDTO.Status
            };
            booking.User = await _context.User.FindAsync(bookingDTO.UserId);
            booking.Room = await _context.Room.FindAsync(bookingDTO.RoomId);

            if (booking.User != null && booking.Room != null)
                _context.Booking.Add(booking);
            else
                return BadRequest("Нет пользователя или комнаты бронирования");

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }



        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}
