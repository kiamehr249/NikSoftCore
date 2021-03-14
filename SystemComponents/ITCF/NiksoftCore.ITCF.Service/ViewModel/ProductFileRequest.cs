using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ITCF.Service
{
    public class ProductFileRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string EnTitle { get; set; }
        public string ArTitle { get; set; }
        public string Description { get; set; }
        public string EnDescription { get; set; }
        public string ArDescription { get; set; }
        public IFormFile FileData { get; set; }
        public string Path { get; set; }
        public int ProductId { get; set; }
    }
}
