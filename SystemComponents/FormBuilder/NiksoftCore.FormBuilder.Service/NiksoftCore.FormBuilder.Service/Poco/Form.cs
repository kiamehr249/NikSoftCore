using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.FormBuilder.Service
{
    public class Form : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public bool LoginRequired { get; set; }
        public string Roles { get; set; }

        public virtual ICollection<FormControl> FormControls { get; set; }
        public virtual ICollection<ControlItem> ControlItems { get; set; }
        public virtual ICollection<FormData> FormDatas { get; set; }
    }
}
