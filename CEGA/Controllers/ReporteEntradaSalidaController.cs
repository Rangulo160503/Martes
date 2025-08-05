using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CEGA.Controllers
{
    public class ReporteEntradaSalidaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteEntradaSalidaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(ReporteEntradaSalida modelo)
        {
            if (!ModelState.IsValid)
            {
                if (modelo.FechaInicio > DateTime.Now || modelo.FechaFin > DateTime.Now)
                {
                    ModelState.AddModelError("FechaInicio", "No se pueden seleccionar fechas futuras");
                    ModelState.AddModelError("FechaFin", "No se pueden seleccionar fechas futuras");
                }

                if (string.IsNullOrEmpty(modelo.NombreReporte) &&
                    string.IsNullOrEmpty(modelo.RolSeleccionado) &&
                    modelo.FechaInicio == default &&
                    modelo.FechaFin == default &&
                    string.IsNullOrEmpty(modelo.Movimiento))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }

                return View(modelo);
            }

            _context.ReportesEntradasSalidas.Add(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Reporte de entradas/salidas generado correctamente";
            return RedirectToAction("Crear");
        }
        // GET: Mostrar formulario con los datos actuales
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var reporte = _context.ReportesEntradasSalidas.FirstOrDefault(r => r.Id == id);
            if (reporte == null)
            {
                TempData["Mensaje"] = "Reporte no encontrado";
                return RedirectToAction("Listar");
            }

            return View(reporte);
        }

        // POST: Guardar cambios
        [HttpPost]
        public IActionResult Editar(ReporteEntradaSalida modelo)
        {
            if (!ModelState.IsValid)
            {
                if (modelo.FechaInicio > DateTime.Now || modelo.FechaFin > DateTime.Now)
                {
                    ModelState.AddModelError("FechaInicio", "No se pueden seleccionar fechas futuras");
                    ModelState.AddModelError("FechaFin", "No se pueden seleccionar fechas futuras");
                }

                if (string.IsNullOrEmpty(modelo.NombreReporte) &&
                    string.IsNullOrEmpty(modelo.RolSeleccionado) &&
                    modelo.FechaInicio == default &&
                    modelo.FechaFin == default &&
                    string.IsNullOrEmpty(modelo.Movimiento))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }

                return View(modelo);
            }

            _context.ReportesEntradasSalidas.Update(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Reporte editado correctamente";
            return RedirectToAction("Listar");
        }
        [HttpGet]
        public IActionResult Listar()
        {
            var reportes = _context.ReportesEntradasSalidas.ToList();
            return View(reportes);
        }

    }
}
