using CEGA.Data;
using CEGA.Models;
using CEGA.Models.Seguimiento;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CEGA.Controllers
{
    [Authorize]
    public class SeguimientoController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SeguimientoController(ApplicationDbContext db) => _db = db;

        /* ============================= INDEX ============================= */
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
                .Select(e => new SelectListItem
                {
                    Value = e.Cedula.ToString(),
                    Text = $"{e.Nombre} {e.SegundoNombre} {e.Apellido1} {e.Apellido2}".Replace("  ", " ").Trim()
                })
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

            // Tabla de proyectos (para el partial inicial)
            var proyectosTabla = await _db.Set<Proyecto>()
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new ProyectoFilaVM
                {
                    IdProyecto = p.IdProyecto,
                    Nombre = p.Nombre,
                    CantidadTareas = _db.Tareas.Count(t => t.ProyectoId == p.IdProyecto)
                })
                .ToListAsync();
            ViewBag.Proyectos = proyectosTabla;
            ViewBag.EmpleadosSelect = empleados;
            ViewBag.ProyectosSelect = proyectosSelect;

            return View(vm);
        }

        /* ======================== TAREAS (existente) ===================== */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTarea(TareaCrearVM vm)
        {
            if (!ModelState.IsValid)
            {
                // recargar dropdowns y listas
                vm.Empleados = await _db.Set<Empleado>().AsNoTracking()
                    .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                    .Select(e => new SelectListItem
                    {
                        Value = e.Cedula.ToString(),
                        Text = $"{e.Nombre} {e.SegundoNombre} {e.Apellido1} {e.Apellido2}".Replace("  ", " ").Trim()
                    })
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

                return View("Index", vm);
            }

            _db.Tareas.Add(new Tarea
            {
                Titulo = vm.Titulo,
                CedulaEmpleado = vm.CedulaEmpleado,
                ComentarioInicial = vm.Comentario,
                ProyectoId = vm.ProyectoId // puede ser null
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { proyectoId = vm.ProyectoId });
        }

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

        /* ====================== PROYECTOS: PARTIALS ====================== */

        // Tabla de proyectos (partial para AJAX)
        [HttpGet]
        public async Task<IActionResult> ProyectosTablaPartial(CancellationToken ct)
        {
            var filas = await _db.Proyectos
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new ProyectoFilaVM
                {
                    IdProyecto = p.IdProyecto,
                    Nombre = p.Nombre,
                    CantidadTareas = _db.Tareas.Count(t => t.ProyectoId == p.IdProyecto)
                })
                .ToListAsync(ct);

            return PartialView("~/Views/Seguimiento/Partials/_ProyectosTablaPartial.cshtml", filas);
        }

        // GET: Form crear (partial)
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

        // GET: Form editar (partial)
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

        /* ==================== PROYECTOS: ACCIONES POST ==================== */

        // POST: Crear (soporta AJAX y no-AJAX) — usa ProyectoFormVM para consistencia
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

        // POST: Editar (AJAX o no-AJAX)
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

        // POST: Eliminar (AJAX o no-AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProyecto(int id, CancellationToken ct)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var strategy = _db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var trx = await _db.Database.BeginTransactionAsync(ct);

                // 1) Borra dependencias (si no tienes ON DELETE CASCADE)
                var tareas = await _db.Tareas.Where(t => t.ProyectoId == id).ToListAsync(ct);
                _db.Tareas.RemoveRange(tareas);

                var pdfLinks = await _db.PdfProyectos.Where(p => p.IdProyecto == id).ToListAsync(ct);
                _db.PdfProyectos.RemoveRange(pdfLinks);

                // 2) Borra el proyecto
                var p = await _db.Proyectos.FirstOrDefaultAsync(x => x.IdProyecto == id, ct);
                if (p == null)
                {
                    await trx.RollbackAsync(ct);
                    throw new InvalidOperationException("Proyecto no encontrado.");
                }

                _db.Proyectos.Remove(p);
                await _db.SaveChangesAsync(ct);

                await trx.CommitAsync(ct);
            });

            if (isAjax) return Ok(new { ok = true });
            return RedirectToAction(nameof(Index));
        }
    }
}
