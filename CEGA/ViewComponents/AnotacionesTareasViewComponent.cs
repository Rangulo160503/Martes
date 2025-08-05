using CEGA.Data;
using CEGA.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CEGA.ViewComponents
{
    public class AnotacionesTareasViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnotacionesTareasViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int planoId)
        {
            var comentarios = _context.ComentariosPlano
                .Where(c => c.PlanoId == planoId)
                .ToList();

            var tareas = _context.TareasPlano
                .Where(t => t.PlanoId == planoId)
                .ToList();

            var viewModel = new VerPlanoViewModel
            {
                Comentarios = comentarios,
                Tareas = tareas
            };

            return View(viewModel);
        }
    }
}
