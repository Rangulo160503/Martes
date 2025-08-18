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
            // ===== TAREAS (Proyecto) =====
            var tareas = _context.TareasProyecto
                .Include(t => t.Proyecto)
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .ToList();

            // ===== ASIGNACIONES RAW (para "Por empleado" y "Sin asignar") =====
            // Proyectamos SOLO columnas reales (Id, TareaId, UsuarioId) para evitar columnas inexistentes
            var asignacionesRaw = _context.AsignacionesTareaEmpleado
                .AsNoTracking()
                .Select(a => new { a.Id, a.TareaId, a.UsuarioId })
                .OrderByDescending(a => a.Id)
                .ToList();

            // ===== SIN ASIGNAR (respecto a TareasProyecto) =====
            var sinAsignar = tareas
                .Where(t => !asignacionesRaw.Any(a => a.TareaId == t.Id))
                .ToList();

            // ===== Usuarios (para mostrar nombre amigable) =====
            var usuariosDisplay = _context.Users.AsNoTracking()
                .Select(u => new {
                    u.Id,
                    Nombre = (((u.Nombre ?? "") + " " + (u.Apellido ?? "")).Trim() != string.Empty)
                                ? ((u.Nombre ?? "") + " " + (u.Apellido ?? "")).Trim()
                                : (u.UserName ?? u.Email ?? u.Id)
                })
                .ToList()
                .ToDictionary(x => x.Id, x => x.Nombre);

            // ===== POR EMPLEADO =====
            var porEmpleado = asignacionesRaw
                .GroupBy(a => a.UsuarioId)
                .Select(g => new CEGA.Models.ViewModels.EmpleadoTareasVM
                {
                    UsuarioId = g.Key,
                    Nombre = usuariosDisplay.ContainsKey(g.Key) ? usuariosDisplay[g.Key] : g.Key,
                    Tareas = tareas.Where(t => g.Any(ax => ax.TareaId == t.Id)).Distinct().ToList()
                })
                .OrderBy(x => x.Nombre)
                .ToList();

            // ===== COMENTARIOS DE PROYECTO =====
            var comentarios = _context.ComentariosProyecto
                .Include(c => c.Proyecto)
                .AsNoTracking()
                .OrderByDescending(c => c.FechaCreacion)
                .ToList();

            // ===== COMBOS =====
            ViewBag.Proyectos = new SelectList(
                _context.Proyectos.AsNoTracking().OrderBy(p => p.Nombre),
                "Id", "Nombre"
            );

            var empleadosCombo = _context.Users
                .AsNoTracking()
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Nombre = (((u.Nombre ?? "") + " " + (u.Apellido ?? "")).Trim() != string.Empty)
                                ? ((u.Nombre ?? "") + " " + (u.Apellido ?? "")).Trim()
                                : (u.UserName ?? u.Email ?? u.Id)
                })
                .ToList();

            ViewBag.Empleados = new SelectList(empleadosCombo, "Id", "Nombre");

            // ===== VM =====
            var vm = new CEGA.Models.ViewModels.TareasPageVM
            {
                Tareas = tareas,
                TareasSinAsignar = sinAsignar,
                TareasPorEmpleado = porEmpleado,
                Comentarios = comentarios   // <-- NUEVO
            };

            return View(vm);
        }



        [HttpGet]
        public IActionResult Crear(int? proyectoId)
        {
            // Si viene desde el menú (sin proyecto), mostrar selector
            if (!proyectoId.HasValue || !_context.Proyectos.Any(p => p.Id == proyectoId.Value))
            {
                ViewBag.Proyectos = new SelectList(_context.Proyectos.OrderBy(p => p.Nombre), "Id", "Nombre");
                return View("SeleccionarProyectoParaTarea");
            }

            var tarea = new TareasProyecto { ProyectoId = proyectoId.Value };
            ViewBag.NombreProyecto = _context.Proyectos.Where(p => p.Id == proyectoId).Select(p => p.Nombre).FirstOrDefault();
            return View(tarea); // Views/Tarea/Crear.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(TareasProyecto tarea, bool asignar = false)
        {
            var proyecto = _context.Proyectos.FirstOrDefault(p => p.Id == tarea.ProyectoId);
            if (proyecto == null)
            {
                TempData["error"] = "Proyecto inválido.";
                return RedirectToAction(nameof(Crear)); // vuelve al selector
            }

            if (!ModelState.IsValid)
            {
                ViewBag.NombreProyecto = proyecto.Nombre;
                return View(tarea);
            }

            _context.TareasProyecto.Add(tarea);
            _context.SaveChanges();

            TempData["mensaje"] = "Tarea creada correctamente.";

            // Botón "Crear y asignar empleado"
            if (asignar)
                return RedirectToAction(nameof(AsignarEmpleado), new { tareaId = tarea.Id });

            return RedirectToAction(nameof(Index));
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
        public IActionResult Editar(TareasProyecto tarea)
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

        // using CEGA.Models.ViewModels;
        // using Microsoft.AspNetCore.Mvc.Rendering;

        [HttpGet]
        public IActionResult AsignarEmpleado(int? tareaId)
        {
            if (tareaId == null)
            {
                var tareas = _context.TareasProyecto
                    .Include(t => t.Proyecto)
                    .Select(t => new
                    {
                        t.Id,
                        Nombre = t.NombreTarea + " — " + (t.Proyecto != null ? t.Proyecto.Nombre : "Sin proyecto")
                    })
                    .OrderBy(x => x.Nombre)
                    .ToList();

                ViewBag.Tareas = new SelectList(tareas, "Id", "Nombre");
                return View("SeleccionarTareaParaAsignar", new CEGA.Models.ViewModels.SeleccionarTareaVM());
            }

            var tarea = _context.TareasProyecto
                .Include(t => t.Proyecto)
                .FirstOrDefault(t => t.Id == tareaId.Value);

            if (tarea == null) return NotFound();

            ViewBag.Tarea = tarea;
            ViewBag.TareaId = tarea.Id;
            ViewBag.TareaNombre = tarea.NombreTarea;
            ViewBag.ProyectoNombre = tarea.Proyecto?.Nombre ?? "Sin proyecto";
            ViewBag.Empleados = _context.Users.ToList();

            return View(new AsignacionesTareaEmpleado { TareaId = tarea.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SeleccionarTareaParaAsignar(CEGA.Models.ViewModels.SeleccionarTareaVM vm)
        {
            if (!ModelState.IsValid || !_context.TareasProyecto.Any(t => t.Id == vm.TareaId))
            {
                var tareas = _context.TareasProyecto
                    .Include(t => t.Proyecto)
                    .Select(t => new
                    {
                        t.Id,
                        Nombre = t.NombreTarea + " — " + (t.Proyecto != null ? t.Proyecto.Nombre : "Sin proyecto")
                    })
                    .OrderBy(x => x.Nombre)
                    .ToList();

                ViewBag.Tareas = new SelectList(tareas, "Id", "Nombre");
                return View("SeleccionarTareaParaAsignar", vm);
            }

            return RedirectToAction(nameof(AsignarEmpleado), new { tareaId = vm.TareaId });
        }

        [HttpPost]
        public IActionResult AsignarEmpleado(AsignacionesTareaEmpleado asignacion)
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
        public IActionResult VerAsignaciones(int? proyectoId)
        {
            // Filtro (dropdown) de proyectos
            var proyectos = _context.Proyectos
                .OrderBy(p => p.Nombre)
                .ToList();
            ViewBag.Proyectos = new SelectList(proyectos, "Id", "Nombre", proyectoId);

            // Query base
            var query = _context.AsignacionesTareaEmpleado
                .Include(a => a.Tarea).ThenInclude(t => t.Proyecto)
                .Include(a => a.Usuario)
                .AsQueryable();

            if (proyectoId.HasValue)
            {
                query = query.Where(a => a.Tarea.ProyectoId == proyectoId.Value);
                ViewBag.ProyectoNombre = proyectos.FirstOrDefault(p => p.Id == proyectoId.Value)?.Nombre;
            }
            else
            {
                ViewBag.ProyectoNombre = "Todos los proyectos";
            }

            var asignaciones = query
                .OrderByDescending(a => a.FechaAsignacion)
                .ThenByDescending(a => a.Id)
                .ToList();

            return View(asignaciones); // usa tu misma vista VerAsignaciones.cshtml
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
            var reporte = _context.TareasProyecto
                .Include(t => t.Proyecto)
                .GroupJoin(
                    _context.AsignacionesTareaEmpleado,   // une asignaciones por tarea
                    t => t.Id,
                    a => a.TareaId,
                    (t, asigns) => new { t, asigns }
                )
                .GroupBy(x => new { x.t.ProyectoId, x.t.Proyecto.Nombre })
                .Select(g => new ReporteProyectoDTO
                {
                    ProyectoId = g.Key.ProyectoId,
                    NombreProyecto = g.Key.Nombre,
                    TotalTareas = g.Count(),
                    // tareas con al menos una asignación (distinct para evitar duplicados)
                    TareasAsignadas = g.SelectMany(x => x.asigns)
                                       .Select(a => a.TareaId)
                                       .Distinct()
                                       .Count()
                })
                .OrderBy(r => r.NombreProyecto)
                .ToList();

            return View(reporte);
        }

        // GET: Eliminar asignación (muestra la confirmación)
        [HttpGet]
        public IActionResult EliminarAsignacion(int id)
        {
            var asign = _context.AsignacionesTareaEmpleado
                                .Include(a => a.Tarea).ThenInclude(t => t.Proyecto)
                                .Include(a => a.Usuario)
                                .FirstOrDefault(a => a.Id == id);
            if (asign == null) return NotFound();

            return View(asign);
        }

        // POST: confirmar eliminación (coincide con tu asp-action="EliminarAsignacionConfirmado")
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAsignacionConfirmado(int id)
        {
            var asign = _context.AsignacionesTareaEmpleado
                                .Include(a => a.Tarea)
                                .FirstOrDefault(a => a.Id == id);
            if (asign == null) return NotFound();

            var proyectoId = asign.Tarea.ProyectoId;

            _context.AsignacionesTareaEmpleado.Remove(asign);
            _context.SaveChanges();

            TempData["mensaje"] = "Asignación eliminada.";
            return RedirectToAction(nameof(VerAsignaciones), new { proyectoId });
        }
        [HttpGet]
        public IActionResult Buscar(string q)
        {
            q = (q ?? "").Trim();

            var query = _context.TareasProyecto
                .Include(t => t.Proyecto)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                // Busca en nombre de tarea, detalles y nombre de proyecto
                query = query.Where(t =>
                    EF.Functions.Like(t.NombreTarea, $"%{q}%") ||
                    EF.Functions.Like(t.Detalles ?? "", $"%{q}%") ||
                    EF.Functions.Like(t.Proyecto.Nombre, $"%{q}%"));
            }

            var resultados = query
                .OrderBy(t => t.Proyecto.Nombre)
                .ThenBy(t => t.NombreTarea)
                .Take(200)
                .ToList();

            ViewBag.Q = q;
            // (opcional) combos si quieres reusar botones en esta pantalla
            ViewBag.Proyectos = new SelectList(_context.Proyectos.OrderBy(p => p.Nombre), "Id", "Nombre");
            ViewBag.Empleados = new SelectList(_context.Users.OrderBy(u => u.UserName), "Id", "UserName");

            return View(resultados); // -> Views/Tarea/Buscar.cshtml
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Buscar(int? proyectoId, string? nombreTarea)
        {
            // Redirige al GET para evitar reenvío de formularios y soportar tu vista antigua
            return RedirectToAction(nameof(Buscar), new { q = nombreTarea, proyectoId });
        }
    }
}
