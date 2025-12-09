#!/bin/bash
cd ~/RiderProjects/TalentoPlus2.0/src/TalentoPlus.Web

echo "=== RECONSTRUYENDO DESDE CERO ==="

# 1. Limpiar todo
rm -rf bin obj
rm -f server.log

# 2. Crear Program.cs mínimo
cat > Program.cs << 'PROGRAM'
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Core.Interfaces;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuración simple
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=../../talentoplus.db"));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Crear base de datos si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
PROGRAM

# 3. Crear HomeController mínimo
mkdir -p Controllers
cat > Controllers/HomeController.cs << 'HOME'
using Microsoft.AspNetCore.Mvc;

namespace TalentoPlus.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
HOME

# 4. Crear vista Home mínimo
mkdir -p Views/Home
cat > Views/Home/Index.cshtml << 'HOMEVIEW'
@{
    ViewData["Title"] = "Home";
}

<h1>TalentoPlus - Sistema de Gestión</h1>

<p><a href="/Employees">Ver Empleados</a></p>
<p><a href="/Import">Importar Datos</a></p>
<p><a href="/Dashboard">Dashboard</a></p>
HOMEVIEW

# 5. Crear Layout mínimo
mkdir -p Views/Shared
cat > Views/Shared/_Layout.cshtml << 'LAYOUT'
<!DOCTYPE html>
<html>
<head>
    <title>@ViewData["Title"]</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        nav { background: #007bff; padding: 10px; margin-bottom: 20px; }
        nav a { color: white; margin-right: 15px; text-decoration: none; }
        nav a:hover { text-decoration: underline; }
    </style>
</head>
<body>
    <nav>
        <a href="/">Home</a>
        <a href="/Employees">Empleados</a>
        <a href="/Import">Importar</a>
        <a href="/Dashboard">Dashboard</a>
    </nav>
    
    <div>
        @RenderBody()
    </div>
</body>
</html>
LAYOUT

# 6. Compilar y ejecutar
dotnet build
echo ""
echo "✅ Reconstruido. Ejecuta: dotnet run"
echo "Luego ve a: http://localhost:5000"
