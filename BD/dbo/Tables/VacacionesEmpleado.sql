CREATE TABLE [dbo].[VacacionesEmpleado] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Cedula] INT            NOT NULL,
    [Fecha]  DATE           NOT NULL,
    [Notas]  NVARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VacacionesEmpleado_Empleado] FOREIGN KEY ([Cedula]) REFERENCES [dbo].[Empleado] ([Cedula])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_VacacionesEmpleado_Cedula_Fecha]
    ON [dbo].[VacacionesEmpleado]([Cedula] ASC, [Fecha] ASC);

