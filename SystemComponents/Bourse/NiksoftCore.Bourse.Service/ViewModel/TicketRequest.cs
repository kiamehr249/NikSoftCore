namespace NiksoftCore.Bourse.Service
{
    public class TicketRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullText { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public int? TargetId { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
    }
}
