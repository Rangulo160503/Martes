CREATE TABLE [dbo].[Clientes] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (120) NOT NULL,
    [Correo] NVARCHAR (256) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Correo] ASC)
);

