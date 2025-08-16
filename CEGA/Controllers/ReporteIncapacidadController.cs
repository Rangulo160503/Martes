using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            var reportes = _context.ReportesIncapacidades.ToList();
            return View(reportes);
        }
        [HttpGet]
        public IActionResult Crear()
        {
            var hoy = DateTime.Today;
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

    }
}
