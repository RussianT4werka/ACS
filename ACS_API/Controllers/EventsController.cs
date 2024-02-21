using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBD.BD;
using System.Text.Json;
using ACS_API.Tools;
using Azure;
using Newtonsoft.Json;

namespace ACS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class EventsController : ControllerBase
    {
        private readonly AcsContext _context;

        public EventsController(AcsContext context)
        {
            _context = context;
        }
          

        /*[HttpPost("GetEvent")]  //Метод принятия проходов Web-Del
        public async Task<IActionResult> GetEvent([FromBody]object jsonSigur) //Нужно получать именно тело object, потому что из-за остальных типов он игнорит метод
        {
            string stringJsonSigur = Convert.ToString(jsonSigur); //конвертируем json в строку
            string responseId = stringJsonSigur.Split(new char[] { ',' })[0].Split(new char[] { ':' })[2]; //полученную строку разбиваю и берём нужный мне парамтр
            var responseSigur = new ResponseSigur //формирую тело для будущего json ответа
            {
                confirmedLogId = Convert.ToInt32(responseId),
            };
            string jsonString = System.Text.Json.JsonSerializer.Serialize(responseSigur); // Сериализую тело с параметром id события
            return Ok(jsonString); //отвечаю серверу Sigur id'шником полученного события
        }*/
        
        [HttpGet("GetListEvents")]
        public async Task<ActionResult<IEnumerable<Event>>> GetListEvents()
        {
          if (_context.Events == null)
          {
              return NotFound();
          }
            return await _context.Events.ToListAsync();
        }

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
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

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (_context.Events == null)
            {
                return NotFound();
            }
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
