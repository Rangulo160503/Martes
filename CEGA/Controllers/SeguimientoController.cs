using CEGA.Models.Seguimiento;
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
        public async Task<IActionResult> Index(int? proyectoId)
        {
            // Tareas (filtradas si viene proyectoId)
            var query = _db.Tareas.AsNoTracking();
            if (proyectoId.HasValue) query = query.Where(t => t.ProyectoId == proyectoId.Value);

            var tareas = await query.OrderByDescending(t => t.Id).ToListAsync();
            ViewBag.Tareas = tareas;

            // Dropdowns
            var empleados = await _db.Set<Empleado>()
                .AsNoTracking()
                .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                .Select(e => new SelectListItem { Value = e.Cedula.ToString(), Text = $"{e.Nombre} {e.SegundoNombre} {e.Apellido1} {e.Apellido2}".Replace("  ", " ").Trim() })
                .ToListAsync();

            var proyectosSelect = await _db.Set<Proyecto>()
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem { Value = p.IdProyecto.ToString(), Text = p.Nombre })
                .ToListAsync();

            // VM de crear tarea (preselecciona proyecto si viene)
            var vm = new TareaCrearVM
            {
                Empleados = empleados,
                Proyectos = proyectosSelect,
                ProyectoId = proyectoId
            };

            // (si usas la tabla de proyectos arriba en la misma vista)
            var proyectosTabla = await _db.Set<Proyecto>()
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new ProyectoFilaVM
                {
                    IdProyecto = p.IdProyecto,
                    Nombre = p.Nombre,
                    CantidadTareas = _db.Tareas.Count(t => t.ProyectoId == p.IdProyecto)
                }).ToListAsync();
            ViewBag.Proyectos = proyectosTabla;

            ViewBag.EmpleadosSelect = empleados;
            ViewBag.ProyectosSelect = proyectosSelect;

            return View(vm);
        }


        // POST: /Seguimiento/CrearTarea   (ya lo tenías)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTarea(TareaCrearVM vm)
        {
            if (!ModelState.IsValid)
            {
                // recargar dropdowns y listas
                vm.Empleados = await _db.Set<Empleado>().AsNoTracking()
                    .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                    .Select(e => new SelectListItem { Value = e.Cedula.ToString(), Text = $"{e.Nombre} {e.SegundoNombre} {e.Apellido1} {e.Apellido2}".Replace("  ", " ").Trim() })
                    .ToListAsync();

                vm.Proyectos = await _db.Set<Proyecto>().AsNoTracking()
                    .OrderBy(p => p.Nombre)
                    .Select(p => new SelectListItem { Value = p.IdProyecto.ToString(), Text = p.Nombre })
                    .ToListAsync();

                ViewBag.Tareas = await _db.Tareas.AsNoTracking().OrderByDescending(t => t.Id).ToListAsync();
                ViewBag.Proyectos = await _db.Set<Proyecto>().AsNoTracking()
                    .OrderBy(p => p.Nombre)
                    .Select(p => new ProyectoFilaVM
                    {
                        IdProyecto = p.IdProyecto,
                        Nombre = p.Nombre,
                        CantidadTareas = _db.Tareas.Count(t => t.ProyectoId == p.IdProyecto)
                    })
                    .ToListAsync();

                ViewBag.ProyectoVM = new ProyectoCrearVM();
                return View("Index", vm);
            }

            var nueva = new Tarea
            {
                Titulo = vm.Titulo,
                CedulaEmpleado = vm.CedulaEmpleado,
                ComentarioInicial = vm.Comentario,
                ProyectoId = vm.ProyectoId // puede ser null
            };

            _db.Tareas.Add(nueva);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { proyectoId = vm.ProyectoId });
        }

        // NUEVO: crear proyecto desde el mismo Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProyecto(ProyectoCrearVM vm)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var p = new Proyecto { Nombre = vm.Nombre.Trim() };
            _db.Add(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { proyectoId = p.IdProyecto });
        }

        // NUEVO: asignar tareas existentes al proyecto (checkboxes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarTareas(int proyectoId, int[] tareaIds)
        {
            if (tareaIds is { Length: > 0 })
            {
                var tareas = await _db.Tareas.Where(t => tareaIds.Contains(t.Id)).ToListAsync();
                foreach (var t in tareas) t.ProyectoId = proyectoId;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { proyectoId });
        }
        // POST: /Seguimiento/EliminarTarea/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarTarea(int id, int? returnProyectoId)
        {
            var t = await _db.Tareas.FindAsync(id);
            if (t != null)
            {
                _db.Tareas.Remove(t);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { proyectoId = returnProyectoId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarTareaInline(TareaEditarInlinePost vm)
        {
            var t = await _db.Tareas.FindAsync(vm.Id);
            if (t == null) return NotFound();

            t.Titulo = vm.Titulo?.Trim() ?? "";
            t.ProyectoId = vm.ProyectoId;
            t.CedulaEmpleado = vm.CedulaEmpleado;
            t.ComentarioInicial = vm.Comentario;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { proyectoId = vm.ReturnProyectoId ?? vm.ProyectoId });
        }
    }
}
