using System.Collections.Generic;
using System.Linq;

namespace CEGA.Models.ViewModels
{
    // Item de conteo para accidentes agrupados por empleado
    public class ConteoAccidentePorEmpleadoItem
    {
        public int Cedula { get; set; }
        public string NombreCompleto { get; set; } = "";
        public int Cantidad { get; set; }
    }

    // ViewModel de pantalla para accidentes por empleado
    public class AccidentesPorEmpleadoVM
    {
        public string Titulo { get; set; } = "Accidentes por empleado";
        public IEnumerable<ConteoAccidentePorEmpleadoItem> Items { get; set; } = Enumerable.Empty<ConteoAccidentePorEmpleadoItem>();
        public int Total { get; set; }
    }
}
