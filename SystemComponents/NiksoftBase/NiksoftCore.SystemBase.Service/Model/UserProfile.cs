using System;

namespace NiksoftCore.SystemBase.Service
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public int UserId { get; set; }
        public string Avatar { get; set; }

        public virtual NikUser User { get; set; } 
    }
}
