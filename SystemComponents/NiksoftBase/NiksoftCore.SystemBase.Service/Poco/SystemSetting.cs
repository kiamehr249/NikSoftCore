namespace NiksoftCore.SystemBase.Service
{
    public class SystemSetting
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public int? CountValue { get; set; }
        public bool Enabled { get; set; }
        public string SettingKey { get; set; }
        public string GroupKey { get; set; }
        public int PortalId { get; set; }
    }
}
