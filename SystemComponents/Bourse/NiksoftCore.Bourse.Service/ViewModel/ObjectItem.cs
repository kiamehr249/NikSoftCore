using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class ObjectItem
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string KeyName { get; set; }
        public string TextValue { get; set; }
        public int? ParentId { get; set; }
        public List<ObjectItem> Items { get; set; }
    }
}
