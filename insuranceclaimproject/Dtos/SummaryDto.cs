namespace insuranceclaimproject.Dtos
{
    public class CategorySummaryDto
    {
        public int ActiveCount { get; set; }
        public int PendingCount { get; set; }
    }

    public class DashboardSummaryDto
    {
        // Dynamic categories like Individual Health, Critical Illness...
        public Dictionary<string, CategorySummaryDto> Categories { get; set; } = new();

        // Totals
        public int TotalPoliciesCount { get; set; }
        public int TotalClaimsCount { get; set; }
        public int TotalPendingClaims { get; set; }
        public int TotalPendingPolicies { get; set; }
        public int TotalOpenTicketCount { get; set; }
        public int TotalResolvedTicketCount { get; set; }
    }
}
