using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectLigaNosWeb.Data;
using ProjectLigaNosWeb.Data.Entities;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly DataContext _context;

        public StatisticsController(IStatisticsRepository statisticsRepository, DataContext context)
        {
            _statisticsRepository = statisticsRepository;
            _context = context; 

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

            PdfFont font = new PdfTrueTypeFont("Arial", 12);

            page.Graphics.DrawString("Statistics List", font, PdfBrushes.Black, new PointF(10, 10));

            float y = 30;
            foreach (var stat in statisticsList)
            {
                page.Graphics.DrawString($"ID: {stat.Id}", font, PdfBrushes.Black, new PointF(10, y));
                y += 20;

                page.Graphics.DrawString($"Club ID: {stat.ClubId}", font, PdfBrushes.Black, new PointF(10, y));
                y += 20;

                page.Graphics.DrawString($"Game ID: {stat.GameId}", font, PdfBrushes.Black, new PointF(10, y));
                y += 20;

                page.Graphics.DrawString($"Matches Played: {stat.MatchesPlayed}", font, PdfBrushes.Black, new PointF(10, y));
                y += 20;

                page.Graphics.DrawString($"Overall Ranking: {stat.OverallRanking}", font, PdfBrushes.Black, new PointF(10, y));
                y += 20;

                page.Graphics.DrawString($"Goals Scored: {stat.GoalsScored}", font, PdfBrushes.Black, new PointF(10, y));
                y += 20;

                page.Graphics.DrawString($"Goals Conceded: {stat.GoalsConceded}", font, PdfBrushes.Black, new PointF(10, y));
                y += 20;

                page.Graphics.DrawString($"Home Wins: {stat.HomeWins}", font, PdfBrushes.Black, new PointF(10, y));
                y += 20;

                page.Graphics.DrawString($"Away Wins: {stat.AwayWins}", font, PdfBrushes.Black, new PointF(10, y));
                y += 30;
            }


            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                document.Close(true);

                return File(stream.ToArray(), "application/pdf", "Estatisticas.pdf");
            }
        }

        public async Task<IActionResult> Index()
        {
            var statisticsList = await _statisticsRepository.GetAllAsync();

            return View(statisticsList);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statistics = await _statisticsRepository.GetByIdAsync(id.Value);
            if (statistics == null)
            {
                return NotFound();
            }

            return View(statistics);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Clubs = await _context.Clubs.ToListAsync();
            ViewBag.Games = await _context.Games.ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Statistics statistics)
        {
            if (ModelState.IsValid)
            {
                await _statisticsRepository.AddAsync(statistics);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clubs = await _context.Clubs.ToListAsync();
            ViewBag.Games = await _context.Games.ToListAsync();

            return View(statistics);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatisticas = await _statisticsRepository.GetByIdAsync(id.Value);
            if (estatisticas == null)
            {
                return NotFound();
            }

            ViewBag.Clubs = await _context.Clubs.ToListAsync();
            ViewBag.Games = await _context.Games.ToListAsync();

            return View(estatisticas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Statistics statistics)
        {
            if (id != statistics.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _statisticsRepository.UpdateAsync(statistics);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _statisticsRepository.ExistsAsync(statistics.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clubs = await _context.Clubs.ToListAsync();
            ViewBag.Games = await _context.Games.ToListAsync();

            return View(statistics);
        }

        // GET: Estatisticas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estatisticas = await _statisticsRepository.GetByIdAsync(id.Value);
            if (estatisticas == null)
            {
                return NotFound();
            }

            return View(estatisticas);
        }

        // POST: Estatisticas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estatisticas = await _statisticsRepository.GetByIdAsync(id);
            await _statisticsRepository.DeleteAsync(estatisticas);
            return RedirectToAction(nameof(Index));
        }
    }
}

