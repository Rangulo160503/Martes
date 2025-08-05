using CEGA.Data;
using CEGA.Models;
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
        public IActionResult Empleados()
        {
            ViewBag.Salarios = _context.EmpleadosSalarios.ToList();
            ViewBag.Vacaciones = _context.VacacionesEmpleados.ToList();
            ViewBag.Puestos = _context.PuestosEmpleado.ToList();
            ViewBag.Incapacidades = _context.IncapacidadesEmpleado.ToList();
            return View();
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
                _context.EmpleadosSalarios.Add(new EmpleadoSalario
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
        public async Task<IActionResult> SubirIncapacidad(IFormFile archivo, string descripcion)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe adjuntar un documento.";
                return RedirectToAction("Empleados");
            }

            var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "incapacidades");
            if (!Directory.Exists(rutaCarpeta))
                Directory.CreateDirectory(rutaCarpeta);

            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
            var rutaFinal = Path.Combine(rutaCarpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFinal, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var incapacidad = new IncapacidadEmpleado
            {
                UsuarioID = user.Id,
                Descripcion = descripcion,
                ArchivoRuta = "/incapacidades/" + nombreArchivo
            };

            _context.IncapacidadesEmpleado.Add(incapacidad);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Incapacidad registrada correctamente.";
            return RedirectToAction("Empleados");
        }
        [HttpPost]
        public async Task<IActionResult> CambiarEstadoIncapacidad(int id, string nuevoEstado)
        {
            var incapacidad = await _context.IncapacidadesEmpleado.FindAsync(id);
            if (incapacidad == null)
            {
                TempData["Error"] = "Incapacidad no encontrada.";
                return RedirectToAction("Empleados");
            }

            incapacidad.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Incapacidad {nuevoEstado.ToLower()} correctamente.";
            return RedirectToAction("Empleados");
        }

    }
}
