using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class UserBankAccount : LogModel
    {
        public int Id { get; set; }
        public string PAN { get; set; }
        public string IBAN { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public int UserId { get; set; }
        

    }
}
