using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class CierreDiario
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public decimal TotalIngresos { get; set; }

        [Required]
        public decimal TotalEgresos { get; set; }

        [Required]
        public decimal BalanceFinal { get; set; }
    }
}
