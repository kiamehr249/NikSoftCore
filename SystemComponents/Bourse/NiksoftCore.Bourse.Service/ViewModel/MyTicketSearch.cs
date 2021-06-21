using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class MyTicketSearch : BaseRequest
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
    }
}
