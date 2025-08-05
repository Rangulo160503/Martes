using System;
using System.ComponentModel.DataAnnotations;

namespace CEGA.Models
{
    public class CierreRango
    {
        public int Id { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public decimal TotalIngresos { get; set; }

        public decimal TotalEgresos { get; set; }

        public decimal BalanceFinal { get; set; }
    }
}
