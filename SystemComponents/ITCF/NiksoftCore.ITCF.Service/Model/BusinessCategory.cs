﻿using System.Collections.Generic;

namespace NiksoftCore.ITCF.Service
{
    public class BusinessCategory
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icone { get; set; }
        public string Image { get; set; }
        public int? ParentId { get; set; }

        public virtual BusinessCategory Parent { get; set; }
        public virtual ICollection<BusinessCategory> Childs { get; set; }
        public virtual ICollection<Business> Businesses { get; set; }
    }
}
