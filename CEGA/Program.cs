using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔗 Configurar la conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🔒 Configurar Identity con ApplicationUser y habilitar roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Opcional: desactiva confirmación por correo
})
.AddRoles<IdentityRole>() // Habilita gestión de roles
.AddEntityFrameworkStores<ApplicationDbContext>();

// 👁️ Mostrar errores detallados de EF Core en desarrollo
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 🌐 Middleware HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Página de errores de migración
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Seguridad HTTPS
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Login, cookies, tokens
app.UseAuthorization();  // Roles y acceso

// Rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// 🌱 Ejecutar el seeding para crear usuario admin y roles si no existen
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

app.Run();
