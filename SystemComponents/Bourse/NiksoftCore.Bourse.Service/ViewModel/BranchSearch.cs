using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class BranchSearch : BaseRequest
    {
        public string Title { get; set; }
        public string Code { get; set; }
    }
}
