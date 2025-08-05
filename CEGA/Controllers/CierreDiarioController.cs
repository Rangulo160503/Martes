using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CEGA.Controllers
{
    public class CierreDiarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CierreDiarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cierres = _context.CierresDiarios
                .OrderByDescending(c => c.Fecha)
                .ToList();

            return View(cierres);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar()
        {
            var fechaHoy = DateTime.Today;

            // Verifica si ya hay un cierre para hoy
            if (_context.CierresDiarios.Any(c => c.Fecha.Date == fechaHoy))
            {
                TempData["Error"] = "Ya existe un cierre registrado para el día de hoy.";
                return RedirectToAction("Index");
            }

            // Calcula ingresos y egresos del día
            var ingresos = _context.Ingresos
                .Where(i => i.Fecha.Date == fechaHoy)
                .Sum(i => (decimal?)i.Monto) ?? 0;

            var egresos = _context.Egresos
                .Where(e => e.Fecha.Date == fechaHoy)
                .Sum(e => (decimal?)e.Monto) ?? 0;

            var balance = ingresos - egresos;

            var cierre = new CierreDiario
            {
                Fecha = fechaHoy,
                TotalIngresos = ingresos,
                TotalEgresos = egresos,
                BalanceFinal = balance
            };

            _context.CierresDiarios.Add(cierre);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Cierre registrado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
