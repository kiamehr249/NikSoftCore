using Microsoft.AspNetCore.Http;

namespace NiksoftCore.Bourse.Service
{
    public class IcoCaseRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Collected { get; set; }
        public string StartDate { get; set; }
        public string Participants { get; set; }
        public string FullText { get; set; }
        public IFormFile IconFile { get; set; }
        public string Icon { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Image { get; set; }
        public string ListObjects { get; set; }
    }
}
