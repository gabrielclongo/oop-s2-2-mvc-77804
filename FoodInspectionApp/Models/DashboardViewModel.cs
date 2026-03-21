namespace FoodInspectionApp.Models;

public class DashboardViewModel
{
    public int TotalInspections { get; set; }
    public int FailedInspections { get; set; }
    public int OverdueFollowUps { get; set; }

    public List<string> Towns { get; set; } = new();
    public List<string> RiskRatings { get; set; } = new();
}