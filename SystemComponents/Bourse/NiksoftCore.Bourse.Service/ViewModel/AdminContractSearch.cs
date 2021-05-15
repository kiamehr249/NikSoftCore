using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class AdminContractSearch : BaseRequest
    {
        public string ContractNumber { get; set; }
        public int ContractType { get; set; }
        public string UserFullName { get; set; }
        public int Status { get; set; }
        public int BranchId { get; set; }
    }
}
