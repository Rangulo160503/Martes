using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class HistorialPuestos
    {
        public int Id { get; set; }

        public string UsuarioID { get; set; }

        public string PuestoAnterior { get; set; }

        public string PuestoNuevo { get; set; }

        public DateTime FechaCambio { get; set; } = DateTime.Now;

        public decimal SalarioAnterior { get; set; }

        public decimal SalarioNuevo { get; set; }
    }
}
