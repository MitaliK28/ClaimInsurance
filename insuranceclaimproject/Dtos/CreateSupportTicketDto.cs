using System.ComponentModel.DataAnnotations;

namespace insuranceclaimproject.Dtos.SupportTicket
{
    public class CreateSupportTicketDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = default!;

        [Required(ErrorMessage = "Issue description is required.")]
        public string IssueDescription { get; set; } = default!;
    }
}