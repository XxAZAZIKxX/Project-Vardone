using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetUserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] string username, [FromHeader] string token,
            [FromQuery] string usernameUser)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("Empty username");
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (string.IsNullOrWhiteSpace(usernameUser)) return BadRequest("Empty user username");
            if (username == usernameUser) return BadRequest("Username equal user username");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { Username = username, Token = token }))
                return Unauthorized("Invalid token");
            if (!Core.UserChecks.IsUserExists(usernameUser)) return BadRequest("User does not exist");

            var users = Program.DataContext.Users;
            users.Include(p => p.Info).Load();

            try
            {
                var user = users.First(p => p.Username == usernameUser);
                return new JsonResult(JsonConvert.SerializeObject(new GetUserModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Description = user.Info?.Description,
                    Base64Avatar = user.Info?.Avatar == null ? null : Convert.ToBase64String(user.Info.Avatar)
                }));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}