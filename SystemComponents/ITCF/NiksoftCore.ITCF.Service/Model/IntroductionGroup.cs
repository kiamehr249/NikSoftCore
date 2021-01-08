﻿using System;
using System.Collections.Generic;

namespace NiksoftCore.ITCF.Service
{
    public class IntroductionGroup
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public bool Enabled { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreateDate { get; set; }
        

        public virtual ICollection<Introduction> Introductions { get; set; }
    }
}
