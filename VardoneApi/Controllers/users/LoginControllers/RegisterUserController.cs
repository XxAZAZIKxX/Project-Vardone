using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Entity.Models;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.LoginControllers
{
    [ApiController, Route("users/[controller]")]
    public class RegisterUserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] RegisterUserModel registerRequestModel)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (registerRequestModel == null) return BadRequest();

                var users = Program.DataContext.Users;
                try
                {
                    var _ = users.First(u => u.Email == registerRequestModel.Email);
                    return BadRequest("Email is already booked");
                }
                catch
                {
                    // ignored
                }

                var user = new UsersTable
                {
                    Username = registerRequestModel.Username,
                    Email = registerRequestModel.Email,
                    Password = registerRequestModel.Password
                };

                users.Add(user);

                try
                {
                    Program.DataContext.SaveChanges();
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