using System.Collections.Generic;

namespace FoodInspectionApp.Models
{
    public class DashboardViewModel
    {
        public int InspectionsThisMonth { get; set; }
        public int FailedInspectionsThisMonth { get; set; }
        public int OverdueFollowUps { get; set; }

        public List<string> Towns { get; set; }
        public List<string> RiskRatings { get; set; }

        public string SelectedTown { get; set; }
        public string SelectedRisk { get; set; }
    }
}