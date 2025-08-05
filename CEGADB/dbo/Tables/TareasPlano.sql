CREATE TABLE [dbo].[TareasPlano] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [PlanoId]     INT            NOT NULL,
    [Descripcion] NVARCHAR (MAX) NOT NULL,
    [Fecha]       DATETIME2 (7)  NOT NULL,
    [Responsable] NVARCHAR (MAX) NOT NULL,
    [CoordenadaX] REAL           NOT NULL,
    [CoordenadaY] REAL           NOT NULL,
    CONSTRAINT [PK_TareasPlano] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TareasPlano_Planos_PlanoId] FOREIGN KEY ([PlanoId]) REFERENCES [dbo].[Planos] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TareasPlano_PlanoId]
    ON [dbo].[TareasPlano]([PlanoId] ASC);

