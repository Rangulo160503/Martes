CREATE TABLE [dbo].[CierresDiarios] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Fecha]         DATETIME2 (7)   NOT NULL,
    [TotalIngresos] DECIMAL (18, 2) NOT NULL,
    [TotalEgresos]  DECIMAL (18, 2) NOT NULL,
    [BalanceFinal]  DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_CierresDiarios] PRIMARY KEY CLUSTERED ([Id] ASC)
);

