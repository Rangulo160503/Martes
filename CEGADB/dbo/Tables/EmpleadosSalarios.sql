CREATE TABLE [dbo].[EmpleadosSalarios] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [UsuarioId]      NVARCHAR (MAX)  NOT NULL,
    [SalarioMensual] DECIMAL (18, 2) NOT NULL,
    [FechaRegistro]  DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_EmpleadosSalarios] PRIMARY KEY CLUSTERED ([Id] ASC)
);

