using CEGA.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CEGA.Utils
{
    public static class GeneradorReportePDF
    {
        public static byte[] CrearReporte(ReporteFinancieroViewModel modelo)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"📊 Reporte Financiero ({modelo.FechaInicio:dd/MM/yyyy} - {modelo.FechaFin:dd/MM/yyyy})")
                                 .SemiBold().FontSize(18).AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Item().Text("💰 Ingresos").Bold().FontSize(14);
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(100);
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            tabla.Header(h =>
                            {
                                h.Cell().Text("Monto").Bold();
                                h.Cell().Text("Fecha").Bold();
                                h.Cell().Text("Descripción").Bold();
                            });

                            foreach (var i in modelo.Ingresos)
                            {
                                tabla.Cell().Text(i.Monto.ToString("C"));
                                tabla.Cell().Text(i.Fecha.ToString("g"));
                                tabla.Cell().Text(i.Descripcion);
                            }
                        });

                        col.Item().PaddingTop(20).Text("💸 Egresos").Bold().FontSize(14);
                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(100);
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            tabla.Header(h =>
                            {
                                h.Cell().Text("Monto").Bold();
                                h.Cell().Text("Fecha").Bold();
                                h.Cell().Text("Concepto").Bold();
                            });

                            foreach (var e in modelo.Egresos)
                            {
                                tabla.Cell().Text(e.Monto.ToString("C"));
                                tabla.Cell().Text(e.Fecha.ToString("g"));
                                tabla.Cell().Text(e.Concepto);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text($"Generado por CEGA • {System.DateTime.Now:g}");
                });
            });

            return documento.GeneratePdf();
        }
    }
}
