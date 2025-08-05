using System;
using System.Collections.Generic;

namespace CEGA.Models
{
    public class ReporteFinancieroViewModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<Ingreso> Ingresos { get; set; } = new();
        public List<Egreso> Egresos { get; set; } = new();
    }
}
