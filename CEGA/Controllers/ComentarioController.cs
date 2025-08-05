using CEGA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CEGA.Models;

namespace CEGA.Controllers
{
    public class ComentarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComentarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comentario/Crear?proyectoId=5
        [HttpGet]
        public IActionResult Crear(int proyectoId)
        {
            var proyecto = _context.Proyectos.Find(proyectoId);
            if (proyecto == null) return NotFound();

            ViewBag.ProyectoId = proyecto.Id;
            ViewBag.ProyectoNombre = proyecto.Nombre;
            return View();
        }

        // POST: Comentario/Crear
        [HttpPost]
        public IActionResult Crear(ComentarioProyecto comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario.NombreComentario) && string.IsNullOrWhiteSpace(comentario.Detalles))
            {
                ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                return View(comentario);
            }

            if (!ModelState.IsValid)
                return View(comentario);

            _context.ComentariosProyecto.Add(comentario);
            _context.SaveChanges();

            TempData["mensaje"] = "Comentario creado correctamente";
            return RedirectToAction("Index", "Proyecto");
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var comentario = _context.ComentariosProyecto
                                     .Include(c => c.Proyecto)
                                     .FirstOrDefault(c => c.Id == id);
            if (comentario == null) return NotFound();

            return View(comentario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(ComentarioProyecto comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario.NombreComentario) && string.IsNullOrWhiteSpace(comentario.Detalles))
            {
                ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                return View(comentario);
            }

            if (!ModelState.IsValid)
                return View(comentario);

            var comentarioExistente = _context.ComentariosProyecto.Find(comentario.Id);
            if (comentarioExistente == null) return NotFound();

            comentarioExistente.NombreComentario = comentario.NombreComentario;
            comentarioExistente.Detalles = comentario.Detalles;

            _context.SaveChanges();

            TempData["mensaje"] = "Comentario editado correctamente";
            return RedirectToAction("Index", "Proyecto");
        }

        [HttpGet]
        public IActionResult Buscar(int proyectoId)
        {
            ViewBag.ProyectoId = proyectoId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Buscar(int proyectoId, string nombreComentario)
        {
            ViewBag.ProyectoId = proyectoId;

            if (string.IsNullOrWhiteSpace(nombreComentario))
            {
                ViewBag.Mensaje = "Se necesita llenar datos";
                return View();
            }

            var comentario = _context.ComentariosProyecto
                .FirstOrDefault(c => c.ProyectoId == proyectoId && c.NombreComentario.ToLower() == nombreComentario.ToLower());

            if (comentario == null)
            {
                ViewBag.Mensaje = "No hay un comentario con ese nombre";
                return View();
            }

            return View("ResultadoBusqueda", comentario);
        }

        [HttpGet]
        public IActionResult Eliminar(int? id)
        {
            if (id == null) return NotFound();

            var comentario = _context.ComentariosProyecto
                .Include(c => c.Proyecto)
                .FirstOrDefault(c => c.Id == id);

            if (comentario == null) return NotFound();

            return View(comentario);
        }

        [HttpPost]
        public IActionResult ConfirmarEliminacion(int id, bool confirmar)
        {
            var comentario = _context.ComentariosProyecto.Find(id);
            if (comentario == null) return NotFound();

            if (!confirmar)
            {
                TempData["mensaje"] = "Comentario no eliminado";
                return RedirectToAction("Index", "Proyecto");
            }

            _context.ComentariosProyecto.Remove(comentario);
            _context.SaveChanges();

            TempData["mensaje"] = "Comentario eliminado exitosamente";
            return RedirectToAction("Index", "Proyecto");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var comentarios = _context.ComentariosProyecto
                                      .Include(c => c.Proyecto)
                                      .ToList();
            return View(comentarios);
        }
    }
}
