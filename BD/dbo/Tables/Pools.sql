CREATE TABLE [dbo].[Pools] (
    [Id]      INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]  NVARCHAR (120)  NOT NULL,
    [Mensaje] NVARCHAR (1000) NOT NULL,
    [Correos] NVARCHAR (MAX)  NOT NULL,
    [Archivo] VARBINARY (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

