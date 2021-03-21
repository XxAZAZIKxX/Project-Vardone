using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.SettingsControllers
{
    [ApiController, Route("users/[controller]")]
    public class CloseCurrentSessionController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                var dataContext = Program.DataContext;
                var tokens = dataContext.Tokens;
                tokens.Include(p => p.User).Load();

                try
                {
                    var first = tokens.First(p => p.User.Id == userId && p.Token == token);
                    tokens.Remove(first);
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch
                {
                    return BadRequest();
                }
            })).GetAwaiter().GetResult();
        }
    }
}