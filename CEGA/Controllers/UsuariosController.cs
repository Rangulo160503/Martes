using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CEGA.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context) => _context = context;

        // ===============================================================
        //       lista, filtra y llena el dropdown de roles
        // ===============================================================
        [HttpGet]
        public IActionResult Index(string? query = null, string? rol = null)
        {
            // 1) Base: Empleados
            var empleados = _context.Empleados.AsQueryable();

            // 2) Filtros
            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                empleados = empleados.Where(e =>
                    (e.Nombre ?? "").ToLower().Contains(q) ||
                    (e.SegundoNombre ?? "").ToLower().Contains(q) ||
                    (e.Apellido1 ?? "").ToLower().Contains(q) ||
                    (e.Apellido2 ?? "").ToLower().Contains(q) ||
                    (e.Username ?? "").ToLower().Contains(q) ||
                    (e.Email ?? "").ToLower().Contains(q) ||
                    (e.Rol.ToString()).ToLower().Contains(q)
                );
            }
            if (!string.IsNullOrWhiteSpace(rol))
            {
                // rol viene como string (nombre del enum o número); aquí lo tratamos como texto del enum
                empleados = empleados.Where(e => e.Rol.ToString() == rol);
            }

            // 3) Roles disponibles (para el dropdown)
            var rolesDisponibles = _context.Empleados
                .Select(e => e.Rol.ToString())
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            // 4) Proyección a UsuarioListaVM
            var lista = empleados
                .OrderBy(e => e.Apellido1).ThenBy(e => e.Apellido2).ThenBy(e => e.Nombre)
                .Select(e => new CEGA.Models.ViewModels.UsuarioListaVM
                {
                    NombreCompleto = string.Join(" ",
                        new[] { e.Nombre, e.SegundoNombre, e.Apellido1, e.Apellido2 }
                        .Where(s => !string.IsNullOrWhiteSpace(s))),
                    Username = e.Username!,
                    Email = e.Email!,
                    TelefonoPersonal = e.TelefonoPersonal,
                    Rol = e.Rol.ToString(),
                    Activo = e.Activo
                })
                .ToList();

            // 5) ViewModel de la vista
            var vm = new CEGA.Models.ViewModels.Usuarios.BuscarUsuarioFiltroViewModel
            {
                Query = query,
                Rol = rol,
                UsuariosFiltrados = lista,
                RolesDisponibles = rolesDisponibles
            };

            return View(vm);
        }

        // =============================
        //       actualizar el rol
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarRol(CEGA.Models.ViewModels.Usuarios.UsuarioRolUpdateVM model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var empleado = await _context.Empleados.FindAsync(model.Cedula);
            if (empleado == null) return RedirectToAction(nameof(Index));

            // Validar que el tinyint recibido existe en el enum
            if (!Enum.IsDefined(typeof(RolEmpleado), model.Rol)) return RedirectToAction(nameof(Index));

            empleado.Rol = (RolEmpleado)model.Rol;   // tinyint → enum
            await _context.SaveChangesAsync();

            TempData["ok"] = "Rol actualizado.";
            return RedirectToAction(nameof(Index));
        }

    }
}
