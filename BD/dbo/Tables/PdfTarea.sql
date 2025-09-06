CREATE TABLE [dbo].[PdfTarea] (
    [IdPdf]   INT NOT NULL,
    [IdTarea] INT NOT NULL,
    CONSTRAINT [PK_PdfTarea] PRIMARY KEY CLUSTERED ([IdPdf] ASC, [IdTarea] ASC),
    CONSTRAINT [FK_PdfTarea_Pdf] FOREIGN KEY ([IdPdf]) REFERENCES [dbo].[Pdf] ([IdPdf]) ON DELETE CASCADE,
    CONSTRAINT [FK_PdfTarea_Tareas] FOREIGN KEY ([IdTarea]) REFERENCES [dbo].[Tareas] ([Id]) ON DELETE CASCADE
);

