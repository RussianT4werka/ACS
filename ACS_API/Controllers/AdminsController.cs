using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBD.BD;
using ACS_API.Tools;
using Humanizer;
using ACS_API.DTO;

namespace ACS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly AcsContext _context;

        public AdminsController(AcsContext context)
        {
            _context = context;
        }

        [HttpPost("Registration")]
        public async Task<ActionResult<int>> Registration([FromBody] UserData user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Login) ||
                    string.IsNullOrWhiteSpace(user.Password) ||
                    string.IsNullOrWhiteSpace(user.Email))
                    return BadRequest("Не все поля заполнены.");

                var checkUserEmail = await _context.Admins.AnyAsync(s => s.Email == user.Email);
                var checkUserLogin = await _context.Admins.AnyAsync(s => s.Login == user.Login);
                if (checkUserEmail)
                    return BadRequest("Email уже используется.");
                if (checkUserLogin)
                    return BadRequest("Логин уже используется");

                string hashPass = Hash.HashPassword(user);

                Admin newAdmin = new Admin
                {
                    Surname = user.Surname,
                    Name = user.Name,
                    Patronymic = user.Patronymic,
                    Login = user.Login,
                    Password = hashPass,
                    Email = user.Email,
                };

                await _context.Admins.AddAsync(newAdmin);
                await _context.SaveChangesAsync();
                await Mail.SendEmailAsync(user.Email, user.Login, user.Password);
                return Ok(newAdmin.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Authorization")]
        public async Task<ActionResult<int>> Authorization([FromBody] UserData user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Login) || string.IsNullOrWhiteSpace(user.Password))
                {
                    return BadRequest("Не все поля заполнены. Введите логин и пароль.");
                }
                else
                {
                    var findUser = await _context.Admins.FirstOrDefaultAsync(s => s.Login == user.Login);
                    if (findUser == null)
                    {
                        return BadRequest("Логина не существует. Убедитесь в правильности введённого логина или свяжитесь с администратором.");
                    }
                    else
                    {
                        string hashPass = Hash.HashPassword(user);
                        if (findUser.Password != hashPass)
                        {
                            return BadRequest("Пароль для аккаунта неверный");
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(user.Login) &&
                            !string.IsNullOrWhiteSpace(user.Password) &&
                            findUser.Login != null && findUser.Password == hashPass)
                            {
                                return Ok(findUser.Id);
                            }

                            return NoContent();
                        }

                        
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAdminInfo")]
        public async Task<ActionResult<DTO.UserData>> GetAdminInfo(int id)
        {
            try
            {
                var user = await _context.Admins.
                    FirstOrDefaultAsync(s => s.Id == id);
                if (user == null)
                    return NotFound();

                UserData userDTO = new() {Id = user.Id, Name = user.Name, Surname = user.Surname, Patronymic = user.Patronymic,
                                            Email = user.Email, Login = user.Login, Password = user.Password};
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [HttpPost("EditAdmin")]
        public async Task<ActionResult> EditAdmin([FromBody] UserData userData)
        {
            try
            {
                if (userData != null)
                {
                    var user = await _context.Admins.FirstOrDefaultAsync(s => s.Id == userData.Id);
                    if(user != null && userData.Password != user.Password)
                    {
                        user.Name = userData.Name;
                        user.Surname = userData.Surname;
                        user.Patronymic = userData.Patronymic;
                        user.Login = userData.Login;
                        string hashPass = Hash.HashPassword(userData);
                        user.Password = hashPass;
                        _context.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        user.Name = userData.Name;
                        user.Surname = userData.Surname;
                        user.Patronymic = userData.Patronymic;
                        user.Login = userData.Login;
                        _context.SaveChanges();
                        return Ok();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
