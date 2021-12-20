using System;
using System.Collections.Generic;

namespace NiksoftCore.LMS.Service
{
    public class Calendar
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }

        public virtual ICollection<CalendarDay> CalendarDays { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}
