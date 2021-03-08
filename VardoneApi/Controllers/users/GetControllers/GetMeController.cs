using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users.GetControllers
{
    [ApiController, Route("users/[controller]")]
    public class GetMeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { UserId = userId, Token = token }))
                return Unauthorized("Invalid token");

            try
            {
                var users = Program.DataContext.Users;
                Program.DataContext.Users.Include(p => p.Info).Load();

                var user = users.First(p => p.Id == userId);
                return new JsonResult(JsonConvert.SerializeObject(new GetMeModel
                {
                    Id = user.Id,
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
        }
    }
}