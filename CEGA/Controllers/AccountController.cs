using CEGA.Data;
using CEGA.Models;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEGA.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
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

            // Permitir email o username en el mismo campo
            ApplicationUser? user = null;
            var input = model.UserOrEmail?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
            {
                user = input.Contains("@")
                    ? await _userManager.FindByEmailAsync(input)
                    : await _userManager.FindByNameAsync(input);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Credenciales no registradas");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, model.Password!, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var target = (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            ? model.ReturnUrl : Url.Action("Index", "Home")!;
                return LocalRedirect(target);
            }

            ModelState.AddModelError("",
                result.IsLockedOut ? "Cuenta bloqueada por intentos fallidos" : "Credenciales incorrectas");
            return View(model);
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
            if (!ModelState.IsValid)
            {
                model.Puestos = await _context.Puestos.Where(p => p.Activo)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre }).ToListAsync();
                return View(model);
            }

            // Validaciones de negocio (mínimas)
            if (model.Cedula < 100000000 || model.Cedula > 999999999)
                ModelState.AddModelError(nameof(model.Cedula), "Cédula debe tener 9 dígitos.");
            if (model.TelefonoPersonal?.Length != 8)
                ModelState.AddModelError(nameof(model.TelefonoPersonal), "Teléfono personal: 8 dígitos.");
            if (model.TelefonoEmergencia?.Length != 8)
                ModelState.AddModelError(nameof(model.TelefonoEmergencia), "Teléfono emergencia: 8 dígitos.");
            if ((model.FechaIngreso - model.FechaNacimiento).TotalDays < 18 * 365)
                ModelState.AddModelError(nameof(model.FechaIngreso), "Debe ser mayor de 18 años.");

            // Unicidad en EMPLEADO
            if (await _context.Empleados.AnyAsync(e => e.Cedula == model.Cedula))
                ModelState.AddModelError(nameof(model.Cedula), "Ya existe un empleado con esa cédula.");
            if (await _context.Empleados.AnyAsync(e => e.Email == model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email ya registrado en empleados.");

            if (!ModelState.IsValid)
            {
                model.Puestos = await _context.Puestos.Where(p => p.Activo)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre }).ToListAsync();
                return View(model);
            }

            // Username por regla (minúsculas, sin tildes, ñ->n)
            var username = GenerarUsername(model.Nombre!, model.Apellido1!, model.Apellido2);
            if (await _context.Empleados.AnyAsync(e => e.Username == username))
                username = GenerarUsernameFallback(model.Nombre!, model.Apellido1!);

            // 1) Crear AspNetUser con ese username
            var user = new ApplicationUser
            {
                UserName = username,
                Email = model.Email,
                EmailConfirmed = true,
                PhoneNumber = model.TelefonoPersonal
            };
            var create = await _userManager.CreateAsync(user, model.Password!);
            if (!create.Succeeded)
            {
                foreach (var e in create.Errors) ModelState.AddModelError("", e.Description);
                model.Puestos = await _context.Puestos.Where(p => p.Activo)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre }).ToListAsync();
                return View(model);
            }

            // 2) Insertar EMPLEADO vinculado
            var emp = new Empleado
            {
                Cedula = model.Cedula,
                Nombre = model.Nombre!,
                SegundoNombre = model.SegundoNombre,
                Apellido1 = model.Apellido1!,
                Apellido2 = model.Apellido2,
                Username = username,
                Email = model.Email!,
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
                AspNetUserId = user.Id
            };

            try
            {
                _context.Empleados.Add(emp);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Si falla EMPLEADO, revertimos el usuario creado
                await _userManager.DeleteAsync(user);
                ModelState.AddModelError("", "No se pudo registrar el empleado. Intenta de nuevo.");
                model.Puestos = await _context.Puestos.Where(p => p.Activo)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre }).ToListAsync();
                return View(model);
            }

            // 3) Login directo
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        // =====================
        //       LOGOUT
        // =====================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        // =====================
        //   Helpers username
        // =====================
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
            return new string(chars.ToArray()).Replace('ñ', 'n').Replace('Ñ', 'n');
        }
    }
}
