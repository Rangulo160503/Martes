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
        public IActionResult Index(string? q)
        {
            var query = _context.ReportesIncidentes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.ToLower();
                query = query.Where(x =>
                    x.NombreReporte.ToLower().Contains(t) ||
                    x.UsuarioID.ToLower().Contains(t) ||
                    x.Incapacidad.ToLower().Contains(t));
            }
            var data = query.OrderByDescending(x => x.FechaAccidente).ToList();
            ViewBag.Q = q;
            return View(data);
        }

        // Crear (GET)
        [HttpGet]
        public IActionResult Crear()
        {
            var modelo = new ReporteIncidente
            {
                FechaAccidente = DateTime.Today
            };
            return View(modelo);
        }

        // Crear (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ReporteIncidente modelo)
        {
            // Reglas adicionales
            if (modelo.FechaAccidente > DateTime.Today)
                ModelState.AddModelError(nameof(modelo.FechaAccidente), "No se permiten fechas futuras");

            if (!ModelState.IsValid)
                return View(modelo);

            _context.ReportesIncidentes.Add(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "Reporte de incidente creado correctamente";
            return RedirectToAction(nameof(Index));
        }

    }
}
