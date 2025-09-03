using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using CEGA.Models;
using CEGA.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CEGA.Controllers
{
    public class MarketingController : Controller
    {
        private readonly string _cs;
        public MarketingController(IConfiguration cfg) => _cs = cfg.GetConnectionString("DefaultConnection");

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new MarketingPageVM { Clientes = await GetClientesAsync() };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCliente(ClienteMarketing input)
        {
            if (!ModelState.IsValid)
                return View("Index", new MarketingPageVM { Clientes = await GetClientesAsync() });

            // ¿correo duplicado?
            using (var con = new SqlConnection(_cs))
            using (var dup = new SqlCommand("SELECT 1 FROM dbo.Clientes WHERE Correo=@Correo", con))
            {
                dup.Parameters.AddWithValue("@Correo", input.Correo);
                await con.OpenAsync();
                var exists = await dup.ExecuteScalarAsync();
                if (exists != null)
                {
                    ModelState.AddModelError("Correo", "Ese correo ya está registrado.");
                    return View("Index", new MarketingPageVM { Clientes = await GetClientesAsync() });
                }
                using (var ins = new SqlCommand("INSERT INTO dbo.Clientes (Nombre, Correo) VALUES (@Nombre,@Correo)", con))
                {
                    ins.Parameters.AddWithValue("@Nombre", input.Nombre);
                    ins.Parameters.AddWithValue("@Correo", input.Correo);
                    await ins.ExecuteNonQueryAsync();
                }
            }

            TempData["Mensaje"] = "Cliente agregado.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<ClienteMarketing>> GetClientesAsync()
        {
            var list = new List<ClienteMarketing>();
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("SELECT Id, Nombre, Correo FROM dbo.Clientes ORDER BY Nombre", con);
            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new ClienteMarketing
                {
                    Id = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    Correo = rd.GetString(2)
                });
            }
            return list;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarCliente([FromForm] CEGA.Models.ClienteMarketing input)
        {
            if (string.IsNullOrWhiteSpace(input.Nombre) || string.IsNullOrWhiteSpace(input.Correo))
            {
                TempData["Error"] = "Nombre y Correo son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var con = new SqlConnection(_cs);
                await con.OpenAsync();

                // Correo único (excluyendo el propio Id)
                using (var dup = new SqlCommand("SELECT 1 FROM dbo.Clientes WHERE Correo=@Correo AND Id<>@Id", con))
                {
                    dup.Parameters.AddWithValue("@Correo", input.Correo);
                    dup.Parameters.AddWithValue("@Id", input.Id);
                    var exists = await dup.ExecuteScalarAsync();
                    if (exists != null)
                    {
                        TempData["Error"] = "Ese correo ya está registrado en otro cliente.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                using (var up = new SqlCommand(
                    "UPDATE dbo.Clientes SET Nombre=@Nombre, Correo=@Correo WHERE Id=@Id", con))
                {
                    up.Parameters.AddWithValue("@Nombre", input.Nombre);
                    up.Parameters.AddWithValue("@Correo", input.Correo);
                    up.Parameters.AddWithValue("@Id", input.Id);
                    var rows = await up.ExecuteNonQueryAsync();
                    TempData["Mensaje"] = rows > 0 ? "Cliente actualizado." : "No se encontró el cliente.";
                }
            }
            catch (SqlException)
            {
                TempData["Error"] = "No se pudo actualizar el cliente.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCliente([FromForm] int Id)
        {
            try
            {
                using var con = new SqlConnection(_cs);
                using var del = new SqlCommand("DELETE FROM dbo.Clientes WHERE Id=@Id", con);
                del.Parameters.AddWithValue("@Id", Id);
                await con.OpenAsync();
                var rows = await del.ExecuteNonQueryAsync();
                TempData["Mensaje"] = rows > 0 ? "Cliente eliminado." : "No se encontró el cliente.";
            }
            catch (SqlException)
            {
                TempData["Error"] = "No se pudo eliminar el cliente.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
