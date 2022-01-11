using System;

namespace VardoneEntities.Models.GeneralModels.Users
{
    public record UpdateUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
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

        public string Phone { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; } = null;
    }
}