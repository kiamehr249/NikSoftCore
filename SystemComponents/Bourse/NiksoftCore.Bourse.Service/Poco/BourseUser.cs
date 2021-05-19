using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class BourseUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public virtual ICollection<UserProfile> UserProfiles { get; set; }
        public virtual ICollection<BranchMaster> BranchMasters { get; set; }
        public virtual ICollection<BranchMarketer> BranchMarketers { get; set; }
        public virtual ICollection<BranchConsultant> BranchConsultants { get; set; }
        public virtual ICollection<BranchConsultant> ConsultantMarketers { get; set; }
        public virtual ICollection<BranchUser> BranchUsers { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Media> Medias { get; set; }
    }
}
