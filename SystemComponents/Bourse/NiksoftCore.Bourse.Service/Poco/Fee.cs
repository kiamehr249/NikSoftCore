using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class Fee : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public FeeType FeeType { get; set; }
        public long FromAmount { get; set; }
        public long ToAmount { get; set; }
        public int AmountPercentage { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
