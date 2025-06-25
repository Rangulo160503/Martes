using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CEGA.Controllers
{
    public class CierresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CierresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CierrePorRango()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CierrePorRango(CierreRango cierre)
        {
            if (!ModelState.IsValid)
                return View(cierre);

            // Validar fechas futuras
            if (cierre.FechaInicio > DateTime.Now || cierre.FechaFin > DateTime.Now)
            {
                ModelState.AddModelError("", "No se pueden seleccionar fechas futuras.");
                return View(cierre);
            }

            // Validar duplicado
            bool existe = _context.CierresRango.Any(c =>
                c.FechaInicio.Date == cierre.FechaInicio.Date &&
                c.FechaFin.Date == cierre.FechaFin.Date);

            if (existe)
            {
                ModelState.AddModelError("", "Ya existe un cierre para ese rango de fechas.");
                return View(cierre);
            }

            // Calcular totales
            cierre.TotalIngresos = _context.Ingresos
                .Where(i => i.Fecha >= cierre.FechaInicio && i.Fecha <= cierre.FechaFin)
                .Sum(i => i.Monto);

            cierre.TotalEgresos = _context.Egresos
                .Where(e => e.Fecha >= cierre.FechaInicio && e.Fecha <= cierre.FechaFin)
                .Sum(e => e.Monto);

            cierre.BalanceFinal = cierre.TotalIngresos - cierre.TotalEgresos;

            _context.CierresRango.Add(cierre);
            _context.SaveChanges();

            TempData["Mensaje"] = "Cierre registrado correctamente.";
            return RedirectToAction("HistorialCierresRango");
        }

        [HttpGet]
        public IActionResult HistorialCierresRango()
        {
            var lista = _context.CierresRango.OrderByDescending(c => c.FechaFin).ToList();
            return View(lista);
        }
        [HttpGet]
        public IActionResult DesgloseReporte()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DesgloseReporte(DesgloseReporteViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            // Validación de selección de al menos una categoría
            if (!modelo.IncluirIngresos && !modelo.IncluirEgresos && !modelo.IncluirSalarios)
            {
                TempData["Error"] = "Debe seleccionar al menos una categoría: ingresos, egresos o salarios.";
                return View(modelo);
            }

            // Validar fechas futuras
            if (modelo.FechaInicio > DateTime.Now || modelo.FechaFin > DateTime.Now)
            {
                ModelState.AddModelError("", "No se pueden seleccionar fechas futuras.");
                return View(modelo);
            }

            // Validación extra de formulario completo
            if (string.IsNullOrWhiteSpace(modelo.NombreReporte))
            {
                TempData["Error"] = "Se necesita llenar todo el formulario";
                return View(modelo);
            }

            // Aquí procesarías la generación del desglose...
            TempData["Mensaje"] = $"Desglose del reporte \"{modelo.NombreReporte}\" generado correctamente.";
            return RedirectToAction("DesgloseReporte");
        }

    }
}
