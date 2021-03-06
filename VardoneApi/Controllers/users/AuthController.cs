using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Entity.Models;
using VardoneApi.Models.Users;

namespace VardoneApi.Controllers.users
{
    [ApiController, Route("users/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] LoginUserModel loginRequestModel)
        {
            if (loginRequestModel == null) return BadRequest("Empty model");

            var users = Program.DataContext.Users;
            var tokens = Program.DataContext.Tokens;
            tokens.Include(p=>p.User).Load();
            Users user;
            try
            {
                user = users.First(usr => usr.Username == loginRequestModel.Username && usr.Password == loginRequestModel.Password);
            }
            catch (Exception)
            {
                return BadRequest("User invalid");
            }

            var newToken = new Tokens
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
                    t.User.Username == loginRequestModel.Username && t.MacAddress == loginRequestModel.MacAddress &&
                    t.IpAddress == loginRequestModel.IpAddress);
                tokens.RemoveRange(remove);
            }
            catch
            {
                // ignored
            }

            tokens.Add(newToken);
            Program.DataContext.SaveChanges();
            var response = new TokenUserModel { Token = newToken.Token, Username = user.Username };
            return new JsonResult(response);
        }

        private static string GenerateToken() =>
            CreateMd5(((int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()).ToString());

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