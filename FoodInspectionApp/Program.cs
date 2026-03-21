using FoodInspectionApp.Data;
using FoodInspectionApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//  SERILOG CONFIG
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "FoodInspectionApp")
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

//  DATABASE (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// IDENTITY + ROLES
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//  GLOBAL ERROR HANDLING
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//  ROLES + ADMIN + SEED
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Seed(context);

    //  ROLES
    string[] roles = { "Admin", "Inspector", "Viewer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    //  ADMIN USER
    string adminEmail = "admin@test.com";
    string password = "Admin123!";

    var user = await userManager.FindByEmailAsync(adminEmail);

    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }

    //  SEED DATA
    if (!context.Premises.Any())
    {
        var premisesList = new List<Premises>
        {
            new Premises { Name = "Cafe Dublin 1", Address = "Street 1", Town = "Dublin", RiskRating = "High" },
            new Premises { Name = "Cafe Dublin 2", Address = "Street 2", Town = "Dublin", RiskRating = "Medium" },
            new Premises { Name = "Cafe Cork 1", Address = "Street 3", Town = "Cork", RiskRating = "Low" },
            new Premises { Name = "Cafe Cork 2", Address = "Street 4", Town = "Cork", RiskRating = "High" },
            new Premises { Name = "Cafe Galway 1", Address = "Street 5", Town = "Galway", RiskRating = "Medium" },
            new Premises { Name = "Cafe Galway 2", Address = "Street 6", Town = "Galway", RiskRating = "Low" },
            new Premises { Name = "Restaurant A", Address = "Street 7", Town = "Dublin", RiskRating = "High" },
            new Premises { Name = "Restaurant B", Address = "Street 8", Town = "Cork", RiskRating = "Medium" },
            new Premises { Name = "Restaurant C", Address = "Street 9", Town = "Galway", RiskRating = "Low" },
            new Premises { Name = "Takeaway A", Address = "Street 10", Town = "Dublin", RiskRating = "High" },
            new Premises { Name = "Takeaway B", Address = "Street 11", Town = "Cork", RiskRating = "Low" },
            new Premises { Name = "Takeaway C", Address = "Street 12", Town = "Galway", RiskRating = "Medium" }
        };

        context.Premises.AddRange(premisesList);
        await context.SaveChangesAsync();

        var random = new Random();
        var inspectionsList = new List<Inspection>();

        foreach (var premises in premisesList)
        {
            for (int i = 0; i < 2; i++)
            {
                var date = DateTime.Now.AddDays(-random.Next(1, 60));

                inspectionsList.Add(new Inspection
                {
                    PremisesId = premises.Id,
                    InspectionDate = date,
                    Score = random.Next(50, 100),
                    Outcome = random.Next(0, 2) == 0 ? "Pass" : "Fail",
                    Notes = "Routine inspection"
                });
            }
        }

        context.Inspections.AddRange(inspectionsList);
        await context.SaveChangesAsync();

        var followUpsList = new List<FollowUp>();
        var selectedInspections = inspectionsList.Take(10).ToList();

        foreach (var inspection in selectedInspections)
        {
            var isClosed = random.Next(0, 2) == 0;

            followUpsList.Add(new FollowUp
            {
                InspectionId = inspection.Id,
                DueDate = inspection.InspectionDate.AddDays(7),
                Status = isClosed ? "Closed" : "Open",
                ClosedDate = isClosed ? DateTime.Now : null
            });
        }

        context.FollowUps.AddRange(followUpsList);
        await context.SaveChangesAsync();
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();