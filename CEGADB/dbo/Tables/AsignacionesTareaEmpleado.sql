CREATE TABLE [dbo].[AsignacionesTareaEmpleado] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [TareaId]   INT            NOT NULL,
    [UsuarioId] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AsignacionesTareaEmpleado] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AsignacionesTareaEmpleado_AspNetUsers_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AsignacionesTareaEmpleado_TareasProyecto_TareaId] FOREIGN KEY ([TareaId]) REFERENCES [dbo].[TareasProyecto] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_AsignacionesTareaEmpleado_UsuarioId]
    ON [dbo].[AsignacionesTareaEmpleado]([UsuarioId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AsignacionesTareaEmpleado_TareaId]
    ON [dbo].[AsignacionesTareaEmpleado]([TareaId] ASC);

