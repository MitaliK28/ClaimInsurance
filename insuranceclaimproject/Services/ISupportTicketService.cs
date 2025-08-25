using insuranceclaimproject.Dtos.SupportTicket;
using insuranceclaimproject.Models;

namespace insuranceclaimproject.Interfaces
{
    public interface ISupportTicketService
    {
        Task<SupportTicket> CreateTicketAsync(CreateSupportTicketDto ticketData);
        Task<SupportTicket?> GetTicketByIdAsync(int ticketId);
        Task<IEnumerable<SupportTicket>> GetAllTicketsAsync();
        Task<IEnumerable<SupportTicket>> GetTicketsByUserAsync(string userId);
        Task<bool> UpdateTicketStatusAsync(int ticketId, TicketStatus status);
    }
}