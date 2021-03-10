using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ITCF.Service
{
    public class BusinessCategoryRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string EnTitle { get; set; }
        public string ArTitle { get; set; }
        public string Description { get; set; }
        public string EnDescription { get; set; }
        public string ArDescription { get; set; }
        public IFormFile IconFile { get; set; }
        public string Icone { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Image { get; set; }
        public int? ParentId { get; set; }
    }
}
