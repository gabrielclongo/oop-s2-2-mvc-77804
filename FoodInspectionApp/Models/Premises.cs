using System.ComponentModel.DataAnnotations;

namespace FoodInspectionApp.Models
{
    public class Premises
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Town { get; set; } = string.Empty;

        [Required]
        public string RiskRating { get; set; } = string.Empty;

        // RELAÇÃO: 1 Premises -> muitos Inspections
        public List<Inspection> Inspections { get; set; } = new List<Inspection>();
    }
}