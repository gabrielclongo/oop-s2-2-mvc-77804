using FoodInspectionApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodInspectionApp.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (context.Premises.Any())
                return; // já tem dados

            var premises = new List<Premises>
            {
                new Premises { Name="Cafe A", Address="Street 1", Town="Dublin", RiskRating="Low" },
                new Premises { Name="Restaurant B", Address="Street 2", Town="Dublin", RiskRating="High" },
                new Premises { Name="Shop C", Address="Street 3", Town="Cork", RiskRating="Medium" },
                new Premises { Name="Cafe D", Address="Street 4", Town="Galway", RiskRating="Low" },
                new Premises { Name="Restaurant E", Address="Street 5", Town="Galway", RiskRating="High" },
                new Premises { Name="Shop F", Address="Street 6", Town="Cork", RiskRating="Medium" },
                new Premises { Name="Cafe G", Address="Street 7", Town="Dublin", RiskRating="Low" },
                new Premises { Name="Restaurant H", Address="Street 8", Town="Cork", RiskRating="High" },
                new Premises { Name="Shop I", Address="Street 9", Town="Galway", RiskRating="Medium" },
                new Premises { Name="Cafe J", Address="Street 10", Town="Dublin", RiskRating="Low" },
                new Premises { Name="Restaurant K", Address="Street 11", Town="Cork", RiskRating="High" },
                new Premises { Name="Shop L", Address="Street 12", Town="Galway", RiskRating="Medium" }
            };

            context.Premises.AddRange(premises);
            context.SaveChanges();

            var inspections = new List<Inspection>();

            for (int i = 0; i < 25; i++)
            {
                inspections.Add(new Inspection
                {
                    PremisesId = premises[i % premises.Count].Id,
                    InspectionDate = DateTime.Now.AddDays(-i),
                    Score = Random.Shared.Next(50, 100),
                    Outcome = i % 2 == 0 ? "Pass" : "Fail",
                    Notes = "Routine inspection"
                });
            }

            context.Inspections.AddRange(inspections);
            context.SaveChanges();

            var followUps = new List<FollowUp>();

            for (int i = 0; i < 10; i++)
            {
                followUps.Add(new FollowUp
                {
                    InspectionId = inspections[i].Id,
                    DueDate = DateTime.Now.AddDays(-i), // alguns overdue
                    Status = i % 2 == 0 ? "Open" : "Closed",
                    ClosedDate = i % 2 == 0 ? null : DateTime.Now
                });
            }

            context.FollowUps.AddRange(followUps);
            context.SaveChanges();
        }
    }
}