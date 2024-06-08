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
    public class URVController : ControllerBase
    {
        private readonly AcsContext _context;
        private List<URV> ListURV;

        public URVController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("GetListURV")]
        public async Task<ActionResult<List<URV>>> GetListURV(string Date)
        {
            ListURV = new();
            try
            {
                if (_context.Events == null)
                {
                    return Problem("Entity set 'AcsContext.Events'  is null.");
                }
                else
                {
                    var listPersoanl = _context.Personals.Where(s => s.Position != "Водитель АБС").ToList();
                    foreach (var person in listPersoanl)
                    {
                        var startTimePerson = _context.Events.ToList().FirstOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == Convert.ToDateTime(Date).Date);
                        var endTimePerson = _context.Events.ToList().LastOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == Convert.ToDateTime(Date).Date);
                        if (startTimePerson != null && endTimePerson != null)
                        {
                            DateTime startTime = Convert.ToDateTime(startTimePerson.Time);
                            DateTime endTime = Convert.ToDateTime(endTimePerson.Time);

                            TimeSpan start = startTime.TimeOfDay;
                            TimeSpan end = endTime.TimeOfDay;

                            TimeSpan totalTime = end - start;

                            var urv = new URV() { Date = Convert.ToDateTime(Date), FIO = startTimePerson.Fio, Position = startTimePerson.Position, StartTime = startTime.TimeOfDay, EndTime = endTime.TimeOfDay, TotalTime = totalTime };
                            ListURV.Add(urv);
                        }
                    }
                    return Ok(ListURV);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
