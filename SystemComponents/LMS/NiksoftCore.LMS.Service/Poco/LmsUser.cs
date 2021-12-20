using System.Collections.Generic;

namespace NiksoftCore.LMS.Service
{
    public class LmsUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public virtual ICollection<UserCourse> UserCourses { get; set; }
    }
}
