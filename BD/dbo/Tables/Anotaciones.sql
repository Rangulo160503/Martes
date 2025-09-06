CREATE TABLE [dbo].[Anotaciones] (
    [IdAnotacion] INT            IDENTITY (1, 1) NOT NULL,
    [IdPdf]       INT            NOT NULL,
    [Cedula]      INT            NOT NULL,
    [Texto]       NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Anotaciones] PRIMARY KEY CLUSTERED ([IdAnotacion] ASC),
    CONSTRAINT [FK_Anotaciones_Empleado] FOREIGN KEY ([Cedula]) REFERENCES [dbo].[Empleado] ([Cedula]) ON DELETE CASCADE,
    CONSTRAINT [FK_Anotaciones_Pdf] FOREIGN KEY ([IdPdf]) REFERENCES [dbo].[Pdf] ([IdPdf]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Anotaciones_Cedula]
    ON [dbo].[Anotaciones]([Cedula] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Anotaciones_IdPdf]
    ON [dbo].[Anotaciones]([IdPdf] ASC);

