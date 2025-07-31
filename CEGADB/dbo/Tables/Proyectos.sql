CREATE TABLE [dbo].[Proyectos] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (MAX) NOT NULL,
    [Detalles]    NVARCHAR (MAX) NOT NULL,
    [FechaInicio] DATETIME2 (7)  NOT NULL,
    [FechaFin]    DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Proyectos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

