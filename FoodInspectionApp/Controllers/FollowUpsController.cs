using FoodInspectionApp.Data;
using FoodInspectionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FollowUpsController> _logger;

    public FollowUpsController(ApplicationDbContext context, ILogger<FollowUpsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var followUps = _context.FollowUps.Include(f => f.Inspection);
        return View(await followUps.ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Inspections = _context.Inspections.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(FollowUp followUp)
    {
        try
        {
            var inspection = await _context.Inspections.FindAsync(followUp.InspectionId);

            if (inspection != null && followUp.DueDate < inspection.InspectionDate)
            {
                _logger.LogWarning("FollowUp with invalid due date for Inspection {InspectionId}", followUp.InspectionId);
            }

            _context.Add(followUp);
            await _context.SaveChangesAsync();

            _logger.LogInformation("FollowUp created {FollowUpId} for Inspection {InspectionId} by {User}",
                followUp.Id, followUp.InspectionId, User.Identity.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating follow-up");
            return View(followUp);
        }
    }
}