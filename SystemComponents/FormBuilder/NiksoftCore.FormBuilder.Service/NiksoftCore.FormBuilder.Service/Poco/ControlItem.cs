namespace NiksoftCore.FormBuilder.Service
{
    public class ControlItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int FormId { get; set; }
        public int ControlId { get; set; }
        public int OrderId { get; set; }

        public virtual Form Form { get; set; }
        public virtual FormControl FormControl { get; set; }
    }
}
