CREATE TABLE [dbo].[Tareas] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [ProyectoId]        INT             NULL,
    [CedulaEmpleado]    INT             NOT NULL,
    [Titulo]            NVARCHAR (120)  NOT NULL,
    [ComentarioInicial] NVARCHAR (1000) NULL,
    [CreadoEn]          DATETIME2 (0)   CONSTRAINT [DF_Tareas_CreadoEn] DEFAULT (sysdatetime()) NOT NULL,
    [ModificadoEn]      DATETIME2 (0)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tareas_Empleado_Cedula] FOREIGN KEY ([CedulaEmpleado]) REFERENCES [dbo].[Empleado] ([Cedula]) ON DELETE CASCADE,
    CONSTRAINT [FK_Tareas_Proyecto_IdProyecto] FOREIGN KEY ([ProyectoId]) REFERENCES [dbo].[Proyecto] ([IdProyecto]) ON DELETE SET NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Tareas_ProyectoId]
    ON [dbo].[Tareas]([ProyectoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tareas_CedulaEmpleado]
    ON [dbo].[Tareas]([CedulaEmpleado] ASC);

