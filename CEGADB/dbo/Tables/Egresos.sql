CREATE TABLE [dbo].[Egresos] (
    [Id]       INT             IDENTITY (1, 1) NOT NULL,
    [Monto]    DECIMAL (18, 2) NOT NULL,
    [Fecha]    DATETIME2 (7)   NOT NULL,
    [Concepto] NVARCHAR (255)  NOT NULL,
    CONSTRAINT [PK_Egresos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

