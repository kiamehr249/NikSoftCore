using NiksoftCore.ViewModel;
using System;

namespace NiksoftCore.Bourse.Service
{
    public class ContractLetter : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public DateTime? CurrentDate { get; set; }
        public string FullText { get; set; }
        public ContractType ContractType { get; set; }
        public bool Enabled { get; set; }
    }
}
