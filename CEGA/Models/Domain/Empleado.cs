namespace CEGA.Models.Domain
{
    public class Empleado
    {
        // PK: cédula CR (9 dígitos, sólo números)
        public int Cedula { get; set; }

        // Nombre
        public string Nombre { get; set; } = "";
        public string? SegundoNombre { get; set; }
        public string Apellido1 { get; set; } = "";
        public string? Apellido2 { get; set; }

        // Reglas: minúsculas, sin tildes/ñ; UNIQUE
        public string Username { get; set; } = "";

        // Contacto (Email UNIQUE)
        public string Email { get; set; } = "";
        public string TelefonoPersonal { get; set; } = "";   // 8 dígitos (UI agrega +506)
        public string TelefonoEmergencia { get; set; } = ""; // 8 dígitos (UI agrega +506)

        // Perfil
        public string Sexo { get; set; } = "M";  // M/F/X/Prefiero no decir
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaIngreso { get; set; } // ≥ FechaNacimiento + 18 años

        // Médicos / emergencia (opcionales)
        public string? TipoSangre { get; set; }
        public string? Alergias { get; set; }
        public string? ContactoEmergenciaNombre { get; set; }
        public string? ContactoEmergenciaTelefono { get; set; }
        public string? PolizaSeguro { get; set; }

        // FK a Puesto (obligatorio)
        public int PuestoId { get; set; }

        // Vínculo 1:1 con AspNetUsers (opcional pero recomendado)
        public string? AspNetUserId { get; set; }
    }
}
