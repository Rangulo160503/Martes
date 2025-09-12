using CEGA.Data;
using CEGA.Models.ViewModels;
using CEGA.Models.ViewModels.Pdf;
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

            TempData["Mensaje"] = "Asociaciones actualizadas.";
            return RedirectToAction(nameof(AsociarPartial), new { idPdf = vm.IdPdf });
        }

        [HttpGet]
        public async Task<IActionResult> ResumenAsociaciones(int id, CancellationToken ct)
        {
            var vm = await GetResumenAsync(id, ct);
            if (vm is null) return NotFound($"No existe el PDF {id}");
            return PartialView("~/Views/Pdf/Partials/_ResumenAsociaciones.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Archivo(int id, CancellationToken ct)
        {
            var pdf = await _db.Pdfs.AsNoTracking().FirstOrDefaultAsync(p => p.IdPdf == id, ct);
            if (pdf is null) return NotFound();

            var bytes = pdf.PdfArchivo ?? Array.Empty<byte>();
            return File(bytes, "application/pdf");
        }

        // Descarga con Content-Disposition: attachment
        [HttpGet]
        public async Task<IActionResult> Descargar(int id, CancellationToken ct)
        {
            var pdf = await _db.Pdfs.AsNoTracking().FirstOrDefaultAsync(p => p.IdPdf == id, ct);
            if (pdf is null) return NotFound();

            var bytes = pdf.PdfArchivo ?? Array.Empty<byte>();
            var fileName = $"PDF_{id}.pdf";
            return File(bytes, "application/pdf", fileName);
        }

        // Vista del visor (usa PdfVisorVM)
        [HttpGet]
        public async Task<IActionResult> VerPlano(int id, CancellationToken ct)
        {
            var existe = await _db.Pdfs.AsNoTracking().AnyAsync(p => p.IdPdf == id, ct);
            if (!existe) return NotFound();

            var vm = new CEGA.Models.ViewModels.Pdf.PdfVisorVM
            {
                IdPdf = id,
                Titulo = $"PDF #{id}",
                VerUrl = Url.Action(nameof(Archivo), "Pdf", new { id })!,
                DescargarUrl = Url.Action(nameof(Descargar), "Pdf", new { id })!
            };
            return View("~/Views/Pdf/VerPlano.cshtml", vm);
        }
        // Abre el Index y precarga el PDF indicado en el combo
        [HttpGet]
        public IActionResult Gestionar(int id)
        {
            // Simplemente manda al Index con querystring ?id=...
            return RedirectToAction(nameof(Index), new { id });
        }
        [HttpGet]
        public async Task<IActionResult> Comparar(int idA, int idB, CancellationToken ct)
        {
            try
            {
                if (idA == idB) return BadRequest("Parámetros inválidos: deben ser distintos.");

                var izq = await GetResumenAsync(idA, ct);
                var der = await GetResumenAsync(idB, ct);

                if (izq is null || der is null) return NotFound("No se encontraron los PDFs.");

                var vm = new ComparacionResumenAsociacionesVM { Izquierdo = izq, Derecho = der };
                return PartialView("~/Views/Pdf/Partials/_ResumenAsociacionesCompare.cshtml", vm);
            }
            catch (Exception ex)
            {
                // Log opcional: _logger.LogError(ex, "Error en Comparar");
                return StatusCode(500, $"Error en Comparar: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> AnotacionesPartial(int? idPdf, int top = 1000, CancellationToken ct = default)
        {
            var q = _db.Anotaciones.AsNoTracking().AsQueryable();

            if (idPdf.HasValue)
                q = q.Where(a => a.IdPdf == idPdf.Value);

            var filas = await q
                .OrderByDescending(a => a.IdAnotacion)
                .Take(top)
                .Select(a => new CEGA.Models.ViewModels.Pdf.AnotacionRowVM
                {
                    IdAnotacion = a.IdAnotacion,
                    IdPdf = a.IdPdf,
                    Cedula = a.Cedula,
                    Texto = a.Texto
                })
                .ToListAsync(ct);

            return PartialView("~/Views/Pdf/Partials/_AnotacionesTabla.cshtml", filas);
        }
        private async Task<ResumenAsociacionesVM?> GetResumenAsync(int idPdf, CancellationToken ct = default)
        {
            var existePdf = await _db.Pdfs.AsNoTracking().AnyAsync(p => p.IdPdf == idPdf, ct);
            if (!existePdf) return null;

            var pdfNombre = $"PDF #{idPdf}";

            var totalProyectos = await _db.PdfProyectos
                .Where(x => x.IdPdf == idPdf).Select(x => x.IdProyecto).Distinct().CountAsync(ct);

            var totalTareas = await _db.PdfTareas
                .Where(x => x.IdPdf == idPdf).Select(x => x.IdTarea).Distinct().CountAsync(ct);

            var totalAnotaciones = await _db.Anotaciones
                .Where(a => a.IdPdf == idPdf).CountAsync(ct);

            var proyectosTop5 = await _db.PdfProyectos
                .Where(pp => pp.IdPdf == idPdf)
                .Join(_db.Proyectos, pp => pp.IdProyecto, pr => pr.IdProyecto, (pp, pr) => pr.Nombre)
                .OrderBy(n => n).Take(5).ToListAsync(ct);

            var tareasTop5 = await _db.PdfTareas
                .Where(pt => pt.IdPdf == idPdf)
                .Join(_db.Tareas, pt => pt.IdTarea, t => t.Id, (pt, t) => t.Titulo)
                .OrderBy(t => t).Take(5).ToListAsync(ct);

            var anotacionesTop5 = await _db.Anotaciones
                .Where(a => a.IdPdf == idPdf)
                .OrderByDescending(a => a.IdAnotacion)
                .Select(a => a.Texto).Take(5).ToListAsync(ct);

            return new ResumenAsociacionesVM
            {
                PdfId = idPdf,
                PdfNombre = pdfNombre,
                TotalProyectos = totalProyectos,
                TotalTareas = totalTareas,
                TotalAnotaciones = totalAnotaciones,
                Proyectos = proyectosTop5,
                Tareas = tareasTop5,
                Anotaciones = anotacionesTop5
            };
        }
        // LISTA (ya la tienes): AnotacionesPartial(int? idPdf, int top = 1000, ...)

        // GET: Crear
        [HttpGet]
        public async Task<IActionResult> AnotacionesCrearPartial(int idPdf, CancellationToken ct)
        {
            var empleados = await _db.Empleados.AsNoTracking()
                .OrderBy(e => e.Nombre) // ajusta al nombre real del campo
                .Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = e.Cedula.ToString(),
                    Text = $"{e.Nombre} ({e.Cedula})" // adapta si no tienes Nombre
                })
                .ToListAsync(ct);

            var vm = new CEGA.Models.ViewModels.Pdf.AnotacionFormVM
            {
                IdPdf = idPdf,
                FormAction = Url.Action(nameof(CrearAnotacion), "Pdf")!,
                SubmitText = "Crear",
                Empleados = empleados
            };
            return PartialView("~/Views/Pdf/Partials/_AnotacionesForm.cshtml", vm);
        }

        // POST: Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearAnotacion(CEGA.Models.ViewModels.Pdf.AnotacionFormVM vm, CancellationToken ct)
        {
            if (vm.IdPdf <= 0 || string.IsNullOrWhiteSpace(vm.Texto)) return BadRequest("Datos inválidos.");
            var existePdf = await _db.Pdfs.AsNoTracking().AnyAsync(p => p.IdPdf == vm.IdPdf, ct);
            if (!existePdf) return NotFound($"No existe el PDF {vm.IdPdf}");

            _db.Anotaciones.Add(new CEGA.Models.Anotacion { IdPdf = vm.IdPdf, Cedula = vm.Cedula, Texto = vm.Texto });
            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true });
        }

        // GET: Editar
        [HttpGet]
        public async Task<IActionResult> AnotacionesEditarPartial(int id, CancellationToken ct)
        {
            var a = await _db.Anotaciones.AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdAnotacion == id, ct);
            if (a is null) return NotFound();

            var empleados = await _db.Empleados.AsNoTracking()
                .OrderBy(e => e.Nombre)
                .Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = e.Cedula.ToString(),
                    Text = $"{e.Nombre} ({e.Cedula})"
                })
                .ToListAsync(ct);

            var vm = new CEGA.Models.ViewModels.Pdf.AnotacionFormVM
            {
                IdAnotacion = a.IdAnotacion,
                IdPdf = a.IdPdf,
                Cedula = a.Cedula,         // esto marca seleccionado en el dropdown
                Texto = a.Texto,
                FormAction = Url.Action(nameof(EditarAnotacion), "Pdf")!,
                SubmitText = "Actualizar",
                Empleados = empleados
            };
            return PartialView("~/Views/Pdf/Partials/_AnotacionesForm.cshtml", vm);
        }
        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAnotacion(CEGA.Models.ViewModels.Pdf.AnotacionFormVM vm, CancellationToken ct)
        {
            if (vm.IdAnotacion is null || vm.IdPdf <= 0 || string.IsNullOrWhiteSpace(vm.Texto))
                return BadRequest("Datos inválidos.");

            var a = await _db.Anotaciones.FirstOrDefaultAsync(x => x.IdAnotacion == vm.IdAnotacion.Value, ct);
            if (a is null) return NotFound();

            a.Cedula = vm.Cedula;
            a.Texto = vm.Texto;
            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true });
        }

        // POST: Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarAnotacion(int id, CancellationToken ct)
        {
            var a = await _db.Anotaciones.FirstOrDefaultAsync(x => x.IdAnotacion == id, ct);
            if (a is null) return NotFound();

            _db.Anotaciones.Remove(a);
            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true });
        }
    }
}
