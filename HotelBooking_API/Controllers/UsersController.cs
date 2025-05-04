using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking_API.Data;
using HotelBooking_API.Data.Models;
using HotelBooking_API.Data.Other;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly HBContext _context;

        public UsersController(HBContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/CreateUser
        [HttpPost("CreateUser")]
        public async Task<ActionResult<bool>> CreateUser(CreateUserDto newUser)
        {
            var user = new User
            {
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                SecondName = newUser.SecondName,
                LastName = newUser.LastName,
                Phone = newUser.Phone,
                PasswordHash = PasswordHasher.HashPassword(newUser.Password),
                CompanyId = newUser.CompanyId,
                Company = _context.Company.Find(newUser.CompanyId)
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        // GET: api/Users/Verification
        [HttpGet("Verification")]
        public async Task<ActionResult<User>> GetVerification(string email, string password)
        {
            var user = _context.User.Where(u => u.Email == email).FirstOrDefault();

            if (user == null)
            {
                return Unauthorized(new { message = "Пользователь не найден" });
            }

            var verify = PasswordHasher.VerifyPassword(password, user.PasswordHash);

            if (verify == true)
            {
                return Ok(user);
            }
            else
            {
                return Unauthorized(new { message = "Неправильный пароль" });
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            // Найдите пользователя в базе данных
            var user = await _context.User.FindAsync(updateUserDto.Id);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            // Обновите данные пользователя
            user.FirstName = updateUserDto.FirstName;
            user.SecondName = updateUserDto.SecondName;
            user.LastName = updateUserDto.LastName;
            user.Phone = updateUserDto.Phone;
            user.CompanyId = updateUserDto.CompanyId;

            // Сохраните изменения
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ошибка при обновлении данных" });
            }

            return Ok(new { message = "Данные пользователя успешно обновлены" });
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
