using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CEGA.Controllers
{
    public class PuestoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PuestoController(ApplicationDbContext context) => _context = context;

        [HttpPost]
        [AllowAnonymous] // se usa en Register sin autenticación
        [ValidateAntiForgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> CrearPuestoRapido(
            // Bind explícito para evitar overposting del Id
            [Bind("Codigo,Nombre,Departamento,Nivel,Descripcion,Requisitos,SalarioBase,Moneda,Jornada,Activo")]
            [FromForm] Puesto f)
        {
            // Helpers de normalización
            static string? T(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

            // Normaliza/limpia entradas
            var codigo = T(f.Codigo);
            var nombre = T(f.Nombre);
            var departamento = T(f.Departamento);
            var nivel = T(f.Nivel);
            var descripcion = T(f.Descripcion);
            var requisitos = T(f.Requisitos);
            var moneda = string.IsNullOrWhiteSpace(f.Moneda) ? "CRC" : f.Moneda!.Trim().ToUpperInvariant();
            var jornada = T(f.Jornada);
            var activo = f.Activo;

            // Validación mínima (puedes endurecer si quieres que Descripción sea obligatoria)
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest(new { error = "El nombre es obligatorio." });

            // Unicidad opcional por Código
            if (!string.IsNullOrEmpty(codigo))
            {
                var existe = await _context.Puestos.AnyAsync(p => p.Codigo == codigo);
                if (existe) return BadRequest(new { error = "El código de puesto ya existe." });
            }

            // Crea una NUEVA entidad (evita adjuntar directamente el objeto bindeado)
            var p = new Puesto
            {
                Codigo = codigo,
                Nombre = nombre!,
                Departamento = departamento,
                Nivel = nivel,
                Descripcion = descripcion,
                Requisitos = requisitos,
                SalarioBase = f.SalarioBase, // el binder ya lo parsea (decimal?)
                Moneda = moneda,
                Jornada = jornada,
                Activo = activo
            };

            try
            {
                _context.Puestos.Add(p);
                await _context.SaveChangesAsync();
                return Json(new { id = p.Id, nombre = p.Nombre });
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { error = "No se pudo crear el puesto.", detalle = msg });
            }
        }
    }
}
