using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CEGA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> UsuariosProyectosData(int top = 10, CancellationToken ct = default)
        {
            // Agrupar por cédula (int) y contar proyectos DISTINTOS
            var lista = await _db.Tareas.AsNoTracking()
                .Where(t => t.CedulaEmpleado != 0 && t.ProyectoId != null) // aquí asumo que 0 = sin asignar
                .GroupBy(t => t.CedulaEmpleado) // CedulaEmpleado es int
                .Select(g => new
                {
                    Cedula = g.Key,
                    Proyectos = g.Select(x => x.ProyectoId!.Value).Distinct().Count()
                })
                .OrderByDescending(x => x.Proyectos)
                .ThenBy(x => x.Cedula)
                .Take(top)
                .ToListAsync(ct);

            // Intentar mapear cédula → nombre
            Dictionary<int, string> nombres = new();
            try
            {
                var cedulas = lista.Select(x => x.Cedula).ToList();
                nombres = await _db.Empleados.AsNoTracking()
                    .Where(e => cedulas.Contains(e.Cedula))
                    .Select(e => new { e.Cedula, e.Nombre })
                    .ToDictionaryAsync(x => x.Cedula, x => (x.Nombre ?? "").Trim(), ct);
            }
            catch
            {
                // fallback
            }

            // Etiquetas
            var labels = lista
                .Select(x => nombres.TryGetValue(x.Cedula, out var n) && !string.IsNullOrWhiteSpace(n)
                             ? n
                             : x.Cedula.ToString())
                .ToArray();

            var data = lista.Select(x => x.Proyectos).ToArray();

            var totalUsuarios = await _db.Tareas.AsNoTracking()
                .Where(t => t.CedulaEmpleado != 0) // si 0 es inválido
                .Select(t => t.CedulaEmpleado)
                .Distinct()
                .CountAsync(ct);

            var totalProyectos = await _db.Proyectos.AsNoTracking().CountAsync(ct);

            return Json(new { labels, data, kpis = new { totalUsuarios, totalProyectos } });
        }


    }
}
