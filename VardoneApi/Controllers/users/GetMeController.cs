using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users
{
    [ApiController, Route("users/[controller]")]
    public class GetMeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            if (!Core.UserChecks.CheckToken(new TokenUserModel { Username = username, Token = token }))
                return Unauthorized("Invalid token");

            try
            {
                var users = Program.DataContext.Users;
                Program.DataContext.Users.Include(p=>p.Info).Load();

                var user = users.First(p => p.Username == username);
                return new JsonResult(JsonConvert.SerializeObject(new GetMeModel
                {
                    Username = user.Username,
                    Email = user.Email,
                    Description = user.Info?.Description
                }));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}