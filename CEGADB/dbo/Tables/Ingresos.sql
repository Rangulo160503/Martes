CREATE TABLE [dbo].[Ingresos] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Monto]       DECIMAL (18, 2) NOT NULL,
    [Fecha]       DATETIME2 (7)   NOT NULL,
    [Descripcion] NVARCHAR (255)  NOT NULL,
    CONSTRAINT [PK_Ingresos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

