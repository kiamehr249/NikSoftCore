using System;
using System.Collections.Generic;

namespace NiksoftCore.LMS.Service
{
    public class Term
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImgSrc { get; set; }
        public int Days { get; set; }
        public DateTime StarDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}
