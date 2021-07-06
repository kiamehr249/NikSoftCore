using System.Collections.Generic;

namespace NiksoftCore.SystemBase.Service
{
    public class Country
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Province> Provinces { get; set; }
        public virtual ICollection<City> Cities { get; set; }
    }
}
