using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetFriendsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token }))
                return Unauthorized("Invalid token");

            var friendsList = Program.DataContext.FriendsList;
            friendsList.Include(p => p.From).Load();
            friendsList.Include(p => p.To).Load();
            friendsList.Include(p => p.From.Info).Load();
            friendsList.Include(p => p.To.Info).Load();

            var users = new List<User>();

            foreach (var row in friendsList.Where(p => p.From.Id == userId || p.To.Id == userId))
            {
                var friend = row.From.Id != userId ? row.From : row.To;

                users.Add(new User
                {
                    Id = friend.Id,
                    Username = friend.Username,
                    Base64Avatar = friend.Info?.Avatar == null ? null : Convert.ToBase64String(friend.Info.Avatar),
                    Description = friend.Info?.Description
                });
            }

            return new JsonResult(JsonConvert.SerializeObject(users));
        }
    }
}