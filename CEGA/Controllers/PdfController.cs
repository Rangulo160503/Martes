using CEGA.Data;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
            return View();
        }

        // 1) GET: renderiza el partial de “Subir PDF”
        [HttpGet]
        public IActionResult SubirPartial(int? cedula = null)
        {
            var vm = new PdfUploadVM { Cedula = cedula ?? 0 };
            return PartialView("~/Views/Pdf/Partials/_SubirPdfPartial.cshtml", vm);
        }
    }
}
