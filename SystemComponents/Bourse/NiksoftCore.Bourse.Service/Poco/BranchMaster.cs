using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class BranchMaster : LogModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }

        public virtual BourseUser User { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
