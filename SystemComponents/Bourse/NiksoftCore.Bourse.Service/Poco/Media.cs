using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class Media : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string BaseLink { get; set; }
        public string FullLink { get; set; }
        public string GeneratedLink { get; set; }
        public int ClickCount { get; set; }
        public MediaStatus Status { get; set; }
        public int CategoryId { get; set; }

        public virtual MediaCategory Category { get; set; }
    }
}
