﻿using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class MediaCategorySearch : BaseRequest
    {
        public string Title { get; set; }
        public int ParentId { get; set; }
    }
}
