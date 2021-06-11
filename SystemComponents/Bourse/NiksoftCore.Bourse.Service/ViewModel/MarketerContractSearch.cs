using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class MarketerContractSearch : BaseRequest
    {
        public int BranchId { get; set; }
        public int UserId { get; set; }
        public int IsOk { get; set; }
        public int Type { get; set; }
    }
}
