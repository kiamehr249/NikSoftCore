using Microsoft.AspNetCore.Http;

namespace NiksoftCore.ViewModel
{
    public class ContentRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Header { get; set; }
        public string BodyText { get; set; }
        public string Footer { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }
        public string SmallImage { get; set; }
        public IFormFile SmallFile { get; set; }
        public string KeyValue { get; set; }
        public string Link { get; set; }
        public string LinkTitle { get; set; }
        public bool Enabled { get; set; }
        public int CategoryId { get; set; }
    }
}
