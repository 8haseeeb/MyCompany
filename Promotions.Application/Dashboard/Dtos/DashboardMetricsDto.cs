namespace Promotions.Application.Dashboard.Dtos;

public class DashboardMetricsDto
{
    public int TotalPromotions { get; set; }
    public int TotalProducts { get; set; }
    public int TotalParticipants { get; set; }
    public int ActiveActions { get; set; }
    public decimal EstimatedRevenue { get; set; }
}
