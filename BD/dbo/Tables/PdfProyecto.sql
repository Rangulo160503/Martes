CREATE TABLE [dbo].[PdfProyecto] (
    [IdPdf]      INT NOT NULL,
    [IdProyecto] INT NOT NULL,
    CONSTRAINT [PK_PdfProyecto] PRIMARY KEY CLUSTERED ([IdPdf] ASC, [IdProyecto] ASC),
    CONSTRAINT [FK_PdfProyecto_Pdf] FOREIGN KEY ([IdPdf]) REFERENCES [dbo].[Pdf] ([IdPdf]) ON DELETE CASCADE,
    CONSTRAINT [FK_PdfProyecto_Proyecto] FOREIGN KEY ([IdProyecto]) REFERENCES [dbo].[Proyecto] ([IdProyecto]) ON DELETE CASCADE
);

