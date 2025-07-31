CREATE TABLE [dbo].[Planos] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Disciplina]    NVARCHAR (MAX) NOT NULL,
    [NombreArchivo] NVARCHAR (MAX) NOT NULL,
    [RutaArchivo]   NVARCHAR (MAX) NOT NULL,
    [FechaSubida]   DATETIME2 (7)  NOT NULL,
    [SubidoPor]     NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Planos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

