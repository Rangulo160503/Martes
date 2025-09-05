namespace CEGA.Models.ViewModels
{
    public class ConteoPorEmpleadoItem
    {
        public int Cedula { get; set; }
        public string NombreCompleto { get; set; } = "";
        public int Cantidad { get; set; }
    }

    public class IncapacidadesPorEmpleadoVM
    {
        public string Titulo { get; set; } = "Incapacidades por empleado (temporal)";
        public IEnumerable<ConteoPorEmpleadoItem> Items { get; set; } = Enumerable.Empty<ConteoPorEmpleadoItem>();
        public int Total { get; set; }
    }
}
