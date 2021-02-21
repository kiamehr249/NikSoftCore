using System.Collections.Generic;

namespace NiksoftCore.ITCF.Service
{
    public class Business
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
        public string Image { get; set; }
        public string Description { get; set; }
        public int? IndustrialParkId { get; set; }
        public int CatgoryId { get; set; }
        public int CreatorId { get; set; }
        public BusinessStatus Status { get; set; }

        public virtual Country Country { get; set; }
        public virtual Province Province { get; set; }
        public virtual City City { get; set; }
        public virtual BusinessCategory Category { get; set; }
        public virtual IndustrialPark IndustrialPark { get; set; }

        public virtual ICollection<Introduction> Introductions { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
