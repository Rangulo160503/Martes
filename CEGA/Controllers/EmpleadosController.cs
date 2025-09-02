using CEGA.Data;
using CEGA.Models.ViewModels;
using CEGA.Models.ViewModels.Empleados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CEGA.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmpleadosController(ApplicationDbContext context) => _context = context;

        // =========================================================
        //  Cambiar puesto de un empleado (desde el partial _Puestos)
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // opcional: [Authorize(Roles = "AdminSistema,RRHH")]
        public async Task<IActionResult> CambiarPuesto(CambiarPuestoVM model)
        {
            if (model.Cedula <= 0 || model.PuestoId <= 0)
            {
                TempData["Error"] = "Seleccione empleado y un puesto válido.";
                return RedirectToAction("Index", "Empleados");
            }

            var emp = await _context.Empleados.FindAsync(model.Cedula);
            if (emp == null)
            {
                TempData["Error"] = "Empleado no encontrado.";
                return RedirectToAction("Index", "Empleados");
            }

            var puesto = await _context.Puestos
                .FirstOrDefaultAsync(p => p.Id == model.PuestoId && p.Activo);
            if (puesto == null)
            {
                TempData["Error"] = "El puesto seleccionado es inválido o está inactivo.";
                return RedirectToAction("Index", "Empleados");
            }

            emp.PuestoId = puesto.Id;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Puesto de {emp.Nombre} actualizado a \"{puesto.Nombre}\".";
            return RedirectToAction("Index", "Empleados");
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // --------- Datos base ----------
            var puestos = await _context.Puestos
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            var empleados = await _context.Empleados
                .AsNoTracking()
                .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                .ToListAsync();

            // --------- Partial: _Puestos (Cambiar puesto) ----------
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

            // --------- Partial: _CrearEmpleado (reusa RegisterViewModel) ----------
            var crearEmpleadoVm = new RegisterViewModel
            {
                Puestos = puestos.Where(p => p.Activo)
                                 .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                                 .ToList()
            };

            // --------- Entregas a la vista (Index.cshtml) ----------
            ViewBag.Salarios = puestos;            // si tu _Salarios usa Puesto.SalarioBase
            ViewBag.Vacaciones = null;               // llena cuando tengas el modelo/lista
            ViewBag.Incapacidades = null;               // llena cuando tengas el modelo/lista
            ViewBag.Puestos = puestos;            // si algún partial lista puestos
            ViewBag.PuestosVm = vmPuestos;          // para Partial _Puestos (dropdowns)
            ViewBag.CrearEmpleadoVm = crearEmpleadoVm;    // para Partial _CrearEmpleado

            return View();
        }
    }
}
