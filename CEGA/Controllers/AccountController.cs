using CEGA.Data;
using CEGA.Models;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace CEGA.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context) => _context = context;

        // =====================
        //        LOGIN
        // =====================
        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var input = model.UserOrEmail?.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                ModelState.AddModelError("", "Ingrese usuario o correo.");
                return View(model);
            }

            // Buscar por Email o Username
            var emp = await _context.Empleados
                .FirstOrDefaultAsync(e => (input.Contains("@") ? e.Email == input : e.Username == input));

            if (emp == null || !emp.Activo)
            {
                ModelState.AddModelError("", "Credenciales no registradas o usuario inactivo.");
                return View(model);
            }

            // Verificar password (bcrypt)
            if (!BCrypt.Net.BCrypt.Verify(model.Password!, emp.PasswordHash))
            {
                ModelState.AddModelError("", "Usuario o contraseña inválidos.");
                return View(model);
            }

            await SignInEmpleadoAsync(emp, isPersistent: false);

            var target = (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                         ? model.ReturnUrl : Url.Action("Index", "Home")!;
            return LocalRedirect(target);
        }

        // =====================
        //      REGISTER
        // =====================
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            var vm = new RegisterViewModel
            {
                Puestos = await _context.Puestos
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                    .ToListAsync()
            };
            return View(vm);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Repoblar combo si algo falla
            async Task LoadPuestosAsync()
            {
                model.Puestos = await _context.Puestos.Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                    .ToListAsync();
            }

            if (!ModelState.IsValid) { await LoadPuestosAsync(); return View(model); }

            // Validaciones de negocio mínimas
            if (model.Cedula < 100000000 || model.Cedula > 999999999)
                ModelState.AddModelError(nameof(model.Cedula), "Cédula debe tener 9 dígitos.");
            if (string.IsNullOrWhiteSpace(model.TelefonoPersonal) || model.TelefonoPersonal.Length != 8)
                ModelState.AddModelError(nameof(model.TelefonoPersonal), "Teléfono personal: 8 dígitos.");
            if (string.IsNullOrWhiteSpace(model.TelefonoEmergencia) || model.TelefonoEmergencia.Length != 8)
                ModelState.AddModelError(nameof(model.TelefonoEmergencia), "Teléfono emergencia: 8 dígitos.");
            if ((model.FechaIngreso - model.FechaNacimiento).TotalDays < 18 * 365)
                ModelState.AddModelError(nameof(model.FechaIngreso), "Debe ser mayor de 18 años.");
            if (model.PuestoId == null)
                ModelState.AddModelError(nameof(model.PuestoId), "Debe seleccionar un puesto.");

            // Unicidad
            if (await _context.Empleados.AnyAsync(e => e.Cedula == model.Cedula))
                ModelState.AddModelError(nameof(model.Cedula), "Ya existe un empleado con esa cédula.");
            if (await _context.Empleados.AnyAsync(e => e.Email == model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email ya registrado.");

            // Username según regla
            var username = GenerarUsername(model.Nombre!, model.Apellido1!, model.Apellido2);
            if (await _context.Empleados.AnyAsync(e => e.Username == username))
                username = GenerarUsernameFallback(model.Nombre!, model.Apellido1!);

            if (!ModelState.IsValid) { await LoadPuestosAsync(); return View(model); }

            // Hash de contraseña (bcrypt)
            var hash = BCrypt.Net.BCrypt.HashPassword(model.Password!);

            var emp = new Empleado
            {
                Cedula = model.Cedula,
                Nombre = model.Nombre!,
                SegundoNombre = model.SegundoNombre,
                Apellido1 = model.Apellido1!,
                Apellido2 = model.Apellido2,
                Username = username,
                Email = model.Email!,
                PasswordHash = hash,
                Activo = true, // por ahora
                // Rol por defecto: usa el valor default del modelo (p.ej. AdminSistema si lo dejaste así)
                TelefonoPersonal = model.TelefonoPersonal!,
                TelefonoEmergencia = model.TelefonoEmergencia!,
                Sexo = model.Sexo!,
                FechaNacimiento = model.FechaNacimiento,
                FechaIngreso = model.FechaIngreso,
                TipoSangre = model.TipoSangre,
                Alergias = model.Alergias,
                ContactoEmergenciaNombre = model.ContactoEmergenciaNombre,
                ContactoEmergenciaTelefono = model.ContactoEmergenciaTelefono,
                PolizaSeguro = model.PolizaSeguro,
                PuestoId = model.PuestoId
            };

            try
            {
                _context.Empleados.Add(emp);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejo de UQ de Username/Email
                var msg = ex.InnerException?.Message ?? ex.Message;
                if (msg.Contains("UQ_EMPLEADO_Username")) ModelState.AddModelError(nameof(model.Email), "Username en uso.");
                else if (msg.Contains("UQ_EMPLEADO_Email")) ModelState.AddModelError(nameof(model.Email), "Email en uso.");
                else ModelState.AddModelError("", "No se pudo registrar el empleado.");
                await LoadPuestosAsync();
                return View(model);
            }

            // Login directo
            await SignInEmpleadoAsync(emp, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        // =====================
        //       LOGOUT
        // =====================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // =====================
        //      Helpers
        // =====================
        private async Task SignInEmpleadoAsync(Empleado emp, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, emp.Cedula.ToString()),
                new Claim(ClaimTypes.Name, emp.Username),
                new Claim(ClaimTypes.Email, emp.Email),
                new Claim(ClaimTypes.Role, emp.Rol.ToString()) // AdminSistema/RRHH/...
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var props = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
        }

        private static string GenerarUsername(string nombre, string ape1, string? ape2)
        {
            string n = QuitarTildes(nombre).ToLower().Trim();
            string a1 = QuitarTildes(ape1).ToLower().Replace(" ", "");
            string a2 = QuitarTildes(ape2 ?? "").ToLower();
            return $"{n[0]}{a1}{(string.IsNullOrEmpty(a2) ? "" : a2[0])}";
        }

        private static string GenerarUsernameFallback(string nombre, string ape1)
        {
            var n = QuitarTildes(nombre).ToLower();
            var a = QuitarTildes(ape1).ToLower().Replace(" ", "");
            return $"{(n.Length > 1 ? n[..2] : n)}{a}";
        }

        private static string QuitarTildes(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var norm = s.Normalize(NormalizationForm.FormD);
            var chars = norm.Where(c =>
                CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            return new string(chars.ToArray())
                .Replace('ñ', 'n')
                .Replace('Ñ', 'n');
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
