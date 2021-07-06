using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class PricingPackage : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long Pric { get; set; }
        public string ColorHex { get; set; }
        public string ListObjects { get; set; }
        public string Description { get; set; }
    }
}
