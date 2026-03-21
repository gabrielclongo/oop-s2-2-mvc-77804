using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodInspectionApp.Data;
using FoodInspectionApp.Models;

[Authorize(Roles = "Admin")]
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
        _logger.LogInformation("User {User} accessed premises", User.Identity?.Name);

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
            if (ModelState.IsValid)
            {
                _context.Add(premises);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Premises created: {Name}", premises.Name);

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid premises data");
            return View(premises);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating premises");
            return View("Error");
        }
    }
}