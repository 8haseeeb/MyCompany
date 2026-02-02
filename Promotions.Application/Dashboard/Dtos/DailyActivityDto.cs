namespace Promotions.Application.Dashboard.Dtos;

public class DailyActivityDto
{
    public string DateLabel { get; set; } = string.Empty; // e.g., "1", "4" or "2023-10-01"
    public int Active { get; set; }
    public int Pending { get; set; }
    public int Failed { get; set; } // Representing Expired/Failed
}
