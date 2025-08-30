using CEGA.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEGA.Models
{
    [Table("EMPLEADO")]
    public class Empleado
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Range(100000000, 999999999)]
        public int Cedula { get; set; }

        [Required, MaxLength(50)] public string Nombre { get; set; } = "";
        [MaxLength(50)] public string? SegundoNombre { get; set; }
        [Required, MaxLength(50)] public string Apellido1 { get; set; } = "";
        [MaxLength(50)] public string? Apellido2 { get; set; }

        [Required, MaxLength(50)] public string Username { get; set; } = ""; // login
        [Required, MaxLength(256), EmailAddress] public string Email { get; set; } = ""; // login
        [Required, MaxLength(100)] public string PasswordHash { get; set; } = ""; // bcrypt (~60) - login
        public bool Activo { get; set; } = true;

        // Rol de aplicación (no confundir con Puesto/cargo)
        [Column(TypeName = "tinyint")]
        public RolEmpleado Rol { get; set; } = RolEmpleado.AdminSistema;

        // Recuperación por email
        [MaxLength(200)] public string? ResetTokenHash { get; set; }
        public DateTime? ResetTokenExpiraEn { get; set; }

        // Contacto
        [Required, MaxLength(8)]
        [RegularExpression(@"^\d{8}$")] public string TelefonoPersonal { get; set; } = "";
        [Required, MaxLength(8)]
        [RegularExpression(@"^\d{8}$")] public string TelefonoEmergencia { get; set; } = "";

        // Perfil
        [Required, MaxLength(20)] public string Sexo { get; set; } = "M";
        [Required, DataType(DataType.Date)] public DateTime FechaNacimiento { get; set; }
        [Required, DataType(DataType.Date)] public DateTime FechaIngreso { get; set; }

        // Médicos / emergencia (opcionales)
        [MaxLength(3)] public string? TipoSangre { get; set; }
        [MaxLength(200)] public string? Alergias { get; set; }
        [MaxLength(100)] public string? ContactoEmergenciaNombre { get; set; }
        [MaxLength(8)]
        [RegularExpression(@"^\d{8}$")] public string? ContactoEmergenciaTelefono { get; set; }
        [MaxLength(50)] public string? PolizaSeguro { get; set; }

        // Puesto/cargo (NULL en BD, pero obligatorio en la UI al guardar)
        public int? PuestoId { get; set; }
        [ForeignKey(nameof(PuestoId))] public Puesto? Puesto { get; set; }
    }

    public enum RolEmpleado : byte { AdminSistema = 1, RRHH = 2, Supervisor = 3, Colaborador = 4 }
}
