namespace NiksoftCore.Bourse.Service
{
    public class ReceiptRequest
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string ReceiptDate { get; set; }
        public int Period { get; set; }
        public long PaymentAmount { get; set; }
        public int UserId { get; set; }
        public string PAN { get; set; }
        public string Description { get; set; }
        public string TrackingCode { get; set; }
        public int TransactionId { get; set; }
        public int Status { get; set; }
    }
}
