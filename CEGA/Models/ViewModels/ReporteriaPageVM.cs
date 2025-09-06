namespace CEGA.Models.ViewModels
{
    public class ReporteriaPageVM
    {
        public IncapacidadesPorEmpleadoVM Incapacidades { get; set; } = new();
        public AccidentesPorEmpleadoVM Accidentes { get; set; } = new();
    }
}
