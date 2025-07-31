CREATE TABLE [dbo].[ComentariosPlano] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [PlanoId] INT            NOT NULL,
    [Texto]   NVARCHAR (MAX) NOT NULL,
    [Fecha]   DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_ComentariosPlano] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ComentariosPlano_Planos_PlanoId] FOREIGN KEY ([PlanoId]) REFERENCES [dbo].[Planos] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ComentariosPlano_PlanoId]
    ON [dbo].[ComentariosPlano]([PlanoId] ASC);

