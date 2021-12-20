using System;
using System.Collections.Generic;

namespace NiksoftCore.LMS.Service
{
    public class Leason
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string HeaderText { get; set; }
        public string BodyText { get; set; }
        public string HtmlText { get; set; }
        public string FooterText { get; set; }
        public string ImgSrc { get; set; }
        public string VideoSrc { get; set; }
        public string FileSrc { get; set; }
        public int CalendarDayId { get; set; }
        public int CourseId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }

        public virtual CalendarDay CalendarDay { get; set; }
        public virtual Course Course { get; set; }
        public ICollection<LeasonFile> LeasonFiles { get; set; }
    }
}
