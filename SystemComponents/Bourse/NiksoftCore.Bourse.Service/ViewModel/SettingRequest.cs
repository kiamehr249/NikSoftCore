namespace NiksoftCore.Bourse.Service
{
    public class SettingRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullText { get; set; }
        public string KeyName { get; set; }
        public int MaxVal { get; set; }
        public int MinVal { get; set; }
        public string ReferenceCode { get; set; }
        public string Message { get; set; }
        public int ParentId { get; set; }
    }
}
