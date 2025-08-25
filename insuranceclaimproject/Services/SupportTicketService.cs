using insuranceclaimproject.Dtos.SupportTicket;
using insuranceclaimproject.Interfaces;
using insuranceclaimproject.Models;
using Microsoft.EntityFrameworkCore;

namespace insuranceclaimproject.Services
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly InsuranceContext _context;

        public SupportTicketService(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<SupportTicket> CreateTicketAsync(CreateSupportTicketDto ticketData)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == ticketData.UserId);
            if (!userExists)
            {
                throw new InvalidOperationException("User not found for the given UserId.");
            }

            var newTicket = new SupportTicket
            {
                UserId = ticketData.UserId,
                IssueDescription = ticketData.IssueDescription,
                TicketStatus = TicketStatus.OPEN,
                CreatedDate = DateTime.UtcNow
            };

            _context.SupportTickets.Add(newTicket);
            await _context.SaveChangesAsync();
            return newTicket;
        }

        public async Task<SupportTicket?> GetTicketByIdAsync(int ticketId)
        {
            return await _context.SupportTickets
                .Include(st => st.User)
                .FirstOrDefaultAsync(st => st.TicketId == ticketId);
        }

        public async Task<IEnumerable<SupportTicket>> GetAllTicketsAsync()
        {
            return await _context.SupportTickets
                .Include(st => st.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportTicket>> GetTicketsByUserAsync(string userId)
        {
            return await _context.SupportTickets
                .Where(st => st.UserId == userId)
                .Include(st => st.User)
                .ToListAsync();
        }

        public async Task<bool> UpdateTicketStatusAsync(int ticketId, TicketStatus status)
        {
            var ticket = await _context.SupportTickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return false;
            }

            ticket.TicketStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
