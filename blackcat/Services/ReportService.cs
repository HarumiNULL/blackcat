using blackcat.Documents;
using blackcat.Models;
using blackcat.Models.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace blackcat.Services
{
    public class ReportService
    {
        private readonly BlackcatDbContext _context;

        public ReportService(BlackcatDbContext context)
        {
            _context = context;
        }

        public byte[] GenerarReporteBusquedas(List<BusquedaDto> busquedas)
        {
            var logoBytes = File.ReadAllBytes("wwwroot/staticFiles/logo.png");
            var document = new ReportBooksDocument(busquedas, logoBytes);

            return document.GeneratePdf(); // Usa QuestPDF para generar el PDF
        }
        
        public byte[] GenerarReporteListaLibros(List<LibrosDto> LibrosL)
        {
            var logoBytes = File.ReadAllBytes("wwwroot/staticFiles/logo.png");
            var document = new ReportListBooks(LibrosL, logoBytes);

            return document.GeneratePdf(); // Usa QuestPDF para generar el PDF
        }
    }
}