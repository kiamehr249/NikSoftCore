using NiksoftCore.ViewModel;
using System.Collections.Generic;

namespace NiksoftCore.Bourse.Service
{
    public class TicketCategory : LogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string KeyValue { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int OrderId { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
