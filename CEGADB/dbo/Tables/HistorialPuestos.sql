CREATE TABLE [dbo].[HistorialPuestos] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [UsuarioID]       NVARCHAR (MAX)  NOT NULL,
    [PuestoAnterior]  NVARCHAR (MAX)  NOT NULL,
    [PuestoNuevo]     NVARCHAR (MAX)  NOT NULL,
    [FechaCambio]     DATETIME2 (7)   NOT NULL,
    [SalarioAnterior] DECIMAL (18, 2) NOT NULL,
    [SalarioNuevo]    DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_HistorialPuestos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

