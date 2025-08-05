CREATE TABLE [dbo].[ReportesIncidentes] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [NombreReporte]  NVARCHAR (MAX) NOT NULL,
    [UsuarioID]      NVARCHAR (MAX) NOT NULL,
    [FechaAccidente] DATETIME2 (7)  NOT NULL,
    [Descripcion]    NVARCHAR (MAX) NOT NULL,
    [Incapacidad]    NVARCHAR (MAX) NOT NULL,
    [FechaCreacion]  DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_ReportesIncidentes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

