namespace NiksoftCore.SystemBase.Service
{
    public class City
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ProvinceId { get; set; }
        public int CountryId { get; set; }
        public bool IsProvinceCenter { get; set; }
        public bool IsMain { get; set; }

        public virtual Province Province { get; set; }
        public virtual Country Country { get; set; }
    }
}
