using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using VardoneApi.Entity.Models;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users
{
    [ApiController, Route("users/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] LoginRequestModel loginRequestModel)
        {
            if (loginRequestModel == null) return BadRequest();
            var users = Program.DataContext.Users;
            var tokens = Program.DataContext.Tokens;

            Users user;
            try
            {
                user = users.First(usr => usr.Username == loginRequestModel.Username && usr.Password == loginRequestModel.Password);
            }
            catch (Exception)
            {
                return BadRequest("User invalid");
            }

            var newToken = new Tokens()
            {
                User = user, CreatedAt = DateTime.Now, IpAddress = loginRequestModel.IpAddress,
                MacAddress = loginRequestModel.MacAddress,
                Token = GenerateToken((int) new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
            };

            tokens.Add(newToken);

            var response = new LoginResponseModel {Token = newToken.Token, Username = user.Username};

            Program.DataContext.SaveChanges();

            return new JsonResult(response);
        }

        private static string GenerateToken(int concretize = 0)
        {
            var rd = new Random();
            return CreateMd5((rd.Next(100000, 999999) + concretize).ToString());
        }

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