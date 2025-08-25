using insuranceclaimproject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace insuranceclaimproject.Models
{
    public enum TicketStatus
    {
        OPEN,
        RESOLVED
    }

    public class SupportTicket
    {
        [Key]

        public int TicketId { get; set; }

        [Required]

        public string UserId { get; set; } = default!;


        [ForeignKey("UserId")]

        public AppUser? User { get; set; }

        [Required]

        public string IssueDescription { get; set; }

        [Required]

        public TicketStatus TicketStatus { get; set; } = TicketStatus.OPEN;

        [Required]

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }

}