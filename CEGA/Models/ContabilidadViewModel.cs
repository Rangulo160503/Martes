using System.Collections.Generic;

namespace CEGA.Models
{
    public class ContabilidadViewModel
    {
        public Ingreso NuevoIngreso { get; set; } = new();
        public Egreso NuevoEgreso { get; set; } = new();

        public List<Ingreso> ListaIngresos { get; set; } = new();
        public List<Egreso> ListaEgresos { get; set; } = new();
    }

}
