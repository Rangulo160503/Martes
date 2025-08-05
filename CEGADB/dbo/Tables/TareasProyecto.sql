CREATE TABLE [dbo].[TareasProyecto] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ProyectoId]  INT            NOT NULL,
    [NombreTarea] NVARCHAR (MAX) NOT NULL,
    [Detalles]    NVARCHAR (MAX) NOT NULL,
    [FechaInicio] DATETIME2 (7)  NOT NULL,
    [FechaFin]    DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_TareasProyecto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TareasProyecto_Proyectos_ProyectoId] FOREIGN KEY ([ProyectoId]) REFERENCES [dbo].[Proyectos] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TareasProyecto_ProyectoId]
    ON [dbo].[TareasProyecto]([ProyectoId] ASC);

