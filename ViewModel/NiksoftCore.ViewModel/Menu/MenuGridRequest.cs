namespace NiksoftCore.ViewModel
{
    public class MenuGridRequest : BaseRequest
    {
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int? ParentId { get; set; }
    }
}
