using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBD.BD;
using Humanizer;

namespace ACS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriberTelegramBotsController : ControllerBase
    {
        private readonly AcsContext _context;

        public SubscriberTelegramBotsController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("GetListSubscribers")]
        public async Task<ActionResult<IEnumerable<SubscriberTelegramBot>>> GetListSubscribers()
        {
            try
            {
                if (_context.SubscriberTelegramBots == null)
                {
                    return NotFound();
                }
                return await _context.SubscriberTelegramBots.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [HttpPost("SubscribeOrNot")]
        public async Task<ActionResult> SubscribeOrNot(SubscriberTelegramBot data)
        {
            try
            {
                var sub = await _context.SubscriberTelegramBots.FirstOrDefaultAsync(s => s.ChatId == data.ChatId);
                if (sub == null)
                    return NotFound();
                if (sub.SubscribeOrNot == 0)
                {
                    sub.SubscribeOrNot = 1;
                }
                else
                {
                    sub.SubscribeOrNot = 0;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [HttpGet("GetSubscriber")]
        public async Task<ActionResult<int?>> GetSubscriber(string id)
        {
            try
            {
                if (_context.SubscriberTelegramBots == null)
                {
                    return NotFound();
                }
                var subscriberTelegramBot = await _context.SubscriberTelegramBots.FirstOrDefaultAsync(s => s.ChatId == id);

                if (subscriberTelegramBot == null)
                {
                    return NotFound();
                }

                return subscriberTelegramBot.SubscribeOrNot;
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("GetSubscriberCheckNull")]
        public async Task<ActionResult<long>> GetSubscriberCheckNull(long id)
        {
            try
            {
                if (_context.SubscriberTelegramBots == null)
                {
                    return NotFound();
                }
                var subscriberTelegramBot = await _context.SubscriberTelegramBots.FirstOrDefaultAsync(s => s.ChatId == Convert.ToString(id));

                if (subscriberTelegramBot == null)
                {
                    return 0;
                }

                return 1;
            }
            catch (Exception ex)
            {
                return BadRequest($"Метод для проверки пользователя в подписчиках выдал ошибку:{ex}");
            }
        }

        [HttpPost("AddSubscriber")]
        public async Task<ActionResult> AddSubscriber(SubscriberTelegramBot sub)
        {
            try
            {
                await _context.SubscriberTelegramBots.AddRangeAsync(sub);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            return NoContent();
        }

        [HttpPost("DelSubscriber")]
        public async Task<ActionResult> DelSubscriber(SubscriberTelegramBot sub)
        {
            try
            {
                 _context.SubscriberTelegramBots.Remove(sub);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return NoContent();
        }
    }
}
