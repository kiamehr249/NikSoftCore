﻿using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class ConsultantSearch : BaseRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
