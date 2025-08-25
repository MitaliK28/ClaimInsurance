using insuranceclaimproject.Models;
using insuranceclaimproject.Dtos;
using Microsoft.EntityFrameworkCore;

namespace insuranceclaimproject.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly InsuranceContext _context;

        public DashboardService(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var summary = new DashboardSummaryDto();

            // Group PolicyCategories by CategoryType
            var categoryGroups = await _context.PolicyCategories
                .GroupBy(pc => pc.CategoryType)
                .Select(g => new
                {
                    Category = g.Key,
                    ActiveCount = g.Count(x => x.Status == PolicyCategoryStatus.Active),
                    PendingCount = g.Count(x => x.Status == PolicyCategoryStatus.Pending)
                })
                .ToListAsync();

            foreach (var g in categoryGroups)
            {
                summary.Categories[g.Category] = new CategorySummaryDto
                {
                    ActiveCount = g.ActiveCount,
                    PendingCount = g.PendingCount
                };
            }

            // Totals
            summary.TotalPoliciesCount = await _context.Policies.CountAsync();
            summary.TotalClaimsCount = await _context.Claims.CountAsync();
            summary.TotalPendingClaims = await _context.Claims.CountAsync(c => c.ClaimStatus == ClaimStatus.Pending);
            summary.TotalPendingPolicies = await _context.PolicyCategories.CountAsync(pc => pc.Status == PolicyCategoryStatus.Pending);
            summary.TotalOpenTicketCount = await _context.SupportTickets.CountAsync(t => t.TicketStatus == TicketStatus.OPEN);
            summary.TotalResolvedTicketCount = await _context.SupportTickets.CountAsync(t => t.TicketStatus == TicketStatus.RESOLVED);

            return summary;
        }
    }
}