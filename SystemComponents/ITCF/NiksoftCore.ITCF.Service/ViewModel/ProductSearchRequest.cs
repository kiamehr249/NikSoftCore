using NiksoftCore.ViewModel;

namespace NiksoftCore.ITCF.Service
{
    public class ProductSearchRequest : BaseRequest
    {
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int BusinessId { get; set; }
    }
}
