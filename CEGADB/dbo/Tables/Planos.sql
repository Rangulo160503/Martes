CREATE TABLE [dbo].[Planos] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Disciplina]    NVARCHAR (MAX)  NOT NULL,
    [NombreArchivo] NVARCHAR (MAX)  NOT NULL,
    [FechaSubida]   DATETIME2 (7)   CONSTRAINT [DF_Planos_FechaSubida] DEFAULT (getdate()) NOT NULL,
    [SubidoPor]     NVARCHAR (MAX)  NOT NULL,
    [Archivo]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_Planos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

