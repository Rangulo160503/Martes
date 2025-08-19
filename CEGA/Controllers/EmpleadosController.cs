using CEGA.Data;
using CEGA.Models;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CEGA.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmpleadosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Nuevo()
        {
            // (Opcional) Roles personalizados
            ViewBag.SubRoles = new[] { "Empleado", "Supervisor", "RRHH", "Admin" };
            return View(); // en esa vista incluyes el partial
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearEmpleadoVM vm)
        {
            if (!ModelState.IsValid)
            {
                // Si fue AJAX, devuelve 400 con errores
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errs = ModelState.Where(kv => kv.Value!.Errors.Any())
                                         .ToDictionary(kv => kv.Key, kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
                    return BadRequest(new { ok = false, errors = errs });
                }

                ViewBag.SubRoles = new[] { "Empleado", "Supervisor", "RRHH", "Admin" };
                return PartialView("_CrearEmpleadoPartial", vm);
            }

            var userName = string.IsNullOrWhiteSpace(vm.UserName) ? vm.Email : vm.UserName;

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                SubRol = string.IsNullOrWhiteSpace(vm.SubRol) ? "Empleado" : vm.SubRol,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(new { ok = false, errors = result.Errors.Select(e => e.Description).ToArray() });

                foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
                ViewBag.SubRoles = new[] { "Empleado", "Supervisor", "RRHH", "Admin" };
                return PartialView("_CrearEmpleadoPartial", vm);
            }

            if (vm.SalarioMensual.HasValue)
            {
                _context.EmpleadosSalarios.Add(new EmpleadosSalarios
                {
                    UsuarioId = user.Id,
                    SalarioMensual = vm.SalarioMensual.Value,
                    FechaRegistro = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    ok = true,
                    empleado = new
                    {
                        id = user.Id,
                        nombre = string.IsNullOrWhiteSpace(((user.Nombre ?? "") + " " + (user.Apellido ?? "")).Trim())
                            ? (user.UserName ?? "")
                            : ((user.Nombre ?? "").Trim() + " " + (user.Apellido ?? "").Trim()),
                        salarioMensual = vm.SalarioMensual
                    }
                });
            }

            TempData["Mensaje"] = "Empleado creado correctamente.";
            return RedirectToAction("Index", "Empleados");
        }

        [HttpGet]
        public IActionResult Empleados()
        {
            var incapacidades = _context.IncapacidadesEmpleado
                .AsNoTracking()
                .OrderByDescending(x => x.FechaPresentacion)
                .ToList();

            // Usuarios existentes (mostrar "Nombre Apellido" o UserName si faltan)
            var empleados = _context.Users
                .AsNoTracking()
                .Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    UserName = string.IsNullOrWhiteSpace(((u.Nombre ?? "") + " " + (u.Apellido ?? "")).Trim())
                                ? (u.UserName ?? "")
                                : ((u.Nombre ?? "").Trim() + " " + (u.Apellido ?? "").Trim()),
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    SubRol = u.SubRol
                })
                .OrderBy(u => u.UserName)
                .ToList();

            ViewBag.Empleados = empleados;
            ViewBag.Salarios = _context.EmpleadosSalarios.AsNoTracking().ToList(); // ver punto 3 para el tipo
            ViewBag.Vacaciones = _context.VacacionesEmpleados.AsNoTracking().ToList();
            ViewBag.Puestos = _context.PuestosEmpleado.AsNoTracking().ToList();
            ViewBag.Incapacidades = incapacidades;

            return View(incapacidades); // tu vista tiene @model IEnumerable<IncapacidadEmpleado>
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarSalario(string usuarioId, decimal salario)
        {
            if (string.IsNullOrWhiteSpace(usuarioId) || salario < 0)
            {
                TempData["Error"] = "Datos inválidos. Verifique el salario o el usuario.";
                return RedirectToAction("Empleados");
            }

            var existe = await _context.EmpleadosSalarios.FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);
            if (existe == null)
            {
                _context.EmpleadosSalarios.Add(new EmpleadosSalarios
                {
                    UsuarioId = usuarioId,
                    SalarioMensual = salario
                });
            }
            else
            {
                existe.SalarioMensual = salario;
                existe.FechaRegistro = DateTime.Now;
                _context.EmpleadosSalarios.Update(existe);
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Salario registrado o modificado exitosamente.";
            return RedirectToAction("Empleados");
        }
        [HttpPost]
        public async Task<IActionResult> SolicitarVacaciones(VacacionesEmpleado model)
        {
            var usuario = await _userManager.GetUserAsync(User);
            model.UsuarioID = usuario.Id;
            model.DiasDisponibles = 15; // Carga desde BD en escenarios reales

            if (model.DiasSolicitados > model.DiasDisponibles)
            {
                ModelState.AddModelError("DiasSolicitados", $"Solo tiene {model.DiasDisponibles} días disponibles. Puede solicitar extensión sin goce.");
                return View("Empleados", model);
            }

            if (ModelState.IsValid)
            {
                _context.VacacionesEmpleados.Add(model);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Solicitud enviada correctamente.";
                return RedirectToAction("Empleados");
            }

            return View("Empleados", model);
        }
        [HttpPost]
        public async Task<IActionResult> AsignarPuesto(PuestoEmpleado nuevo)
        {
            var user = await _userManager.FindByIdAsync(nuevo.UsuarioID);
            if (user == null)
            {
                TempData["Error"] = "Empleado no encontrado.";
                return RedirectToAction("Empleados");
            }

            var actual = _context.PuestosEmpleado.FirstOrDefault(p => p.UsuarioID == nuevo.UsuarioID);
            if (actual != null)
            {
                // Guardar en historial
                var historial = new HistorialPuestos
                {
                    UsuarioID = nuevo.UsuarioID,
                    PuestoAnterior = actual.Puesto,
                    PuestoNuevo = nuevo.Puesto,
                    FechaCambio = DateTime.Now,
                    SalarioAnterior = actual.SalarioAsignado,
                    SalarioNuevo = nuevo.SalarioAsignado
                };

                _context.HistorialPuestos.Add(historial);

                // Actualizar puesto
                actual.Puesto = nuevo.Puesto;
                actual.SalarioAsignado = nuevo.SalarioAsignado;
                actual.FechaAsignacion = DateTime.Now;
            }
            else
            {
                _context.PuestosEmpleado.Add(nuevo);
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Puesto actualizado correctamente.";
            return RedirectToAction("Empleados");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirIncapacidad(IFormFile archivo, string descripcion, string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "Debe seleccionar un empleado.";
                return RedirectToAction("Empleados");
            }
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                TempData["Error"] = "La descripción es obligatoria.";
                return RedirectToAction("Empleados");
            }
            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe adjuntar un documento.";
                return RedirectToAction("Empleados");
            }

            // Leer bytes del archivo en memoria
            byte[] contenido;
            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                contenido = ms.ToArray();
            }

            var incapacidad = new IncapacidadEmpleado
            {
                UsuarioID = usuarioId,
                Descripcion = descripcion.Trim(),
                ArchivoContenido = contenido,
                ArchivoNombre = Path.GetFileName(archivo.FileName),
                ArchivoTipo = archivo.ContentType,
                ArchivoTamano = archivo.Length
            };

            _context.IncapacidadesEmpleado.Add(incapacidad);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Incapacidad registrada correctamente.";
            return RedirectToAction("Empleados");
        }

        [HttpGet]
        public async Task<IActionResult> DescargarIncapacidad(int id)
        {
            var inc = await _context.IncapacidadesEmpleado.FindAsync(id);
            if (inc == null || inc.ArchivoContenido == null)
                return NotFound();

            var nombre = string.IsNullOrWhiteSpace(inc.ArchivoNombre) ? $"incapacidad_{id}" : inc.ArchivoNombre;
            var tipo = string.IsNullOrWhiteSpace(inc.ArchivoTipo) ? "application/octet-stream" : inc.ArchivoTipo;

            return File(inc.ArchivoContenido, tipo, nombre);
        }
    }
}
