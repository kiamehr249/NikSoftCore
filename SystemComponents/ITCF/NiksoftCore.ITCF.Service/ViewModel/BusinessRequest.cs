using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ITCF.Service
{
    public class BusinessRequest
    {
        public int Id { get; set; }
        public string CoName { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public BusinessType BusinessType { get; set; }
        public int CountryId { get; set; }
        public int? ProvinceId { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string Logo { get; set; }
        public IFormFile LogoFile { get; set; }
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Description { get; set; }
        public int? IndustrialParkId { get; set; }
        public int CatgoryId { get; set; }
        public int CreatorId { get; set; }
        public BusinessStatus Status { get; set; }
    }
}
