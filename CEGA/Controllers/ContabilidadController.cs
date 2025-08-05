using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using CEGA.Utils;


namespace CEGA.Controllers
{
    public class ContabilidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContabilidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Ingresos()
        {
            var modelo = new ContabilidadViewModel
            {
                ListaIngresos = _context.Ingresos.OrderByDescending(i => i.Fecha).ToList()
            };
            return View("Ingresos", modelo); // Usa la vista Ingresos.cshtml
        }


        // INGRESOS
        [HttpPost]
        public IActionResult CrearIngreso(ContabilidadViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.ListaIngresos = _context.Ingresos.ToList();
                modelo.ListaEgresos = _context.Egresos.ToList();
                TempData["Error"] = "Error al registrar el ingreso.";
                return View("Index", modelo);
            }

            _context.Ingresos.Add(modelo.NuevoIngreso);
            _context.SaveChanges();
            TempData["Mensaje"] = "Ingreso registrado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditarIngreso(int id)
        {
            var ingreso = await _context.Ingresos.FindAsync(id);
            if (ingreso == null)
            {
                TempData["Error"] = "Ingreso no encontrado.";
                return RedirectToAction("Index");
            }
            return View("EditarIngreso", ingreso);
        }

        [HttpPost]
        public async Task<IActionResult> EditarIngreso(Ingreso ingreso)
        {
            if (!ModelState.IsValid)
                return View("EditarIngreso", ingreso);

            _context.Ingresos.Update(ingreso);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Ingreso actualizado.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EliminarIngreso(int id)
        {
            var ingreso = await _context.Ingresos.FindAsync(id);
            if (ingreso != null)
            {
                _context.Ingresos.Remove(ingreso);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Ingreso eliminado.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Egresos()
        {
            var modelo = new EgresoViewModel
            {
                ListaEgresos = _context.Egresos.OrderByDescending(e => e.Fecha).ToList()
            };
            return View(modelo);
        }

        [HttpPost]
        public IActionResult CrearEgreso(EgresoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.ListaEgresos = _context.Egresos.OrderByDescending(e => e.Fecha).ToList();
                TempData["Error"] = "Error al registrar el egreso.";
                return View("Egresos", modelo);
            }

            _context.Egresos.Add(modelo.NuevoEgreso);
            _context.SaveChanges();
            TempData["Mensaje"] = "Egreso registrado correctamente.";
            return RedirectToAction("Egresos");
        }
        [HttpGet]
        public async Task<IActionResult> EditarEgreso(int id)
        {
            var egreso = await _context.Egresos.FindAsync(id);
            if (egreso == null)
            {
                TempData["Error"] = "Egreso no encontrado.";
                return RedirectToAction("Egresos");
            }
            return View(egreso);
        }

        [HttpPost]
        public async Task<IActionResult> EditarEgreso(Egreso egreso)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Corrige los errores antes de guardar.";
                return View(egreso);
            }

            _context.Egresos.Update(egreso);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Egreso modificado correctamente.";
            return RedirectToAction("Egresos");
        }

        [HttpGet]
        public async Task<IActionResult> EliminarEgreso(int id)
        {
            var egreso = await _context.Egresos.FindAsync(id);
            if (egreso == null)
            {
                TempData["Error"] = "Egreso no encontrado.";
            }
            else
            {
                _context.Egresos.Remove(egreso);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Egreso eliminado correctamente.";
            }

            return RedirectToAction("Egresos");
        }
        [HttpGet]

        //Reporte
        public IActionResult ReporteFinanciero()
        {
            return View(new ReporteFinancieroViewModel());
        }

        [HttpPost]
        public IActionResult ReporteFinanciero(ReporteFinancieroViewModel modelo)
        {
            if (modelo.FechaInicio == default || modelo.FechaFin == default)
            {
                TempData["Error"] = "Debe seleccionar ambas fechas.";
                return View(modelo);
            }

            if (modelo.FechaInicio > DateTime.Now || modelo.FechaFin > DateTime.Now)
            {
                ModelState.AddModelError("", "No se pueden seleccionar fechas futuras.");
                return View(modelo);
            }

            if (modelo.FechaInicio > modelo.FechaFin)
            {
                ModelState.AddModelError("", "La fecha de inicio no puede ser mayor a la fecha final.");
                return View(modelo);
            }

            modelo.Ingresos = _context.Ingresos
                .Where(i => i.Fecha >= modelo.FechaInicio && i.Fecha <= modelo.FechaFin)
                .ToList();

            modelo.Egresos = _context.Egresos
                .Where(e => e.Fecha >= modelo.FechaInicio && e.Fecha <= modelo.FechaFin)
                .ToList();

            return View(modelo);
        }
        [HttpPost]
        public IActionResult ExportarPDF(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaInicio > fechaFin || fechaInicio > DateTime.Today || fechaFin > DateTime.Today)
            {
                TempData["Error"] = "No se pueden seleccionar fechas futuras o inválidas.";
                return RedirectToAction("ReporteFinanciero");
            }

            var ingresos = _context.Ingresos
                .Where(i => i.Fecha >= fechaInicio && i.Fecha <= fechaFin)
                .ToList();

            var egresos = _context.Egresos
                .Where(e => e.Fecha >= fechaInicio && e.Fecha <= fechaFin)
                .ToList();

            var modelo = new ReporteFinancieroViewModel
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Ingresos = ingresos,
                Egresos = egresos
            };

            var pdfBytes = GeneradorReportePDF.CrearReporte(modelo);

            return File(pdfBytes, "application/pdf", $"ReporteFinanciero_{DateTime.Now:yyyyMMdd}.pdf");
        }


    }
}
