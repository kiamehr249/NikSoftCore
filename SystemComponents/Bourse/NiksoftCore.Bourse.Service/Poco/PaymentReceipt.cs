using NiksoftCore.ViewModel;
using System;

namespace NiksoftCore.Bourse.Service
{
    public class PaymentReceipt : LogModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int Period { get; set; }
        public long PaymentAmount { get; set; }
        public int UserId { get; set; }
        public string PAN { get; set; }
        public string Description { get; set; }
        public string TrackingCode { get; set; }
        public int TransactionId { get; set; }
        public ReceiptStatus Status { get; set; }

        public virtual BourseUser User { get; set; }
        public virtual BaseTransaction Transaction { get; set; }

    }
}
