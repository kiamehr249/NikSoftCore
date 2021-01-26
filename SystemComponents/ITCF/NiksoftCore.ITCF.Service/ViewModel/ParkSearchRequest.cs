using NiksoftCore.ViewModel;

namespace NiksoftCore.ITCF.Service
{
    public class ParkSearchRequest : BaseRequest
    {
        public string Title { get; set; }
        public int? ProvinceId { get; set; }
        public int CityId { get; set; }
    }
}
