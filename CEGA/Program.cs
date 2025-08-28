using CEGA.Data;
using CEGA.Models;
using CEGA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure())
);

//  Identity + Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

//  Cookies de autenticación: rutas de login/denegado
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/Login";
});

//  MVC con política global: requiere usuario autenticado en TODO
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Razor Pages: permitir anónimos y crear rutas amigables
builder.Services.AddRazorPages(o =>
{
    // seguir permitiendo anónimo a las páginas reales de Identity
    o.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
    o.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Register");

    // Rutas de SECCIÓN hacia esas mismas páginas
    o.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "seccion/acceso");
    o.Conventions.AddAreaPageRoute("Identity", "/Account/Register", "seccion/registro");
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Cache en memoria para OTP (no requiere BD ni Redis)
builder.Services.AddDistributedMemoryCache();

// Servicio de correo (usa tu implementación)
builder.Services.AddScoped<CEGA.Services.IEmailSender, CEGA.Services.SmtpEmailSender>();

// Servicio OTP
builder.Services.AddScoped<OtpService>();

builder.Services.AddRateLimiter(o =>
{
    o.AddFixedWindowLimiter("pwdreset", opts =>
    {
        opts.PermitLimit = 3;
        opts.Window = TimeSpan.FromMinutes(15);
    });
});


var app = builder.Build();

//  Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting(); 
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

//  Redirección raíz: si no hay sesión → Login; si hay sesión → Home
app.MapGet("/", context =>
{
    var target = (context.User?.Identity?.IsAuthenticated == true)
        ? "/Home/Index"          // cambia esto si tu “inicio con sesión” es otro
        : "/seccion/acceso";      // NUEVO login de sección
    context.Response.Redirect(target);
    return Task.CompletedTask;
});

// Rutas MVC y Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//  Seeding (roles/usuario admin si aplica)


app.Run();
