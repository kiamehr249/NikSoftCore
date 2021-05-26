using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class SettingSearch : BaseRequest
    {
        public string Title { get; set; }
        public int ParentId { get; set; }
    }
}
