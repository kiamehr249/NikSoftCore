using System;

namespace NiksoftCore.Bourse.Service
{
    public class ContractLetterRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string CurrentDate { get; set; }
        public string FullText { get; set; }
        public int ContractType { get; set; }
        public bool Enabled { get; set; }
    }
}
