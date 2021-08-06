using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.Settings
{
    [ApiController, Route("users/[controller]")]
    public class SetOnlineController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                var usersOnline = dataContext.UsersOnline;
                usersOnline.Include(p => p.User).Load();

                try
                {
                    var user = usersOnline.First(p => p.User.Id == userId);
                    user.LastOnlineTime = DateTime.Now;
                    usersOnline.Update(user);
                    dataContext.SaveChanges();
                    return Ok();
                }
                catch
                {
                    try
                    {
                        usersOnline.Add(new UsersOnlineTable
                        {
                            User = users.First(p => p.Id == userId),
                            LastOnlineTime = DateTime.Now
                        });
                        dataContext.SaveChanges();
                        return Ok();
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e);
                    }
                }
            })).GetAwaiter().GetResult();
        }
    }
}