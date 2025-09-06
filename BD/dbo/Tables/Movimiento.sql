CREATE TABLE [dbo].[Movimiento] (
    [MovimientoId] INT             IDENTITY (1, 1) NOT NULL,
    [Fecha]        DATE            NOT NULL,
    [Concepto]     NVARCHAR (200)  NOT NULL,
    [CategoriaId]  INT             NOT NULL,
    [Tipo]         CHAR (1)        NOT NULL,
    [Monto]        DECIMAL (18, 2) NOT NULL,
    [Moneda]       CHAR (3)        DEFAULT ('CRC') NOT NULL,
    [EsFijo]       BIT             DEFAULT ((0)) NOT NULL,
    [CreadoEn]     DATETIME2 (0)   DEFAULT (sysutcdatetime()) NOT NULL,
    [ModificadoEn] DATETIME2 (0)   NULL,
    PRIMARY KEY CLUSTERED ([MovimientoId] ASC),
    CONSTRAINT [CK_Mov_Mon] CHECK ([Moneda]='EUR' OR [Moneda]='USD' OR [Moneda]='CRC'),
    CONSTRAINT [CK_Mov_Monto] CHECK ([Monto]>(0)),
    CONSTRAINT [CK_Mov_Tipo] CHECK ([Tipo]='G' OR [Tipo]='I'),
    CONSTRAINT [FK_Mov_Cat] FOREIGN KEY ([CategoriaId]) REFERENCES [dbo].[CategoriaMovimiento] ([CategoriaId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Mov_Tipo]
    ON [dbo].[Movimiento]([Tipo] ASC, [Fecha] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Mov_Fecha]
    ON [dbo].[Movimiento]([Fecha] ASC);

