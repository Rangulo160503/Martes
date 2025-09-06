using CEGA.Data;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CEGA.Controllers
{
    public class PdfController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _cfg;
        private readonly string _cs;

        public PdfController(ApplicationDbContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
            _cs = cfg.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult Index() => View();

        // GET: renderiza el partial de “Subir PDF” (ahora con modelo)
        [HttpGet]
        public IActionResult SubirPartial()
            => PartialView("~/Views/Pdf/Partials/_SubirPdfPartial.cshtml", new PdfUploadVM());

        [HttpGet]
        public async Task<IActionResult> AsociarPartial(int idPdf, int[]? proyectoIds, CancellationToken ct)
        {
            var existePdf = await _db.Pdfs.AsNoTracking().AnyAsync(p => p.IdPdf == idPdf, ct);
            if (!existePdf) return NotFound($"No existe el PDF {idPdf}");

            // Proyectos asociados / disponibles
            var idsProyAsociados = await _db.PdfProyectos
                .Where(pp => pp.IdPdf == idPdf)
                .Select(pp => pp.IdProyecto)
                .ToListAsync(ct);

            var proyectosAsociados = await _db.Proyectos.AsNoTracking()
                .Where(p => idsProyAsociados.Contains(p.IdProyecto))
                .OrderBy(p => p.Nombre)
                .Select(p => new PdfAsociacionesVM.Item { Id = p.IdProyecto, Titulo = p.Nombre })
                .ToListAsync(ct);

            var proyectosDisponibles = await _db.Proyectos.AsNoTracking()
                .Where(p => !idsProyAsociados.Contains(p.IdProyecto))
                .OrderBy(p => p.Nombre)
                .Select(p => new PdfAsociacionesVM.Item { Id = p.IdProyecto, Titulo = p.Nombre })
                .ToListAsync(ct);

            // Tareas asociadas
            var idsTareasAsociadas = await _db.PdfTareas
                .Where(pt => pt.IdPdf == idPdf)
                .Select(pt => pt.IdTarea)
                .ToListAsync(ct);

            // Tareas disponibles (filtra por proyectos seleccionados si vienen)
            var tareasDisponiblesQuery = _db.Tareas.AsNoTracking()
                .Where(t => !idsTareasAsociadas.Contains(t.Id));

            if (proyectoIds != null && proyectoIds.Length > 0)
            {
                tareasDisponiblesQuery = tareasDisponiblesQuery
                    .Where(t => t.ProyectoId.HasValue && proyectoIds.Contains(t.ProyectoId.Value));
            }

            var tareasAsociadas = await _db.Tareas.AsNoTracking()
                .Where(t => idsTareasAsociadas.Contains(t.Id))
                .OrderBy(t => t.Titulo)
                .Select(t => new PdfAsociacionesVM.Item { Id = t.Id, Titulo = t.Titulo })
                .ToListAsync(ct);

            var tareasDisponibles = await tareasDisponiblesQuery
                .OrderBy(t => t.Titulo)
                .Select(t => new PdfAsociacionesVM.Item { Id = t.Id, Titulo = t.Titulo })
                .ToListAsync(ct);

            var vm = new PdfAsociacionesVM
            {
                IdPdf = idPdf,
                ProyectosAsociados = proyectosAsociados,
                ProyectosDisponibles = proyectosDisponibles,
                TareasAsociadas = tareasAsociadas,
                TareasDisponibles = tareasDisponibles
            };

            return PartialView("~/Views/Pdf/Partials/_AsociarPartial.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> ListarPdfs(CancellationToken ct)
        {
            var lista = await _db.Pdfs.AsNoTracking()
                .OrderByDescending(p => p.IdPdf)
                .Select(p => new { id = p.IdPdf, text = $"PDF #{p.IdPdf}" })
                .ToListAsync(ct);

            return Json(lista);
        }

        // POST: guarda el PDF y vuelve al Index (PRG)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subir(PdfUploadVM vm, CancellationToken ct)
        {
            const long MAX_BYTES = 20 * 1024 * 1024; // 20 MB

            if (!ModelState.IsValid || vm.PdfFile is null || vm.PdfFile.Length == 0)
            {
                TempData["Error"] = "Selecciona un archivo PDF.";
                return RedirectToAction(nameof(Index));
            }

            if (!vm.PdfFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "El archivo debe ser un PDF válido.";
                return RedirectToAction(nameof(Index));
            }

            if (vm.PdfFile.Length > MAX_BYTES)
            {
                TempData["Error"] = $"El PDF supera {MAX_BYTES / (1024 * 1024)} MB.";
                return RedirectToAction(nameof(Index));
            }

            using var ms = new MemoryStream();
            await vm.PdfFile.CopyToAsync(ms, ct);

            _db.Pdfs.Add(new CEGA.Models.Pdf { PdfArchivo = ms.ToArray() });
            await _db.SaveChangesAsync(ct);

            TempData["Mensaje"] = "PDF subido correctamente.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asociar(CEGA.Models.ViewModels.PdfAsociacionesVM vm, CancellationToken ct)
        {
            if (!await _db.Pdfs.AsNoTracking().AnyAsync(p => p.IdPdf == vm.IdPdf, ct))
                return NotFound($"No existe el PDF {vm.IdPdf}");

            vm.ProyectosAgregarIds ??= Array.Empty<int>();
            vm.ProyectosQuitarIds ??= Array.Empty<int>();
            vm.TareasAgregarIds ??= Array.Empty<int>();
            vm.TareasQuitarIds ??= Array.Empty<int>();

            // --- Proyectos ---
            var proyActuales = await _db.PdfProyectos
                .Where(pp => pp.IdPdf == vm.IdPdf)
                .Select(pp => pp.IdProyecto)
                .ToListAsync(ct);

            var proyAgregar = vm.ProyectosAgregarIds.Except(proyActuales).Distinct().ToList();
            var proyQuitar = vm.ProyectosQuitarIds.Intersect(proyActuales).Distinct().ToList();

            _db.PdfProyectos.AddRange(proyAgregar.Select(id => new CEGA.Models.PdfProyecto { IdPdf = vm.IdPdf, IdProyecto = id }));
            foreach (var id in proyQuitar)
            {
                var stub = new CEGA.Models.PdfProyecto { IdPdf = vm.IdPdf, IdProyecto = id };
                _db.Attach(stub);
                _db.PdfProyectos.Remove(stub);
            }

            // --- Tareas ---
            var tareasActuales = await _db.PdfTareas
                .Where(pt => pt.IdPdf == vm.IdPdf)
                .Select(pt => pt.IdTarea)
                .ToListAsync(ct);

            var tareasAgregar = vm.TareasAgregarIds.Except(tareasActuales).Distinct().ToList();
            var tareasQuitar = vm.TareasQuitarIds.Intersect(tareasActuales).Distinct().ToList();

            _db.PdfTareas.AddRange(tareasAgregar.Select(id => new CEGA.Models.PdfTarea { IdPdf = vm.IdPdf, IdTarea = id }));
            foreach (var id in tareasQuitar)
            {
                var stub = new CEGA.Models.PdfTarea { IdPdf = vm.IdPdf, IdTarea = id };
                _db.Attach(stub);
                _db.PdfTareas.Remove(stub);
            }

            await _db.SaveChangesAsync(ct);

            // Reconstruir VM para devolver el partial actualizado
            var idsProyAsociados = await _db.PdfProyectos.Where(x => x.IdPdf == vm.IdPdf).Select(x => x.IdProyecto).ToListAsync(ct);
            var idsTareasAsociadas = await _db.PdfTareas.Where(x => x.IdPdf == vm.IdPdf).Select(x => x.IdTarea).ToListAsync(ct);

            vm.ProyectosAsociados = await _db.Proyectos.AsNoTracking()
                .Where(p => idsProyAsociados.Contains(p.IdProyecto))
                .OrderBy(p => p.Nombre)
                .Select(p => new CEGA.Models.ViewModels.PdfAsociacionesVM.Item { Id = p.IdProyecto, Titulo = p.Nombre })
                .ToListAsync(ct);

            vm.ProyectosDisponibles = await _db.Proyectos.AsNoTracking()
                .Where(p => !idsProyAsociados.Contains(p.IdProyecto))
                .OrderBy(p => p.Nombre)
                .Select(p => new CEGA.Models.ViewModels.PdfAsociacionesVM.Item { Id = p.IdProyecto, Titulo = p.Nombre })
                .ToListAsync(ct);

            vm.TareasAsociadas = await _db.Tareas.AsNoTracking()
                .Where(t => idsTareasAsociadas.Contains(t.Id))
                .OrderBy(t => t.Titulo)
                .Select(t => new CEGA.Models.ViewModels.PdfAsociacionesVM.Item { Id = t.Id, Titulo = t.Titulo })
                .ToListAsync(ct);

            vm.TareasDisponibles = await _db.Tareas.AsNoTracking()
                .Where(t => !idsTareasAsociadas.Contains(t.Id))
                .OrderBy(t => t.Titulo)
                .Select(t => new CEGA.Models.ViewModels.PdfAsociacionesVM.Item { Id = t.Id, Titulo = t.Titulo })
                .ToListAsync(ct);

            ViewBag.Success = "Asociaciones actualizadas.";
            return PartialView("~/Views/Pdf/Partials/_AsociarPartial.cshtml", vm);
        }

    }
}
