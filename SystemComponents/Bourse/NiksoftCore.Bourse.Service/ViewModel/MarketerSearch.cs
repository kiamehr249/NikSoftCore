using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class MarketerSearch : BaseRequest
    {
        public int BranchId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int IsOk { get; set; }
    }
}
