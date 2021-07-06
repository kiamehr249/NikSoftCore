using System.Collections.Generic;

namespace NiksoftCore.SystemBase.Service
{
    public class PanelMenu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public string Controller { get; set; }
        public string Roles { get; set; }
        public bool Enabled { get; set; }
        public int Ordering { get; set; }
        public int? ParentId { get; set; }

        public virtual PanelMenu Parent { get; set; }
        public virtual ICollection<PanelMenu> Childs { get; set; }

    }
}
