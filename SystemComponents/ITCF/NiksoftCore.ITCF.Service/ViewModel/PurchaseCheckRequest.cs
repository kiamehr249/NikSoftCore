using System;

namespace NiksoftCore.ITCF.Service
{
    public class PurchaseCheckRequest
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public PurchaseStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string lang { get; set; }
    }
}
