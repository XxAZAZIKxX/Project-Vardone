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
        public IActionResult Post([FromHeader] string username, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("Empty username");
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (!Core.UserChecks.CheckToken(new TokenUserModel {Username = username, Token = token}))
                return Unauthorized();
            
            var users = Program.DataContext.Users;
            
            try
            {
                users.Remove(users.First(p => p.Username == username));
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