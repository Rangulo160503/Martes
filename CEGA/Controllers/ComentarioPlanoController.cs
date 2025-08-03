using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CEGA.Controllers
{
    public class ComentarioPlanoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComentarioPlanoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult GuardarComentario(ComentarioPlano comentario)
        {
            if (comentario == null || string.IsNullOrWhiteSpace(comentario.Texto))
            {
                TempData["Error"] = "El comentario no puede estar vacío.";
                return RedirectToAction("VerPlano", "Pdf", new { id = comentario.PlanoId });
            }

            comentario.FechaCreacion = DateTime.UtcNow;
            _context.ComentariosPlano.Add(comentario);
            _context.SaveChanges();

            TempData["Mensaje"] = "Comentario guardado correctamente.";
            return RedirectToAction("VerPlano", "Pdf", new { id = comentario.PlanoId });
        }
    }
}