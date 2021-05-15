﻿using NiksoftCore.ViewModel;
using System;

namespace NiksoftCore.Bourse.Service
{
    public class Contract : LogModel
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public ContractType ContractType { get; set; }
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string UserFullName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int FeeId { get; set; }
        public int Deadline { get; set; }
        public ContractStatus Status { get; set; }
        public DateTime ContractDate { get; set; }
        public int BranchId { get; set; }

        public virtual BourseUser User { get; set; }
        public virtual Fee Fee { get; set; }
        public virtual Branch Branch { get; set; }

    }
}
