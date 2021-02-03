using System.Collections.Generic;

namespace NiksoftCore.SystemBase.Service
{
    public class NikRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }

        //public virtual ICollection<NikUserRole> NikUserRoles { get; set; }
    }
}
