namespace CEGA.Models.ViewModels
{
    public class CierreItem
    {
        public DateTime Fecha { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalEgresos { get; set; }
        public decimal BalanceFinal => TotalIngresos - TotalEgresos;
    }

    public class CierresRangoViewModel
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        // Lista de resultados para mostrar en la tabla
        public List<CierreItem> Cierres { get; set; } = new();
    }
}
