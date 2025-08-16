namespace CEGA.Models.ViewModels
{
    public class ReporteFinancieroViewModel
    {
        public PeriodoReporte Periodo { get; set; } = PeriodoReporte.Rango;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<Ingreso> Ingresos { get; set; } = new();
        public List<Egreso> Egresos { get; set; } = new();

        public decimal TotalIngresos => Ingresos.Sum(i => i.Monto);
        public decimal TotalEgresos => Egresos.Sum(e => e.Monto);
        public decimal Balance => TotalIngresos - TotalEgresos;
    }

}
