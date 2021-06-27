using NiksoftCore.ViewModel;

namespace NiksoftCore.FormBuilder.Service
{
    public class ControlSearch : BaseRequest
    {
        public string Title { get; set; }
        public int FormId { get; set; }
    }
}
