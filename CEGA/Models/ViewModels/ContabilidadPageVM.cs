namespace CEGA.Models.ViewModels
{
    public class ContabilidadPageVM
    {
        public ResumenFinanciero Resumen { get; set; } = new();

        public IEnumerable<MovimientoContable> Ingresos { get; set; } = Enumerable.Empty<MovimientoContable>();
        public IEnumerable<MovimientoContable> Gastos { get; set; } = Enumerable.Empty<MovimientoContable>();
        public IEnumerable<CierreDiarioVM> Cierres { get; set; } = Enumerable.Empty<CierreDiarioVM>();
    }

    public class ResumenFinanciero
    {
        public decimal SaldoTotal { get; set; }
        public decimal IngresosMes { get; set; }
        public decimal GastosMes { get; set; }
        public decimal VariacionMes => IngresosMes - GastosMes;
    }

    public class MovimientoContable
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty; // Ej: "CR - Datafonos"
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = "CRC";
        public bool EsFijo { get; set; }
    }

    public class CierreDiarioVM
    {
        public DateTime Fecha { get; set; }
        public decimal TotalIngresos { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal SaldoDia { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
