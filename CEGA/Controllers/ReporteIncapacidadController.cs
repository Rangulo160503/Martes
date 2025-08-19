using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CEGA.Controllers
{
    public class ReporteIncapacidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteIncapacidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listar todos los reportes creados
        [HttpGet]
        public IActionResult Index()
        {
            // Incapacidades para el @model IEnumerable<IncapacidadEmpleado>
            var incapacidades = _context.IncapacidadesEmpleado
                .AsNoTracking()
                .OrderByDescending(x => x.FechaPresentacion)
                .ToList();

            // ===== Empleados (AspNetUsers) para el <select> =====
            // Mostramos "Nombre Apellido" y si no hay, cae a UserName
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

            // ===== Salarios para sugerir salario diario =====
            var salarios = _context.EmpleadosSalarios
                .AsNoTracking()
                .ToList();

            ViewBag.Empleados = empleados; // IEnumerable<ApplicationUser>
            ViewBag.Salarios = salarios;  // IEnumerable<EmpleadoSalario>

            return View(incapacidades);
        }
        [HttpGet]
        public IActionResult Crear()
        {
            var hoy = DateTime.Today;

            // 1) Empleados desde AspNetUsers
            var empleados = _context.Users
                .AsNoTracking()
                .Select(u => new
                {
                    u.Id,
                    Nombre = (
                        ((u.Nombre ?? "").Trim() + " " + (u.Apellido ?? "").Trim()).Trim()
                    ) != string.Empty
                        ? ((u.Nombre ?? "").Trim() + " " + (u.Apellido ?? "").Trim()).Trim()
                        : (u.UserName ?? "")
                })
                .OrderBy(x => x.Nombre)
                .ToList();

            // 2) Salario mensual más reciente por UsuarioId
            var salarios = _context.EmpleadosSalarios
                .AsNoTracking()
                .GroupBy(s => s.UsuarioId)
                .Select(g => new
                {
                    UsuarioId = g.Key,
                    SalarioMensual = g
                        .OrderByDescending(x => x.FechaRegistro)
                        .Select(x => x.SalarioMensual)
                        .FirstOrDefault()
                })
                .ToList()
                .ToDictionary(x => x.UsuarioId, x => x.SalarioMensual);

            // 3) Pasar a la vista (coincide con tu snippet)
            ViewBag.Empleados = empleados;               // lista { Id, Nombre }
            ViewBag.SalPorUsuario = salarios;            // dict<string, decimal>

            var modelo = new ReporteIncapacidad
            {
                FechaInicio = hoy,
                FechaFin = hoy
            };
            return View(modelo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ReporteIncapacidad modelo)
        {
            if (modelo.FechaInicio > DateTime.Today)
                ModelState.AddModelError(nameof(modelo.FechaInicio), "No se pueden seleccionar fechas futuras");
            if (modelo.FechaFin > DateTime.Today)
                ModelState.AddModelError(nameof(modelo.FechaFin), "No se pueden seleccionar fechas futuras");
            if (modelo.FechaFin < modelo.FechaInicio)
                ModelState.AddModelError(nameof(modelo.FechaFin), "La fecha final no puede ser menor que la fecha de inicio");

            if (!ModelState.IsValid) return View(modelo);

            _context.ReportesIncapacidades.Add(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Incapacidad creada correctamente";
            return RedirectToAction(nameof(Index));
        }


        // Vista para editar
        public IActionResult Editar(int id)
        {
            var reporte = _context.ReportesIncapacidades.Find(id);
            if (reporte == null)
                return NotFound();

            return View(reporte);
        }

        [HttpPost]
        public IActionResult Editar(ReporteIncapacidad modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            if (modelo.FechaInicio > DateTime.Now || modelo.FechaFin > DateTime.Now)
            {
                ModelState.AddModelError("FechaInicio", "No se pueden seleccionar fechas futuras");
                ModelState.AddModelError("FechaFin", "No se pueden seleccionar fechas futuras");
                return View(modelo);
            }

            if (string.IsNullOrEmpty(modelo.NombreReporte) &&
                string.IsNullOrEmpty(modelo.RolSeleccionado) &&
                modelo.FechaInicio == default &&
                modelo.FechaFin == default)
            {
                ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                return View(modelo);
            }

            _context.ReportesIncapacidades.Update(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Reporte editado correctamente";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Buscar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["Mensaje"] = "Se necesita llenar datos";
                return View(new List<ReporteIncapacidad>());
            }

            var resultados = _context.ReportesIncapacidades
                .Where(r => r.NombreReporte.ToLower().Contains(nombre.ToLower()))
                .ToList();

            if (!resultados.Any())
            {
                TempData["Mensaje"] = "No hay reportes con ese nombre";
            }

            return View(resultados);
        }
        // Confirmación de eliminación
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var reporte = _context.ReportesIncapacidades.FirstOrDefault(r => r.Id == id);
            if (reporte == null)
            {
                TempData["Mensaje"] = "Reporte no eliminado";
                return RedirectToAction("Index");
            }

            return View(reporte); // Mostrar confirmación
        }

        // Acción final para eliminar
        [HttpPost, ActionName("Eliminar")]
        public IActionResult ConfirmarEliminar(int id)
        {
            var reporte = _context.ReportesIncapacidades.FirstOrDefault(r => r.Id == id);
            if (reporte == null)
            {
                TempData["Mensaje"] = "Reporte no eliminado";
                return RedirectToAction("Index");
            }

            _context.ReportesIncapacidades.Remove(reporte);
            _context.SaveChanges();

            TempData["Mensaje"] = "Reporte eliminado exitosamente";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> SubirIncapacidad(IFormFile archivo, string descripcion, string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(usuarioId) || archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un empleado y adjuntar un documento.";
                return RedirectToAction("Empleados");
            }

            // Leer bytes del archivo
            byte[] contenido;
            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                contenido = ms.ToArray();
            }

            var incapacidad = new IncapacidadEmpleado
            {
                UsuarioID = usuarioId,
                Descripcion = descripcion,
                ArchivoContenido = contenido,
                ArchivoNombre = Path.GetFileName(archivo.FileName),
                ArchivoTipo = archivo.ContentType,
                ArchivoTamano = archivo.Length,
                FechaPresentacion = DateTime.Now,
                Estado = "Pendiente"
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
            if (inc == null) return NotFound();

            if (inc.ArchivoContenido != null)
                return File(inc.ArchivoContenido, inc.ArchivoTipo ?? "application/octet-stream", inc.ArchivoNombre ?? "archivo");

            if (!string.IsNullOrWhiteSpace(inc.ArchivoRuta))
            {
                var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", inc.ArchivoRuta.TrimStart('/'));
                if (System.IO.File.Exists(physical))
                    return PhysicalFile(physical, "application/octet-stream", Path.GetFileName(physical));
            }

            return NotFound();
        }

    }
}
