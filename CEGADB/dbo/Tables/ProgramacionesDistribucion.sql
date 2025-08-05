CREATE TABLE [dbo].[ProgramacionesDistribucion] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]         NVARCHAR (MAX) NOT NULL,
    [Descripcion]    NVARCHAR (MAX) NOT NULL,
    [FechaHoraEnvio] DATETIME2 (7)  NOT NULL,
    [NombrePool]     NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ProgramacionesDistribucion] PRIMARY KEY CLUSTERED ([Id] ASC)
);

