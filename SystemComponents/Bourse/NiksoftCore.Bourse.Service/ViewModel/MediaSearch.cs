using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class MediaSearch : BaseRequest
    {
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int Order { get; set; }
    }
}
