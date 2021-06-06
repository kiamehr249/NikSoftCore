using System;

namespace NiksoftCore.Bourse.Service
{
    public class ConsultantReport
    {
        public int Period { get; set; }
        public string MarketerCode { get; set; }
        public string MarketerName { get; set; }
        public string ConsultantCode { get; set; }
        public string ConsultantName { get; set; }
        public int CustomerCount { get; set; }
        public double TotalInputAmount { get; set; }
        public double TotalConsultantWage { get; set; }
    }
}
