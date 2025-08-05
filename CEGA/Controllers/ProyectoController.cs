using CEGA.Data;
using Microsoft.AspNetCore.Mvc;
using CEGA.Models;

namespace CEGA.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProyectoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Proyecto proyecto)
        {
            if (!ModelState.IsValid)
                return View(proyecto);

            if (proyecto.FechaInicio < DateTime.Today || proyecto.FechaFin < DateTime.Today)
            {
                if (proyecto.FechaInicio < DateTime.Today)
                    ModelState.AddModelError("FechaInicio", "No se pueden seleccionar fechas pasadas");

                if (proyecto.FechaFin < DateTime.Today)
                    ModelState.AddModelError("FechaFin", "No se pueden seleccionar fechas pasadas");

                return View(proyecto);
            }

            _context.Proyectos.Add(proyecto);
            _context.SaveChanges();

            TempData["mensaje"] = "Proyecto creado exitosamente";
            return RedirectToAction("Index", "Home");
        }
        // GET: Proyecto/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var proyecto = _context.Proyectos.Find(id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return View(proyecto);
        }

        // POST: Proyecto/Editar/5
        [HttpPost]
        public IActionResult Editar(Proyecto proyecto)
        {
            if (!ModelState.IsValid)
                return View(proyecto);

            if (proyecto.FechaInicio < DateTime.Today || proyecto.FechaFin < DateTime.Today)
            {
                if (proyecto.FechaInicio < DateTime.Today)
                    ModelState.AddModelError("FechaInicio", "No se pueden seleccionar fechas pasadas");

                if (proyecto.FechaFin < DateTime.Today)
                    ModelState.AddModelError("FechaFin", "No se pueden seleccionar fechas pasadas");

                return View(proyecto);
            }

            var proyectoExistente = _context.Proyectos.Find(proyecto.Id);
            if (proyectoExistente == null)
            {
                return NotFound();
            }

            proyectoExistente.Nombre = proyecto.Nombre;
            proyectoExistente.Detalles = proyecto.Detalles;
            proyectoExistente.FechaInicio = proyecto.FechaInicio;
            proyectoExistente.FechaFin = proyecto.FechaFin;

            _context.SaveChanges();

            TempData["mensaje"] = "Proyecto editado exitosamente";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Buscar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Buscar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                ViewBag.Error = "Se necesita llenar datos";
                return View();
            }

            var proyecto = _context.Proyectos.FirstOrDefault(p => p.Nombre.ToLower() == nombre.ToLower());

            if (proyecto == null)
            {
                ViewBag.Error = "No hay proyectos con ese nombre";
                return View();
            }

            return View("ResultadoBusqueda", proyecto);
        }
        [HttpGet]
        public IActionResult Eliminar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Eliminar(string nombre, bool confirmar = false)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                ViewBag.Mensaje = "Se necesita llenar el campo nombre del proyecto";
                return View();
            }

            var proyecto = _context.Proyectos.FirstOrDefault(p => p.Nombre.ToLower() == nombre.ToLower());

            if (proyecto == null)
            {
                ViewBag.Mensaje = "No hay proyectos con ese nombre";
                return View();
            }

            if (!confirmar)
            {
                ViewBag.ProyectoEncontrado = proyecto.Nombre;
                return View();
            }

            _context.Proyectos.Remove(proyecto);
            _context.SaveChanges();

            TempData["mensaje"] = "Proyecto eliminado exitosamente";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Index()
        {
            var proyectos = _context.Proyectos.ToList();
            return View(proyectos);
        }

    }
}
