CREATE TABLE [dbo].[Proyecto] (
    [IdProyecto] INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]     NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Proyecto_IdProyecto] PRIMARY KEY CLUSTERED ([IdProyecto] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Proyecto_Nombre]
    ON [dbo].[Proyecto]([Nombre] ASC);

