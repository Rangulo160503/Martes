using CEGA.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === DB ===
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? throw new InvalidOperationException("Missing 'DefaultConnection'.");

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(cs, sql => sql.EnableRetryOnFailure())
);

// === Auth por cookies (sin Identity) ===
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Account/Login";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // <- importante antes de Authorization
app.UseAuthorization();

// Raíz: si hay sesión → Home; si no → Login propio
app.MapGet("/", ctx =>
{
    var target = (ctx.User?.Identity?.IsAuthenticated == true) ? "/Home/Index" : "/Account/Login";
    ctx.Response.Redirect(target);
    return Task.CompletedTask;
});

// Rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
