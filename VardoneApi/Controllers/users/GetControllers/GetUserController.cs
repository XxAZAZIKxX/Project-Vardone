using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetUserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (userId == secondId) return BadRequest("Username equal user userId");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest("User does not exist");
                if (!Core.UserChecks.CanGetUser(userId, secondId)) return BadRequest("You not allowed to do this");

                var users = Program.DataContext.Users;
                users.Include(p => p.Info).Load();

                try
                {
                    var user = users.First(p => p.Id == secondId);
                    return new JsonResult(JsonConvert.SerializeObject(new User
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        Description = user.Info?.Description,
                        Base64Avatar = user.Info?.Avatar == null ? null : Convert.ToBase64String(user.Info.Avatar)
                    }));
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            })).GetAwaiter().GetResult();
        }
    }
}