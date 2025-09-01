# 📚 Modelo de Datos – CEGA

Este proyecto contiene la base de datos y módulos para la gestión de CEGA.  
A continuación se muestra el diagrama entidad–relación (ERD) en **Mermaid**:

```mermaid
erDiagram
    %% ======================
    %%   MÓDULO DE EMPLEADOS
    %% ======================
      EMPLEADO {
        int      Cedula PK
        string   Nombre
        string   SegundoNombre
        string   Apellido1
        string   Apellido2
        string   Username UK
        string   Email UK
        string   PasswordHash
        boolean  Activo
        string   TelefonoPersonal
        string   TelefonoEmergencia
        string   Sexo
        date     FechaNacimiento
        date     FechaIngreso
        string   TipoSangre
        string   Alergias
        string   ContactoEmergenciaNombre
        string   ContactoEmergenciaTelefono
        string   PolizaSeguro
        int      PuestoId FK
        tinyint  Rol
        string   ResetTokenHash
        datetime ResetTokenExpiraEn
    }

    PUESTO {
        int     Id PK
        string  Nombre
        boolean Activo
        string  Codigo
        string  Departamento
        string  Descripcion
        string  Requisitos
        string  Nivel
        decimal SalarioBase
        string  Moneda
        string  Jornada
    }

    SALARIO {
        int    Id PK
        int    EmpleadoCedula FK "→ EMPLEADO.Cedula"
        int    PuestoId FK "Snapshot del puesto"
        int    TipoSalario "1=Mensual, 2=Hora, 3=Quincenal, 4=Semanal"
        decimal MontoBase "CRC"
        string Moneda "CRC"
        int    HorasPorSemana
        date   FechaInicio
        date   FechaFin "NULL = vigente"
    }

    VACACION {
        int      Id PK
        int      EmpleadoCedula FK "→ EMPLEADO.Cedula"
        int      PuestoId FK "Snapshot del puesto"
        int      Estado "0=Solicitada,1=Aprobada,2=Rechazada"
        datetime FechaSolicitud
        datetime FechaAprobacion
        date     FechaInicio "Inclusiva"
        date     FechaFin "Inclusiva"
    }

    INCAPACIDAD {
        int     EmpleadoCedula PK, FK "→ EMPLEADO.Cedula"
        string  CodigoIncapacidad PK "Boleta/código"
        int     PuestoId FK "Snapshot del puesto"
        int     Tipo "1=Enfermedad,2=Accidente,3=Maternidad,4=Otro"
        int     Estado "0=Registrada,1=Validada,2=Pagada,3=Rechazada"
        int     Emisor "1=CCSS,2=INS,3=Privado"
        date    FechaInicio "Inclusiva"
        date    FechaFin "Inclusiva"
        decimal Dias "1 decimal (opcional)"
        datetime FechaRegistro
        int     UsuarioRegistroCedula FK "→ EMPLEADO.Cedula"
        datetime FechaValidacion
        int     ValidadoPorCedula FK "→ EMPLEADO.Cedula"
        string  Notas
    }

    %% Relaciones (Empleados)
    PUESTO ||--o{ EMPLEADO : "asignado a"
    EMPLEADO ||--o{ SALARIO     : "tiene"
    PUESTO   ||--o{ SALARIO     : "aplica a"
    EMPLEADO ||--o{ VACACION    : "solicita"
    PUESTO   ||--o{ VACACION    : "aplica a"
    EMPLEADO ||--o{ INCAPACIDAD : "tiene"
    PUESTO   ||--o{ INCAPACIDAD : "aplica a"

    %% ===================
    %%   MÓDULO MARKETING
    %% ===================
    CLIENTE {
        int    Id PK
        string Nombre
        string Email UK "Único"
    }

    POOL {
        int      Id PK
        string   Nombre UK
        string   FiltroTexto "Palabras OR; busca en Nombre/Email"
        datetime FechaCreacion
        boolean  Snapshot "true: no se edita; borrar y recrear"
    }

    POOL_CLIENTE {
        int PoolId   FK
        int ClienteId FK
        %% PK compuesta (PoolId, ClienteId)
    }

    CAMPANIA {
        int      Id PK
        string   Nombre UK
        string   Asunto
        string   Cuerpo "Texto plano"
        boolean  Deduplicar "true por defecto"
        int      Estado "0=Borrador,1=Enviada"
        datetime FechaCreacion
        int      TotalDestinatarios
    }

    CAMPANIA_POOL {
        int CampaniaId FK
        int PoolId     FK
        %% PK compuesta (CampaniaId, PoolId)
    }

    CAMPANIA_CLIENTE {
        int      CampaniaId FK
        int      ClienteId  FK
        int      Estado "0=Pendiente,1=Enviado,2=Fallido"
        datetime FechaEnvio
        string   Error
        %% PK compuesta (CampaniaId, ClienteId)
    }

    %% Relaciones (Marketing)
    POOL        ||--o{ POOL_CLIENTE      : "incluye"
    CLIENTE     ||--o{ POOL_CLIENTE      : "pertenece a"
    CAMPANIA    ||--o{ CAMPANIA_POOL     : "usa"
    POOL        ||--o{ CAMPANIA_POOL     : "es parte de"
    CAMPANIA    ||--o{ CAMPANIA_CLIENTE  : "genera envíos a"
    CLIENTE     ||--o{ CAMPANIA_CLIENTE  : "recibe"

    %% ======================
    %%   MÓDULO CONTABILIDAD
    %% ======================
    INGRESO {
        int    Id PK
        date   Fecha "día contable"
        decimal Monto "CRC"
        string Descripcion
        int    CierreId FK "NULL hasta cerrar"
    }

    EGRESO {
        int    Id PK
        date   Fecha "día contable"
        decimal Monto "CRC"
        string Descripcion
        int    CierreId FK "NULL hasta cerrar"
    }

    CIERRE_DIARIO {
        int      Id PK
        date     Fecha UK "un cierre por día"
        decimal  TotalIngresos "snapshot"
        decimal  TotalEgresos "snapshot"
        decimal  Balance "Ingresos - Egresos"
        datetime FechaRegistro "al presionar 'Registrar cierre hoy'"
    }

    %% Relaciones (Contabilidad)
    CIERRE_DIARIO ||--o{ INGRESO : "contiene (snapshot)"
    CIERRE_DIARIO ||--o{ EGRESO  : "contiene (snapshot)"

    %% ===========================
    %%   MÓDULO REPORTERÍA/OBRAS
    %% ===========================
    PROYECTO {
        int     Id PK
        string  Codigo UK "corto"
        string  Nombre UK
        string  Descripcion
        boolean Activo
        string  Ubicacion
        int     Estado "0=Abierto,1=Cerrado"
        date    FechaInicio
        date    FechaFin
        int     ResponsableCedula FK "→ EMPLEADO.Cedula"
    }

    ACCIDENTE {
        int      Id PK
        int      ProyectoId FK "→ PROYECTO.Id"
        int      EmpleadoCedula FK "Lesionado → EMPLEADO.Cedula"
        datetime FechaHora
        int      Gravedad "1=Leve,2=Moderada,3=Grave"
        string   Descripcion
        string   Ubicacion
        datetime FechaRegistro
        int      ReportadoPorCedula FK "→ EMPLEADO.Cedula"
        int      Estado "0=Registrado,1=Cerrado"
        string   Notas
    }

    %% Relaciones (Reportería/Obras)
    PROYECTO ||--o{ ACCIDENTE : "tiene"
    EMPLEADO ||--o{ ACCIDENTE : "lesionado"
    EMPLEADO ||--o{ ACCIDENTE : "reportado por"

    %% ======================
    %%   MÓDULO SEGUIMIENTO
    %% ======================
    TAREA {
        int    Id PK
        int    ProyectoId FK "→ PROYECTO.Id (obligatorio)"
        int    AsignadoCedula FK "→ EMPLEADO.Cedula (único asignatario)"
        string Titulo "obligatorio"
        string Descripcion "opcional"
        int    Estado "0=Pendiente,1=Hecha"
    }

    TAREA_COMENTARIO {
        int    Id PK
        int    TareaId FK "→ TAREA.Id"
        string Texto "solo texto"
    }

    %% Relaciones (Seguimiento)
    PROYECTO ||--o{ TAREA            : "tiene"
    EMPLEADO ||--o{ TAREA            : "asignado"
    TAREA    ||--o{ TAREA_COMENTARIO : "comentarios"

    %% =================
    %%   MÓDULO DE PDF
    %% =================
    PDF {
        int      Id PK
        int      ProyectoId FK "→ PROYECTO.Id (PDF asociado al proyecto)"
        string   Nombre "Etiqueta o título"
        string   ArchivoNombre "nombre original"
        string   ArchivoRuta "ruta/URL de almacenamiento"
        int      TamanoBytes
        datetime FechaSubida
        int      SubidoPorCedula FK "→ EMPLEADO.Cedula"
    }

    PDF_ANOTACION {
        int      Id PK
        int      PdfId FK "→ PDF.Id"
        int      AutorCedula FK "→ EMPLEADO.Cedula"
        int      Pagina "opcional"
        decimal  PosX "opcional"
        decimal  PosY "opcional"
        string   Texto "anotación"
        datetime Fecha
    }

    TAREA_PDF {
        int TareaId FK "→ TAREA.Id"
        int PdfId   FK "→ PDF.Id"
        %% PK compuesta (TareaId, PdfId)
    }

    %% Relaciones (PDF)
    PROYECTO ||--o{ PDF           : "tiene"
    PDF      ||--o{ PDF_ANOTACION : "tiene"
    TAREA    ||--o{ TAREA_PDF     : "usa"
    PDF      ||--o{ TAREA_PDF     : "referenciado por"

    %% =========================
    %%   MÓDULO USUARIOS / PERFIL
    %% =========================
    USUARIO {
        int Cedula PK, FK "→ EMPLEADO.Cedula (1:1)"
        boolean Activo "soft enable/disable"
        %% Sin PasswordHash (auth delegada)
    }

    ROL {
        int    Id PK
        string Nombre UK "AdminSistema, RRHH, Supervisor, Colaborador"
        boolean Activo
    }

    USUARIO_ROL {
        int Cedula FK "→ USUARIO.Cedula"
        int RolId  FK "→ ROL.Id"
        %% PK compuesta (Cedula, RolId)
    }

    %% Relaciones (Usuarios)
    EMPLEADO ||--|| USUARIO     : "perfil de acceso"
    USUARIO  ||--o{ USUARIO_ROL : "tiene"
    ROL      ||--o{ USUARIO_ROL : "asignado"

    %% Reglas (comentarios)
    %% USUARIOS/PERFIL:
    %%  - Login por Username (EMPLEADO.Username) o Email (EMPLEADO.Email).
    %%  - Edición de perfil por el usuario: Email y TeléfonoPersonal en EMPLEADO.
    %%  - Username y Nombres/Apellidos: solo RRHH.
    %%  - Al 'inactivar' EMPLEADO -> USUARIO.Activo=false. Al borrar EMPLEADO -> borrado en cascada de USUARIO/USUARIO_ROL.
    %%  - Roles simples: AdminSistema, RRHH, Supervisor, Colaborador.
