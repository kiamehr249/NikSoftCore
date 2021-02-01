namespace NiksoftCore.ViewModel
{
    public class CategoryGridRequest : BaseRequest
    {
        public string Title { get; set; }
        public int ParentId { get; set; }
    }
}
