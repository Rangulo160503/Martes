using CEGA.Models;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CEGA.Controllers
{
    public class ContabilidadController : Controller
    {
        private readonly string _cs;

        public ContabilidadController(IConfiguration cfg)
        {
            _cs = cfg.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ingresos = await GetMovimientosAsync("I");
            var gastos = await GetMovimientosAsync("G");
            var cierres = await GetCierresAsync();
            var categorias = await GetCategoriasAsync();

            var vm = new ContabilidadPageVM
            {
                Resumen = new ResumenFinanciero
                {
                    SaldoTotal = ingresos.Sum(i => i.Monto) - gastos.Sum(g => g.Monto),
                    IngresosMes = ingresos.Where(i => i.Fecha.Month == DateTime.Now.Month).Sum(i => i.Monto),
                    GastosMes = gastos.Where(g => g.Fecha.Month == DateTime.Now.Month).Sum(g => g.Monto)
                },
                Ingresos = ingresos,
                Gastos = gastos,
                Cierres = cierres
            };

            ViewBag.Categorias = categorias; 
            return View(vm);
        }

        private async Task<IEnumerable<CategoriaMovimiento>> GetCategoriasAsync()
        {
            var list = new List<CategoriaMovimiento>();
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(
                "SELECT CategoriaId, Nombre, Tipo FROM dbo.CategoriaMovimiento WHERE Activo=1 ORDER BY Nombre", con);
            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new CategoriaMovimiento
                {
                    CategoriaId = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    Tipo = rd.GetString(2)
                });
            }
            return list;
        }


        /* =====================
           INGRESOS
           ===================== */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearIngreso(DateTime Fecha, string Concepto, int CategoriaId, decimal Monto)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Movimiento (Fecha, Concepto, CategoriaId, Tipo, Monto, Moneda, EsFijo, CreadoEn)
                VALUES (@Fecha, @Concepto, @CategoriaId, 'I', @Monto, 'CRC', 0, SYSUTCDATETIME())", con);

            cmd.Parameters.AddWithValue("@Fecha", Fecha);
            cmd.Parameters.AddWithValue("@Concepto", Concepto);
            cmd.Parameters.AddWithValue("@CategoriaId", CategoriaId);
            cmd.Parameters.AddWithValue("@Monto", Monto);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            TempData["Mensaje"] = "Ingreso registrado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarIngreso(int id)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("DELETE FROM dbo.Movimiento WHERE MovimientoId=@Id AND Tipo='I'", con);
            cmd.Parameters.AddWithValue("@Id", id);
            await con.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            TempData["Mensaje"] = rows > 0 ? "Ingreso eliminado." : "No se encontró el ingreso.";
            return RedirectToAction(nameof(Index));
        }

        /* =====================
           GASTOS
           ===================== */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearGasto(DateTime Fecha, string Concepto, int CategoriaId, decimal Monto)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Movimiento (Fecha, Concepto, CategoriaId, Tipo, Monto, Moneda, EsFijo, CreadoEn)
                VALUES (@Fecha, @Concepto, @CategoriaId, 'G', @Monto, 'CRC', 0, SYSUTCDATETIME())", con);

            cmd.Parameters.AddWithValue("@Fecha", Fecha);
            cmd.Parameters.AddWithValue("@Concepto", Concepto);
            cmd.Parameters.AddWithValue("@CategoriaId", CategoriaId);
            cmd.Parameters.AddWithValue("@Monto", Monto);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            TempData["Mensaje"] = "Gasto registrado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarGasto(int id)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("DELETE FROM dbo.Movimiento WHERE MovimientoId=@Id AND Tipo='G'", con);
            cmd.Parameters.AddWithValue("@Id", id);
            await con.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            TempData["Mensaje"] = rows > 0 ? "Gasto eliminado." : "No se encontró el gasto.";
            return RedirectToAction(nameof(Index));
        }

        /* =====================
           CIERRE DIARIO
           ===================== */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarCierre(DateTime fecha)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("EXEC dbo.GenerarCierreDiario @Dia", con);
            cmd.Parameters.AddWithValue("@Dia", fecha);
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            TempData["Mensaje"] = $"Cierre generado para {fecha:yyyy-MM-dd}.";
            return RedirectToAction(nameof(Index));
        }

        /* =====================
           Métodos privados de carga
           ===================== */
        private async Task<IEnumerable<MovimientoContable>> GetMovimientosAsync(string tipo)
        {
            var list = new List<MovimientoContable>();
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(@"
                SELECT m.MovimientoId, m.Fecha, m.Concepto, c.Nombre AS Categoria, m.Monto, m.Moneda
                FROM dbo.Movimiento m
                JOIN dbo.CategoriaMovimiento c ON c.CategoriaId = m.CategoriaId
                WHERE m.Tipo=@Tipo
                ORDER BY m.Fecha DESC", con);

            cmd.Parameters.AddWithValue("@Tipo", tipo);
            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new MovimientoContable
                {
                    Id = rd.GetInt32(0),
                    Fecha = rd.GetDateTime(1),
                    Concepto = rd.GetString(2),
                    Categoria = rd.GetString(3),
                    Monto = rd.GetDecimal(4),
                    Moneda = rd.GetString(5)
                });
            }
            return list;
        }

        private async Task<IEnumerable<CierreDiarioVM>> GetCierresAsync()
        {
            var list = new List<CierreDiarioVM>();
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(@"
                SELECT Fecha, TotalIngresos, TotalGastos, SaldoDia, CreadoEn
                FROM dbo.CierreDiario ORDER BY Fecha DESC", con);

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new CierreDiarioVM
                {
                    Fecha = rd.GetDateTime(0),
                    TotalIngresos = rd.GetDecimal(1),
                    TotalGastos = rd.GetDecimal(2),
                    SaldoDia = rd.GetDecimal(3),
                    CreadoEn = rd.GetDateTime(4)
                });
            }
            return list;
        }
    }
}
