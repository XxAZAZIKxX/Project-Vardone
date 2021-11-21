using System;

namespace VardoneEntities.Entities.Guild
{
    public class BannedMember
    {
        public User BannedUser { get; set; }
        public User BannedByUser { get; set; }
        public string Reason { get; set; }
        public DateTime BanDateTime { get; set; }
        public Guild Guild { get; set; }
    }
}