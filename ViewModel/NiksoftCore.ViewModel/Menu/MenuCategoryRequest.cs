using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ViewModel
{
    public class MenuCategoryRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string KeyValue { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public bool Enabled { get; set; }
    }
}
