using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class Branch : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }

        public virtual ICollection<BranchMaster> BranchMasters { get; set; }
        public virtual ICollection<BranchMarketer> BranchMarketers { get; set; }
        public virtual ICollection<BranchConsultant> BranchConsultants { get; set; }
        public virtual ICollection<BranchUser> BranchUsers { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<Media> Medias { get; set; }
    }
}
