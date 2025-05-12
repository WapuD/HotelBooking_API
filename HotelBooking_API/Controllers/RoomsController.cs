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
    public class RoomsController : ControllerBase
    {
        private readonly HBContext _context;

        public RoomsController(HBContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoom()
        {
            return await _context.Room.ToListAsync();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoomById(int id)
        {
            var room = await _context.Room.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            room.RoomImages = await _context.RoomImages
                .Where(ri => ri.RoomId == room.Id)
                .ToListAsync();

            return room;
        }

        // GET: api/Rooms/Hotel/5
        [HttpGet("Hotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomByHotelId(int hotelId)
        {
            var rooms = await _context.Room.Where(r => r.HotelId == hotelId)
                                           .ToListAsync();
            if (rooms == null)
            {
                return NotFound();
            }

            foreach (var room in rooms)
            {
                if (room.RoomImages == null)
                {
                    room.RoomImages = await _context.RoomImages
                        .Where(ri => ri.RoomId == room.Id)
                        .Take(1)
                        .ToListAsync();
                }
            }

            return rooms;
        }

        // PUT: api/Rooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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

        // POST: api/Rooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bool>> PostRoom(Room room)
        {
            try
            {
                if (room.Hotel == null)
                {
                    room.Hotel = await _context.Hotel.FindAsync(room.HotelId);
                }

                _context.Room.Add(room);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Room.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.Id == id);
        }

        // GET: api/Rooms/2/2000-12-22_2000-12-24
        [HttpGet("{roomId}/{checkIn}_{checkOut}")]
        public int GetAvailableRoomCount(int roomId, DateTimeOffset checkIn, DateTimeOffset checkOut)
        {
            var room = _context.Room.FirstOrDefault(r => r.Id == roomId);
            if (room == null) return 0;

            var utcCheckIn = new DateTime(checkIn.Year, checkIn.Month, checkIn.Day, 5, 0, 0).ToUniversalTime();
            var utcCheckOut = new DateTime(checkOut.Year, checkOut.Month, checkOut.Day, 5, 0, 0).ToUniversalTime();


            var bookings = _context.Booking
                .Where(b => b.RoomId == roomId)
                .ToList();

            // Словарь: дата -> количество занятых комнат в этот день
            var occupancy = new Dictionary<DateTime, int>();

            foreach (var booking in bookings)
            {
                var startDate = booking.CheckInDate.Date;
                var endDate = booking.CheckOutDate.Date;

                for (var date = startDate; date < endDate; date = date.AddDays(1))
                {
                    if (occupancy.ContainsKey(date))
                        occupancy[date]++;
                    else
                        occupancy[date] = 1;
                }
            }

            int maxOccupied = 0;
            for (var date = utcCheckIn; date < utcCheckOut; date = date.AddDays(1))
            {
                if (occupancy.TryGetValue(date, out int count))
                {
                    if (count > maxOccupied)
                        maxOccupied = count;
                }
            }

            int available = room.Count - maxOccupied;
            return available > 0 ? available : 0;
        }
    }
}
