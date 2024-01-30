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
    public class OffendersController : ControllerBase
    {
        private readonly AcsContext _context;

        public OffendersController(AcsContext context)
        {
            _context = context;
        }

        // GET: api/Offenders
        [HttpGet("GetOffenders")]
        public async Task<List<Offender>> GetOffenders()
        {
            if (_context.Offenders == null)
            {
                return null;
            }
            return await _context.Offenders.ToListAsync();
        }

        // GET: api/Offenders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Offender>> GetOffender(int id)
        {
            if (_context.Offenders == null)
            {
                return NotFound();
            }
            var offender = await _context.Offenders.FindAsync(id);

            if (offender == null)
            {
                return NotFound();
            }

            return offender;
        }

        // PUT: api/Offenders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOffender(int id, Offender offender)
        {
            if (id != offender.Id)
            {
                return BadRequest();
            }

            _context.Entry(offender).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OffenderExists(id))
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

        // POST: api/Offenders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Offender>> PostOffender(Offender offender)
        {
            if (_context.Offenders == null)
            {
                return Problem("Entity set 'AcsContext.Offenders'  is null.");
            }
            _context.Offenders.Add(offender);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOffender", new { id = offender.Id }, offender);
        }

        [HttpPost("SendOrNot")]
        public async Task<ActionResult<Offender>> SendOrNot(Offender offender)
        {
            try
            {
                offender.SendOrNot = (byte)1;
                _context.Offenders.Update(offender);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        // DELETE: api/Offenders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffender(int id)
        {
            if (_context.Offenders == null)
            {
                return NotFound();
            }
            var offender = await _context.Offenders.FindAsync(id);
            if (offender == null)
            {
                return NotFound();
            }

            _context.Offenders.Remove(offender);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OffenderExists(int id)
        {
            return (_context.Offenders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
