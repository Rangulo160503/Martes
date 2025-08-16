using System;
using System.Collections.Generic;
using System.Linq;

namespace CEGA.Models
{
    public class CierreDiarioViewModel
    {
        public DateTime Fecha { get; set; }
        public List<Ingreso> Ingresos { get; set; } = new();
        public List<Egreso> Egresos { get; set; } = new();

        public decimal TotalIngresos => Ingresos.Sum(x => x.Monto);
        public decimal TotalEgresos => Egresos.Sum(x => x.Monto);
        public decimal Balance => TotalIngresos - TotalEgresos;
    }
}
