using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class MediaCategory : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int? ParentId { get; set; }

        public virtual MediaCategory Parent { get; set; }
        public virtual ICollection<MediaCategory> Childs { get; set; }
        public virtual ICollection<Media> Medias { get; set; }
    }
}
