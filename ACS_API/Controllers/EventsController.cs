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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<ActionResult<List<Event>>> GetListEvents(string DateFiltr)
        { // "2024-05-18 00:00:00" = "0001-01-01 00:00:00"
            try 
            { 
                if (_context.Events == null)
                {
                    return NotFound();
                }
                else
                {
                    if(Convert.ToDateTime(DateFiltr).Day == 01 && Convert.ToDateTime(DateFiltr).Month == 01 && Convert.ToDateTime(DateFiltr).Year == 0001 && Convert.ToDateTime(DateFiltr).Hour == 0 && Convert.ToDateTime(DateFiltr).Minute == 00 && Convert.ToDateTime(DateFiltr).Second == 00)
                    {
                        return await _context.Events.ToListAsync();
                    }
                    else
                    {
                        var listEvent = await _context.Events.ToListAsync();
                        var filtrListEvent = listEvent.Where(s => Convert.ToDateTime(s.Time).Date == Convert.ToDateTime(DateFiltr).Date).ToList();
                        await _context.Database.CloseConnectionAsync();
                        return filtrListEvent;
                    }
                }

            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetEvent")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            try
            {
                if (id != null)
                {
                    var evenT = await _context.Events.FirstOrDefaultAsync(s => s.Id == id);
                    await _context.Database.CloseConnectionAsync();
                    return evenT;
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
