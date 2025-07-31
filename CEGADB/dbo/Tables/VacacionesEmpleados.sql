CREATE TABLE [dbo].[VacacionesEmpleados] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioID]       NVARCHAR (MAX) NOT NULL,
    [FechaInicio]     DATETIME2 (7)  NOT NULL,
    [DiasSolicitados] INT            NOT NULL,
    [DiasDisponibles] INT            NOT NULL,
    [Estado]          NVARCHAR (MAX) NOT NULL,
    [FechaSolicitud]  DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_VacacionesEmpleados] PRIMARY KEY CLUSTERED ([Id] ASC)
);

