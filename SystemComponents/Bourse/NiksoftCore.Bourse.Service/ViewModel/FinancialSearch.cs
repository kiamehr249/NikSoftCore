using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class FinancialSearch : BaseRequest
    {
        public int Period { get; set; }
        public string BranchCode { get; set; }
        public string ConsultantCode { get; set; }
        public string ConsultantName { get; set; }
    }
}
