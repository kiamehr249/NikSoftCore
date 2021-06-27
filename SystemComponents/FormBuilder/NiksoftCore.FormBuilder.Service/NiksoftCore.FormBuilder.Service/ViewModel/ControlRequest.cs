namespace NiksoftCore.FormBuilder.Service
{
    public class ControlRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ControlType { get; set; }
        public int MaxValue { get; set; }
        public string MaxMessage { get; set; }
        public int MinValue { get; set; }
        public string MinMessage { get; set; }
        public bool IsRequired { get; set; }
        public string EmptyMessage { get; set; }
        public int OrderId { get; set; }
        public int FormId { get; set; }
    }
}
