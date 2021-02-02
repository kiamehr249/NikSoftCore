using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ViewModel
{
    public class MenuRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public bool Enabled { get; set; }
        public int OrderId { get; set; }
        public int CategoryId { get; set; }
        public int ParentId { get; set; }
    }
}
