using System.Collections.Generic;

namespace NiksoftCore.SystemBase.Service
{
    public class ContentCategory
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string KeyValue { get; set; }
        public int? ParentId { get; set; }

        public virtual ContentCategory Parent { get; set; }
        public virtual ICollection<ContentCategory> Childs { get; set; }
        public virtual ICollection<GeneralContent> Contents { get; set; }
    }
}
