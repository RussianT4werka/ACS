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
    public class CyclesController : ControllerBase
    {
        private readonly AcsContext _context;
        private List<ACS_API.DTO.Cycle> fullCycles = new();
        private ACS_API.DTO.Cycle fullCycle = new();
        public CyclesController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("GetCycles")]
        public async Task<ActionResult<List<ACS_API.DTO.Cycle>>> GetCycles()
        {
            try
            {
                var Cycle = await _context.Cycles.Include( s => s.Event).ToListAsync();
                
                if (Cycle == null)
                {
                    return NotFound();
                }
                else
                {
                    foreach (var cycle in Cycle)
                    {
                        fullCycle = new();
                        fullCycle.W26 = cycle.W26;
                        fullCycle.TimeP1 = cycle.TimeP1;
                        fullCycle.TimeP2 = cycle.TimeP2;
                        fullCycle.Delta = cycle.Delta;
                        fullCycle.Fio = cycle.Event.Fio;
                        fullCycle.Position = cycle.Event.Position;
                        fullCycles.Add(fullCycle);
                    }
                    return fullCycles;
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            
            
        }

        [HttpPost("CreateCycle")]
        public async Task<ActionResult> CreateCycle()
        {
            try
            {
                var Events = await _context.Events.ToListAsync();
                var Cycles = _context.Cycles.Include(s => s.Event).ToList();
                foreach (var events in Events.Where(s => s.PointId == 4))
                {
                    var cycleP2 = _context.Cycles.FirstOrDefault(s => s.TimeP2 == null);
                    if (events != null && cycleP2 == null)
                    {
                        var lastCycle = _context.Cycles.ToList().LastOrDefault();
                        if (Cycles.Count() == 0)
                        {
                            var newCycle = new Cycle() { EventId = events.Id, W26 = events.W26, TimeP1 = Convert.ToDateTime(events.Time) };
                            _context.Cycles.Add(newCycle);
                            _context.SaveChanges();
                            return Ok();
                        }
                        bool aa = Cycles.Any(s => s.EventId == events.Id);
                        if (lastCycle.EventId != events.Id && aa == false)
                        {
                            var newCycle = new Cycle() { EventId = events.Id, W26 = events.W26, TimeP1 = Convert.ToDateTime(events.Time) };
                            _context.Cycles.Add(newCycle);
                            _context.SaveChanges();
                            return Ok();
                        }
                    }
                    else
                    {
                        var ff = _context.Cycles.FirstOrDefault(s => s.TimeP2 == null);

                        if (ff != null)
                        {
                            foreach (var events2 in Events.Where(s => s.PointId == 8 && s.W26 == ff.W26))
                            {
                                bool gg = _context.Cycles.Any(s => s.TimeP1 > Convert.ToDateTime(events2.Time));
                                if (gg == false)
                                {
                                    foreach (var cycle in Cycles.Where(s => s.TimeP1 != null && s.TimeP2 == null && s.W26 == events2.W26))
                                    {
                                        cycle.TimeP2 = Convert.ToDateTime(events2.Time);
                                        cycle.Delta = cycle.TimeP2 - cycle.TimeP1;
                                        _context.Cycles.Update(cycle);
                                        _context.SaveChanges();
                                    }
                                }

                            }
                        }

                    }
                }
            }
            catch
            {
                return BadRequest();
            }
            return null;
        }
    }
}
