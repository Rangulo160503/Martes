CREATE TABLE [dbo].[Accidentes] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [ProyectoId]     INT           NULL,
    [CedulaEmpleado] INT           NOT NULL,
    [Fecha]          DATETIME2 (0) CONSTRAINT [DF_Accidentes_Fecha] DEFAULT (sysdatetime()) NOT NULL,
    [ModificadoEn]   DATETIME2 (0) NULL,
    CONSTRAINT [PK_Accidentes_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Accidentes_Empleado_Cedula] FOREIGN KEY ([CedulaEmpleado]) REFERENCES [dbo].[Empleado] ([Cedula]) ON DELETE CASCADE,
    CONSTRAINT [FK_Accidentes_Proyecto_IdProyecto] FOREIGN KEY ([ProyectoId]) REFERENCES [dbo].[Proyecto] ([IdProyecto]) ON DELETE SET NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Accidentes_Fecha]
    ON [dbo].[Accidentes]([Fecha] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Accidentes_CedulaEmpleado]
    ON [dbo].[Accidentes]([CedulaEmpleado] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Accidentes_ProyectoId]
    ON [dbo].[Accidentes]([ProyectoId] ASC);

