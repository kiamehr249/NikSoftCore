using System;

namespace NiksoftCore.ITCF.Service
{
    public class PurchaseCheckRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public long PrePayment { get; set; }
        public long UnitAmount { get; set; }
        public long TransportAmount { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public PurchaseStatus Status { get; set; }
    }
}
