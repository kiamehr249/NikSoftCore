using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class MasterSearch : BaseRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int BranchId { get; set; }
    }
}
