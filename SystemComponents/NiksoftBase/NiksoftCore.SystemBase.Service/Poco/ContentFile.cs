namespace NiksoftCore.SystemBase.Service
{
    public class ContentFile
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public FileType FileType { get; set; }
        public int FileId { get; set; }
        public int ContentId { get; set; }

        public virtual GeneralContent Content { get; set; }
    }
}
