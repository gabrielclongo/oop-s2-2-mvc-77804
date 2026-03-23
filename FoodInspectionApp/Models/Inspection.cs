using System.ComponentModel.DataAnnotations;

namespace FoodInspectionApp.Models
{
    public class Inspection
    {
        public int Id { get; set; }

        public int PremisesId { get; set; }

        public DateTime InspectionDate { get; set; }

        public int Score { get; set; }

        public string Outcome { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public Premises? Premises { get; set; }

        public List<FollowUp> FollowUps { get; set; } = new();
    }
}
