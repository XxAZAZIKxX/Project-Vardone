using System;

namespace VardoneApi.Models.Users
{
    public class UpdateUserModel
    {
        public string Username { get; set; } = null;
        public string Password { get; set; } = null;
        public string Email { get; set; } = null;
        public string Description { get; set; } = null;
        public string Base64Image { get; set; }
        public byte[] GetImageBytes() => Base64Image is null ? null : Convert.FromBase64String(Base64Image);
    }
}