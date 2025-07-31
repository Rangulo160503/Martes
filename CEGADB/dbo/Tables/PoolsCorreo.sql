CREATE TABLE [dbo].[PoolsCorreo] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (MAX) NOT NULL,
    [Descripcion] NVARCHAR (MAX) NOT NULL,
    [Correos]     NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_PoolsCorreo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

