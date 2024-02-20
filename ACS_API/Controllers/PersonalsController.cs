using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBD.BD;

namespace ACS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalsController : ControllerBase
    {
        private readonly AcsContext _context;

        public PersonalsController(AcsContext context)
        {
            _context = context;
        }

        // GET: api/Personals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personal>>> GetPersonals()
        {
          if (_context.Personals == null)
          {
              return NotFound();
          }
            return await _context.Personals.ToListAsync();
        }

        // GET: api/Personals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Personal>> GetPersonal(int id)
        {
          if (_context.Personals == null)
          {
              return NotFound();
          }
            var personal = await _context.Personals.FindAsync(id);

            if (personal == null)
            {
                return NotFound();
            }

            return personal;
        }

        // PUT: api/Personals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersonal(int id, Personal personal)
        {
            if (id != personal.Id)
            {
                return BadRequest();
            }

            _context.Entry(personal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonalExists(id))
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

        // POST: api/Personals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Personal>> PostPersonal(Personal personal)
        {
          if (_context.Personals == null)
          {
              return Problem("Entity set 'AcsContext.Personals'  is null.");
          }
            _context.Personals.Add(personal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersonal", new { id = personal.Id }, personal);
        }

        // DELETE: api/Personals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonal(int id)
        {
            if (_context.Personals == null)
            {
                return NotFound();
            }
            var personal = await _context.Personals.FindAsync(id);
            if (personal == null)
            {
                return NotFound();
            }

            _context.Personals.Remove(personal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonalExists(int id)
        {
            return (_context.Personals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
