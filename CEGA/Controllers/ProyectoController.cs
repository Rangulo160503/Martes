// Controllers/ProyectoController.cs
using CEGA.Data;
using CEGA.Models;
using CEGA.Models.Seguimiento;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CEGA.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProyectoController(ApplicationDbContext db) => _db = db;

        // GET: /Proyecto/Index  (reúne partials; si hay id, muestra panel de asignación dentro del partial de crear)
        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            // lista + conteo
            var filas = await _db.Set<Proyecto>()
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new ProyectoFilaVM
                {
                    IdProyecto = p.IdProyecto,
                    Nombre = p.Nombre,
                    CantidadTareas = _db.Tareas.Count(t => t.ProyectoId == p.IdProyecto)
                })
                .ToListAsync();

            ViewBag.Proyectos = filas;

            // si viene id, preparamos el panel de asignación para incrustarlo en el mismo partial de crear
            if (id.HasValue)
            {
                var proyecto = await _db.Set<Proyecto>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.IdProyecto == id.Value);
                if (proyecto != null)
                {
                    var asignar = new ProyectoAsignarVM
                    {
                        IdProyecto = proyecto.IdProyecto,
                        NombreProyecto = proyecto.Nombre,

                        NuevaTarea = new TareaCrearVM
                        {
                            ProyectoId = proyecto.IdProyecto,
                            Empleados = await _db.Set<Empleado>()
                                .AsNoTracking()
                                .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                                .Select(e => new SelectListItem
                                {
                                    Value = e.Cedula.ToString(),
                                    Text = $"{e.Nombre} {e.SegundoNombre} {e.Apellido1} {e.Apellido2}".Replace("  ", " ").Trim()
                                })
                                .ToListAsync()
                        },

                        TareasDelProyecto = await _db.Tareas.AsNoTracking()
                            .Where(t => t.ProyectoId == proyecto.IdProyecto)
                            .OrderByDescending(t => t.Id)
                            .ToListAsync(),

                        TareasSinProyecto = await _db.Tareas.AsNoTracking()
                            .Where(t => t.ProyectoId == null)
                            .OrderByDescending(t => t.Id)
                            .ToListAsync()
                    };

                    ViewBag.Asignar = asignar; // 👈 lo leerá el partial _CrearProyectoPartial
                }
            }

            return View(new ProyectoCrearVM()); // modelo del partial de creación
        }

        // POST: /Proyecto/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProyectoCrearVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Proyectos = await _db.Set<Proyecto>()
                    .AsNoTracking()
                    .OrderBy(p => p.Nombre)
                    .Select(p => new ProyectoFilaVM
                    {
                        IdProyecto = p.IdProyecto,
                        Nombre = p.Nombre,
                        CantidadTareas = _db.Tareas.Count(t => t.ProyectoId == p.IdProyecto)
                    })
                    .ToListAsync();

                return View("Index", vm);
            }

            var p = new Proyecto { Nombre = vm.Nombre.Trim() };
            _db.Add(p);
            await _db.SaveChangesAsync();

            // Después de crear, volvemos a Index mostrando DENTRO del partial el panel de asignación
            return RedirectToAction(nameof(Index), new { id = p.IdProyecto });
        }

        // POST: /Proyecto/AsignarTareas  (asigna tareas existentes al proyecto)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarTareas(int id, int[] tareaIds)
        {
            if (tareaIds is { Length: > 0 })
            {
                var tareas = await _db.Tareas
                    .Where(t => tareaIds.Contains(t.Id))
                    .ToListAsync();

                foreach (var t in tareas)
                    t.ProyectoId = id;

                await _db.SaveChangesAsync();
            }

            // Volver al Index con el panel de asignación abierto
            return RedirectToAction(nameof(Index), new { id });
        }
    }
}
