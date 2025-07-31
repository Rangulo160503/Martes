CREATE TABLE [dbo].[ComentariosProyecto] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [ProyectoId]       INT            NOT NULL,
    [NombreComentario] NVARCHAR (MAX) NOT NULL,
    [Detalles]         NVARCHAR (MAX) NOT NULL,
    [FechaCreacion]    DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_ComentariosProyecto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ComentariosProyecto_Proyectos_ProyectoId] FOREIGN KEY ([ProyectoId]) REFERENCES [dbo].[Proyectos] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ComentariosProyecto_ProyectoId]
    ON [dbo].[ComentariosProyecto]([ProyectoId] ASC);

