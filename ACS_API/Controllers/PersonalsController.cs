using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBD.BD;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace ACS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalsController : ControllerBase
    {
        private readonly AcsContext _context;

        public PersonalsController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("GetPersonals")]
        public async Task<ActionResult<List<Personal>>> GetPersonals()
        {
            if (_context.Personals == null)
            {
                return NotFound();
            }
            return await _context.Personals.ToListAsync();
        }

        [HttpPost("EditPersonal")]
        public async Task<ActionResult> EditPersonal(Personal personal)
        {
            if (personal == null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    var editPers = _context.Personals.FirstOrDefault(s => s.Id == personal.Id);
                    editPers.Fio = personal.Fio;
                    editPers.Department = personal.Department;
                    editPers.Position = personal.Position;
                    editPers.Hex = personal.Hex;

                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch
                {
                    return BadRequest();
                }
            }
        }

        [HttpPost("CreatePersonal")]
        public async Task<ActionResult> CreatePersonal(Personal personal)
        {
            if (personal == null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    var Pers = new Personal() { Fio = personal.Fio, Department = personal.Department, Position = personal.Position, Hex = personal.Hex };
                    _context.Personals.Add(Pers);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch
                {
                    return BadRequest();
                }
            }
        }

        [HttpPost("DelPers")]
        public async Task<ActionResult> DelPers(Personal personal)
        {
            if (personal == null)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    var editPers = _context.Personals.FirstOrDefault(s => s.Id == personal.Id);
                    _context.Personals.Remove(editPers);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch
                {
                    return BadRequest();
                }
            }
        }
    }
}
