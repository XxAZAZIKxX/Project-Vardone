using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.Settings
{
    [ApiController, Route("users/[controller]")]
    public class UpdatePasswordController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromHeader] long userId, [FromHeader] string token, [FromBody] UpdatePasswordModel updatePasswordModel)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (string.IsNullOrWhiteSpace(token)) return BadRequest("Empty token");
                if (updatePasswordModel is null) return BadRequest("Empty updatePasswordModel");
                if (!Core.UserChecks.CheckToken(new UserTokenModel { UserId = userId, Token = token })) return Unauthorized("Invalid token");

                var dataContext = Program.DataContext;
                try
                {
                    var users = dataContext.Users;
                    var user = users.First(p => p.Id == userId);

                    if (updatePasswordModel.PreviousPassword != user.Password) return BadRequest("Incorrect previous password");
                    if (string.IsNullOrWhiteSpace(updatePasswordModel.NewPassword))
                        return BadRequest("Incorrect new password");
                    user.Password = updatePasswordModel.NewPassword;
                    users.Update(user);
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