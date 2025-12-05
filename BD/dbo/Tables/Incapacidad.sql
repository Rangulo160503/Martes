CREATE TABLE [dbo].[Incapacidad] (
    [Cedula]  INT             NOT NULL,
    [Archivo] VARBINARY (MAX) NOT NULL,
    [Id]      INT             IDENTITY (1, 1) NOT NULL,
    [Fecha]   DATE            NOT NULL,
    CONSTRAINT [PK_Incapacidad_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Incapacidad_Empleado] FOREIGN KEY ([Cedula]) REFERENCES [dbo].[Empleado] ([Cedula]) ON DELETE CASCADE
);

