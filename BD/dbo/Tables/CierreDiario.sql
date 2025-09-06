CREATE TABLE [dbo].[CierreDiario] (
    [CierreId]      INT             IDENTITY (1, 1) NOT NULL,
    [Fecha]         DATE            NOT NULL,
    [TotalIngresos] DECIMAL (18, 2) NOT NULL,
    [TotalGastos]   DECIMAL (18, 2) NOT NULL,
    [SaldoDia]      DECIMAL (18, 2) NOT NULL,
    [CreadoEn]      DATETIME2 (7)   DEFAULT (sysutcdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([CierreId] ASC),
    UNIQUE NONCLUSTERED ([Fecha] ASC)
);

