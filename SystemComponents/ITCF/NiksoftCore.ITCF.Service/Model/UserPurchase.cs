using System;

namespace NiksoftCore.ITCF.Service
{
    public class UserPurchase
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
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual Product Product { get; set; }
        public virtual UserModel User { get; set; } 
    }
}
