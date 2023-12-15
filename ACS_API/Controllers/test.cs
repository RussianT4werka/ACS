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
    public class test : ControllerBase
    {
        private readonly AcsContext _context;

        public test(AcsContext context)
        {
            _context = context;
        }

        // GET: api/test
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriberTelegramBot>>> GetSubscriberTelegramBots()
        {
          if (_context.SubscriberTelegramBots == null)
          {
              return NotFound();
          }
            return await _context.SubscriberTelegramBots.ToListAsync();
        }

        // GET: api/test/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriberTelegramBot>> GetSubscriberTelegramBot(int id)
        {
          if (_context.SubscriberTelegramBots == null)
          {
              return NotFound();
          }
            var subscriberTelegramBot = await _context.SubscriberTelegramBots.FindAsync(id);

            if (subscriberTelegramBot == null)
            {
                return NotFound();
            }

            return subscriberTelegramBot;
        }

        // PUT: api/test/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscriberTelegramBot(int id, SubscriberTelegramBot subscriberTelegramBot)
        {
            if (id != subscriberTelegramBot.ChatId)
            {
                return BadRequest();
            }

            _context.Entry(subscriberTelegramBot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriberTelegramBotExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/test
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SubscriberTelegramBot>> PostSubscriberTelegramBot(SubscriberTelegramBot subscriberTelegramBot)
        {
          if (_context.SubscriberTelegramBots == null)
          {
              return Problem("Entity set 'AcsContext.SubscriberTelegramBots'  is null.");
          }
            _context.SubscriberTelegramBots.Add(subscriberTelegramBot);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubscriberTelegramBotExists(subscriberTelegramBot.ChatId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSubscriberTelegramBot", new { id = subscriberTelegramBot.ChatId }, subscriberTelegramBot);
        }

        // DELETE: api/test/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscriberTelegramBot(int id)
        {
            if (_context.SubscriberTelegramBots == null)
            {
                return NotFound();
            }
            var subscriberTelegramBot = await _context.SubscriberTelegramBots.FindAsync(id);
            if (subscriberTelegramBot == null)
            {
                return NotFound();
            }

            _context.SubscriberTelegramBots.Remove(subscriberTelegramBot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriberTelegramBotExists(int id)
        {
            return (_context.SubscriberTelegramBots?.Any(e => e.ChatId == id)).GetValueOrDefault();
        }
    }
}
