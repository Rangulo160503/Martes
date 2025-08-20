using CEGA.Data;
using CEGA.Models;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CEGA.Controllers
{
    public class MarketingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MarketingController(ApplicationDbContext context) => _context = context;

        // -------------------- Helpers --------------------
        // Controllers/MarketingController.cs
        private MarketingPageVM BuildVM() => new MarketingPageVM
        {
            Campanias = _context.CampsMarketing
                .AsNoTracking()
                .Select(c => new CampMarketing
                {
                    Id = c.Id,
                    Nombre = c.Nombre ?? "",
                    AsuntoCorreo = c.AsuntoCorreo ?? "",
                    Descripcion = c.Descripcion ?? "",
                    ImagenUrl = c.ImagenUrl,          // déjala nullable en el modelo
                    NombrePool = c.NombrePool         // déjala nullable en el modelo
                })
                .OrderBy(x => x.Nombre)
                .ToList(),

            Pools = _context.PoolsCorreo
                .AsNoTracking()
                .Select(p => new PoolCorreo
                {
                    Id = p.Id,
                    Nombre = p.Nombre ?? "",
                    Descripcion = p.Descripcion ?? "",
                    Correos = p.Correos ?? ""
                })
                .OrderBy(x => x.Nombre)
                .ToList(),

            Programaciones = _context.ProgramacionesDistribucion
                .AsNoTracking()
                .Select(d => new ProgramacionDistribucion
                {
                    Id = d.Id,
                    Nombre = d.Nombre ?? "",
                    Descripcion = d.Descripcion ?? "",
                    NombrePool = d.NombrePool ?? "",
                    FechaHoraEnvio = d.FechaHoraEnvio // mejor que sea DateTime? en el modelo
                })
                .OrderByDescending(x => x.FechaHoraEnvio ?? DateTime.MinValue)
                .ToList(),

            Clientes = _context.ClientesMarketing
                .AsNoTracking()
                .Select(c => new ClienteMarketing
                {
                    Id = c.Id,
                    Nombre = c.Nombre ?? "",
                    Correo = c.Correo ?? "",
                    Telefono = c.Telefono // si es nullable en BD, que sea string? en el modelo
                })
                .OrderBy(x => x.Nombre)
                .ToList()
        };


        private static string Norm(string? s) => (s ?? string.Empty).Trim();

        // -------------------- Página --------------------
        [HttpGet]
        public IActionResult Marketing()
        {
            var vm = BuildVM();
            return View(vm);
        }

        // -------------------- Campañas --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCampania(CampMarketing model)
        {
            if (!ModelState.IsValid)
            {
                // Regresa a la vista con el VM armado
                var vmErr = BuildVM();
                TempData["Error"] = "Formulario inválido. Completa todos los campos requeridos.";
                return View("Marketing", vmErr);
            }

            // Normaliza entradas
            string Norm(string? s) => (s ?? string.Empty).Trim();

            model.Nombre = Norm(model.Nombre);
            model.AsuntoCorreo = Norm(model.AsuntoCorreo);
            model.Descripcion = Norm(model.Descripcion);
            model.ImagenUrl = string.IsNullOrWhiteSpace(model.ImagenUrl) ? null : Norm(model.ImagenUrl);
            model.NombrePool = string.IsNullOrWhiteSpace(model.NombrePool) ? null : Norm(model.NombrePool);

            // Único por nombre (si así lo quieres)
            if (_context.CampsMarketing.Any(c => c.Nombre == model.Nombre))
            {
                ModelState.AddModelError(nameof(model.Nombre), "Esta campaña ya existe");
                var vmDup = BuildVM();
                return View("Marketing", vmDup);
            }

            try
            {
                _context.CampsMarketing.Add(model);
                _context.SaveChanges();

                TempData["Mensaje"] = "Campaña creada exitosamente.";
                return RedirectToAction(nameof(Marketing));
            }
            catch (Exception ex)
            {
                // Muestra la causa real (inner) en el banner
                var root = ex;
                while (root.InnerException != null) root = root.InnerException;

                TempData["Error"] = "No se pudo guardar la campaña. " + root.Message;

                var vmErr = BuildVM();
                return View("Marketing", vmErr);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarCampania(int id) // id = campaña
        {
            var camp = _context.CampsMarketing.FirstOrDefault(c => c.Id == id);
            if (camp == null)
            { TempData["Error"] = "Campaña no encontrada."; return RedirectToAction(nameof(Marketing)); }

            if (string.IsNullOrWhiteSpace(camp.NombrePool))
            { TempData["Error"] = "La campaña no tiene un pool asignado."; return RedirectToAction(nameof(Marketing)); }

            var pool = _context.PoolsCorreo.FirstOrDefault(p => p.Nombre == camp.NombrePool);
            if (pool == null)
            { TempData["Error"] = "El pool asignado no existe."; return RedirectToAction(nameof(Marketing)); }

            var correos = (pool.Correos ?? "")
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!correos.Any())
            { TempData["Error"] = "El pool no contiene correos válidos."; return RedirectToAction(nameof(Marketing)); }

            // TODO: integrar tu envío real (SMTP/MailKit). Por ahora, stub:
            await Task.CompletedTask;

            TempData["Mensaje"] = $"Se disparó el envío de \"{camp.Nombre}\" al pool \"{pool.Nombre}\" ({correos.Count} destinatario(s)).";
            return RedirectToAction(nameof(Marketing));
        }

        [HttpGet]
        public IActionResult EditarCampania(int id)
        {
            var camp = _context.CampsMarketing.FirstOrDefault(c => c.Id == id);
            if (camp == null) { TempData["Error"] = "Campaña no encontrada."; return RedirectToAction(nameof(Marketing)); }

            var vm = BuildVM(); vm.CampaniaForm = camp; return View("Marketing", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCampania(CampMarketing model)
        {
            if (!ModelState.IsValid) { var vm = BuildVM(); vm.CampaniaForm = model; return View("Marketing", vm); }

            var existente = _context.CampsMarketing.FirstOrDefault(c => c.Id == model.Id);
            if (existente == null) { TempData["Error"] = "Campaña no encontrada."; return RedirectToAction(nameof(Marketing)); }

            var nombre = Norm(model.Nombre);
            if (_context.CampsMarketing.Any(c => c.Id != model.Id && c.Nombre == nombre))
            {
                ModelState.AddModelError(nameof(model.Nombre), "Esta campaña ya existe");
                var vm = BuildVM(); vm.CampaniaForm = model; return View("Marketing", vm);
            }

            existente.Nombre = nombre;
            existente.AsuntoCorreo = model.AsuntoCorreo;
            existente.Descripcion = model.Descripcion;
            existente.ImagenUrl = model.ImagenUrl;

            _context.SaveChanges();
            TempData["Mensaje"] = "Campaña actualizada exitosamente";
            return RedirectToAction(nameof(Marketing));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuscarCampania(string nombreCampania)
        {
            var criterio = Norm(nombreCampania);
            if (string.IsNullOrWhiteSpace(criterio))
            { TempData["Error"] = "Se necesita llenar datos"; return RedirectToAction(nameof(Marketing)); }

            var vm = BuildVM();
            vm.ResultadoCampanias = _context.CampsMarketing
                .Where(c => c.Nombre == criterio)
                .ToList();

            if (!vm.ResultadoCampanias.Any())
                TempData["Error"] = "No hay campañas con ese nombre";

            return View("Marketing", vm);
        }

        [HttpGet]
        public IActionResult EliminarCampania(int id)
        {
            var camp = _context.CampsMarketing.FirstOrDefault(c => c.Id == id);
            if (camp == null) { TempData["Error"] = "Campaña no encontrada."; return RedirectToAction(nameof(Marketing)); }
            return View("ConfirmarEliminar", camp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarEliminarCampania(int id)
        {
            var camp = _context.CampsMarketing.FirstOrDefault(c => c.Id == id);
            if (camp == null) { TempData["Error"] = "Campaña no encontrada."; return RedirectToAction(nameof(Marketing)); }

            _context.CampsMarketing.Remove(camp);
            _context.SaveChanges();
            TempData["Mensaje"] = "Campaña eliminada exitosamente.";
            return RedirectToAction(nameof(Marketing));
        }

        // -------------------- Pools de Correos --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPoolCorreo(
    [FromForm] string Nombre,
    [FromForm] string Descripcion,
    [FromForm] string Correos,           // viene del input hidden
    IFormFile? ArchivoAdjunto = null)    // opcional: por ahora se ignora
        {
            // Normalizar
            Nombre = Norm(Nombre);
            Descripcion = Norm(Descripcion);

            // Correos: aceptar ; , y saltos de línea, quitar vacíos y duplicados
            var correos = Norm(Correos)
                .Replace(",", ";")
                .Replace("\n", ";")
                .Replace("\r", ";");

            var listaCorreos = correos
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Descripcion) ||
                !listaCorreos.Any())
            {
                ModelState.AddModelError("Correos", "Selecciona al menos un correo.");
                var vm = BuildVM();
                vm.PoolForm = new PoolCorreo
                {
                    Nombre = Nombre,
                    Descripcion = Descripcion,
                    Correos = string.Join("; ", listaCorreos)
                };
                return View("Marketing", vm);
            }

            // Unicidad por nombre
            if (_context.PoolsCorreo.Any(p => p.Nombre == Nombre))
            {
                ModelState.AddModelError(nameof(Nombre), "Este pool ya existe.");
                var vm = BuildVM();
                vm.PoolForm = new PoolCorreo
                {
                    Nombre = Nombre,
                    Descripcion = Descripcion,
                    Correos = string.Join("; ", listaCorreos)
                };
                return View("Marketing", vm);
            }

            // Guardar
            var pool = new PoolCorreo
            {
                Nombre = Nombre,
                Descripcion = Descripcion,
                Correos = string.Join("; ", listaCorreos)
            };

            _context.PoolsCorreo.Add(pool);
            _context.SaveChanges();

            TempData["Mensaje"] = "Pool de correos creado exitosamente.";
            return RedirectToAction(nameof(Marketing));
        }


        [HttpGet]
        public IActionResult EditarPoolCorreo(int id)
        {
            var pool = _context.PoolsCorreo.FirstOrDefault(p => p.Id == id);
            if (pool == null) { TempData["Error"] = "Pool no encontrado."; return RedirectToAction(nameof(Marketing)); }

            var vm = BuildVM(); vm.PoolForm = pool; return View("Marketing", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPoolCorreo(PoolCorreo model)
        {
            if (!ModelState.IsValid) { var vm = BuildVM(); vm.PoolForm = model; return View("Marketing", vm); }

            var pool = _context.PoolsCorreo.FirstOrDefault(p => p.Id == model.Id);
            if (pool == null) { TempData["Error"] = "Pool no encontrado."; return RedirectToAction(nameof(Marketing)); }

            if (_context.PoolsCorreo.Any(p => p.Id != model.Id && p.Correos == model.Correos))
            {
                ModelState.AddModelError(nameof(model.Correos), "Correo(s) ya está(n) dentro de un pool");
                var vm2 = BuildVM(); vm2.PoolForm = model; return View("Marketing", vm2);
            }

            pool.Nombre = Norm(model.Nombre);
            pool.Descripcion = model.Descripcion;
            pool.Correos = model.Correos;

            _context.SaveChanges();
            TempData["Mensaje"] = "Pool de correos editado exitosamente.";
            return RedirectToAction(nameof(Marketing));
        }

        [HttpGet]
        public IActionResult BuscarPoolCorreo(string nombre)
        {
            var criterio = Norm(nombre);
            if (string.IsNullOrWhiteSpace(criterio))
            { TempData["Error"] = "Se necesita llenar datos"; return RedirectToAction(nameof(Marketing)); }

            var vm = BuildVM();
            vm.PoolEncontrado = _context.PoolsCorreo.FirstOrDefault(p => p.Nombre == criterio);
            if (vm.PoolEncontrado == null) TempData["Error"] = "No hay pools con ese nombre";
            return View("Marketing", vm);
        }

        // -------------------- Programaciones de Distribución --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearProgramacionDistribucion(ProgramacionDistribucion model)
        {
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(model.Nombre) ||
                string.IsNullOrWhiteSpace(model.Descripcion) ||
                string.IsNullOrWhiteSpace(model.NombrePool))
            {
                TempData["Error"] = "Se necesita llenar todo el formulario";
                return RedirectToAction(nameof(Marketing));
            }

            var nombre = Norm(model.Nombre);
            var pool = Norm(model.NombrePool);

            if (_context.ProgramacionesDistribucion.Any(p => p.Nombre == nombre))
            {
                TempData["Error"] = "Este programa de distribución ya existe";
                return RedirectToAction(nameof(Marketing));
            }

            var poolExiste = _context.PoolsCorreo.Any(p => p.Nombre == pool);
            if (!poolExiste)
            {
                TempData["Error"] = "Este pool no existe";
                return RedirectToAction(nameof(Marketing));
            }

            model.Nombre = nombre;
            model.NombrePool = pool;

            _context.ProgramacionesDistribucion.Add(model);
            _context.SaveChanges();

            TempData["Mensaje"] = "Programación creada";
            return RedirectToAction(nameof(Marketing));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarProgramacion(ProgramacionDistribucion model)
        {
            if (!ModelState.IsValid) { var vm = BuildVM(); vm.ProgramacionForm = model; return View("Marketing", vm); }

            var existente = _context.ProgramacionesDistribucion.FirstOrDefault(d => d.Id == model.Id);
            if (existente == null) { TempData["Error"] = "Programación no encontrada."; return RedirectToAction(nameof(Marketing)); }

            var pool = Norm(model.NombrePool);
            if (!_context.PoolsCorreo.Any(p => p.Nombre == pool))
            {
                ModelState.AddModelError(nameof(model.NombrePool), "Este pool no existe");
                var vm = BuildVM(); vm.ProgramacionForm = model; return View("Marketing", vm);
            }

            existente.Nombre = Norm(model.Nombre);
            existente.Descripcion = model.Descripcion;
            existente.FechaHoraEnvio = model.FechaHoraEnvio;
            existente.NombrePool = pool;

            _context.SaveChanges();
            TempData["Mensaje"] = "Programación editada correctamente.";
            return RedirectToAction(nameof(Marketing));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuscarProgramacion(string nombreDistribucion)
        {
            var criterio = Norm(nombreDistribucion);
            if (string.IsNullOrWhiteSpace(criterio))
            { TempData["ErrorBusqueda"] = "Se necesita llenar datos"; return RedirectToAction(nameof(Marketing)); }

            var vm = BuildVM();
            vm.ProgramacionEncontrada = _context.ProgramacionesDistribucion.FirstOrDefault(p => p.Nombre == criterio);

            if (vm.ProgramacionEncontrada == null)
                TempData["ErrorBusqueda"] = "No hay programación de distribución con ese nombre";

            return View("Marketing", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarProgramacion(string nombreDistribucion, bool confirmar)
        {
            var criterio = Norm(nombreDistribucion);
            if (string.IsNullOrWhiteSpace(criterio))
            { TempData["ErrorEliminacion"] = "Debe ingresar el nombre"; return RedirectToAction(nameof(Marketing)); }

            var distribucion = _context.ProgramacionesDistribucion.FirstOrDefault(d => d.Nombre == criterio);
            if (distribucion == null) { TempData["ErrorEliminacion"] = "Programación no encontrada"; return RedirectToAction(nameof(Marketing)); }
            if (!confirmar) { TempData["MensajeEliminacion"] = "No eliminada"; return RedirectToAction(nameof(Marketing)); }

            _context.ProgramacionesDistribucion.Remove(distribucion);
            _context.SaveChanges();
            TempData["MensajeEliminacion"] = "Eliminada exitosamente";
            return RedirectToAction(nameof(Marketing));
        }

        // -------------------- Clientes --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCliente(ClienteMarketing model)
        {
            if (!ModelState.IsValid) { var vm = BuildVM(); vm.ClienteForm = model; return View("Marketing", vm); }

            var nombre = Norm(model.Nombre);
            var correo = Norm(model.Correo);

            if (_context.ClientesMarketing.Any(c => c.Nombre == nombre))
            {
                ModelState.AddModelError(nameof(model.Nombre), "Este registro de cliente ya existe");
                var vm = BuildVM(); vm.ClienteForm = model; return View("Marketing", vm);
            }

            if (_context.ClientesMarketing.Any(c => c.Correo == correo))
            {
                ModelState.AddModelError(nameof(model.Correo), "Correo electrónico ya existe");
                var vm = BuildVM(); vm.ClienteForm = model; return View("Marketing", vm);
            }

            model.Nombre = nombre;
            model.Correo = correo;

            _context.ClientesMarketing.Add(model);
            _context.SaveChanges();

            TempData["MensajeCliente"] = "Registro de cliente creado exitosamente";
            return RedirectToAction(nameof(Marketing));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCliente(ClienteMarketing model)
        {
            if (!ModelState.IsValid) { var vm = BuildVM(); vm.ClienteForm = model; return View("Marketing", vm); }

            var existente = _context.ClientesMarketing.FirstOrDefault(c => c.Id == model.Id);
            if (existente == null) { TempData["Error"] = "No se encontró el cliente."; return RedirectToAction(nameof(Marketing)); }

            existente.Nombre = Norm(model.Nombre);
            existente.Correo = Norm(model.Correo);
            existente.Telefono = model.Telefono;

            _context.SaveChanges();
            TempData["Mensaje"] = "Cliente editado correctamente.";
            return RedirectToAction(nameof(Marketing));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuscarCliente(string nombreCliente)
        {
            var criterio = Norm(nombreCliente);
            if (string.IsNullOrWhiteSpace(criterio))
            { TempData["Error"] = "Se necesita llenar datos"; return RedirectToAction(nameof(Marketing)); }

            var vm = BuildVM();
            vm.ClienteEncontrado = _context.ClientesMarketing.FirstOrDefault(c => c.Nombre == criterio);
            if (vm.ClienteEncontrado == null) TempData["Error"] = "No existe cliente con ese nombre";
            return View("Marketing", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCliente(string nombreCliente, string confirmar)
        {
            var criterio = Norm(nombreCliente);
            var cliente = _context.ClientesMarketing.FirstOrDefault(c => c.Nombre == criterio);
            if (cliente == null) { TempData["Error"] = "No existe cliente con ese nombre"; return RedirectToAction(nameof(Marketing)); }

            if (Norm(confirmar).ToLower() == "si")
            {
                _context.ClientesMarketing.Remove(cliente);
                _context.SaveChanges();
                TempData["Mensaje"] = "Cliente eliminado exitosamente";
            }
            else
            {
                TempData["Mensaje"] = "Cliente no eliminado";
            }
            return RedirectToAction(nameof(Marketing));
        }
    }
}
