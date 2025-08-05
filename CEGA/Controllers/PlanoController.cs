using CEGA.Data;
using CEGA.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

public class PlanoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public PlanoController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: SubirPlano
    public IActionResult SubirPlano()
    {
        return View();
    }

    // POST: SubirPlano
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubirPlano(Plano plano, IFormFile archivoPlano)
    {
        if (archivoPlano == null || Path.GetExtension(archivoPlano.FileName).ToLower() != ".pdf")
        {
            ModelState.AddModelError("archivoPlano", "Solo se permiten archivos PDF");
        }

        if (!ModelState.IsValid)
        {
            return View(plano);
        }

        string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoPlano.FileName);
        string rutaCarpeta = Path.Combine(_env.WebRootPath, "docs", "planos");
        Directory.CreateDirectory(rutaCarpeta);

        string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
        {
            await archivoPlano.CopyToAsync(stream);
        }

        plano.NombreArchivo = archivoPlano.FileName;

        _context.Add(plano);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Plano subido correctamente";
        return RedirectToAction("ListaPlanos");
    }

    // GET: Lista de planos
    public async Task<IActionResult> ListaPlanos()
    {
        var planos = await _context.Planos.ToListAsync();
        return View(planos);
    }
}
