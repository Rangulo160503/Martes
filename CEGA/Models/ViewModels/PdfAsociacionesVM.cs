namespace CEGA.Models.ViewModels
{
    public class PdfAsociacionesVM
    {
        public int IdPdf { get; set; }

        // Listas para mostrar en el partial
        public List<Item> ProyectosAsociados { get; set; } = new();
        public List<Item> ProyectosDisponibles { get; set; } = new();
        public List<Item> TareasAsociadas { get; set; } = new();
        public List<Item> TareasDisponibles { get; set; } = new();

        // Selecciones que enviará el formulario
        public int[] ProyectosAgregarIds { get; set; } = Array.Empty<int>();
        public int[] ProyectosQuitarIds { get; set; } = Array.Empty<int>();
        public int[] TareasAgregarIds { get; set; } = Array.Empty<int>();
        public int[] TareasQuitarIds { get; set; } = Array.Empty<int>();
        public class Item
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = string.Empty; // para Proyecto: Nombre; para Tarea: Titulo
        }
    }
}
