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

        [HttpGet("GetVideoStreams")]
        public async Task<ActionResult<List<VideoStream>>> GetVideoStreams()
        {
            try
            {
                if (_context.VideoStreams == null)
                {
                    return NotFound();
                }
                return await _context.VideoStreams.ToListAsync();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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

        [HttpPost("EditStream")]
        public async Task<ActionResult<VideoStream>> EditStream(VideoStream videoStream)
        {
            try
            {
                var oldStream = await _context.VideoStreams.FirstOrDefaultAsync(s => s.Id == videoStream.Id);
                oldStream.Name = videoStream.Name;
                oldStream.Link = videoStream.Link;
                oldStream.LinkOpenDoor = videoStream.LinkOpenDoor;
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
