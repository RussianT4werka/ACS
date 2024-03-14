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
    public class VideoStreamsController : ControllerBase
    {
        private readonly AcsContext _context;

        public VideoStreamsController(AcsContext context)
        {
            _context = context;
        }

        // GET: api/VideoStreams
        [HttpGet("GetVideoStreams")]
        public async Task<ActionResult<List<VideoStream>>> GetVideoStreams()
        {
          if (_context.VideoStreams == null)
          {
              return NotFound();
          }
            return await _context.VideoStreams.ToListAsync();
        }

        // GET: api/VideoStreams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VideoStream>> GetVideoStream(int id)
        {
          if (_context.VideoStreams == null)
          {
              return NotFound();
          }
            var videoStream = await _context.VideoStreams.FindAsync(id);

            if (videoStream == null)
            {
                return NotFound();
            }

            return videoStream;
        }

        // PUT: api/VideoStreams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideoStream(int id, VideoStream videoStream)
        {
            if (id != videoStream.Id)
            {
                return BadRequest();
            }

            _context.Entry(videoStream).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoStreamExists(id))
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

        // POST: api/VideoStreams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VideoStream>> PostVideoStream(VideoStream videoStream)
        {
          if (_context.VideoStreams == null)
          {
              return Problem("Entity set 'AcsContext.VideoStreams'  is null.");
          }
            _context.VideoStreams.Add(videoStream);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVideoStream", new { id = videoStream.Id }, videoStream);
        }

        // DELETE: api/VideoStreams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideoStream(int id)
        {
            if (_context.VideoStreams == null)
            {
                return NotFound();
            }
            var videoStream = await _context.VideoStreams.FindAsync(id);
            if (videoStream == null)
            {
                return NotFound();
            }

            _context.VideoStreams.Remove(videoStream);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VideoStreamExists(int id)
        {
            return (_context.VideoStreams?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
