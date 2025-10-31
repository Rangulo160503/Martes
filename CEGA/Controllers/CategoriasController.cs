using CEGA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace CEGA.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly string _cs;
        public CategoriasController(IConfiguration cfg)
        {
            _cs = cfg.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = new List<CategoriaMovimiento>();
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("SELECT CategoriaId, Nombre, Tipo, EsFijo, Activo, CreadoEn FROM dbo.CategoriaMovimiento ORDER BY Nombre", con);
            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new CategoriaMovimiento
                {
                    CategoriaId = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    Tipo = rd.GetString(2),
                    EsFijo = rd.GetBoolean(3),
                    Activo = rd.GetBoolean(4),
                    CreadoEn = rd.GetDateTime(5)
                });
            }
            return RedirectToAction("Index", "Contabilidad");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(string Nombre, string Tipo, bool EsFijo = false)
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Tipo))
            {
                TempData["Error"] = "Nombre y Tipo son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.CategoriaMovimiento (Nombre, Tipo, EsFijo, Activo, CreadoEn)
                VALUES (@Nombre, @Tipo, @EsFijo, 1, SYSUTCDATETIME())", con);

            cmd.Parameters.AddWithValue("@Nombre", Nombre);
            cmd.Parameters.AddWithValue("@Tipo", Tipo);
            cmd.Parameters.AddWithValue("@EsFijo", EsFijo);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            TempData["Mensaje"] = "Categoría creada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("DELETE FROM dbo.CategoriaMovimiento WHERE CategoriaId=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            await con.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            TempData["Mensaje"] = rows > 0 ? "Categoría eliminada." : "No se encontró la categoría.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCategoria(string Nombre, string Tipo, bool EsFijo = false)
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Tipo))
            {
                TempData["Error"] = "Nombre y Tipo son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(@"
        INSERT INTO dbo.CategoriaMovimiento (Nombre, Tipo, EsFijo, Activo, CreadoEn)
        VALUES (@Nombre, @Tipo, @EsFijo, 1, SYSUTCDATETIME())", con);

            cmd.Parameters.AddWithValue("@Nombre", Nombre);
            cmd.Parameters.AddWithValue("@Tipo", Tipo);
            cmd.Parameters.AddWithValue("@EsFijo", EsFijo);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            TempData["Mensaje"] = "Categoría agregada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("DELETE FROM dbo.CategoriaMovimiento WHERE CategoriaId=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            await con.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            TempData["Mensaje"] = rows > 0 ? "Categoría eliminada." : "No se encontró la categoría.";
            return RedirectToAction(nameof(Index));
        }

    }
}
