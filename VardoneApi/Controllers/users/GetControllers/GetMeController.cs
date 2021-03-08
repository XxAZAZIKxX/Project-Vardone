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
        public IActionResult Post([FromHeader] string username, [FromHeader] string token)
        {
            if (string.IsNullOrWhiteSpace(username)) return BadRequest("Empty username");
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
            if (!Core.UserChecks.CheckToken(new TokenUserModel { Username = username, Token = token }))
                return Unauthorized("Invalid token");

            try
            {
                var users = Program.DataContext.Users;
                Program.DataContext.Users.Include(p => p.Info).Load();

                var user = users.First(p => p.Username == username);
                return new JsonResult(JsonConvert.SerializeObject(new GetMeModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Description = user.Info?.Description,
                    Base64Avatar = user.Info == null ? null : Convert.ToBase64String(user.Info.Avatar)
                }));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}