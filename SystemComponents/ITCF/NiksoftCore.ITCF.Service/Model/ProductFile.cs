namespace NiksoftCore.ITCF.Service
{
    public class ProductFile
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string EnTitle { get; set; }
        public string ArTitle { get; set; }
        public string Description { get; set; }
        public string EnDescription { get; set; }
        public string ArDescription { get; set; }
        public string Path { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
