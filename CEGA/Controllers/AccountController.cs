using CEGA.Data;
using CEGA.Models;
using CEGA.Models.ViewModels;
using CEGA.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization; // CharUnicodeInfo
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;         // NormalizationForm

namespace CEGA.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public AccountController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

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

            var inputLower = input.ToLowerInvariant();
            var byEmail = input.Contains("@");
            var isDigits = input.All(char.IsDigit);

            var emp = await _context.Empleados.FirstOrDefaultAsync(e =>
                byEmail ? e.Email.ToLower() == inputLower :
                isDigits ? e.Cedula.ToString() == input :
                           e.Username.ToLower() == inputLower);

            if (emp == null || !emp.Activo)
            {
                ModelState.AddModelError("", "Credenciales no registradas o usuario inactivo.");
                return View(model);
            }

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
            async Task LoadPuestosAsync()
            {
                model.Puestos = await _context.Puestos
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                    .ToListAsync();
            }

            if (!ModelState.IsValid) { await LoadPuestosAsync(); return View(model); }

            // Validaciones mínimas
            var cedStr = model.Cedula.ToString();
            if (model.Cedula < 100000000 || model.Cedula > 999999999 || cedStr.Length != 9)
                ModelState.AddModelError(nameof(model.Cedula), "Cédula debe tener 9 dígitos.");

            bool TelOk(string? s) => !string.IsNullOrWhiteSpace(s) &&
                                     System.Text.RegularExpressions.Regex.IsMatch(s!, @"^\d{8}$");
            if (!TelOk(model.TelefonoPersonal))
                ModelState.AddModelError(nameof(model.TelefonoPersonal), "Teléfono personal: 8 dígitos.");
            if (!TelOk(model.TelefonoEmergencia))
                ModelState.AddModelError(nameof(model.TelefonoEmergencia), "Teléfono emergencia: 8 dígitos.");

            if (model.FechaNacimiento.AddYears(18) > model.FechaIngreso)
                ModelState.AddModelError(nameof(model.FechaIngreso), "Debe ser mayor de 18 años a la fecha de ingreso.");

            if (!model.PuestoId.HasValue)
            {
                ModelState.AddModelError(nameof(model.PuestoId), "Debe seleccionar un puesto.");
            }
            else
            {
                var puestoId = model.PuestoId.Value;
                var puestoValido = await _context.Puestos.AnyAsync(p => p.Id == puestoId && p.Activo);
                if (!puestoValido)
                    ModelState.AddModelError(nameof(model.PuestoId), "El puesto seleccionado no es válido o está inactivo.");
            }

            var emailNorm = (model.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (await _context.Empleados.AnyAsync(e => e.Cedula == model.Cedula))
                ModelState.AddModelError(nameof(model.Cedula), "Ya existe un empleado con esa cédula.");
            if (await _context.Empleados.AnyAsync(e => e.Email.ToLower() == emailNorm))
                ModelState.AddModelError(nameof(model.Email), "Email ya registrado.");

            // Username único
            var username = GenerarUsername(model.Nombre!, model.Apellido1!, model.Apellido2);
            if (await _context.Empleados.AnyAsync(e => e.Username == username))
            {
                username = GenerarUsernameFallback(model.Nombre!, model.Apellido1!);
                if (await _context.Empleados.AnyAsync(e => e.Username == username))
                {
                    var baseU = username;
                    int suf = 2;
                    while (await _context.Empleados.AnyAsync(e => e.Username == $"{baseU}{suf}")) suf++;
                    username = $"{baseU}{suf}";
                }
            }

            if (!ModelState.IsValid) { await LoadPuestosAsync(); return View(model); }

            // Crear empleado
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
                Activo = true,
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
                PuestoId = model.PuestoId,
                Rol = RolEmpleado.Colaborador
            };

            try
            {
                _context.Empleados.Add(emp);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                if (msg.Contains("UQ_EMPLEADO_Username", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError("", "El nombre de usuario generado ya está en uso. Intente nuevamente.");
                else if (msg.Contains("UQ_EMPLEADO_Email", StringComparison.OrdinalIgnoreCase))
                    ModelState.AddModelError(nameof(model.Email), "Email en uso.");
                else
                    ModelState.AddModelError("", "No se pudo registrar el empleado. " + msg);

                await LoadPuestosAsync();
                return View(model);
            }

            // Login directo
            await SignInEmpleadoAsync(emp, isPersistent: false);

            // AJAX: JSON para que el modal redirija; no-AJAX: redirect normal
            var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            if (isAjax)
                return Json(new { ok = true, redirectUrl = Url.Action("Index", "Home") });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult PuestoForm() => PartialView("Partials/_PuestoForm");

        // =====================
        //       LOGOUT
        // =====================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult RecuperarAcceso()
        {
            return View(); // busca Views/Account/RecuperarAcceso.cshtml
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RecuperarAcceso(string email, [FromServices] IEmailSender mail)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { ok = false, msg = "Debe ingresar un correo válido." });
            }

            var emp = await _context.Empleados.FirstOrDefaultAsync(e => e.Email.ToLower() == email.Trim().ToLower() && e.Activo);
            if (emp == null)
            {
                // Por seguridad, no confirmamos si el correo existe
                return Json(new { ok = true, msg = "Si el correo existe, se enviarán las instrucciones de acceso." });
            }

            // 1️⃣ Generar contraseña temporal
            string tempPassword = GenerarPasswordTemporal(8);
            emp.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            await _context.SaveChangesAsync();

            // 2️⃣ Leer plantilla HTML
            var templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplates", "RecuperarAcceso.html");
            var html = await System.IO.File.ReadAllTextAsync(templatePath, Encoding.UTF8);
            html = html.Replace("{{Nombre}}", emp.Nombre ?? "Usuario")
                       .Replace("{{Contrasenna}}", tempPassword);

            // 3️⃣ Enviar correo
            await mail.SendAsync(emp.Email, "Recuperación de acceso - CEGA", html);

            return Json(new { ok = true, msg = "Si el correo existe, se enviarán las instrucciones de acceso." });
        }

        private static string GenerarPasswordTemporal(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var bytes = RandomNumberGenerator.GetBytes(len);
            var sb = new StringBuilder(len);
            for (int i = 0; i < len; i++) sb.Append(chars[bytes[i] % chars.Length]);
            return sb.ToString();
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
                new Claim(ClaimTypes.Role, emp.Rol.ToString())
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

        public IActionResult Index() => View();
    }
}
