CREATE TABLE [dbo].[ReportesIncapacidades] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [NombreReporte]   NVARCHAR (MAX) NOT NULL,
    [RolSeleccionado] NVARCHAR (MAX) NOT NULL,
    [FechaInicio]     DATETIME2 (7)  NOT NULL,
    [FechaFin]        DATETIME2 (7)  NOT NULL,
    [FechaCreacion]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_ReportesIncapacidades] PRIMARY KEY CLUSTERED ([Id] ASC)
);

