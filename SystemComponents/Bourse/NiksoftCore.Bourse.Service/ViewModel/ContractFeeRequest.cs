namespace NiksoftCore.Bourse.Service
{
    public class ContractFeeRequest
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int FeeId { get; set; }
        public int FeeType { get; set; }
    }
}
