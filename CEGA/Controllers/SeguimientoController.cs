using CEGA.Models.Seguimiento;                // <— usa tu modelo
using CEGA.Data;
using CEGA.Models;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CEGA.Controllers
{
    public class SeguimientoController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SeguimientoController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tareas = await _db.Tareas
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            var vm = new TareaCrearVM
            {
                Empleados = await _db.Set<Empleado>()
                    .AsNoTracking()
                    .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                    .Select(e => new SelectListItem
                    {
                        Value = e.Cedula.ToString(),
                        Text = $"{e.Nombre} {e.SegundoNombre} {e.Apellido1} {e.Apellido2}".Replace("  ", " ").Trim()
                    })
                    .ToListAsync()
            };

            ViewBag.Tareas = tareas;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTarea(TareaCrearVM vm)
        {
            if (!ModelState.IsValid)
            {
                // volver a cargar dropdown
                vm.Empleados = await _db.Set<Empleado>()
                    .AsNoTracking()
                    .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                    .Select(e => new SelectListItem
                    {
                        Value = e.Cedula.ToString(),
                        Text = $"{e.Nombre} {e.SegundoNombre} {e.Apellido1} {e.Apellido2}".Replace("  ", " ").Trim()
                    })
                    .ToListAsync();

                ViewBag.Tareas = await _db.Tareas
                    .AsNoTracking()
                    .OrderByDescending(t => t.Id)
                    .ToListAsync();

                return View("Index", vm);
            }

            var nueva = new Tarea
            {
                Titulo = vm.Titulo,
                CedulaEmpleado = vm.CedulaEmpleado,
                ComentarioInicial = vm.Comentario,
                ProyectoId = null
            };

            _db.Tareas.Add(nueva);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
