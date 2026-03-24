using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodInspectionApp.Data;
using FoodInspectionApp.Models;

[Authorize]
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
        var inspections = _context.Inspections
            .Include(i => i.Premises);

        return View(await inspections.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var inspection = await _context.Inspections
            .Include(i => i.Premises)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (inspection == null) return NotFound();

        return View(inspection);
    }

    
    public IActionResult Create()
    {
        ViewBag.PremisesId = new SelectList(_context.Premises, "Id", "Id");
        return View();
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inspection inspection)
    {
        if (ModelState.IsValid)
        {
            _context.Add(inspection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inspection created: {Id}", inspection.Id);

            return RedirectToAction(nameof(Index));
        }

        ViewBag.PremisesId = new SelectList(_context.Premises, "Id", "Id", inspection.PremisesId);
        return View(inspection);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var inspection = await _context.Inspections.FindAsync(id);
        if (inspection == null) return NotFound();

        ViewBag.PremisesId = new SelectList(_context.Premises, "Id", "Id", inspection.PremisesId);
        return View(inspection);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Inspection inspection)
    {
        if (id != inspection.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(inspection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inspection updated: {Id}", inspection.Id);

            return RedirectToAction(nameof(Index));
        }

        ViewBag.PremisesId = new SelectList(_context.Premises, "Id", "Id", inspection.PremisesId);
        return View(inspection);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var inspection = await _context.Inspections
            .Include(i => i.Premises)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (inspection == null) return NotFound();

        return View(inspection);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var inspection = await _context.Inspections.FindAsync(id);

        if (inspection != null)
        {
            _context.Inspections.Remove(inspection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Inspection deleted: {Id}", id);
        }

        return RedirectToAction(nameof(Index));
    }
}