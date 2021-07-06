using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.SystemBase.Service
{
    public class Menu : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool Enabled { get; set; }
        public int OrderId { get; set; }
        public int CategoryId { get; set; }
        public int? ParentId { get; set; }

        public virtual MenuCategory MenuCategory { get; set; }
        public virtual Menu Parent { get; set; }
        public virtual ICollection<Menu> Childs { get; set; }

    }
}
