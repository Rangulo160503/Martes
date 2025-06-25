using System.Collections.Generic;

namespace CEGA.Models
{
    public class IngresoViewModel
    {
        public Ingreso NuevoIngreso { get; set; } = new Ingreso();
        public List<Ingreso> ListaIngresos { get; set; } = new();
    }
}
