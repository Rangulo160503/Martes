using CEGA.Data;
using CEGA.Models;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CEGA.Controllers
{
    public class PdfController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PdfController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Pdf/SubirPlano
        public IActionResult SubirPlano()
        {
            return View();
        }

        // POST: Pdf/SubirPlano
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirPlano(Plano plano, IFormFile archivoPlano)
        {
            if (archivoPlano == null || Path.GetExtension(archivoPlano.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError("archivoPlano", "Solo se permiten archivos PDF");
                return View(plano);
            }

            if (string.IsNullOrWhiteSpace(plano.Disciplina))
            {
                ModelState.AddModelError("Disciplina", "Debe seleccionar una disciplina");
                return View(plano);
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await archivoPlano.CopyToAsync(memoryStream);
                    plano.Archivo = memoryStream.ToArray();
                }

                plano.NombreArchivo = archivoPlano.FileName;

                plano.FechaSubida = DateTime.Now;
                plano.SubidoPor = User.Identity?.Name ?? "Anónimo";

                

                _context.Planos.Add(plano);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Plano subido correctamente";
                return RedirectToAction("ListaPlanos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al subir el archivo: {ex.Message}";
                return View(plano);
            }
        }
        [HttpGet]
        public async Task<IActionResult> VerArchivoPlano(int id)
        {
            var plano = await _context.Planos.FindAsync(id);
            if (plano == null || plano.Archivo == null || plano.Archivo.Length == 0)
                return NotFound();

            return File(plano.Archivo, "application/pdf");
        }




        // GET: Pdf/ListaPlanos
        public async Task<IActionResult> ListaPlanos()
        {
            var planos = await _context.Planos.ToListAsync();
            return View(planos);
        }

        // GET: Pdf/VerPlano/5
        public IActionResult VerPlano(int id)
        {
            var plano = _context.Planos.FirstOrDefault(p => p.Id == id);
            if (plano == null)
                return NotFound();


            var comentarios = _context.ComentariosPlano
                .Where(c => c.PlanoId == id)
                .ToList();

            var tareas = _context.TareasPlano
                .Where(t => t.PlanoId == id)
                .ToList();

            var vm = new VerPlanoViewModel
            {
                Plano = plano,
                Comentarios = comentarios,
                Tareas = tareas
            };

            return View(vm);
        }


        // GET: Pdf/CompararPlanos
        public async Task<IActionResult> CompararPlanos()
        {
            var planos = await _context.Planos.ToListAsync();
            return View(planos);
        }

        [HttpPost]
        public async Task<IActionResult> VerComparacion(int plano1, int plano2)
        {
            if (plano1 == plano2)
            {
                TempData["Error"] = "Debe seleccionar dos planos diferentes.";
                return RedirectToAction("CompararPlanos");
            }

            var p1 = await _context.Planos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == plano1);
            var p2 = await _context.Planos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == plano2);

            if (p1 == null || p2 == null)
            {
                TempData["Error"] = "Uno de los planos no existe.";
                return RedirectToAction("CompararPlanos");
            }

            // endpoint que sirve el PDF inline
            var src1 = Url.Action(nameof(VerArchivoPlano), new { id = plano1 })!;
            var src2 = Url.Action(nameof(VerArchivoPlano), new { id = plano2 })!;

            var vm = new ResultadoComparacionVM
            {
                Titulo1 = p1.NombreArchivo,
                Titulo2 = p2.NombreArchivo,
                Src1 = src1,
                Src2 = src2
            };

            return View("ResultadoComparacion", vm);
        }


        // GET: Pdf/Comentarios/5
        public async Task<IActionResult> Comentarios(int id)
        {
            var plano = await _context.Planos.FindAsync(id);
            if (plano == null)
                return NotFound();

            ViewBag.Plano = plano;

            var comentarios = await _context.ComentariosPlano
                .Where(c => c.PlanoId == id)
                .ToListAsync();

            return View(comentarios);
        }

        // POST: Pdf/AgregarComentario
        [HttpPost]
        public async Task<IActionResult> AgregarComentario(ComentarioPlano comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario.Texto))
            {
                TempData["mensaje"] = "El comentario no puede estar vacío";
                return RedirectToAction("Comentarios", new { id = comentario.PlanoId });
            }

            comentario.FechaCreacion = DateTime.Now;

            _context.ComentariosPlano.Add(comentario);
            await _context.SaveChangesAsync();

            TempData["mensaje"] = "Comentario agregado correctamente";
            return RedirectToAction("Comentarios", new { id = comentario.PlanoId });
        }

        // POST: Pdf/EliminarComentario/5
        [HttpPost]
        public async Task<IActionResult> EliminarComentario(int id)
        {
            var comentario = await _context.ComentariosPlano.FindAsync(id);
            if (comentario != null)
            {
                _context.ComentariosPlano.Remove(comentario);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminacionComentario(int id)
        {
            var comentario = await _context.ComentariosPlano.FindAsync(id);
            if (comentario == null) return NotFound();

            _context.ComentariosPlano.Remove(comentario);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Comentario eliminado exitosamente";
            return RedirectToAction("Comentarios", new { id = comentario.PlanoId });
        }

        // GET: Pdf/PanelLateral/5
        [HttpGet]
        public async Task<IActionResult> PanelLateral(int id)
        {
            var plano = await _context.Planos.FindAsync(id);
            if (plano == null)
                return NotFound();

            var comentarios = await _context.ComentariosPlano
                .Where(c => c.PlanoId == id)
                .ToListAsync();

            var tareas = await _context.TareasPlano
                .Where(t => t.PlanoId == id)
                .ToListAsync();

            ViewBag.Plano = plano;
            ViewBag.Comentarios = comentarios;
            ViewBag.Tareas = tareas;

            return PartialView("_PanelLateral");
        }

        // POST: Pdf/AgregarTarea
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarTarea(TareaPlano tarea)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "La tarea no se pudo agregar. Verifica los datos.";
                return RedirectToAction("Comentarios", new { id = tarea.PlanoId });
            }

            _context.TareasPlano.Add(tarea);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Tarea agregada correctamente.";
            return RedirectToAction("Comentarios", new { id = tarea.PlanoId });
        }
        [HttpPost]
        public IActionResult GuardarComentario([FromBody] ComentarioPlano comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario.Texto))
            {
                return BadRequest("El comentario no puede estar vacío.");
            }

            // Verifica si ya existe uno con misma posición exacta y texto
            bool duplicado = _context.ComentariosPlano.Any(c =>
                c.PlanoId == comentario.PlanoId &&
                Math.Abs(c.CoordenadaX - comentario.CoordenadaX) < 0.01 &&
                Math.Abs(c.CoordenadaY - comentario.CoordenadaY) < 0.01 &&
                c.Texto == comentario.Texto);

            if (duplicado)
            {
                return BadRequest("Ya existe una anotación en esa posición con el mismo texto.");
            }

            _context.ComentariosPlano.Add(comentario);
            _context.SaveChanges();

            return Ok();
        }
        public async Task<IActionResult> PanelLateralPartial(int planoId)
        {
            return ViewComponent("AnotacionesTareas", new { planoId });
        }
        [HttpPost]
        public IActionResult GuardarTarea([FromBody] TareaPlano tarea)
        {
            if (string.IsNullOrWhiteSpace(tarea.Descripcion) || string.IsNullOrWhiteSpace(tarea.Responsable))
                return BadRequest("Descripción y responsable son obligatorios.");

            tarea.Fecha = DateTime.Now;

            _context.TareasPlano.Add(tarea);
            _context.SaveChanges();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            var tarea = await _context.TareasPlano.FindAsync(id);
            if (tarea != null)
            {
                _context.TareasPlano.Remove(tarea);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }
        
        [HttpGet]
        public IActionResult BuscarComentario(int id)
        {
            ViewBag.PlanoId = id;
            return View(); // Este archivo aún falta crearlo.
        }
        [HttpPost]
        public IActionResult BuscarComentario(int id, string textoComentario)
        {
            if (string.IsNullOrWhiteSpace(textoComentario))
            {
                ViewBag.Mensaje = "Ingrese un texto de comentario";
                return View();
            }

            var comentario = _context.ComentariosPlano
                .FirstOrDefault(c => c.PlanoId == id && c.Texto.ToLower() == textoComentario.ToLower());

            if (comentario == null)
            {
                ViewBag.Mensaje = "Comentario no encontrado";
                return View();
            }

            return View("ResultadoBusqueda", comentario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPlano(int id)
        {
            var plano = await _context.Planos.FindAsync(id);

            if (plano == null)
                return NotFound();

            // Elimina comentarios asociados
            var comentarios = _context.ComentariosPlano.Where(c => c.PlanoId == id);
            _context.ComentariosPlano.RemoveRange(comentarios);

            // Elimina tareas asociadas
            var tareas = _context.TareasPlano.Where(t => t.PlanoId == id);
            _context.TareasPlano.RemoveRange(tareas);

            // Elimina el plano
            _context.Planos.Remove(plano);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Plano eliminado correctamente.";
            return RedirectToAction("ListaPlanos");
        }


    }
}
