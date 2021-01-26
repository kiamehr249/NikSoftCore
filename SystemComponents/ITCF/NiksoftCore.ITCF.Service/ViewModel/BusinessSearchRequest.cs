using NiksoftCore.ViewModel;

namespace NiksoftCore.ITCF.Service
{
    public class BusinessSearchRequest : BaseRequest
    {
        public int Id { get; set; }
        public string CoName { get; set; }
        public int CountryId { get; set; }
        public int? ProvinceId { get; set; }
        public int CityId { get; set; }
        public int ParkId { get; set; }
        public int CatgoryId { get; set; }
    }
}
