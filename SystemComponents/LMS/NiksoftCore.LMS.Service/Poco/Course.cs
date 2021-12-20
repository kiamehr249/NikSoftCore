using System;
using System.Collections.Generic;

namespace NiksoftCore.LMS.Service
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImgSrc { get; set; }
        public long Price { get; set; }
        public int CalendarId { get; set; }
        public int TermId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }

        public virtual Calendar Calendar { get; set; }
        public virtual Term Term { get; set; }
        public virtual ICollection<UserCourse> UserCourses { get; set; }
        public virtual ICollection<Leason> Leasons { get; set; }
    }
}
