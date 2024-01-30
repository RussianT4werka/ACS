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
    public class LogsController : ControllerBase
    {
        private readonly AcsContext _context;

        public LogsController(AcsContext context)
        {
            _context = context;
        }

        // GET: api/Logs
        [HttpGet("GetListLogs")]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs()
        {
          if (_context.Logs == null)
          {
              return NotFound();
          }
            return await _context.Logs.ToListAsync();
        }

        // POST: api/Logs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("WriteLog")]
        public async Task<ActionResult> WriteLog(Log log)
        {
            try
            {
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            { 
                BadRequest(ex.Message);
            }
            return NoContent();
        }

        [HttpPost("DeleteListLogs")]
        public async Task<ActionResult> DeleteListLogs(List<Log> logs)
        {
            try
            {
                foreach(var log in logs)
                {
                    _context.Remove(log);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return NoContent();
        }
    }
}
