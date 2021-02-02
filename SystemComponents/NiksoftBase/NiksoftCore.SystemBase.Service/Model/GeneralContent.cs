using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.SystemBase.Service
{
    public class GeneralContent : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Header { get; set; }
        public string BodyText { get; set; }
        public string Footer { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string KeyValue { get; set; }
        public string Link { get; set; }
        public string LinkTitle { get; set; }
        public bool Enabled { get; set; }
        public int CategoryId { get; set; }

        public virtual ContentCategory ContentCategory { get; set; }
        public virtual ICollection<ContentFile> ContentFiles { get; set; }
    }
}
