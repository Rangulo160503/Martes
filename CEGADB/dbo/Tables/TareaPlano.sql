CREATE TABLE [dbo].[TareaPlano] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [PlanoId]     INT            NOT NULL,
    [Descripcion] NVARCHAR (MAX) NOT NULL,
    [Fecha]       DATETIME2 (7)  NOT NULL,
    [Responsable] NVARCHAR (200) NOT NULL,
    [CoordenadaX] FLOAT (53)     NOT NULL,
    [CoordenadaY] FLOAT (53)     NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TareaPlano_Planos] FOREIGN KEY ([PlanoId]) REFERENCES [dbo].[Planos] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TareaPlano_PlanoId]
    ON [dbo].[TareaPlano]([PlanoId] ASC);

