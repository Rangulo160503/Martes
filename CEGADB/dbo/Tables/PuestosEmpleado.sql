CREATE TABLE [dbo].[PuestosEmpleado] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [UsuarioID]       NVARCHAR (MAX)  NOT NULL,
    [Puesto]          NVARCHAR (MAX)  NOT NULL,
    [SalarioAsignado] DECIMAL (18, 2) NOT NULL,
    [FechaAsignacion] DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_PuestosEmpleado] PRIMARY KEY CLUSTERED ([Id] ASC)
);

