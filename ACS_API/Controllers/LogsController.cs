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

        [HttpGet("GetListLogs")]
        public async Task<ActionResult<List<Log>>> GetLogs(DateTime DateFiltr)
        {
            try
            {
                if (_context.Logs == null)
                {
                    await _context.Database.CloseConnectionAsync();
                    return NotFound();
                }
                else
                {
                    if (DateFiltr.Day == 01 && DateFiltr.Month == 01 && DateFiltr.Year == 0001 && DateFiltr.Hour == 0 && DateFiltr.Minute == 00 && DateFiltr.Second == 00)
                    {
                        var logs = await _context.Logs.ToListAsync();
                        await _context.Database.CloseConnectionAsync();
                        return logs;
                    }
                    else
                    {
                        var logs = await _context.Logs.Where(s => s.DateTime.Date == DateFiltr.Date).ToListAsync();
                        await _context.Database.CloseConnectionAsync();
                        return logs;
                    }
                }
                
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("WriteLog")]
        public async Task<ActionResult> WriteLog(Log log)
        {
            try
            {
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                await _context.Database.CloseConnectionAsync();
            }
            catch(Exception ex)
            { 
                return BadRequest(ex.Message);
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
                await _context.Database.CloseConnectionAsync();
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return NoContent();
        }
    }
}
