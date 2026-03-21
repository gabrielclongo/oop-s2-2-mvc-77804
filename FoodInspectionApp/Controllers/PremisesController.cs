using FoodInspectionApp.Data;
using FoodInspectionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PremisesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PremisesController> _logger;

    public PremisesController(ApplicationDbContext context, ILogger<PremisesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Premises.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Premises premises)
    {
        try
        {
            _context.Add(premises);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Premises created {PremisesId} by {User}", premises.Id, User.Identity.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating premises");
            return View(premises);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var premises = await _context.Premises.FindAsync(id);
        if (premises == null)
        {
            _logger.LogWarning("Premises not found {PremisesId}", id);
            return NotFound();
        }
        return View(premises);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Premises premises)
    {
        if (id != premises.Id) return NotFound();

        try
        {
            _context.Update(premises);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Premises updated {PremisesId} by {User}", premises.Id, User.Identity.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating premises {PremisesId}", premises.Id);
            return View(premises);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var premises = await _context.Premises.FindAsync(id);
        return View(premises);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var premises = await _context.Premises.FindAsync(id);

        if (premises == null)
        {
            _logger.LogWarning("Attempt to delete non-existing Premises {PremisesId}", id);
            return NotFound();
        }

        _context.Premises.Remove(premises);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Premises deleted {PremisesId} by {User}", id, User.Identity.Name);

        return RedirectToAction(nameof(Index));
    }
}