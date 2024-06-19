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
        private List<Personal> ListPersonal { get; set; } = new();

        public PersonalsController(AcsContext context)
        {
            _context = context;
        }

        [HttpGet("GetPersonals")]
        public async Task<ActionResult<List<Personal>>> GetPersonals(int adminOn)
        {
            try
            {
                if(adminOn == 0)
                {
                    if (_context.Personals == null)
                    {
                        return NotFound();
                    }
                    ListPersonal = await _context.Personals.ToListAsync();
                    await _context.Database.CloseConnectionAsync();
                    return ListPersonal;
                }
                else if (adminOn == 1)
                {
                    var admins = await _context.Admins.ToListAsync();
                    await _context.Database.CloseConnectionAsync();
                    foreach(var newPers in admins)
                    {
                        string fio = $"{newPers.Surname} {newPers.Name} {newPers.Patronymic}";
                        Personal newPersAdmin = new() { Fio = fio, Department = "Охрана", Position = "Администратор" };
                        ListPersonal.Add(newPersAdmin);
                    }
                    return ListPersonal;
                }
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("EditPersonal")]
        public async Task<ActionResult> EditPersonal(Personal personal)
        {
            try
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
                        editPers.W26 = personal.W26;

                        await _context.SaveChangesAsync();
                        await _context.Database.CloseConnectionAsync();
                        return Ok();
                    }
                    catch
                    {
                        return BadRequest();
                    }
                }
            }
            catch(Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreatePersonal")]
        public async Task<ActionResult> CreatePersonal(Personal personal)
        {
            try
            {
                if (personal == null)
                {
                    return BadRequest();
                }
                else
                {
                    try
                    {
                        var Pers = new Personal() { Fio = personal.Fio, Department = personal.Department, Position = personal.Position, W26 = personal.W26 };
                        _context.Personals.Add(Pers);
                        await _context.SaveChangesAsync();
                        await _context.Database.CloseConnectionAsync();
                        return Ok();
                    }
                    catch
                    {
                        return BadRequest();
                    }
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DelPers")]
        public async Task<ActionResult> DelPers(Personal personal)
        {
            try
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
                        await _context.Database.CloseConnectionAsync();
                        return Ok();
                    }
                    catch(Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
