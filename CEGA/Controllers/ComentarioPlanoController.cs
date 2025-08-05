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
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] ComentarioPlano dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos o incompletos.");

            var comentario = new ComentarioPlano
            {
                PlanoId = dto.PlanoId,
                Texto = dto.Texto,
                CoordenadaX = dto.CoordenadaX,
                CoordenadaY = dto.CoordenadaY,
                FechaCreacion = DateTime.Now
            };

            _context.ComentariosPlano.Add(comentario);
            await _context.SaveChangesAsync();

            // Retornar todos los comentarios actualizados para ese plano
            var comentarios = _context.ComentariosPlano
                .Where(c => c.PlanoId == dto.PlanoId)
                .OrderByDescending(c => c.FechaCreacion)
                .Select(c => new
                {
                    c.Id,
                    c.Texto,
                    c.CoordenadaX,
                    c.CoordenadaY,
                    Fecha = c.FechaCreacion.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToList();

            return Ok(comentarios);
        }

    }
}