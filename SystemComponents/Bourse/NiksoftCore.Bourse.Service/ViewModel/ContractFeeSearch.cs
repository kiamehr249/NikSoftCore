using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class ContractFeeSearch : BaseRequest
    {
        public int ContractId { get; set; }
        public int BranchId { get; set; }
        public int UserId { get; set; }

    }
}
