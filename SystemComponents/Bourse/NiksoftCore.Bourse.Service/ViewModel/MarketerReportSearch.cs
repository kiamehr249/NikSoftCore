using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class MarketerReportSearch : BaseRequest
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string BranchCode { get; set; }
    }
}
