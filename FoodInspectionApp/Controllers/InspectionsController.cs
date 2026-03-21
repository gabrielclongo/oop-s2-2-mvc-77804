using FoodInspectionApp.Data;
using FoodInspectionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        var inspections = _context.Inspections.Include(i => i.Premises);
        return View(await inspections.ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Premises = _context.Premises.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Inspection inspection)
    {
        try
        {
            _context.Add(inspection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inspection created {InspectionId} for Premises {PremisesId} by {User}",
                inspection.Id, inspection.PremisesId, User.Identity.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inspection");
            return View(inspection);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var inspection = await _context.Inspections.FindAsync(id);
        return View(inspection);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var inspection = await _context.Inspections.FindAsync(id);

        if (inspection == null)
        {
            _logger.LogWarning("Attempt to delete non-existing Inspection {InspectionId}", id);
            return NotFound();
        }

        _context.Inspections.Remove(inspection);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Inspection deleted {InspectionId} by {User}", id, User.Identity.Name);

        return RedirectToAction(nameof(Index));
    }
}