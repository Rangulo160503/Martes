using System.Collections.Generic;

namespace CEGA.Models
{
    public class EgresoViewModel
    {
        public Egreso NuevoEgreso { get; set; } = new();
        public List<Egreso> ListaEgresos { get; set; } = new();
    }
}
