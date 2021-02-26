using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Entity.Models;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users
{
    [ApiController, Route("users/[controller]")]
    public class RegisterController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] RegisterRequestModel registerRequestModel)
        {
            if (registerRequestModel == null) return BadRequest();
            var users = Program.DataContext.Users;
            
            try
            {
                var unused = users.First(u => u.Username == registerRequestModel.Username);
                return BadRequest("Username is already booked");
            }
            catch
            {
                // ignored
            }

            var user = new Users
            {
                Username = registerRequestModel.Username, Email = registerRequestModel.Email,
                Password = registerRequestModel.Password
            };

            users.Add(user);
            try
            {
                Program.DataContext.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest("Error");
            }
        }
    }
}