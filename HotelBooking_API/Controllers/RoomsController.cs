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
        [HttpPost("AddOneRoomToEachHotelAsync")]
        public async Task AddOneRoomToEachHotelAsync()
        {
            // Получаем все отели из базы
            var hotels = await _context.Hotel.ToListAsync();

            foreach (var hotel in hotels)
            {
                var newRoom = new Room
                {
                    HotelId = hotel.Id,
                    RoomName = $"Комната для отеля {hotel.Name}",
                    PricePerNight = 1000, // Можно заменить на случайное или любое другое значение
                    Capacity = 2,
                    Description = "Автоматически добавленная комната",
                    Count = 5
                };

                _context.Room.Add(newRoom);
            }

            // Сохраняем все добавленные комнаты одним запросом
            await _context.SaveChangesAsync();
        }


        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoom()
        {
            return await _context.Room.ToListAsync();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoomById(int id)
        {
            var room = await _context.Room
                .Include(r => r.RoomImages)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var roomDto = new RoomDto
            {
                Id = room.Id,
                RoomName = room.RoomName,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                Description = room.Description,
                Count = room.Count,
                Amenities = room.RoomAmenities?.Select(ra => new AmenityDto
                {
                    Id = ra.Amenity.Id,
                    Name = ra.Amenity.Name
                }).ToList() ?? new List<AmenityDto>(),
                RoomImages = room.RoomImages?.Select(img => new RoomImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl
                }).ToList() ?? new List<RoomImageDto>()
            };

            return Ok(roomDto);
        }


        // GET: api/Rooms/Hotel/5
        [HttpGet("Hotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomByHotelId(int hotelId)
        {
            var rooms = await _context.Room
                .Where(r => r.HotelId == hotelId)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .Include(r => r.RoomImages)
                .ToListAsync();

            var roomDtos = rooms.Select(room => new RoomDto
            {
                Id = room.Id,
                RoomName = room.RoomName,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                Description = room.Description,
                Count = room.Count,
                Amenities = room.RoomAmenities?
                    .Select(ra => new AmenityDto
                    {
                        Id = ra.Amenity.Id,
                        Name = ra.Amenity.Name
                    }).ToList() ?? new List<AmenityDto>(),
                RoomImages = room.RoomImages?
                    .Select(img => new RoomImageDto
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl
                    }).ToList() ?? new List<RoomImageDto>()
            }).ToList();

            return Ok(roomDtos);
        }


        // Метод для получения HotelId по id комнаты
        [HttpGet("HotelIdByRoom/{roomId}")]
        public async Task<int?> GetHotelIdByRoomIdAsync(int roomId)
        {
            var room = await _context.Room.FirstOrDefaultAsync(r => r.Id == roomId);
            return room.HotelId;
        }


        // PUT: api/Rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, RoomDto roomDto)
        {
            if (id != roomDto.Id)
            {
                return BadRequest();
            }

            var existingRoom = await _context.Room.FindAsync(id);
            if (existingRoom == null)
            {
                return NotFound();
            }

            existingRoom.RoomName = roomDto.RoomName;
            existingRoom.PricePerNight = roomDto.PricePerNight;
            existingRoom.Capacity = roomDto.Capacity;
            existingRoom.Description = roomDto.Description;
            existingRoom.Count = roomDto.Count;

            await _context.SaveChangesAsync();

            return NoContent();
        }



        // POST: api/Rooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bool>> PostRoom(Room room)
        {
            try
            {
                room.Hotel = await _context.Hotel.FindAsync(room.HotelId);

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
