using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.Settings
{
    [ApiController, Route("users/[controller]")]
    public class DeleteMeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized();

                var dataContext = Program.DataContext;
                var users = dataContext.Users;

                try
                {
                    users.Remove(users.First(p => p.Id == userId));
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            })).GetAwaiter().GetResult();
        }
    }
}