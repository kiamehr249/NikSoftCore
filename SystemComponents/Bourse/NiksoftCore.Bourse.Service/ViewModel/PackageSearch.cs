using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class PackageSearch : BaseRequest
    {
        public string Title { get; set; }
        public long Pric { get; set; }
    }
}
