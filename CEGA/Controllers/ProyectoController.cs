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
        // Tabla (partial) para recargar por AJAX
        [HttpGet]
        public async Task<IActionResult> ProyectosTablaPartial(CancellationToken ct)
        {
            var filas = await _db.Proyectos.AsNoTracking()
                .OrderBy(p => p.IdProyecto)
                .Select(p => new ProyectoFilaVM
                {
                    IdProyecto = p.IdProyecto,
                    Nombre = p.Nombre,
                    CantidadTareas = _db.Tareas.Count(t => t.ProyectoId == p.IdProyecto)
                })
                .ToListAsync(ct);

            return PartialView("~/Views/Seguimiento/Partials/_ProyectosTablaPartial.cshtml", filas);
        }

        // GET: form crear
        [HttpGet]
        public IActionResult ProyectoCrearPartial()
        {
            var vm = new ProyectoFormVM
            {
                FormAction = Url.Action(nameof(CrearProyecto), "Seguimiento")!,
                SubmitText = "Crear"
            };
            return PartialView("~/Views/Seguimiento/Partials/_ProyectoFormPartial.cshtml", vm);
        }

        // GET: form editar
        [HttpGet]
        public async Task<IActionResult> ProyectoEditarPartial(int id, CancellationToken ct)
        {
            var p = await _db.Proyectos.AsNoTracking().FirstOrDefaultAsync(x => x.IdProyecto == id, ct);
            if (p is null) return NotFound();

            var vm = new ProyectoFormVM
            {
                IdProyecto = p.IdProyecto,
                Nombre = p.Nombre,
                FormAction = Url.Action(nameof(EditarProyecto), "Seguimiento")!,
                SubmitText = "Actualizar"
            };
            return PartialView("~/Views/Seguimiento/Partials/_ProyectoFormPartial.cshtml", vm);
        }

        // POST: crear (soporta AJAX y no-AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProyecto(ProyectoFormVM vm, CancellationToken ct)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    vm.FormAction = Url.Action(nameof(CrearProyecto), "Seguimiento")!;
                    vm.SubmitText = "Crear";
                    Response.StatusCode = 400;
                    return PartialView("~/Views/Seguimiento/Partials/_ProyectoFormPartial.cshtml", vm);
                }
                return RedirectToAction(nameof(Index));
            }

            var p = new Proyecto { Nombre = vm.Nombre.Trim() };
            _db.Add(p);
            await _db.SaveChangesAsync(ct);

            if (isAjax) return Ok(new { ok = true, id = p.IdProyecto });
            return RedirectToAction(nameof(Index), new { proyectoId = p.IdProyecto });
        }

        // POST: editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProyecto(ProyectoFormVM vm, CancellationToken ct)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid || vm.IdProyecto is null)
            {
                if (isAjax)
                {
                    vm.FormAction = Url.Action(nameof(EditarProyecto), "Seguimiento")!;
                    vm.SubmitText = "Actualizar";
                    Response.StatusCode = 400;
                    return PartialView("~/Views/Seguimiento/Partials/_ProyectoFormPartial.cshtml", vm);
                }
                return RedirectToAction(nameof(Index));
            }

            var p = await _db.Proyectos.FirstOrDefaultAsync(x => x.IdProyecto == vm.IdProyecto.Value, ct);
            if (p is null) return NotFound();

            p.Nombre = vm.Nombre.Trim();
            await _db.SaveChangesAsync(ct);

            if (isAjax) return Ok(new { ok = true, id = p.IdProyecto });
            return RedirectToAction(nameof(Index), new { proyectoId = p.IdProyecto });
        }

        // POST: eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProyecto(int id, CancellationToken ct)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var p = await _db.Proyectos.FirstOrDefaultAsync(x => x.IdProyecto == id, ct);
            if (p is null) return NotFound();

            // opcional: impedir eliminar si tiene tareas
            // if (await _db.Tareas.AnyAsync(t => t.ProyectoId == id, ct))
            //     return BadRequest("No se puede eliminar: el proyecto tiene tareas asignadas.");

            _db.Proyectos.Remove(p);
            await _db.SaveChangesAsync(ct);

            if (isAjax) return Ok(new { ok = true });
            return RedirectToAction(nameof(Index));
        }

    }
}
