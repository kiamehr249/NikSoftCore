using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class FeeSearch : BaseRequest
    {
        public string Title { get; set; }
        public int FeeType { get; set; }
    }
}
