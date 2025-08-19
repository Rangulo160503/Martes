CREATE TABLE [dbo].[IncapacidadEmpleado] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [UsuarioID]         NVARCHAR (MAX)  NOT NULL,
    [Descripcion]       NVARCHAR (MAX)  NOT NULL,
    [ArchivoRuta]       NVARCHAR (MAX)  NULL,
    [FechaPresentacion] DATETIME2 (7)   NULL,
    [Estado]            NVARCHAR (MAX)  NULL,
    [ArchivoContenido]  VARBINARY (MAX) NULL,
    [ArchivoNombre]     NVARCHAR (260)  NULL,
    [ArchivoTipo]       NVARCHAR (100)  NULL,
    [ArchivoTamano]     BIGINT          NULL,
    CONSTRAINT [PK_IncapacidadEmpleado] PRIMARY KEY CLUSTERED ([Id] ASC)
);

