using System;
using System.Collections.Generic;

namespace NiksoftCore.LMS.Service
{
    public class CalendarDay
    {
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public DateTime DayDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public virtual Calendar Calendar { get; set; }
        public virtual ICollection<Leason> Leasons { get; set; }
    }
}
