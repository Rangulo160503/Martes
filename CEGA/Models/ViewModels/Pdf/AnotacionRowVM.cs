namespace CEGA.Models.ViewModels.Pdf
{
    public class AnotacionRowVM
    {
        public int IdAnotacion { get; set; }
        public int IdPdf { get; set; }
        public int Cedula { get; set; }      // requerido según tu modelo
        public string? Texto { get; set; }
    }
}
