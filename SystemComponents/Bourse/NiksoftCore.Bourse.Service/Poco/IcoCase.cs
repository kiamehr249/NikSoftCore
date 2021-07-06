using NiksoftCore.ViewModel;
using System;

namespace NiksoftCore.Bourse.Service
{
    public class IcoCase : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Collected { get; set; }
        public DateTime StartDate { get; set; }
        public string Participants { get; set; }
        public string FullText { get; set; }
        public string Icon { get; set; }
        public string Image { get; set; }
        public string ListObjects { get; set; }
    }
}
