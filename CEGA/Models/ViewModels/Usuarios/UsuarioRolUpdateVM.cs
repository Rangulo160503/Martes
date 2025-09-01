namespace CEGA.Models.ViewModels.Usuarios
{
    public class UsuarioRolUpdateVM
    {
        public int Cedula { get; set; }      // PK del empleado
        public byte Rol { get; set; }        // tinyint en BD
    }
}
