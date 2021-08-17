﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.GetControllers.Users
{
    [ApiController, Route("users/[controller]")]
    public class GetUserOnlineController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromQuery] long secondId)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                    return Unauthorized("Invalid token");
                if (!Core.UserChecks.IsUserExists(secondId)) return BadRequest("User is not exists");
                if (!Core.UserChecks.CanGetUser(userId, secondId)) return BadRequest("No access");

                try
                {
                    var dataContext = Program.DataContext;
                    var usersOnline = dataContext.UsersOnline;
                    usersOnline.Include(p => p.User).Load();

                    try
                    {
                        var user = usersOnline.First(p => p.User.Id == secondId);
                        var span = TimeSpan.FromTicks(DateTime.Now.Ticks) - TimeSpan.FromTicks(user.LastOnlineTime.Ticks);
                        var res = span.Minutes < 1;
                        return Ok(res);
                    }
                    catch
                    {
                        return Ok(false);
                    }
                }
                catch (Exception e)
                {
                    return Problem(e.Message);
                }
            })).GetAwaiter().GetResult();
        }
    }
}