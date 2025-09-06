using CEGA.Models;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CEGA.Controllers
{
    public class ReporteriaController : Controller
    {
        private readonly string _cs;
        private readonly IConfiguration _cfg;

        public ReporteriaController(IConfiguration cfg)
        {
            _cfg = cfg;
            _cs = cfg.GetConnectionString("DefaultConnection");
        }

        // GET /Reporteria
        // Muestra "Incapacidades por empleado" (temporal, mientras definimos Proyecto/EmpleadoProyecto)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var incap = await GetIncapacidadesPorEmpleadoAsync(); // ya lo tienes
            var acc = await GetAccidentesPorEmpleadoAsync();    // ya lo hiciste

            var vm = new ReporteriaPageVM
            {
                Incapacidades = new IncapacidadesPorEmpleadoVM
                {
                    Titulo = "Incapacidades por empleado",
                    Items = incap,
                    Total = incap.Sum(x => x.Cantidad)
                },
                Accidentes = new AccidentesPorEmpleadoVM
                {
                    Titulo = "Accidentes por empleado",
                    Items = acc.Select(x => new ConteoAccidentePorEmpleadoItem
                    {
                        Cedula = x.Cedula,
                        NombreCompleto = x.NombreCompleto,
                        Cantidad = x.Cantidad
                    }),
                    Total = acc.Sum(x => x.Cantidad)
                }
            };

            return View(vm);
        }

        // ===== Helper ADO.NET ===========
        private async Task<IEnumerable<ConteoPorEmpleadoItem>> GetIncapacidadesPorEmpleadoAsync()
        {
            var list = new List<ConteoPorEmpleadoItem>();

            const string sql = @"
SELECT
    e.Cedula,
    LTRIM(RTRIM(
        CONCAT(
            e.Nombre, ' ',
            NULLIF(e.SegundoNombre, ''), CASE WHEN ISNULL(e.SegundoNombre,'') <> '' THEN ' ' ELSE '' END,
            e.Apellido1, ' ',
            e.Apellido2
        )
    )) AS NombreCompleto,
    COUNT(*) AS Cantidad
FROM dbo.Incapacidad AS i
JOIN dbo.Empleado     AS e ON e.Cedula = i.Cedula
GROUP BY e.Cedula, e.Nombre, e.SegundoNombre, e.Apellido1, e.Apellido2
ORDER BY Cantidad DESC, NombreCompleto ASC;";

            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(sql, con);
            await con.OpenAsync();

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new ConteoPorEmpleadoItem
                {
                    Cedula = rd.GetInt32(0),
                    NombreCompleto = rd.GetString(1),
                    Cantidad = rd.GetInt32(2)
                });
            }

            return list;
        }
        // GET /Reporteria/Accidentes
        [HttpGet]
        public async Task<IActionResult> Accidentes()
        {
            var items = await GetAccidentesPorEmpleadoAsync();

            var vm = new IncapacidadesPorEmpleadoVM // reutilizamos el VM
            {
                Titulo = "Accidentes por empleado",
                Items = items,
                Total = items.Sum(x => x.Cantidad)
            };

            return View(vm); // Views/Reporteria/Accidentes.cshtml
        }

        // ===== Helper ADO.NET (Accidentes) ===========
        private async Task<IEnumerable<ConteoPorEmpleadoItem>> GetAccidentesPorEmpleadoAsync()
        {
            var list = new List<ConteoPorEmpleadoItem>();

            const string sql = @"
SELECT
    e.Cedula,
    LTRIM(RTRIM(
        CONCAT(
            e.Nombre, ' ',
            NULLIF(e.SegundoNombre, ''), CASE WHEN ISNULL(e.SegundoNombre,'') <> '' THEN ' ' ELSE '' END,
            e.Apellido1, ' ',
            e.Apellido2
        )
    )) AS NombreCompleto,
    COUNT(*) AS Cantidad
FROM dbo.Accidentes AS a               -- 👈 tabla plural
JOIN dbo.Empleado   AS e ON e.Cedula = a.CedulaEmpleado  -- 👈 columna real
GROUP BY e.Cedula, e.Nombre, e.SegundoNombre, e.Apellido1, e.Apellido2
ORDER BY Cantidad DESC, NombreCompleto ASC;";

            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand(sql, con);
            await con.OpenAsync();

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new ConteoPorEmpleadoItem
                {
                    Cedula = rd.GetInt32(0),
                    NombreCompleto = rd.GetString(1),
                    Cantidad = rd.GetInt32(2)
                });
            }

            return list;
        }
    }
}
