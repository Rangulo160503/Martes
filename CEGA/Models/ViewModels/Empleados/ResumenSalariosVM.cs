namespace CEGA.Models.ViewModels.Empleados
{
    public class ResumenSalariosVM
    {
        public int Cedula { get; set; }
        public string NombreCompleto { get; set; } = "";
        public bool Activo { get; set; }

        public string PuestoNombre { get; set; } = "(sin puesto)";
        public decimal? SalarioBase { get; set; }

        public int VacacionesTomadas { get; set; }   // conteo total de registros en VacacionesEmpleado
        public bool TieneIncapacidad { get; set; }   // true si existe registro en Incapacidad
    }
}
