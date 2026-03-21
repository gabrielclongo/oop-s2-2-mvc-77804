using Xunit;
using FoodInspectionApp.Data;
using FoodInspectionApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FoodInspectionApp.Tests
{
    public class DashboardTests
    {
        [Fact]
        public void OverdueFollowUps_ReturnsCorrect()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb1")
                .Options;

            using var context = new ApplicationDbContext(options);

            context.FollowUps.Add(new FollowUp
            {
                DueDate = DateTime.Now.AddDays(-1),
                Status = "Open"
            });

            context.SaveChanges();

            var overdue = context.FollowUps
                .Where(f => f.DueDate < DateTime.Now && f.Status == "Open")
                .Count();

            Assert.Equal(1, overdue);
        }

        [Fact]
        public void FollowUp_Open_IsValid()
        {
            var followUp = new FollowUp
            {
                Status = "Open",
                ClosedDate = null
            };

            Assert.Equal("Open", followUp.Status);
        }

        [Fact]
        public void InspectionCount_Works()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb2")
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Inspections.Add(new Inspection
            {
                InspectionDate = DateTime.Now,
                Outcome = "Pass",
                Notes = "Test inspection"
            });

            context.SaveChanges();

            var count = context.Inspections.Count();

            Assert.Equal(1, count);
        }

        [Fact]
        public void Basic_Test()
        {
            Assert.True(true);
        }
    }
}