namespace NiksoftCore.ITCF.Service
{
    public class ParkSearchRequest
    {
        public string Title { get; set; }
        public int? ProvinceId { get; set; }
        public int CityId { get; set; }
        public string lang { get; set; }
        public int part { get; set; }
    }
}
