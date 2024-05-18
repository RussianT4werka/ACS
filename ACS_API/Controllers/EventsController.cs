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
        public async Task<ActionResult<List<Event>>> GetListEvents(DateTime DateFiltr)
        {
            try 
            { 
                if (_context.Events == null)
                {
                    return NotFound();
                }
                else
                {
                    if(DateFiltr.Day == 01 && DateFiltr.Month == 01 && DateFiltr.Year == 0001 && DateFiltr.Hour == 0 && DateFiltr.Minute == 00 && DateFiltr.Second == 00)
                    {
                        return await _context.Events.ToListAsync();
                    }
                    else
                    {
                        //string formattedDate = $"{DateFiltr:yyyy-MM-dd HH:mm:ss}";
                        return await _context.Events.Where(s => Convert.ToDateTime(s.Time).Day == DateFiltr.Day &&
                        Convert.ToDateTime(s.Time).Month == DateFiltr.Month &&
                        Convert.ToDateTime(s.Time).Year == DateFiltr.Year).ToListAsync();
                    }
                }

            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("GetEvent")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            if(id != null)
            {
                return await _context.Events.FirstOrDefaultAsync( s => s.Id == id);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
