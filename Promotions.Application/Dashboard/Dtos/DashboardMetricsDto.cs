namespace Promotions.Application.Dashboard.Dtos;

public class DashboardMetricsDto
{
    public int TotalPromotions { get; set; }
    public int TotalProducts { get; set; }
    public int TotalParticipants { get; set; }
    public int ActiveActions { get; set; }
    public int PendingActions { get; set; }  // Upcoming
    public int ExpiredActions { get; set; }  // Completed
    public decimal EstimatedRevenue { get; set; }
    public List<DailyActivityDto> ActivityTrend { get; set; } = new();
}
