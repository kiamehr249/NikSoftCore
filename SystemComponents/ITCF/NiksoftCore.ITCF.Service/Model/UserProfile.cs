using System;

namespace NiksoftCore.ITCF.Service
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

        public virtual UserModel User { get; set; }
    }
}
