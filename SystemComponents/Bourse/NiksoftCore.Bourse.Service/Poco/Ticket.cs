using System;
using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullText { get; set; }
        public TicketStatus Status { get; set; }
        public int UserId { get; set; }
        public int? TargetId { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? EditDate { get; set; }

        public virtual BourseUser User { get; set; }
        public virtual BourseUser Target { get; set; }
        public virtual TicketCategory Category { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public virtual ICollection<TicketAnswer> TicketAnswers { get; set; }
    }
}
