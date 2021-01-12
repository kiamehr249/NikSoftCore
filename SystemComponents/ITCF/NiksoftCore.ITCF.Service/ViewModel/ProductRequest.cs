﻿using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ITCF.Service
{
    public class ProductRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Video { get; set; }
        public IFormFile VideoFile { get; set; }
        public int GroupId { get; set; }
        public int BusinessId { get; set; }
    }
}
