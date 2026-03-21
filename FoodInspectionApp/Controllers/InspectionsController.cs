using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodInspectionApp.Data;
using FoodInspectionApp.Models;

[Authorize(Roles = "Admin")]
public class InspectionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InspectionsController> _logger;

    public InspectionsController(ApplicationDbContext context, ILogger<InspectionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("User {User} accessed inspections list", User.Identity?.Name);

        var inspections = _context.Inspections.Include(i => i.Premises);
        return View(await inspections.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Inspection inspection)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspection);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Inspection created. PremisesId: {PremisesId}", inspection.PremisesId);

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid inspection data submitted");
            return View(inspection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inspection");
            return View("Error");
        }
    }
}