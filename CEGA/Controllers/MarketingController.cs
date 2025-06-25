using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace CEGA.Controllers
{
    public class MarketingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarketingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Marketing()
        {
            ViewBag.Campanias = _context.CampsMarketing.ToList();
            ViewBag.Pools = _context.PoolsCorreo.ToList();
            ViewBag.Distribuciones = _context.ProgramacionesDistribucion.ToList();
            ViewBag.Clientes = _context.ClientesMarketing.ToList();

            return View();
        }
        [HttpPost]
        public IActionResult CrearCampaña(CampMarketing model)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Nombre) &&
                    string.IsNullOrWhiteSpace(model.AsuntoCorreo) &&
                    string.IsNullOrWhiteSpace(model.Descripcion) &&
                    string.IsNullOrWhiteSpace(model.ImagenUrl))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }
                return View("Marketing", model);
            }

            if (_context.CampsMarketing.Any(c => c.Nombre == model.Nombre))
            {
                ModelState.AddModelError("Nombre", "Esta campaña ya existe");
                return View("Marketing", model);
            }

            _context.CampsMarketing.Add(model);
            _context.SaveChanges();

            TempData["Mensaje"] = "Campaña creada exitosamente";
            return RedirectToAction("Marketing");
        }
        [HttpGet]
        public IActionResult EditarCampaña(int id)
        {
            var campaña = _context.CampsMarketing.FirstOrDefault(c => c.Id == id);
            if (campaña == null)
            {
                TempData["Error"] = "Campaña no encontrada.";
                return RedirectToAction("Marketing");
            }

            return View("Marketing", campaña); // Usamos la misma vista "Marketing"
        }
        [HttpPost]
        public IActionResult EditarCampaña(CampMarketing model)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Nombre) &&
                    string.IsNullOrWhiteSpace(model.AsuntoCorreo) &&
                    string.IsNullOrWhiteSpace(model.Descripcion) &&
                    string.IsNullOrWhiteSpace(model.ImagenUrl))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }
                return View("Marketing", model);
            }

            var campañaExistente = _context.CampsMarketing.FirstOrDefault(c => c.Id == model.Id);
            if (campañaExistente == null)
            {
                TempData["Error"] = "Campaña no encontrada para editar.";
                return RedirectToAction("Marketing");
            }

            // Validar nombre duplicado (excepto la misma campaña)
            if (_context.CampsMarketing.Any(c => c.Id != model.Id && c.Nombre == model.Nombre))
            {
                ModelState.AddModelError("Nombre", "Esta campaña ya existe");
                return View("Marketing", model);
            }

            campañaExistente.Nombre = model.Nombre;
            campañaExistente.AsuntoCorreo = model.AsuntoCorreo;
            campañaExistente.Descripcion = model.Descripcion;
            campañaExistente.ImagenUrl = model.ImagenUrl;

            _context.SaveChanges();

            TempData["Mensaje"] = "Campaña actualizada exitosamente";
            return RedirectToAction("Marketing");
        }
        [HttpPost]
        public IActionResult BuscarCampaña(string nombreCampaña)
        {
            if (string.IsNullOrWhiteSpace(nombreCampaña))
            {
                ModelState.AddModelError("", "Se necesita llenar datos");
                return View("Marketing");
            }

            var campaña = _context.CampsMarketing
                .Where(c => c.Nombre.ToLower() == nombreCampaña.ToLower())
                .ToList();

            if (!campaña.Any())
            {
                ModelState.AddModelError("", "No hay campañas con ese nombre");
                return View("Marketing");
            }

            ViewBag.ResultadoBusqueda = campaña;
            return View("Marketing");
        }
        [HttpGet]
        public IActionResult EliminarCampaña(int id)
        {
            var campaña = _context.CampsMarketing.FirstOrDefault(c => c.Id == id);
            if (campaña == null)
            {
                TempData["Error"] = "Campaña no encontrada.";
                return RedirectToAction("Marketing");
            }

            return View("ConfirmarEliminar", campaña);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarEliminarCampaña(int id)
        {
            var campaña = _context.CampsMarketing.FirstOrDefault(c => c.Id == id);
            if (campaña == null)
            {
                TempData["Error"] = "Campaña no encontrada.";
                return RedirectToAction("Marketing");
            }

            _context.CampsMarketing.Remove(campaña);
            _context.SaveChanges();

            TempData["Mensaje"] = "Campaña eliminada exitosamente.";
            return RedirectToAction("Marketing");
        }
        [HttpPost]
        public IActionResult CrearPoolCorreo(PoolCorreo model)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Nombre) &&
                    string.IsNullOrWhiteSpace(model.Descripcion) &&
                    string.IsNullOrWhiteSpace(model.Correos))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }

                return View("Marketing", model);
            }

            // Verificar duplicado de nombre
            if (_context.PoolsCorreo.Any(p => p.Nombre == model.Nombre))
            {
                ModelState.AddModelError("Nombre", "Este pool ya existe");
                return View("Marketing", model);
            }

            // Verificar correo duplicado
            if (_context.PoolsCorreo.Any(p => p.Correos == model.Correos))
            {
                ModelState.AddModelError("Correo", "Correo electrónico ya está dentro del pool");
                return View("Marketing", model);
            }

            _context.PoolsCorreo.Add(model);
            _context.SaveChanges();

            TempData["Mensaje"] = "Pool de correos creado exitosamente.";
            return RedirectToAction("Marketing");
        }
        [HttpGet]
        public IActionResult EditarPoolCorreo(int id)
        {
            var pool = _context.PoolsCorreo.FirstOrDefault(p => p.Id == id);
            if (pool == null)
            {
                TempData["Error"] = "Pool de correos no encontrado.";
                return RedirectToAction("Marketing");
            }

            return View("Marketing", pool);
        }
        [HttpPost]
        public IActionResult EditarPoolCorreo(PoolCorreo model)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Nombre) &&
                    string.IsNullOrWhiteSpace(model.Descripcion) &&
                    string.IsNullOrWhiteSpace(model.Correos))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }

                return View("Marketing", model);
            }

            var poolExistente = _context.PoolsCorreo.FirstOrDefault(p => p.Id == model.Id);
            if (poolExistente == null)
            {
                TempData["Error"] = "Pool no encontrado.";
                return RedirectToAction("Marketing");
            }

            // Validación de formato de correo ya la maneja [EmailAddress]

            // Validar que el correo no esté duplicado en otro registro
            if (_context.PoolsCorreo.Any(p => p.Correos == model.Correos && p.Id != model.Id))
            {
                ModelState.AddModelError("Correo", "Correo electrónico ya está dentro del pool");
                return View("Marketing", model);
            }

            poolExistente.Nombre = model.Nombre;
            poolExistente.Descripcion = model.Descripcion;
            poolExistente.Correos = model.Correos;
            
            _context.SaveChanges();

            TempData["Mensaje"] = "Pool de correos editado exitosamente.";
            return RedirectToAction("Marketing");
        }
        [HttpGet]
        public IActionResult BuscarPoolCorreo(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["Error"] = "Se necesita llenar datos";
                return RedirectToAction("Marketing");
            }

            var pool = _context.PoolsCorreo.FirstOrDefault(p => p.Nombre.ToLower() == nombre.ToLower());

            if (pool == null)
            {
                TempData["Error"] = "No hay pools de correos con ese nombre";
                return RedirectToAction("Marketing");
            }

            // Puedes enviar el pool encontrado a la vista principal para mostrarlo en un resultado
            ViewBag.PoolEncontrado = pool;
            return View("Marketing");
        }
        [HttpGet]
        public IActionResult ConfirmarEliminarPool(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["Error"] = "Nombre del pool requerido";
                return RedirectToAction("Marketing");
            }

            var pool = _context.PoolsCorreo.FirstOrDefault(p => p.Nombre.ToLower() == nombre.ToLower());
            if (pool == null)
            {
                TempData["Error"] = "Pool de correos no encontrado";
                return RedirectToAction("Marketing");
            }

            return View("ConfirmarEliminarPool", pool); // Puedes crear una vista simple de confirmación
        }
        [HttpPost]
        public IActionResult EliminarPoolConfirmado(int id, bool confirmar)
        {
            var pool = _context.PoolsCorreo.FirstOrDefault(p => p.Id == id);
            if (pool == null)
            {
                TempData["Error"] = "Pool de correos no encontrado";
                return RedirectToAction("Marketing");
            }

            if (!confirmar)
            {
                TempData["Mensaje"] = "Pool de correos no eliminado";
                return RedirectToAction("Marketing");
            }

            _context.PoolsCorreo.Remove(pool);
            _context.SaveChanges();

            TempData["Mensaje"] = "Pool de correos eliminado exitosamente";
            return RedirectToAction("Marketing");
        }
        [HttpPost]
        public IActionResult CrearProgramacionDistribucion(ProgramacionDistribucion model)
        {
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(model.Nombre) ||
                string.IsNullOrWhiteSpace(model.Descripcion) ||
                string.IsNullOrWhiteSpace(model.NombrePool))
            {
                TempData["Error"] = "Se necesita llenar todo el formulario";
                return RedirectToAction("Marketing");
            }

            if (_context.ProgramacionesDistribucion.Any(p => p.Nombre == model.Nombre))
            {
                TempData["Error"] = "Este programa de distribución ya existe";
                return RedirectToAction("Marketing");
            }

            var poolExiste = _context.PoolsCorreo.Any(p => p.Nombre == model.NombrePool);
            if (!poolExiste)
            {
                TempData["Error"] = "Este pool no existe";
                return RedirectToAction("Marketing");
            }

            _context.ProgramacionesDistribucion.Add(model);
            _context.SaveChanges();

            TempData["UltimaDistribucionNombre"] = model.Nombre;
            TempData["UltimaDistribucionDescripcion"] = model.Descripcion;
            TempData["UltimaDistribucionFecha"] = model.FechaHoraEnvio.ToString("g");
            TempData["UltimaDistribucionPool"] = model.NombrePool;

            return RedirectToAction("Marketing");
        }

        [HttpPost]
        public IActionResult EditarProgramacion(DistribucionMarketing model)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Nombre) &&
                    string.IsNullOrWhiteSpace(model.Descripcion) &&
                    model.FechaHoraEnvio == default &&
                    string.IsNullOrWhiteSpace(model.NombrePool))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }

                return View("Marketing", model);
            }

            var distribucionExistente = _context.DistribucionesMarketing.FirstOrDefault(d => d.Id == model.Id);
            if (distribucionExistente == null)
            {
                TempData["Error"] = "No se encontró la programación de distribución a editar.";
                return RedirectToAction("Marketing");
            }

            // Verifica que el pool especificado exista
            var poolExiste = _context.PoolsCorreo.Any(p => p.Nombre == model.NombrePool);
            if (!poolExiste)
            {
                ModelState.AddModelError("NombrePool", "Este pool no existe");
                return View("Marketing", model);
            }

            // Actualiza los datos
            distribucionExistente.Nombre = model.Nombre;
            distribucionExistente.Descripcion = model.Descripcion;
            distribucionExistente.FechaHoraEnvio = model.FechaHoraEnvio;
            distribucionExistente.NombrePool = model.NombrePool;

            _context.SaveChanges();

            TempData["Mensaje"] = "Programación de distribución editada correctamente.";
            return RedirectToAction("Marketing");
        }
        [HttpPost]
        public IActionResult BuscarProgramacion(string nombreDistribucion)
        {
            if (string.IsNullOrWhiteSpace(nombreDistribucion))
            {
                TempData["ErrorBusqueda"] = "Se necesita llenar datos";
                return RedirectToAction("Marketing");
            }

            var resultado = _context.DistribucionesMarketing
                .FirstOrDefault(p => p.Nombre.ToLower() == nombreDistribucion.ToLower());

            if (resultado == null)
            {
                TempData["ErrorBusqueda"] = "No hay programación de distribución con ese nombre";
                return RedirectToAction("Marketing");
            }

            ViewBag.ResultadoDistribucion = resultado;
            return View("Marketing");
        }
        [HttpPost]
        public IActionResult EliminarProgramacion(string nombreDistribucion, bool confirmar)
        {
            if (string.IsNullOrWhiteSpace(nombreDistribucion))
            {
                TempData["ErrorEliminacion"] = "Debe ingresar el nombre de la programación a eliminar";
                return RedirectToAction("Marketing");
            }

            var distribucion = _context.DistribucionesMarketing.FirstOrDefault(d => d.Nombre.ToLower() == nombreDistribucion.ToLower());

            if (distribucion == null)
            {
                TempData["ErrorEliminacion"] = "Programación no encontrada";
                return RedirectToAction("Marketing");
            }

            if (!confirmar)
            {
                TempData["MensajeEliminacion"] = "Programación de distribución no eliminada";
                return RedirectToAction("Marketing");
            }

            _context.DistribucionesMarketing.Remove(distribucion);
            _context.SaveChanges();

            TempData["MensajeEliminacion"] = "Programación de distribución eliminada exitosamente";
            return RedirectToAction("Marketing");
        }
        [HttpPost]
        public IActionResult CrearCliente(ClienteMarketing model)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Nombre) &&
                    string.IsNullOrWhiteSpace(model.Correo) &&
                    string.IsNullOrWhiteSpace(model.Telefono))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }
                return View("Marketing", model);
            }

            if (_context.ClientesMarketing.Any(c => c.Nombre.ToLower() == model.Nombre.ToLower()))
            {
                ModelState.AddModelError("Nombre", "Este registro de cliente ya existe");
                return View("Marketing", model);
            }

            if (_context.ClientesMarketing.Any(c => c.Correo.ToLower() == model.Correo.ToLower()))
            {
                ModelState.AddModelError("Correo", "Correo electrónico ya existe");
                return View("Marketing", model);
            }

            _context.ClientesMarketing.Add(model);
            _context.SaveChanges();

            TempData["MensajeCliente"] = "Registro de cliente creado exitosamente";
            return RedirectToAction("Marketing");
        }
        [HttpPost]
        public IActionResult EditarCliente(ClienteMarketing model)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Nombre) &&
                    string.IsNullOrWhiteSpace(model.Correo) &&
                    string.IsNullOrWhiteSpace(model.Telefono))
                {
                    ModelState.AddModelError(string.Empty, "Se necesita llenar todo el formulario");
                }
                return View("Marketing", model);
            }

            var clienteExistente = _context.ClientesMarketing.FirstOrDefault(c => c.Id == model.Id);
            if (clienteExistente == null)
            {
                TempData["Error"] = "No se encontró el cliente a editar.";
                return RedirectToAction("Marketing");
            }

            clienteExistente.Nombre = model.Nombre;
            clienteExistente.Correo = model.Correo;
            clienteExistente.Telefono = model.Telefono;

            _context.SaveChanges();

            TempData["Mensaje"] = "Cliente editado correctamente.";
            return RedirectToAction("Marketing");
        }
        [HttpPost]
        public IActionResult BuscarCliente(string nombreCliente)
        {
            if (string.IsNullOrWhiteSpace(nombreCliente))
            {
                TempData["Error"] = "Se necesita llenar datos";
                return RedirectToAction("Marketing");
            }

            var cliente = _context.ClientesMarketing.FirstOrDefault(c => c.Nombre == nombreCliente);

            if (cliente == null)
            {
                TempData["Error"] = "No hay un registro de cliente con ese nombre";
                return RedirectToAction("Marketing");
            }

            ViewBag.ClienteEncontrado = cliente;
            return View("Marketing");
        }
        [HttpPost]
        public IActionResult EliminarCliente(string nombreCliente, string confirmar)
        {
            var cliente = _context.ClientesMarketing.FirstOrDefault(c => c.Nombre == nombreCliente);

            if (cliente == null)
            {
                TempData["Error"] = "No hay un registro de cliente con ese nombre";
                return RedirectToAction("Marketing");
            }

            if (confirmar == "si")
            {
                _context.ClientesMarketing.Remove(cliente);
                _context.SaveChanges();
                TempData["Mensaje"] = "Registro de cliente eliminado exitosamente";
            }
            else
            {
                TempData["Mensaje"] = "Registro de cliente no eliminado";
            }

            return RedirectToAction("Marketing");
        }

    }
}
