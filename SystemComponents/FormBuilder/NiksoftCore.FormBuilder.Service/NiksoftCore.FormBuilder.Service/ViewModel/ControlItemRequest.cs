namespace NiksoftCore.FormBuilder.Service
{
    public class ControlItemRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int FormId { get; set; }
        public int ControlId { get; set; }
        public int OrderId { get; set; }
    }
}
