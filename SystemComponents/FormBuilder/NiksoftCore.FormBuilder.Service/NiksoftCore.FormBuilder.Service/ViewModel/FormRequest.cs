namespace NiksoftCore.FormBuilder.Service
{
    public class FormRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public bool LoginRequired { get; set; }
        public string Roles { get; set; }
    }
}
