using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Core;
using VardoneApi.Entity.Models;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.guilds
{
    [ApiController, Route("guilds/[controller]")]
    public class CreateGuildController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] string name)
        {
            if (!UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

            try
            {
                var dataContext = Program.DataContext;
                var guilds = dataContext.Guilds;
                var users = dataContext.Users;

                var user = users.First(p => p.Id == userId);

                guilds.Add(new GuildsTable
                {
                    CreatedAt = DateTime.Now,
                    Name = name ?? $"{user.Username}'s server",
                    Owner = user
                });

                dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

            return Ok("Created");
        }
    }
}