using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class BranchArea : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }
    }
}
