CREATE TABLE [dbo].[Puesto] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]       NVARCHAR (100)  NOT NULL,
    [Activo]       BIT             DEFAULT ((1)) NOT NULL,
    [Codigo]       NVARCHAR (20)   NULL,
    [Departamento] NVARCHAR (200)  NULL,
    [Descripcion]  NVARCHAR (500)  NULL,
    [Requisitos]   NVARCHAR (500)  NULL,
    [Nivel]        NVARCHAR (100)  NULL,
    [SalarioBase]  DECIMAL (18, 2) NULL,
    [Moneda]       NVARCHAR (50)   NULL,
    [Jornada]      NVARCHAR (100)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

