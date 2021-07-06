using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.SystemBase.Service
{
    public class MenuCategory : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string KeyValue { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool Enabled { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
