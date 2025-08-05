CREATE TABLE [dbo].[CierresRango] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [FechaInicio]   DATETIME2 (7)   NOT NULL,
    [FechaFin]      DATETIME2 (7)   NOT NULL,
    [TotalIngresos] DECIMAL (18, 2) NOT NULL,
    [TotalEgresos]  DECIMAL (18, 2) NOT NULL,
    [BalanceFinal]  DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_CierresRango] PRIMARY KEY CLUSTERED ([Id] ASC)
);

