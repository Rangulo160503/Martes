using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // IFormFile
using System;
using System.Collections.Generic;
using System.IO;                 // MemoryStream, Path
using System.Linq;
using System.Threading.Tasks;

namespace CEGA.Controllers
{
    public class ReporteIncapacidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteIncapacidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ====== Incapacidades (modelo IncapacidadEmpleado) ======

        // Lista de incapacidades para @model IEnumerable<IncapacidadEmpleado>
        [HttpGet]
        public IActionResult Index()
        {
            var incapacidades = _context.IncapacidadesEmpleado
                .AsNoTracking()
                .OrderByDescending(x => x.Id) // Antes: .OrderByDescending(x => x.FechaPresentacion)
                .ToList();

            // Empleados (AspNetUsers) para selects en vistas
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

            // Salarios para sugerencias en UI
            var salarios = _context.EmpleadosSalarios
                .AsNoTracking()
                .ToList();

            ViewBag.Empleados = empleados;
            ViewBag.Salarios = salarios;

            return View(incapacidades);
        }

        [HttpPost]
        public async Task<IActionResult> SubirIncapacidad(IFormFile archivo, string descripcion, string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(usuarioId) || archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un empleado y adjuntar un documento.";
                return RedirectToAction("Empleados");
            }

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
                ArchivoTamano = archivo.Length
                // Ya no se asigna FechaPresentacion ni Estado (no existen en el modelo)
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

            // Se elimina la rama de ArchivoRuta porque el modelo no la tiene
            return NotFound();
        }

        // ====== Reportes de incapacidades (modelo ReporteIncapacidad) ======

        [HttpGet]
        public IActionResult Crear()
        {
            var hoy = DateTime.Today;

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

            ViewBag.Empleados = empleados;
            ViewBag.SalPorUsuario = salarios;

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

        // Editar ReporteIncapacidad
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

            return View(reporte);
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
    }
}
