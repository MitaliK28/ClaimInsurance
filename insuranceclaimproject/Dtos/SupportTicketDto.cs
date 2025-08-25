using insuranceclaimproject.Models;

namespace insuranceclaimproject.Dtos.SupportTicket
{
    public class SupportTicketDto
    {
        public int TicketId { get; set; }
        public string UserId { get; set; } = default!;
        public string IssueDescription { get; set; } = default!;
        public string TicketStatus { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
    }
}