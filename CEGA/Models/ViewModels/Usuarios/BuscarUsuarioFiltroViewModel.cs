using System.ComponentModel.DataAnnotations;

namespace CEGA.Models.ViewModels.Usuarios
{
    public class BuscarUsuarioFiltroViewModel         // 5
    {                                                 // 6
        [Display(Name = "Buscar")]                    // 7
        public string? Query { get; set; }            // 8

        [Display(Name = "Rol")]                       // 9
        public string? Rol { get; set; }              //10

        public IEnumerable<UsuarioListaVM>            //11
            UsuariosFiltrados
        { get; set; }           //12
                = new List<UsuarioListaVM>();         //13

        public IEnumerable<string>                    //14
            RolesDisponibles
        { get; set; }            //15
                = new List<string>();                 //16
    }
}
