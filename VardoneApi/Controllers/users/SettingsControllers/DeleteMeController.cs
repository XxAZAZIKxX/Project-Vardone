using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users.SettingsControllers
{
    [ApiController, Route("users/[controller]")]
    public class DeleteMeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { UserId = userId, Token = token }))
                return Unauthorized();

            var users = Program.DataContext.Users;

            try
            {
                users.Remove(users.First(p => p.Id == userId));
                Program.DataContext.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}