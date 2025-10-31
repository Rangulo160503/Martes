using CEGA.Data;
using CEGA.Models;                      // Empleado, Puesto, Incapacidad
using CEGA.Models.Seguimiento;
using CEGA.Models.ViewModels;
using CEGA.Models.ViewModels.Empleados; // CambiarPuestoVM
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;                        // MemoryStream

namespace CEGA.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmpleadosController(ApplicationDbContext context) => _context = context;

        // GET: /Empleados
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var puestos = await _context.Puestos
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            var empleados = await _context.Empleados
                .AsNoTracking()
                .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                .ToListAsync();

            var vmPuestos = new CambiarPuestoVM
            {
                Empleados = empleados.Select(e => new SelectListItem
                {
                    Value = e.Cedula.ToString(),
                    Text = $"{e.Nombre} {e.Apellido1} {e.Apellido2}".Trim()
                }).ToList(),
                Puestos = puestos.Where(p => p.Activo).Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                }).ToList()
            };

            var empleadosSelect = empleados.Select(e => new SelectListItem
            {
                Value = e.Cedula.ToString(),
                Text = $"{e.Cedula} — {e.Nombre} {e.Apellido1} {e.Apellido2}".Trim()
            }).ToList();

            var incapacidades = await _context.Incapacidades
                .AsNoTracking()
                .Include(i => i.Empleado)
                .OrderBy(i => i.Cedula)
                .ToListAsync();

            var vacaciones = await _context.Vacaciones
                .AsNoTracking()
                .Include(v => v.Empleado)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();

            // ===== Resumen para la pestaña "Salarios" (renombrado a _ResumenEmpleados) =====
            var resumenEmpleados = await _context.Empleados
                .AsNoTracking()
                .Select(e => new CEGA.Models.ViewModels.Empleados.ResumenSalariosVM
                {
                    Cedula = e.Cedula,
                    NombreCompleto = (e.Nombre + " " + e.Apellido1 + " " + (e.Apellido2 ?? "")).Trim(),
                    Activo = e.Activo,
                    PuestoNombre = _context.Puestos
                                        .Where(p => p.Id == e.PuestoId)
                                        .Select(p => p.Nombre)
                                        .FirstOrDefault() ?? "(sin puesto)",
                    SalarioBase = _context.Puestos
                                        .Where(p => p.Id == e.PuestoId)
                                        .Select(p => p.SalarioBase)
                                        .FirstOrDefault(),
                    VacacionesTomadas = _context.Vacaciones.Count(v => v.Cedula == e.Cedula),
                    TieneIncapacidad = _context.Incapacidades.Any(i => i.Cedula == e.Cedula)
                })
                .OrderBy(r => r.NombreCompleto)
                .ToListAsync();

            ViewBag.PuestosVm = vmPuestos;
            ViewBag.Puestos = puestos;
            ViewBag.Empleados = empleadosSelect;
            ViewBag.ResumenVacaciones = Enumerable.Empty<VacacionesResumenVM>(); // (si luego lo llenas)
            ViewBag.Incapacidades = incapacidades;
            ViewBag.Vacaciones = vacaciones;
            ViewBag.ResumenEmpleados = resumenEmpleados; // ← NUEVO

            if (ViewBag.EmpleadosSelect == null)
            {
                ViewBag.EmpleadosSelect = await _context.Empleados
                    .AsNoTracking()
                    .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                    .Select(e => new SelectListItem
                    {
                        Value = e.Cedula.ToString(),
                        Text = ($"{e.Cedula} — {e.Nombre} {e.SegundoNombre} {e.Apellido1} {e.Apellido2}")
                                .Replace("  ", " ").Trim()
                    }).ToListAsync();
            }

            // Dropdowns para el partial
            ViewBag.ProyectosSelect = await _context.Set<Proyecto>()
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem { Value = p.IdProyecto.ToString(), Text = p.Nombre })
                .ToListAsync();

            
            // Filtros
            DateTime? accFecha = DateTime.TryParse(Request.Query["accFecha"], out var f)
                                 ? f.Date : (DateTime?)null;

            var aq = _context.Accidentes.AsNoTracking();
            if (accFecha.HasValue)
            {
                var d = accFecha.Value;
                aq = aq.Where(a => a.Fecha >= d && a.Fecha < d.AddDays(1));
            }

            ViewBag.Accidentes = await (
                from a in aq
                join p in _context.Set<Proyecto>().AsNoTracking() on a.ProyectoId equals p.IdProyecto into pj
                from p in pj.DefaultIfEmpty()
                join e in _context.Empleados.AsNoTracking() on a.CedulaEmpleado equals e.Cedula
                orderby a.Fecha descending, a.Id descending
                select new AccidenteFilaVM
                {
                    Id = a.Id,
                    Fecha = a.Fecha,
                    ProyectoId = a.ProyectoId,
                    ProyectoNombre = p != null ? p.Nombre : null,
                    CedulaEmpleado = a.CedulaEmpleado,
                    EmpleadoNombre = (e.Nombre + " " + e.SegundoNombre + " " + e.Apellido1 + " " + e.Apellido2)
                                        .Replace("  ", " ").Trim()
                }
            ).ToListAsync();

            // VM para el form de crear
            ViewBag.AccidentesVm = new AccidenteCrearVM
            {
                Empleados = (IEnumerable<SelectListItem>)ViewBag.EmpleadosSelect,
                Proyectos = (IEnumerable<SelectListItem>)ViewBag.ProyectosSelect
            };


            return View();
        }
        // POST: crear accidente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearAccidente(AccidenteCrearVM vm)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var nuevo = new Accidente
            {
                Fecha = vm.Fecha,
                ProyectoId = vm.ProyectoId,
                CedulaEmpleado = vm.CedulaEmpleado
            };

            _context.Accidentes.Add(nuevo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { accFecha = vm.Fecha.ToString("yyyy-MM-dd") });

        }
        // POST: actualizar accidente (inline)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarAccidenteInline(AccidenteEditarInlinePost vm)
        {
            var a = await _context.Accidentes.FindAsync(vm.Id);
            if (a == null) return NotFound();

            a.Fecha = vm.Fecha;
            a.ProyectoId = vm.ProyectoId;
            a.CedulaEmpleado = vm.CedulaEmpleado;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { accFecha = vm.Fecha.ToString("yyyy-MM-dd") });

        }
        // POST: eliminar accidente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarAccidente(int id, int? returnCedula, int? returnProyectoId, string? desde, string? hasta)
        {
            var a = await _context.Accidentes.FindAsync(id);
            if (a != null)
            {
                _context.Accidentes.Remove(a);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { accFecha = ("yyyy-MM-dd") });
        }
        // Helper dentro del controller
        private async Task<List<string>> GetAllowedValuesFromCheckAsync(string tableName, string columnName)
        {
            const string sql = @"
SELECT cc.definition
FROM sys.check_constraints cc
JOIN sys.columns col
  ON col.object_id = cc.parent_object_id
 AND col.column_id = cc.parent_column_id
WHERE OBJECT_NAME(cc.parent_object_id) = @table
  AND col.name = @column";

            await _context.Database.OpenConnectionAsync();   // ← abrir conexión del contexto
            try
            {
                using var cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = sql;

                var p1 = cmd.CreateParameter(); p1.ParameterName = "@table"; p1.Value = tableName;
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@column"; p2.Value = columnName;
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);

                var defObj = await cmd.ExecuteScalarAsync();
                if (defObj == null || defObj == DBNull.Value) return new List<string>();

                var def = defObj.ToString() ?? string.Empty;
                var inIdx = def.IndexOf("IN", StringComparison.OrdinalIgnoreCase);
                if (inIdx < 0) return new List<string>();
                var open = def.IndexOf('(', inIdx);
                var close = def.IndexOf(')', open + 1);
                if (open < 0 || close < 0 || close <= open) return new List<string>();

                var inner = def.Substring(open + 1, close - open - 1);
                var matches = Regex.Matches(inner, "N?'([^']*)'");
                return matches.Select(m => m.Groups[1].Value)
                              .Where(v => !string.IsNullOrWhiteSpace(v))
                              .ToList();
            }
            finally
            {
                await _context.Database.CloseConnectionAsync(); // ← cerrar conexión del contexto
            }
        }


        // POST: /Empleados/CambiarPuesto
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // opcional
        public async Task<IActionResult> CambiarPuesto(CambiarPuestoVM model)
        {
            if (model.Cedula <= 0 || model.PuestoId <= 0)
            {
                TempData["Error"] = "Seleccione empleado y un puesto válido.";
                return RedirectToAction(nameof(Index));
            }

            var emp = await _context.Empleados.FindAsync(model.Cedula);
            if (emp == null)
            {
                TempData["Error"] = "Empleado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var puesto = await _context.Puestos.FirstOrDefaultAsync(p => p.Id == model.PuestoId && p.Activo);
            if (puesto == null)
            {
                TempData["Error"] = "El puesto seleccionado es inválido o está inactivo.";
                return RedirectToAction(nameof(Index));
            }

            emp.PuestoId = puesto.Id;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Puesto de {emp.Nombre} actualizado a \"{puesto.Nombre}\".";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Empleados/SubirIncapacidad
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // opcional
        public async Task<IActionResult> SubirIncapacidad(int cedula, IFormFile archivo)
        {
            if (cedula <= 0)
            {
                TempData["Error"] = "Debe seleccionar un empleado.";
                return RedirectToAction(nameof(Index));
            }
            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe adjuntar un archivo válido.";
                return RedirectToAction(nameof(Index));
            }

            var existeEmpleado = await _context.Empleados.AnyAsync(e => e.Cedula == cedula);
            if (!existeEmpleado)
            {
                TempData["Error"] = "Empleado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            // PK = Cedula (upsert)
            var inc = await _context.Incapacidades.FindAsync(cedula);
            if (inc == null)
            {
                _context.Incapacidades.Add(new Incapacidad
                {
                    Cedula = cedula,
                    Archivo = bytes
                });
            }
            else
            {
                inc.Archivo = bytes;
                _context.Incapacidades.Update(inc);
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Incapacidad cargada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        // === READ (descargar) ===
        [HttpGet]
        public async Task<IActionResult> DescargarIncapacidad(int cedula)
        {
            var inc = await _context.Incapacidades.FindAsync(cedula);
            if (inc == null || inc.Archivo == null || inc.Archivo.Length == 0)
                return NotFound();

            // Usa un nombre/tipo genérico; si tus archivos son PDF, cambia el mime.
            return File(inc.Archivo, "application/octet-stream", $"incapacidad_{cedula}.bin");
        }

        // === UPDATE (abrir modal) ===
        [HttpGet]
        public IActionResult EditarIncapacidad(int cedula)
            => PartialView("Partials/_IncapacidadEditar", cedula);

        // === UPDATE (guardar reemplazo) ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarIncapacidad(int cedula, IFormFile archivo)
        {
            if (cedula <= 0 || archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Adjunte un archivo válido.";
                return RedirectToAction(nameof(Index));
            }

            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            var inc = await _context.Incapacidades.FindAsync(cedula);
            if (inc == null)
            {
                _context.Incapacidades.Add(new Incapacidad
                {
                    Cedula = cedula,
                    Archivo = bytes
                });
            }
            else
            {
                inc.Archivo = bytes; // la entidad ya está trackeada; no hace falta Update()
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Incapacidad actualizada.";

            var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            if (isAjax) return Json(new { ok = true, redirectUrl = Url.Action("Index", "Empleados") });

            return RedirectToAction(nameof(Index));
        }

        // === DELETE ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarIncapacidad(int cedula)
        {
            var inc = await _context.Incapacidades.FindAsync(cedula);
            if (inc == null)
            {
                TempData["Error"] = "Registro no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            _context.Incapacidades.Remove(inc);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Incapacidad eliminada.";
            return RedirectToAction(nameof(Index));
        }
        // ===================== Vacaciones =====================

        // POST: /Empleados/AgregarVacacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // opcional
        public async Task<IActionResult> AgregarVacacion(int cedula, DateTime fecha)
        {
            if (cedula <= 0)
            {
                TempData["Error"] = "Seleccione un empleado.";
                return RedirectToAction(nameof(Index));
            }
            if (fecha == default)
            {
                TempData["Error"] = "Seleccione una fecha válida.";
                return RedirectToAction(nameof(Index));
            }

            var existeEmpleado = await _context.Empleados.AnyAsync(e => e.Cedula == cedula);
            if (!existeEmpleado)
            {
                TempData["Error"] = "Empleado no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            _context.Vacaciones.Add(new VacacionesEmpleado { Cedula = cedula, Fecha = fecha.Date });
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Vacación registrada.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Empleados/EliminarVacacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // opcional
        public async Task<IActionResult> EliminarVacacion(int id)
        {
            var vac = await _context.Vacaciones.FindAsync(id);
            if (vac == null)
            {
                TempData["Error"] = "Registro de vacación no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            _context.Vacaciones.Remove(vac);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Vacación eliminada.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> CrearVacacion(int Cedula, DateTime Fecha)
    => AgregarVacacion(Cedula, Fecha);
        [HttpPost]
        [ValidateAntiForgeryToken]
            public async Task<IActionResult> ActualizarVacacion(int Id, DateTime Fecha)
        {
            var vac = await _context.Vacaciones.FindAsync(Id);
            if (vac == null)
            {
                TempData["Error"] = "Registro de vacación no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            vac.Fecha = Fecha.Date;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Vacación actualizada.";
            return RedirectToAction(nameof(Index));
        }

    }
}
