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
    public class URVController : ControllerBase
    {
        private readonly AcsContext _context;

        public URVController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<ActionResult<List<Event>>> GetEvents()
        {
          if (_context.Events == null)
          {
              return NotFound();
          }
            return await _context.Events.ToListAsync();
        }

        
        [HttpPost("")]
        public async Task<ActionResult<Event>> PostEvent(Event @event)
        {
          if (_context.Events == null)
          {
              return Problem("Entity set 'AcsContext.Events'  is null.");
          }
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
        }
    }
}
