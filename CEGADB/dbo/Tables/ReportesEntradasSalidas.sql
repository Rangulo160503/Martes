CREATE TABLE [dbo].[ReportesEntradasSalidas] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [NombreReporte]   NVARCHAR (MAX) NOT NULL,
    [RolSeleccionado] NVARCHAR (MAX) NOT NULL,
    [FechaInicio]     DATETIME2 (7)  NOT NULL,
    [FechaFin]        DATETIME2 (7)  NOT NULL,
    [Movimiento]      NVARCHAR (MAX) NOT NULL,
    [FechaCreacion]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_ReportesEntradasSalidas] PRIMARY KEY CLUSTERED ([Id] ASC)
);

