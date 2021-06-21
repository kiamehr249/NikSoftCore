namespace NiksoftCore.Bourse.Service
{
    public class TicketAnswerRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullText { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
    }
}
