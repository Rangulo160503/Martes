CREATE TABLE [dbo].[VacacionesEmpleado] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Cedula] INT            NOT NULL,
    [Fecha]  DATE           NOT NULL,
    [Notas]  NVARCHAR (500) NULL,
    [Activa] BIT            CONSTRAINT [DF_VacacionesEmpleado_Activa] DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VacacionesEmpleado_Empleado] FOREIGN KEY ([Cedula]) REFERENCES [dbo].[Empleado] ([Cedula]) ON DELETE CASCADE
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_VacacionesEmpleado_Cedula_Fecha]
    ON [dbo].[VacacionesEmpleado]([Cedula] ASC, [Fecha] ASC);

