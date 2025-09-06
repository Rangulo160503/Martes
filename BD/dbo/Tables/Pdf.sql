CREATE TABLE [dbo].[Pdf] (
    [IdPdf]      INT             IDENTITY (1, 1) NOT NULL,
    [PdfArchivo] VARBINARY (MAX) NOT NULL,
    CONSTRAINT [PK_Pdf] PRIMARY KEY CLUSTERED ([IdPdf] ASC)
);

