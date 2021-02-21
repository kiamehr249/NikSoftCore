using NiksoftCore.ViewModel;

namespace NiksoftCore.ITCF.Service
{
    public class UserPurchaseGridRequest : BaseRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public PurchaseStatus Status { get; set; }
        public string Name { get; set; }

    }
}
