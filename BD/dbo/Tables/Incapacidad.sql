CREATE TABLE [dbo].[Incapacidad] (
    [Cedula]  INT             NOT NULL,
    [Archivo] VARBINARY (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Cedula] ASC),
    CONSTRAINT [FK_Incapacidad_Empleado] FOREIGN KEY ([Cedula]) REFERENCES [dbo].[Empleado] ([Cedula])
);

