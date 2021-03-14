using System;

namespace VardoneEntities.Models.GeneralModels.Users
{
    public class UpdateUserModel
    {
        public string Username { get; set; } = null;
        public string Email { get; set; } = null;
        public string Description { get; set; } = null;
        private string _base64Image;
        public string Base64Image
        {
            get => _base64Image;
            set
            {
                if (value is not null && Convert.TryFromBase64String(value, new Span<byte>(new byte[value.Length]), out _))
                    _base64Image = value;
            }
        }
    }
}