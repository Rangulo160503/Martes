using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

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
                string nombreArchivo = Guid.NewGuid() + ".pdf";
                string rutaCarpeta = Path.Combine(_env.WebRootPath, "docs", "planos");
                Directory.CreateDirectory(rutaCarpeta);

                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivoPlano.CopyToAsync(stream);
                }

                plano.NombreArchivo = archivoPlano.FileName;
                plano.RutaArchivo = "/docs/planos/" + nombreArchivo;
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

        // GET: Pdf/ListaPlanos
        public async Task<IActionResult> ListaPlanos()
        {
            var planos = await _context.Planos.ToListAsync();
            return View(planos);
        }

        // GET: Pdf/VerPlano/5
        public async Task<IActionResult> VerPlano(int id)
        {
            var plano = await _context.Planos.FirstOrDefaultAsync(p => p.Id == id);
            if (plano == null)
                return NotFound();

            return View(plano);
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

            var p1 = await _context.Planos.FindAsync(plano1);
            var p2 = await _context.Planos.FindAsync(plano2);

            if (p1 == null || p2 == null)
            {
                TempData["Error"] = "Uno de los planos no existe.";
                return RedirectToAction("CompararPlanos");
            }

            ViewData["Plano1"] = p1;
            ViewData["Plano2"] = p2;

            return View("ResultadoComparacion");
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarComentario(ComentarioPlano comentario)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "El comentario no se pudo agregar. Verifica los datos.";
                return RedirectToAction("Comentarios", new { id = comentario.PlanoId });
            }

            _context.ComentariosPlano.Add(comentario);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Comentario agregado correctamente.";
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
                return RedirectToAction("Comentarios", new { id = comentario.PlanoId });
            }

            return NotFound();
        }
    }
}
