CREATE TABLE [dbo].[CategoriaMovimiento] (
    [CategoriaId] INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (120) NOT NULL,
    [Tipo]        CHAR (1)       NOT NULL,
    [EsFijo]      BIT            DEFAULT ((0)) NOT NULL,
    [Activo]      BIT            DEFAULT ((1)) NOT NULL,
    [CreadoEn]    DATETIME2 (0)  DEFAULT (sysutcdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([CategoriaId] ASC),
    CONSTRAINT [CK_CatMov_Tipo] CHECK ([Tipo]='G' OR [Tipo]='I')
);

