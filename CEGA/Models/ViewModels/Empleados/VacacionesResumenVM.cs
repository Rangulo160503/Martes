namespace CEGA.Models.ViewModels.Empleados
{
    public class VacacionesResumenVM
    {
        public int Cedula { get; set; }
        public string NombreCompleto { get; set; } = "";
        public DateTime FechaIngreso { get; set; }

        public DateTime CicloInicio { get; set; }
        public DateTime CicloFin { get; set; }

        public int DiasAnuales { get; set; }          // p.ej. 12
        public int DiasAcumuladosAlDia { get; set; }  // prorrata a hoy (opcional)
        public int DiasTomadosEnCiclo { get; set; }   // suma en [CicloInicio, CicloFin]
        public int DiasDisponibles { get; set; }
    }
}
