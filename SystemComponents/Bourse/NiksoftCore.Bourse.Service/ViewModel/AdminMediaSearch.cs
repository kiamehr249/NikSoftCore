using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class AdminMediaSearch : BaseRequest
    {
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int Order { get; set; }
        public int Status { get; set; }
        public int BranchId { get; set; }
    }
}
