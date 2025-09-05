using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CEGA.Controllers
{
    public class ContabilidadController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new ContabilidadPageVM
            {
                Resumen = new ResumenFinanciero { SaldoTotal = 0, IngresosMes = 0, GastosMes = 0 },
                Ingresos = Enumerable.Empty<MovimientoContable>(),
                Gastos = Enumerable.Empty<MovimientoContable>(),
                Cuentas = Enumerable.Empty<CuentaBancaria>()
            };
            return View(vm);
        }
    }
}
