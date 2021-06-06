using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class ReceiptSearch : BaseRequest
    {
        public int TransactionId { get; set; }
        public string Number { get; set; }
        public string TrackingCode { get; set; }
    }
}
