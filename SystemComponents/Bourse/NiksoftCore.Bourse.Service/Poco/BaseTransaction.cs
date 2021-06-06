using System;
using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class BaseTransaction
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ContactNo { get; set; }
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public Single InputAmount { get; set; }
        public Single WageAmount { get; set; }
        public string MarketerCode { get; set; }
        public string MarketerName { get; set; }
        public Single MarketerWage { get; set; }
        public Single MarketerCost { get; set; }
        public Single InsuranceAmount { get; set; }
        public Single TaxAmount { get; set; }
        public string ConsultantCode { get; set; }
        public string ConsultantName { get; set; }
        public Single ConsultantWage { get; set; }
        public long PaymentAmount { get; set; }

        public virtual ICollection<PaymentReceipt> PaymentReceipts { get; set; }

    }
}
