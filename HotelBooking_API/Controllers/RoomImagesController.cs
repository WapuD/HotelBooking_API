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
    public class RoomImagesController : ControllerBase
    {
        private readonly HBContext _context;

        public RoomImagesController(HBContext context)
        {
            _context = context;
        }

        // GET: api/RoomImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomImages>>> GetRoomImages()
        {
            return await _context.RoomImages.ToListAsync();
        }

        // GET: api/RoomImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomImages>> GetRoomImages(int id)
        {
            var roomImages = await _context.RoomImages.FindAsync(id);

            if (roomImages == null)
            {
                return NotFound();
            }

            return roomImages;
        }

        // PUT: api/RoomImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomImages(int id, RoomImages roomImages)
        {
            if (id != roomImages.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomImages).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomImagesExists(id))
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

        // POST: api/RoomImages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomImages>> PostRoomImages(RoomImages roomImages)
        {
            roomImages.RoomId = _context.Room.OrderByDescending(r => r.Id).FirstOrDefault().Id;
            roomImages.Room = await _context.Room.FindAsync(roomImages.RoomId);
            _context.RoomImages.Add(roomImages);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomImages", new { id = roomImages.Id }, roomImages);
        }

        // DELETE: api/RoomImages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomImages(int id)
        {
            var roomImages = await _context.RoomImages.FindAsync(id);
            if (roomImages == null)
            {
                return NotFound();
            }

            _context.RoomImages.Remove(roomImages);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomImagesExists(int id)
        {
            return _context.RoomImages.Any(e => e.Id == id);
        }
    }
}
