using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CEGA.Controllers
{
    public class ReporteIncidenteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteIncidenteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(ReporteIncidente modelo)
        {
            if (!ModelState.IsValid)
            {
                // Validación total vacía
                if (string.IsNullOrEmpty(modelo.NombreReporte) &&
                    string.IsNullOrEmpty(modelo.UsuarioID) &&
                    modelo.FechaAccidente == default &&
                    string.IsNullOrEmpty(modelo.Descripcion) &&
                    string.IsNullOrEmpty(modelo.Incapacidad))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }

                return View(modelo);
            }

            _context.ReportesIncidentes.Add(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Reporte de incidente generado correctamente";
            return RedirectToAction("Crear");
        }
        // GET: Mostrar formulario con datos actuales
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var reporte = _context.ReportesIncidentes.FirstOrDefault(r => r.Id == id);
            if (reporte == null)
            {
                TempData["Mensaje"] = "Reporte no encontrado";
                return RedirectToAction("Listar");
            }

            return View(reporte);
        }

        // POST: Guardar los cambios
        [HttpPost]
        public IActionResult Editar(ReporteIncidente modelo)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(modelo.NombreReporte) &&
                    string.IsNullOrEmpty(modelo.UsuarioID) &&
                    modelo.FechaAccidente == default &&
                    string.IsNullOrEmpty(modelo.Descripcion) &&
                    string.IsNullOrEmpty(modelo.Incapacidad))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }

                return View(modelo);
            }

            _context.ReportesIncidentes.Update(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Reporte editado correctamente";
            return RedirectToAction("Listar");
        }
        public IActionResult Listar()
        {
            var incidentes = _context.ReportesIncidentes.ToList();
            return View(incidentes);
        }

    }
}
