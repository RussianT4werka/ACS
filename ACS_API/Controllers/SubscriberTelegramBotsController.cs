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

        // GET: api/SubscriberTelegramBots
        [HttpGet("GetSubscriberTelegramBots")]
        public async Task<ActionResult<IEnumerable<SubscriberTelegramBot>>> GetSubscriberTelegramBots()
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
                if(sub.SubscribeOrNot == 0)
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
        public async Task<ActionResult<int?>> GetSubscriber(int id)
        {
            if (_context.SubscriberTelegramBots == null)
            {
                return NotFound();
            }
            var subscriberTelegramBot = await _context.SubscriberTelegramBots.FirstOrDefaultAsync( s => s.ChatId == id);

            if (subscriberTelegramBot == null)
            {
                return NotFound();
            }

            return subscriberTelegramBot.SubscribeOrNot;
        }

        [HttpGet("GetSubscriberCheckNull")]
        public async Task<ActionResult<SubscriberTelegramBot>> GetSubscriberCheckNull(long id)
        {
            if (_context.SubscriberTelegramBots == null)
            {
                return NotFound();
            }
            var subscriberTelegramBot = await _context.SubscriberTelegramBots.FirstOrDefaultAsync(s => s.ChatId == (int)id);

            if (subscriberTelegramBot == null)
            {
                return NotFound();
            }

            return subscriberTelegramBot;
        }
    }
}
