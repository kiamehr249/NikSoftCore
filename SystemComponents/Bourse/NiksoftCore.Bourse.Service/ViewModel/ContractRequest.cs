namespace NiksoftCore.Bourse.Service
{
    public class ContractRequest
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public ContractType ContractType { get; set; }
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string UserFullName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int FeeType { get; set; }
        public int FeeId { get; set; }
        public int Deadline { get; set; }
        public ContractStatus Status { get; set; }
        public string ContractDate { get; set; }
        public int BranchId { get; set; }
    }
}
