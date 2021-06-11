using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class BranchAdvertiser : LogModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int LeaderId { get; set; }

        public virtual BourseUser User { get; set; }
        public virtual BourseUser AdLeader { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
