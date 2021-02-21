using NiksoftCore.ViewModel;

namespace NiksoftCore.ITCF.Service
{
    public class UserPurchaseGridRequest : BaseRequest
    {
        public int Count { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public PurchaseStatus Status { get; set; }
        public string UserName { get; set; }

    }
}
