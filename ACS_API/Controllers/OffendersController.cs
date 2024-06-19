using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBD.BD;
using Microsoft.Extensions.Logging;

namespace ACS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffendersController : ControllerBase
    {
        private readonly AcsContext _context;
        private Offender newOffender;
        private List<Offender> newListOffender { get; set; }
        public OffendersController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("GetOffenders")]
        public async Task<ActionResult<List<Offender>>> GetOffenders()
        {
            try
            {
                if (_context.Offenders == null)
                {
                    await _context.Database.CloseConnectionAsync();
                    return null;
                }
                var offenders = await _context.Offenders.ToListAsync();
                await _context.Database.CloseConnectionAsync();
                return offenders;
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("GetOffendersForPage")]
        public async Task<ActionResult<List<Offender>>> GetOffendersForPage(DateTime DateFiltr)
        {
            try
            {
                if (_context.Offenders == null)
                {
                    await _context.Database.CloseConnectionAsync();
                    return null;
                }
                else
                {
                    if (DateFiltr.Day == 01 && DateFiltr.Month == 01 && DateFiltr.Year == 0001 && DateFiltr.Hour == 0 && DateFiltr.Minute == 00 && DateFiltr.Second == 00)
                    {
                        var offenders = await _context.Offenders.ToListAsync();
                        await _context.Database.CloseConnectionAsync();
                        return offenders;
                    }
                    else
                    {
                        var offenders = await _context.Offenders.Where(s => s.Time.Value.Day == DateFiltr.Day && s.Time.Value.Month == DateFiltr.Month && s.Time.Value.Year == DateFiltr.Year).ToListAsync();
                        await _context.Database.CloseConnectionAsync();
                        return offenders;
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("CreateOffender")]
        public async Task<ActionResult<Offender>> CreateOffender()
        {
            try
            {
                newListOffender = new();
                var newListEvents = _context.Events.ToList().Where(s => s.PassOrDeny == "DENY" && s.SendOrNot == 0);
                var deadLine = new TimeSpan(0, 15, 00);
                var newListCycle = _context.Cycles.ToList().Where(s => s.Delta > deadLine && s.SendOrNot == 0);
                if (newListEvents.Count() != 0 || newListEvents != null)
                {
                    foreach (var events in newListEvents)
                    {
                        events.SendOrNot = 1;
                        if (!string.IsNullOrEmpty(events.Fio))
                        {
                            newOffender = new Offender() { Name = events.Fio, Position = events.Position, Dec = events.Dec, W26 = events.W26, Hex = events.Hex, Time = Convert.ToDateTime(events.Time) };
                        }
                        else
                        {
                            newOffender = new Offender() { Name = "Неизвестно", Position = events.Position, Dec = events.Dec, W26 = events.W26, Hex = events.Hex, Time = Convert.ToDateTime(events.Time) };
                        }
                        newListOffender.Add(newOffender);
                        await _context.SaveChangesAsync();
                    }
                    foreach (var cycle in newListCycle)
                    {
                        cycle.SendOrNot = 1;
                        if (!string.IsNullOrEmpty(cycle.Event.Fio))
                        {
                            newOffender = new Offender() { Name = cycle.Event.Fio, Position = cycle.Event.Position, Dec = cycle.Event.Dec, W26 = cycle.Event.W26, Hex = cycle.Event.Hex, Time = Convert.ToDateTime(cycle.TimeP2) };
                        }
                        else
                        {
                            newOffender = new Offender() { Name = "Неизвестно", Position = cycle.Event.Position, Dec = cycle.Event.Dec, W26 = cycle.Event.W26, Hex = cycle.Event.Hex, Time = Convert.ToDateTime(cycle.TimeP2) };
                        }
                        newListOffender.Add(newOffender);
                        await _context.SaveChangesAsync();
                    }
                }
                _context.Offenders.AddRange(newListOffender);
                await _context.SaveChangesAsync();
                await _context.Database.CloseConnectionAsync();
                return null;
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SendOrNot")]
        public async Task<ActionResult<Offender>> SendOrNot(Offender offender)
        {
            try
            {
                offender.SendOrNot = (byte)1;
                _context.Offenders.Update(offender);
                await _context.SaveChangesAsync();
                await _context.Database.CloseConnectionAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
