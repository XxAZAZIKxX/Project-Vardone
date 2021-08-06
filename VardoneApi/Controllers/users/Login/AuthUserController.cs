using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Models.ApiModels;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Controllers.users.Login
{
    [ApiController, Route("users/[controller]")]
    public class AuthUserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] GetUserTokenApiModel loginRequestModel)
        {
            return Task.Run(new Func<IActionResult>(() =>
            {
                if (loginRequestModel == null) return BadRequest("Empty model");

                var dataContext = Program.DataContext;
                var users = dataContext.Users;
                var tokens = dataContext.Tokens;
                tokens.Include(p => p.User).Load();
                UsersTable user;
                try
                {
                    user = users.First(usr =>
                        usr.Email == loginRequestModel.Email && usr.Password == loginRequestModel.Password);
                }
                catch
                {
                    return BadRequest("Login failed");
                }

                var newToken = new TokensTable
                {
                    User = user,
                    CreatedAt = DateTime.Now,
                    IpAddress = loginRequestModel.IpAddress,
                    MacAddress = loginRequestModel.MacAddress,
                    Token = GenerateToken()
                };

                try
                {
                    var remove = tokens.First(t =>
                        t.User.Email == loginRequestModel.Email && t.MacAddress == loginRequestModel.MacAddress &&
                        t.IpAddress == loginRequestModel.IpAddress);
                    tokens.RemoveRange(remove);
                }
                catch
                {
                    // ignored
                }

                tokens.Add(newToken);
                dataContext.SaveChanges();
                var response = new UserTokenModel {Token = newToken.Token, UserId = user.Id};
                return new JsonResult(response);
            })).GetAwaiter().GetResult();
        }

        private static string GenerateToken() => CreateMd5(((int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()).ToString());

        private static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var t in hashBytes) sb.Append(t.ToString("X2"));
            return sb.ToString();
        }
    }
}