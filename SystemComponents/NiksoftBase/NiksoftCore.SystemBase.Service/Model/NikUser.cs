using System.Collections.Generic;

namespace NiksoftCore.SystemBase.Service
{
    public class NikUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        //public virtual ICollection<NikUserRole> NikUserRoles { get; set; }
    }
}
