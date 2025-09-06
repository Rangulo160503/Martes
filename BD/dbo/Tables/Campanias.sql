CREATE TABLE [dbo].[Campanias] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]       NVARCHAR (120)  NOT NULL,
    [AsuntoCorreo] NVARCHAR (200)  NOT NULL,
    [Descripcion]  NVARCHAR (1000) NOT NULL,
    [NombrePool]   NVARCHAR (120)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

