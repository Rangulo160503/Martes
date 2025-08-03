CREATE TABLE [dbo].[AnotacionesPlano] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [Texto]   NVARCHAR (MAX) NOT NULL,
    [PlanoId] INT            NOT NULL,
    [Usuario] NVARCHAR (MAX) NOT NULL,
    [Fecha]   DATETIME2 (7)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnotacionesPlano_Planos] FOREIGN KEY ([PlanoId]) REFERENCES [dbo].[Planos] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AnotacionesPlano_PlanoId]
    ON [dbo].[AnotacionesPlano]([PlanoId] ASC);

