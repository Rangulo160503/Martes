using CEGA.Data;
using CEGA.Models;
using CEGA.Models.ViewModels;
using CEGA.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CEGA.Controllers
{
    public class ContabilidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContabilidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------- Ingresos ----------------
        [HttpGet]
        public IActionResult Ingresos()
        {
            var modelo = new ContabilidadViewModel
            {
                NuevoIngreso = new Ingreso { Fecha = DateTime.Today },
                ListaIngresos = _context.Ingresos.OrderByDescending(i => i.Fecha).ToList()
            };
            return View("Ingresos", modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearIngreso([Bind(Prefix = "NuevoIngreso")] Ingreso ingreso)
        {
            if (ingreso.Fecha == default)
                ingreso.Fecha = DateTime.Today;

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                TempData["Error"] = "Error al registrar el ingreso: " + errores;

                var vm = new ContabilidadViewModel
                {
                    NuevoIngreso = new Ingreso { Fecha = DateTime.Today },
                    ListaIngresos = _context.Ingresos.OrderByDescending(i => i.Fecha).ToList()
                };
                TempData["Error"] = "Error al registrar el ingreso.";
                return View("Ingresos", vm);
            }

            _context.Ingresos.Add(ingreso);
            _context.SaveChanges();
            TempData["Mensaje"] = "Ingreso registrado correctamente.";
            return RedirectToAction("Ingresos");
        }


        [HttpGet]
        public async Task<IActionResult> EditarIngreso(int id)
        {
            var ingreso = await _context.Ingresos.FindAsync(id);
            if (ingreso == null)
            {
                TempData["Error"] = "Ingreso no encontrado.";
                return RedirectToAction("Ingresos");
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
            return RedirectToAction("Ingresos");
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
            return RedirectToAction("Ingresos");
        }

        // ---------------- Egresos ----------------
        [HttpGet]
        public IActionResult Egresos()
        {
            var modelo = new EgresoViewModel
            {
                NuevoEgreso = new Egreso { Fecha = DateTime.Today },
                ListaEgresos = _context.Egresos.OrderByDescending(e => e.Fecha).ToList()
            };
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearEgreso([Bind(Prefix = "NuevoEgreso")] Egreso egreso)
        {
            if (egreso.Fecha == default) egreso.Fecha = DateTime.Today; 

            if (!ModelState.IsValid)
            {
                var vm = new EgresoViewModel
                {
                    NuevoEgreso = new Egreso { Fecha = DateTime.Today },
                    ListaEgresos = _context.Egresos.OrderByDescending(e => e.Fecha).ToList()
                };
                TempData["Error"] = "Error al registrar el egreso.";
                return View("Egresos", vm);
            }

            _context.Egresos.Add(egreso);
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

        // ---------------- Reporte Financiero ----------------
        [HttpGet]
        public IActionResult ReporteFinanciero()
        {
            // Por defecto: Rango manual sin datos
            return View(new ReporteFinancieroViewModel());
        }

        [HttpPost]
        public IActionResult ReporteFinanciero(ReporteFinancieroViewModel modelo)
        {
            var (ini, fin) = Rango(modelo.Periodo, modelo.FechaInicio, modelo.FechaFin);

            if (ini == default || fin == default || ini >= fin)
            {
                ModelState.AddModelError("", "Rango de fechas inválido.");
                return View(modelo);
            }
            if (ini > DateTime.Today || fin > DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("", "No se permiten fechas futuras.");
                return View(modelo);
            }

            modelo.FechaInicio = ini;
            modelo.FechaFin = fin;

            modelo.Ingresos = _context.Ingresos.Where(i => i.Fecha >= ini && i.Fecha < fin).ToList();
            modelo.Egresos = _context.Egresos.Where(e => e.Fecha >= ini && e.Fecha < fin).ToList();

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

            var modelo = new ReporteFinancieroViewModel
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Ingresos = _context.Ingresos.Where(i => i.Fecha >= fechaInicio && i.Fecha < fechaFin).ToList(),
                Egresos = _context.Egresos.Where(e => e.Fecha >= fechaInicio && e.Fecha < fechaFin).ToList()
            };

            var pdfBytes = GeneradorReportePDF.CrearReporte(modelo);
            return File(pdfBytes, "application/pdf", $"ReporteFinanciero_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // Helper de rangos (dentro del controller)
        private (DateTime ini, DateTime fin) Rango(PeriodoReporte periodo, DateTime ini, DateTime fin)
        {
            var hoy = DateTime.Today;

            return periodo switch
            {
                PeriodoReporte.SemanaActual =>
                    (hoy.AddDays(-(int)hoy.DayOfWeek), hoy.AddDays(1)), // dom..hoy(+1 exclusivo)
                PeriodoReporte.MesActual =>
                    (new DateTime(hoy.Year, hoy.Month, 1), new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1)),
                PeriodoReporte.AnioActual =>
                    (new DateTime(hoy.Year, 1, 1), new DateTime(hoy.Year + 1, 1, 1)),
                _ => (ini, fin == default ? ini.AddDays(1) : fin) // Rango manual
            };
        }
    }
}
