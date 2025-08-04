CREATE TABLE [dbo].[ComentariosPlano] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [PlanoId]       INT            NOT NULL,
    [Texto]         NVARCHAR (MAX) NOT NULL,
    [CoordenadaX]   REAL           DEFAULT (CONVERT([real],(0))) NOT NULL,
    [CoordenadaY]   REAL           DEFAULT (CONVERT([real],(0))) NOT NULL,
    [FechaCreacion] DATETIME2 (7)  DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    CONSTRAINT [PK_ComentariosPlano] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ComentariosPlano_Planos_PlanoId] FOREIGN KEY ([PlanoId]) REFERENCES [dbo].[Planos] ([Id]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_ComentariosPlano_PlanoId]
    ON [dbo].[ComentariosPlano]([PlanoId] ASC);

