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

        // POST: api/VideoStreams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("AddStream")]
        public async Task<ActionResult<VideoStream>> AddStream(VideoStream videoStream)
        {
            try
            {
                _context.VideoStreams.Add(videoStream);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);
            }
        }

        [HttpPost("DelStream")]
        public async Task<ActionResult<VideoStream>> DelStream(VideoStream videoStream)
        {
            try
            {
                _context.VideoStreams.Remove(videoStream);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
