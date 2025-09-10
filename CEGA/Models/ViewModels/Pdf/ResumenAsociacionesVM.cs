// CEGA/Models/ViewModels/Pdf/ResumenAsociacionesVM.cs
using System.Collections.Generic;

namespace CEGA.Models.ViewModels.Pdf
{
    public class ResumenAsociacionesVM
    {
        public int PdfId { get; set; }
        public string? PdfNombre { get; set; }

        public int TotalProyectos { get; set; }
        public int TotalTareas { get; set; }
        public int TotalAnotaciones { get; set; }

        public IEnumerable<string> Proyectos { get; set; } = new List<string>();
        public IEnumerable<string> Tareas { get; set; } = new List<string>();
        public IEnumerable<string> Anotaciones { get; set; } = new List<string>();
    }
}
