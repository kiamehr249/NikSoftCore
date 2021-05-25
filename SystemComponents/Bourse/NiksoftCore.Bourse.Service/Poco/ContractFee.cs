namespace NiksoftCore.Bourse.Service
{
    public class ContractFee
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int FeeId { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Fee Fee { get; set; }
    }
}
