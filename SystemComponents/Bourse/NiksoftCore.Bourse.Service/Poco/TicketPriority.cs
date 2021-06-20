using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class TicketPriority : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string HexCode { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int OrderId { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
