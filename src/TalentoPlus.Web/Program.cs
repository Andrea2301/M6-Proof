using Microsoft.EntityFrameworkCore;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;
using TalentoPlus.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=../../talentoplus.db"));

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Import Service
builder.Services.AddScoped<TalentoPlus.Core.Interfaces.IImportService, ImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Create database if it doesn't exist
    context.Database.EnsureCreated();
    
    // Seed initial data
    SeedData(context);
}

app.Run();

// Seed data method
void SeedData(ApplicationDbContext context)
{
    // Add positions if none exist
    if (!context.Positions.Any())
    {
        context.Positions.AddRange(
            new TalentoPlus.Core.Entities.Position { Name = "Desarrollador" },
            new TalentoPlus.Core.Entities.Position { Name = "Analista" },
            new TalentoPlus.Core.Entities.Position { Name = "Gerente" }
        );
    }
    
    // Add departments
    if (!context.Departments.Any())
    {
        context.Departments.AddRange(
            new TalentoPlus.Core.Entities.Department { Name = "TI" },
            new TalentoPlus.Core.Entities.Department { Name = "RRHH" },
            new TalentoPlus.Core.Entities.Department { Name = "Ventas" }
        );
    }
    
    // Add employee statuses
    if (!context.EmployeeStatuses.Any())
    {
        context.EmployeeStatuses.AddRange(
            new TalentoPlus.Core.Entities.EmployeeStatus { Name = "Activo" },
            new TalentoPlus.Core.Entities.EmployeeStatus { Name = "Inactivo" },
            new TalentoPlus.Core.Entities.EmployeeStatus { Name = "Vacaciones" }
        );
    }
    
    // Add education levels
    if (!context.EducationLevels.Any())
    {
        context.EducationLevels.AddRange(
            new TalentoPlus.Core.Entities.EducationLevel { Name = "Secundaria" },
            new TalentoPlus.Core.Entities.EducationLevel { Name = "Técnico" },
            new TalentoPlus.Core.Entities.EducationLevel { Name = "Universitario" }
        );
    }
    
    // Add sample employees if none exist
    if (!context.Employees.Any())
    {
        var position = context.Positions.FirstOrDefault();
        var department = context.Departments.FirstOrDefault();
        var status = context.EmployeeStatuses.FirstOrDefault();
        var education = context.EducationLevels.FirstOrDefault();
        
        if (position != null && department != null && status != null && education != null)
        {
            context.Employees.Add(new TalentoPlus.Core.Entities.Employee
            {
                Document = "12345678",
                FirstName = "Juan",
                LastName = "Pérez",
                Email = "juan.perez@example.com",
                Phone = "555-1234",
                Address = "Calle Principal 123",
                BirthDate = new DateTime(1990, 5, 15),
                PositionId = position.Id,
                DepartmentId = department.Id,
                EmployeeStatusId = status.Id,
                EducationLevelId = education.Id,
                Salary = 50000,
                HireDate = new DateTime(2023, 1, 1),
                ProfessionalProfile = "Desarrollador con experiencia",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Temp123!"),
                IsEnabled = true,
                RegistrationDate = DateTime.UtcNow
            });
        }
    }
    
    context.SaveChanges();
}
