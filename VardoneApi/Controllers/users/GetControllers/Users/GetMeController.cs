using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.GetControllers.Users
{
    [ApiController, Route("users/[controller]")]
    public class GetMeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");

                try
                {
                    var dataContext = Program.DataContext;
                    var users = dataContext.Users;
                    dataContext.Users.Include(p => p.Info).Load();

                    var user = users.First(p => p.Id == userId);
                    return new JsonResult(JsonConvert.SerializeObject(new User
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        Email = user.Email,
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