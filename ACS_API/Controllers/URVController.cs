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
        private DateTime DateStart;
        private DateTime DateEnd;

        public URVController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("GetListURV")]
        public async Task<ActionResult<List<URV>>> GetListURV(string dateStart, string dateEnd)
        {
            ListURV = new();
            DateStart = Convert.ToDateTime(dateStart);
            DateEnd = Convert.ToDateTime(dateEnd);
            try
            {
                if (_context.Events == null)
                {
                    return Problem("Entity set 'AcsContext.Events'  is null.");
                }
                else
                {
                    var listPersoanl = _context.Personals.Where(s => s.Position != "Водитель АБС").ToList();
                    if (DateStart == DateEnd)
                    {
                        foreach (var person in listPersoanl)
                        {
                            var startTimePerson = _context.Events.ToList().FirstOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == DateStart.Date);
                            var endTimePerson = _context.Events.ToList().LastOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == DateStart.Date);
                            if (startTimePerson != null && endTimePerson != null)
                            {
                                DateTime startTime = Convert.ToDateTime(startTimePerson.Time);
                                DateTime endTime = Convert.ToDateTime(endTimePerson.Time);

                                TimeSpan start = startTime.TimeOfDay;
                                TimeSpan end = endTime.TimeOfDay;

                                TimeSpan totalTime = end - start;

                                var urv = new URV() { Date = Convert.ToDateTime(DateStart), FIO = startTimePerson.Fio, Position = startTimePerson.Position, StartTime = startTime.TimeOfDay, EndTime = endTime.TimeOfDay, TotalTime = totalTime };
                                ListURV.Add(urv);
                            }
                        }
                    }
                    else
                    {
                        
                        while (DateStart <= DateEnd)
                        {
                            foreach (var person in listPersoanl)
                            {
                                var startTimePerson = _context.Events.ToList().FirstOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == DateStart.Date);
                                var endTimePerson = _context.Events.ToList().LastOrDefault(s => s.Fio == person.Fio && Convert.ToDateTime(s.Time).Date == DateStart.Date);
                                if (startTimePerson != null && endTimePerson != null)
                                {
                                    DateTime startTime = Convert.ToDateTime(startTimePerson.Time);
                                    DateTime endTime = Convert.ToDateTime(endTimePerson.Time);

                                    TimeSpan start = startTime.TimeOfDay;
                                    TimeSpan end = endTime.TimeOfDay;

                                    TimeSpan totalTime = end - start;

                                    var urv = new URV() { Date = Convert.ToDateTime(DateStart), FIO = startTimePerson.Fio, Position = startTimePerson.Position, StartTime = startTime.TimeOfDay, EndTime = endTime.TimeOfDay, TotalTime = totalTime };
                                    ListURV.Add(urv);
                                }
                            }
                            DateStart += new TimeSpan(1, 0, 0, 0);
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

        [HttpPost("CreateReportExcel")]
        public async Task<ActionResult> CreateReportExcel()
        {

            return Ok();
        }
    }
}
