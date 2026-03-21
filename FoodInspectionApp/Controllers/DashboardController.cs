using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodInspectionApp.Data;
using FoodInspectionApp.Models;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string town, string risk)
    {
        _logger.LogInformation("Dashboard accessed by {User}", User.Identity?.Name);

        var now = DateTime.Now;

        var inspections = _context.Inspections.Include(i => i.Premises).AsQueryable();

        if (!string.IsNullOrEmpty(town))
            inspections = inspections.Where(i => i.Premises != null && i.Premises.Town == town);

        if (!string.IsNullOrEmpty(risk))
            inspections = inspections.Where(i => i.Premises != null && i.Premises.RiskRating == risk);

        var model = new DashboardViewModel
        {
            TotalInspections = await inspections.CountAsync(i => i.InspectionDate.Month == now.Month),
            FailedInspections = await inspections.CountAsync(i => i.Outcome == "Fail"),
            OverdueFollowUps = await _context.FollowUps.CountAsync(f => f.DueDate < now && f.Status == "Open"),
            Towns = await _context.Premises.Select(p => p.Town).Distinct().ToListAsync(),
            RiskRatings = new List<string> { "Low", "Medium", "High" }
        };

        return View(model);
    }
}