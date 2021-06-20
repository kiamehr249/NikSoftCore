namespace NiksoftCore.Bourse.Service
{
    public class TicketAnswer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullText { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }

        public virtual BourseUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
