using System;

namespace NiksoftCore.LMS.Service
{
    public class UserCourse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        public virtual LmsUser User { get; set; }
        public virtual Course Course { get; set; }
    }
}
