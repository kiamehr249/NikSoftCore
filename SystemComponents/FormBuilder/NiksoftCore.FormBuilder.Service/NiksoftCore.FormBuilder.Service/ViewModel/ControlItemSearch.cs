using NiksoftCore.ViewModel;

namespace NiksoftCore.FormBuilder.Service
{
    public class ControlItemSearch : BaseRequest
    {
        public int ControlId { get; set; }
        public string Title { get; set; }
    }
}
