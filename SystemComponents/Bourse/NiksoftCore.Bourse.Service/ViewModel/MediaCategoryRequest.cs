namespace NiksoftCore.Bourse.Service
{
    public class MediaCategoryRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int ParentId { get; set; }
    }
}
