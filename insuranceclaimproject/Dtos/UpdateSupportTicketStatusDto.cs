using insuranceclaimproject.Models;
using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Dtos.SupportTicket
{
    public class UpdateSupportTicketStatusDto
    {
        [Required]
        public TicketStatus TicketStatus { get; set; }
    }
}