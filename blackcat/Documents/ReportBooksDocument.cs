using blackcat.Models;
using blackcat.Models.Dtos;

namespace blackcat.Documents;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

public class ReportBooksDocument : IDocument
{
    public List<BusquedaDto> Busqueda { get; set; }
    private byte[] LogoBytes;

    public ReportBooksDocument(List<BusquedaDto> busqueda, byte[]  logoBytes)
    {
        Busqueda = busqueda;
        LogoBytes = logoBytes;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header()
                .Element(ComposeHeader);

            page.Content()
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(160); //    Nombre del libro
                        columns.RelativeColumn(); // CantB
                        columns.ConstantColumn(140); // CantBn 
                        columns.ConstantColumn(60); //    Estado
                    });

                    // Encabezado de la tabla
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Libro").Bold();
                        header.Cell().Element(CellStyle).Text("Cantidad de Busquedas").Bold();
                        header.Cell().Element(CellStyle).Text("Cant. No Encontrados").Bold();
                        header.Cell().Element(CellStyle).Text("Estado").Bold();
                    });

                    // Cuerpo de la tabla
                    foreach (var b in Busqueda)
                    {
                        table.Cell().Element(CellStyle).Text(b.NomLib);
                        table.Cell().Element(CellStyle).Text(b.CantB?.ToString() ?? "0");
                        ;
                        table.Cell().Element(CellStyle).Text(b.CantBn?.ToString() ?? "0");
                        ;
                        table.Cell().Element(CellStyle).Text(b.Estado ?? "Sin estado");
                    }

                    // Estilo de celda para reutilizar
                    IContainer CellStyle(IContainer container) =>
                        container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                });

            page.Footer()
                .AlignCenter()
                .Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
        });
    }
    void ComposeHeader(IContainer container)
    {
        container.Column(column => 
        {
            column.Item().Row(row =>
            {
                row.RelativeItem()
                    .Width(100)
                    .AlignLeft()
                    .Element(LogoBytes != null
                        ? e => e.Image(LogoBytes, ImageScaling.FitArea)
                        : e => e.Text("Black Cat")
                            .FontColor(Colors.Black)
                            .Bold()
                            .FontSize(16));

                row.RelativeItem()
                    .Column(innerColumn =>
                    {
                        innerColumn.Item()
                            .AlignRight()
                            .Text("Reporte de Búsqueda de Libros")
                            .FontSize(16)
                            .SemiBold()
                            .FontColor(Colors.Blue.Medium);

                        innerColumn.Item()
                            .AlignRight()
                            .Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                            .FontSize(10)
                            .FontColor(Colors.Grey.Medium);
                    });
            });

            column.Item()
                .PaddingBottom(5)
                .LineHorizontal(1)
                .LineColor(Colors.Grey.Medium);
        });
    }
}