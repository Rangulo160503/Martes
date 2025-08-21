CREATE TABLE [dbo].[ClientesMarketing] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]   NVARCHAR (MAX) NOT NULL,
    [Correo]   NVARCHAR (MAX) NOT NULL,
    [Telefono] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ClientesMarketing] PRIMARY KEY CLUSTERED ([Id] ASC)
);

