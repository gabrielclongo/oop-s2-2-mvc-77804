using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        return View(await _context.FollowUps.Include(f => f.Inspection).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var followUp = await _context.FollowUps
            .Include(f => f.Inspection)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (followUp == null) return NotFound();

        return View(followUp);
    }

    
    public IActionResult Create()
    {
        ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id");
        return View();
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FollowUp followUp)
    {
        if (ModelState.IsValid)
        {
            _context.Add(followUp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔥 importante: recriar dropdown se der erro
        ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
        return View(followUp);
    }

    
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var followUp = await _context.FollowUps.FindAsync(id);
        if (followUp == null) return NotFound();

        ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
        return View(followUp);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FollowUp followUp)
    {
        if (id != followUp.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(followUp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
        return View(followUp);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var followUp = await _context.FollowUps
            .Include(f => f.Inspection)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (followUp == null) return NotFound();

        return View(followUp);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var followUp = await _context.FollowUps.FindAsync(id);

        if (followUp != null)
        {
            _context.FollowUps.Remove(followUp);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}