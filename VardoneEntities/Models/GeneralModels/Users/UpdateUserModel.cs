using System;

namespace VardoneEntities.Models.GeneralModels.Users
{
    public record UpdateUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Base64Image { get; set; }

        public string Phone { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Position { get; set; }
    }
}