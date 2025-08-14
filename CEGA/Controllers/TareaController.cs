using CEGA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CEGA.Models;
using CEGA.Models.DTOs;

namespace CEGA.Controllers
{
    public class TareaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TareaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tareas = _context.TareasProyecto.Include(t => t.Proyecto).ToList();
            return View(tareas);
        }

        [HttpGet]
        public IActionResult Crear(int? proyectoId)
        {
            if (proyectoId == null || !_context.Proyectos.Any(p => p.Id == proyectoId))
            {
                TempData["error"] = "Proyecto no válido.";
                return RedirectToAction("Index", "Proyecto");
            }

            var tarea = new TareaProyecto
            {
                ProyectoId = proyectoId.Value
            };

            ViewBag.NombreProyecto = _context.Proyectos.FirstOrDefault(p => p.Id == proyectoId)?.Nombre;
            return View(tarea);
        }


        [HttpPost]
        public IActionResult Crear(TareaProyecto tarea)
        {
            var proyecto = _context.Proyectos.FirstOrDefault(p => p.Id == tarea.ProyectoId);
            if (proyecto == null)
            {
                TempData["error"] = "Proyecto inválido.";
                return RedirectToAction("Index", "Proyecto");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProyectoNombre = proyecto.Nombre;
                return View(tarea);
            }

            _context.TareasProyecto.Add(tarea);
            _context.SaveChanges();

            TempData["mensaje"] = "Tarea creada correctamente.";
            return RedirectToAction("Index", "Proyecto");
        }





        [HttpGet]
        public IActionResult Editar(int id)
        {
            var tarea = _context.TareasProyecto
                                .Include(t => t.Proyecto)
                                .FirstOrDefault(t => t.Id == id);

            if (tarea == null)
                return NotFound();

            ViewBag.Proyectos = new SelectList(_context.Proyectos, "Id", "Nombre");
            return View(tarea);
        }

        [HttpPost]
        public IActionResult Editar(TareaProyecto tarea)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Proyectos = new SelectList(_context.Proyectos, "Id", "Nombre");
                return View(tarea);
            }

            var tareaExistente = _context.TareasProyecto.Find(tarea.Id);
            if (tareaExistente == null)
                return NotFound();

            tareaExistente.NombreTarea = tarea.NombreTarea;
            tareaExistente.Detalles = tarea.Detalles;
            tareaExistente.FechaInicio = tarea.FechaInicio;
            tareaExistente.FechaFin = tarea.FechaFin;
            tareaExistente.ProyectoId = tarea.ProyectoId;

            _context.SaveChanges();

            TempData["mensaje"] = "Tarea editada correctamente";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AsignarEmpleado(int? tareaId)
        {
            if (tareaId == null)
            {
                TempData["mensaje"] = "No se proporcionó la tarea a asignar.";
                return RedirectToAction("Index");
            }

            var tarea = _context.TareasProyecto
                .Include(t => t.Proyecto)
                .FirstOrDefault(t => t.Id == tareaId.Value);

            if (tarea == null)
                return NotFound();

            ViewBag.Tarea = tarea;
            ViewBag.TareaId = tarea.Id;
            ViewBag.TareaNombre = tarea.NombreTarea;
            ViewBag.ProyectoNombre = tarea.Proyecto?.Nombre ?? "Sin proyecto";
            ViewBag.Empleados = _context.Users.ToList();

            return View(new AsignacionTareaEmpleado { TareaId = tarea.Id });
        }

        [HttpPost]
        public IActionResult AsignarEmpleado(AsignacionTareaEmpleado asignacion)
        {
            if (string.IsNullOrWhiteSpace(asignacion.UsuarioId))
                ModelState.AddModelError("UsuarioId", "Se necesita seleccionar un empleado");

            var tarea = _context.TareasProyecto
                .Include(t => t.Proyecto)
                .FirstOrDefault(t => t.Id == asignacion.TareaId);

            if (!ModelState.IsValid || tarea == null)
            {
                ViewBag.Tarea = tarea;
                ViewBag.TareaId = asignacion.TareaId;
                ViewBag.TareaNombre = tarea?.NombreTarea ?? "Tarea no encontrada";
                ViewBag.ProyectoNombre = tarea?.Proyecto?.Nombre ?? "Sin proyecto";
                ViewBag.Empleados = _context.Users.ToList();
                return View(asignacion);
            }

            _context.AsignacionesTareaEmpleado.Add(asignacion);
            _context.SaveChanges();

            TempData["mensaje"] = "Empleado asignado correctamente a la tarea";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var tarea = _context.TareasProyecto
                .Include(t => t.Proyecto)
                .FirstOrDefault(t => t.Id == id);

            if (tarea == null)
                return NotFound();

            return View(tarea);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminacion(int id, bool confirmar)
        {
            var tarea = _context.TareasProyecto.Find(id);
            if (tarea == null)
                return NotFound();

            if (!confirmar)
            {
                TempData["mensaje"] = "Tarea no eliminada";
                return RedirectToAction("Index");
            }

            _context.TareasProyecto.Remove(tarea);
            _context.SaveChanges();

            TempData["mensaje"] = "Tarea eliminada exitosamente";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult VerAsignaciones(int proyectoId)
        {
            var proyecto = _context.Proyectos.FirstOrDefault(p => p.Id == proyectoId);
            if (proyecto == null)
                return NotFound();

            var asignaciones = _context.AsignacionesTareaEmpleado
                .Include(a => a.Tarea)
                .Include(a => a.Usuario)
                .Where(a => a.Tarea.ProyectoId == proyectoId)
                .ToList();

            ViewBag.ProyectoNombre = proyecto.Nombre;
            ViewBag.ProyectoId = proyecto.Id;

            return View(asignaciones);
        }

        [HttpGet]
        public IActionResult TareasSinAsignar(int proyectoId)
        {
            var proyecto = _context.Proyectos.FirstOrDefault(p => p.Id == proyectoId);
            if (proyecto == null)
                return NotFound();

            var tareasAsignadasIds = _context.AsignacionesTareaEmpleado
                .Where(a => a.Tarea.ProyectoId == proyectoId)
                .Select(a => a.TareaId)
                .Distinct()
                .ToList();

            var tareasSinAsignar = _context.TareasProyecto
                .Where(t => t.ProyectoId == proyectoId && !tareasAsignadasIds.Contains(t.Id))
                .ToList();

            ViewBag.ProyectoNombre = proyecto.Nombre;
            ViewBag.ProyectoId = proyecto.Id;

            return View(tareasSinAsignar);
        }

        [HttpGet]
        public IActionResult TareasPorEmpleado(string usuarioId)
        {
            var usuario = _context.Users.FirstOrDefault(u => u.Id == usuarioId);
            if (usuario == null)
                return NotFound();

            var asignaciones = _context.AsignacionesTareaEmpleado
                .Include(a => a.Tarea)
                .ThenInclude(t => t.Proyecto)
                .Where(a => a.UsuarioId == usuarioId)
                .ToList();

            ViewBag.Usuario = usuario;
            return View(asignaciones);
        }

        [HttpGet]
        public IActionResult ReporteConsolidado()
        {
            var reporte = _context.Proyectos
                .GroupJoin(
                    _context.TareasProyecto,
                    p => p.Id,
                    t => t.ProyectoId,
                    (p, tareas) => new { p, tareas }
                )
                .Select(x => new ReporteProyectoDTO
                {
                    ProyectoId = x.p.Id,
                    NombreProyecto = x.p.Nombre,
                    TotalTareas = x.tareas.Count(),
                    TareasAsignadas = x.tareas
                        .GroupJoin(
                            _context.AsignacionesTareaEmpleado,
                            t => t.Id,
                            a => a.TareaId,
                            (t, asigns) => asigns.Select(a => a.TareaId)
                        )
                        .SelectMany(ids => ids)
                        .Distinct()
                        .Count()
                })
                .OrderBy(r => r.NombreProyecto)
                .ToList();

            return View(reporte);
        }


    }
}
