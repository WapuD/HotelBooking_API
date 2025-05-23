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
    public class CompaniesController : ControllerBase
    {
        private readonly HBContext _context;

        public CompaniesController(HBContext context)
        {
            _context = context;
        }

        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompany()
        {
            return await _context.Company.ToListAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Company.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        // GET: api/Companies/5/Hotels
        [HttpGet("{id}/Hotels")]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotelsByCompany(int id)
        {
            var companyExists = await _context.Company.AnyAsync(c => c.Id == id);
            if (!companyExists)
                return NotFound();

            var hotels = await _context.Hotel.Where(h => h.CompanyId == id).ToListAsync();
            return hotels;
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, CompanyCreateDto dto)
        {
            var company = await _context.Company.FindAsync(id);
            if (company == null)
                return NotFound();

            company.Name = dto.Name;
            company.Description = dto.Description;
            company.Email = dto.Email;
            company.Phone = dto.Phone;
            company.Website = dto.Website;
            company.LogoUrl = dto.LogoUrl;
            company.TaxId = dto.TaxId;
            company.LegalAddress = dto.LegalAddress;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                    return NotFound();
                else
                    throw;
            }

            return Ok(true);
        }




        // POST: api/Companies
        [HttpPost]
        public async Task<ActionResult<bool>> PostCompany(Company company)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
                return BadRequest(new { Errors = errors });
            }

            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            return Ok(true);
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
