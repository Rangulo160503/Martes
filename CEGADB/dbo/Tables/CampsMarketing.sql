CREATE TABLE [dbo].[CampsMarketing] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]       NVARCHAR (MAX) NOT NULL,
    [AsuntoCorreo] NVARCHAR (MAX) NOT NULL,
    [Descripcion]  NVARCHAR (MAX) NOT NULL,
    [ImagenUrl]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_CampsMarketing] PRIMARY KEY CLUSTERED ([Id] ASC)
);

