namespace CEGA.Models.ViewModels
{
    public class ContabilidadPageVM
    {
        public ResumenFinanciero Resumen { get; set; } = new();
        public IEnumerable<MovimientoContable> Ingresos { get; set; } = Enumerable.Empty<MovimientoContable>();
        public IEnumerable<MovimientoContable> Gastos { get; set; } = Enumerable.Empty<MovimientoContable>();
        public IEnumerable<CuentaBancaria> Cuentas { get; set; } = Enumerable.Empty<CuentaBancaria>();
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
        public string Concepto { get; set; } = "";
        public string Categoria { get; set; } = ""; // p.ej. “CR - Datafonos”, “Ventas”, “Servicios”
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = "CRC";
        public bool EsFijo { get; set; } // true para “CR - Datafonos”
    }

    public class CuentaBancaria
    {
        public int Id { get; set; }
        public string Banco { get; set; } = "";
        public string Numero { get; set; } = "";
        public decimal Saldo { get; set; }
        public string Moneda { get; set; } = "CRC";
        public DateTime ActualizadoEn { get; set; }
    }
}
