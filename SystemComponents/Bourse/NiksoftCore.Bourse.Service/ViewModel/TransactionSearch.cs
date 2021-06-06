using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class TransactionSearch : BaseRequest
    {
        public int Period { get; set; }
        public string BranchCode { get; set; }
        public string FullName { get; set; }
        public string MarketerName { get; set; }
        public string ConsultantName { get; set; }
    }
}
