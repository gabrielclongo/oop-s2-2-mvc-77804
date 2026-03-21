using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodInspectionApp.Data;
using FoodInspectionApp.Models;

[Authorize]
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
        _logger.LogInformation("User {User} viewed follow-ups", User.Identity?.Name);

        return View(await _context.FollowUps.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(FollowUp followUp)
    {
        try
        {
            if (followUp.DueDate < DateTime.Now)
            {
                _logger.LogWarning("FollowUp created with past due date");
            }

            _context.Add(followUp);
            await _context.SaveChangesAsync();

            _logger.LogInformation("FollowUp created for InspectionId {InspectionId}", followUp.InspectionId);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating follow-up");
            return View("Error");
        }
    }
}