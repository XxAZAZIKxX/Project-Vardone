using System;

namespace VardoneEntities.Entities.User
{
    public record AdditionalUserInformation
    {
        public string Email { get; init; }
        public string Phone { get; init; }
        public DateTime? BirthDate { get; set; }
        public string FullName { get; set; }
    }
}