namespace NiksoftCore.Bourse.Service
{
    public class FeeRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int FeeType { get; set; }
        public long FromAmount { get; set; }
        public long ToAmount { get; set; }
        public int AmountPercentage { get; set; }
    }
}
