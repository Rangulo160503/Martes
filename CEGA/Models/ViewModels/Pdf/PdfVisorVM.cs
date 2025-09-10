namespace CEGA.Models.ViewModels.Pdf
{
    public class PdfVisorVM
    {
        public int IdPdf { get; set; }
        public string Titulo { get; set; } = "";
        public string VerUrl { get; set; } = "";       // /Pdf/Archivo/123
        public string DescargarUrl { get; set; } = ""; // /Pdf/Descargar/123
    }
}
