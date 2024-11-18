using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.IO;
using Syncfusion.Drawing;
using System.Threading.Tasks;
using ProjectLigaNosWeb.Data;
using System.Linq; 


namespace ProjectLigaNosWeb.Controllers
{

    public class PdfController : Controller
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public PdfController(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GeneratePdf()
        {
            var statisticsList = await _statisticsRepository.GetAllAsync();
            if (statisticsList == null || !statisticsList.Any())
            {
                return NotFound();
            }

            PdfDocument document = new PdfDocument();
            PdfPage page = document.Pages.Add();

            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            PdfFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
            PdfFont footerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);

            page.Graphics.DrawString("Cinel, David Manuel Jesus Gonçalves", headerFont, PdfBrushes.Black, new PointF(10, 10));

            page.Graphics.DrawString("Statistics List", font, PdfBrushes.Black, new PointF(10, 30));

            float y = 50; 
            float pageWidth = page.GetClientSize().Width;
            float margin = 10; 

            foreach (var stat in statisticsList)
            {
                string text = $"Club: {stat.Club?.Name}, Game: {stat.Game?.Name}, " +
                 $"Matches Played: {stat.MatchesPlayed}, Goals Scored: {stat.GoalsScored}, " +
                 $"Goals Conceded: {stat.GoalsConceded}, Home Wins: {stat.HomeWins}, " +
                 $"Away Wins: {stat.AwayWins}, Overall Ranking: {stat.OverallRanking}";


                string[] words = text.Split(' ');
                string line = string.Empty;

                foreach (string word in words)
                {
                    string testLine = string.IsNullOrEmpty(line) ? word : line + " " + word;
                    if (font.MeasureString(testLine).Width > (pageWidth - margin * 2)) 
                    {
                        page.Graphics.DrawString(line, font, PdfBrushes.Black, new PointF(margin, y));
                        line = word; 
                        y += 20; 
                    }
                    else
                    {
                        line = testLine;
                    }
                }
                if (!string.IsNullOrEmpty(line))
                {
                    page.Graphics.DrawString(line, font, PdfBrushes.Black, new PointF(margin, y));
                    y += 20;
                }
            }

            page.Graphics.DrawString("Project Web 2024", footerFont, PdfBrushes.Black, new PointF(margin, y + 20));

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                document.Close(true);

                return File(stream.ToArray(), "application/pdf", "Estatisticas.pdf");
            }
        }
    }
}
