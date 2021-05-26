namespace NiksoftCore.Bourse.Service
{
    public class MediaRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string BaseLink { get; set; }
        public string FullLink { get; set; }
        public string GeneratedLink { get; set; }
        public int ClickCount { get; set; }
        public int Status { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public bool Ownership { get; set; }
    }
}
