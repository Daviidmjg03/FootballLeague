using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectLigaNosWeb.Data;
using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Controllers
{
    [Authorize(Roles = "Admin, Funcionario")]
    public class GamesController : Controller
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly IClubsRepository _clubsRepository;
        private readonly DataContext _context;
        private readonly IStatisticsRepository _statisticsRepository;

        public GamesController(IGamesRepository gamesRepository, IClubsRepository clubsRepository,  DataContext context, IStatisticsRepository statisticsRepository)
        {
            _gamesRepository = gamesRepository;
            _clubsRepository = clubsRepository;
            _context = context;
            _statisticsRepository = statisticsRepository;
        }
        public async Task<IActionResult> Index()
        {
            var games = await _gamesRepository.GetAllAsync();
            if (games == null || !games.Any())
            {
                ModelState.AddModelError("", "No game found.");
                return View(new List<Games>()); 
            }

            return View(games);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogo = await _gamesRepository.GetAllAsync()
                .ContinueWith(t => t.Result.FirstOrDefault(g => g.Id == id.Value));

            if (jogo == null)
            {
                return NotFound();
            }

            await _context.Entry(jogo)
                .Reference(g => g.ClubHome) 
                .LoadAsync();
            await _context.Entry(jogo)
                .Reference(g => g.ClubAway) 
                .LoadAsync();

            return View(jogo);
        }


        // GET: Jogos/Create
        public async Task<IActionResult> Create()
        {
            var clubs = await _clubsRepository.GetAllAsync();
            var model = new GameViewModel
            {
                ClubsHome = clubs.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),
                ClubsAway = clubs.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(model);
        }

        // POST: Jogos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Create(GameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingGame = await _gamesRepository.GetAllAsync();
                var gameOnSameDay = existingGame.FirstOrDefault(g =>
                    g.Data == model.Data &&
                    ((g.ClubHomeId == model.ClubHomeId && g.ClubAwayId == model.ClubAwayId) ||
                    (g.ClubHomeId == model.ClubAwayId && g.ClubAwayId == model.ClubHomeId))); 

                if (gameOnSameDay != null)
                {
                    ModelState.AddModelError("", "A game between these teams already exists on the same day.");

                    var clubs = await _clubsRepository.GetAllAsync();
                    model.ClubsHome = clubs.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
                    model.ClubsAway = clubs.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();

                    return View(model);
                }

                var game = new Games
                {
                    Name = model.Name,
                    Data = model.Data,
                    ClubHomeId = model.ClubHomeId,
                    ClubAwayId = model.ClubAwayId,
                    HomeGoals = model.HomeGoals,
                    AwayGoals = model.AwayGoals,
                    Localizacao = model.Localizacao
                };

                await _gamesRepository.CreateAsync(game);

                if (game.Id == 0) 
                {
                    ModelState.AddModelError("", "The game was not saved correctly.");
                    return View(model);
                }

                await UpdateStatistics(game); 

                return RedirectToAction(nameof(Index));
            }

            var clubsFallback = await _clubsRepository.GetAllAsync();
            model.ClubsHome = clubsFallback.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            model.ClubsAway = clubsFallback.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(model);
        }


        private async Task UpdateStatistics(Games game)
        {
            var homeStat = await _statisticsRepository.GetAllAsync()
                .ContinueWith(t => t.Result.FirstOrDefault(s => s.ClubId == game.ClubHomeId));

            if (homeStat == null)
            {
                homeStat = new Statistics
                {
                    ClubId = game.ClubHomeId,
                    MatchesPlayed = 1,
                    GoalsScored = game.HomeGoals,
                    GoalsConceded = game.AwayGoals,
                    HomeWins = game.HomeGoals > game.AwayGoals ? 1 : 0,
                    AwayWins = 0,
                    OverallRanking = game.HomeGoals > game.AwayGoals ? 3 : (game.HomeGoals == game.AwayGoals ? 1 : 0), 
                    GameId = game.Id
                };
                await _statisticsRepository.AddAsync(homeStat);
            }
            else
            {
                homeStat.MatchesPlayed++;
                homeStat.GoalsScored += game.HomeGoals;
                homeStat.GoalsConceded += game.AwayGoals;

                if (game.HomeGoals > game.AwayGoals)
                {
                    homeStat.HomeWins++;
                    homeStat.OverallRanking += 3; 
                }
                else if (game.HomeGoals == game.AwayGoals)
                {
                    homeStat.OverallRanking += 1; 
                }

                await _statisticsRepository.UpdateAsync(homeStat);
            }

            var awayStat = await _statisticsRepository.GetAllAsync()
                .ContinueWith(t => t.Result.FirstOrDefault(s => s.ClubId == game.ClubAwayId));

            if (awayStat == null)
            {
                awayStat = new Statistics
                {
                    ClubId = game.ClubAwayId,
                    MatchesPlayed = 1,
                    GoalsScored = game.AwayGoals,
                    GoalsConceded = game.HomeGoals,
                    HomeWins = 0,
                    AwayWins = game.AwayGoals > game.HomeGoals ? 1 : 0,
                    OverallRanking = game.AwayGoals > game.HomeGoals ? 3 : (game.AwayGoals == game.HomeGoals ? 1 : 0), 
                    GameId = game.Id
                };
                await _statisticsRepository.AddAsync(awayStat);
            }
            else
            {
                awayStat.MatchesPlayed++;
                awayStat.GoalsScored += game.AwayGoals;
                awayStat.GoalsConceded += game.HomeGoals;

                if (game.AwayGoals > game.HomeGoals)
                {
                    awayStat.AwayWins++;
                    awayStat.OverallRanking += 3;
                }
                else if (game.AwayGoals == game.HomeGoals)
                {
                    awayStat.OverallRanking += 1;
                }

                await _statisticsRepository.UpdateAsync(awayStat);
            }
        }

        // GET: Jogos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogos = await _gamesRepository.GetByIdAsync(id.Value);
            if (jogos == null)
            {
                return NotFound();
            }

            var allClubs = await _clubsRepository.GetAllAsync();
            if (allClubs == null || !allClubs.Any())
            {
                ModelState.AddModelError("", "No club found.");
                return View(jogos);
            }

            ViewBag.ClubsHome = allClubs.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.ClubsAway = allClubs.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(jogos);
        }




        // POST: Jogos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Games jogos)
        {
            if (id != jogos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingGame = await _gamesRepository.GetByIdAsync(id);
                    if (existingGame != null)
                    {
                        existingGame.Data = jogos.Data;
                        existingGame.ClubHomeId = jogos.ClubHomeId;
                        existingGame.ClubAwayId = jogos.ClubAwayId;
                        existingGame.HomeGoals = jogos.HomeGoals;
                        existingGame.AwayGoals = jogos.AwayGoals;
                        existingGame.Localizacao = jogos.Localizacao;

                        await _gamesRepository.UpdateAsync(existingGame);
                        return RedirectToAction(nameof(Index));
                    }
                    return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GamesExists(jogos.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error updating the game: " + ex.Message);
                }
            }

            var clubs = await _clubsRepository.GetAllAsync();
            ViewBag.ClubsHome = clubs.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            ViewBag.ClubsAway = clubs.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(jogos); 
        }


        private async Task<bool> GamesExists(int id)
        {
            var game = await _gamesRepository.GetByIdAsync(id);
            return game != null;
        }


        // GET: Jogos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogos = await _gamesRepository.GetByIdAsync(id.Value);
            if (jogos == null)
            {
                return NotFound();
            }

            return View(jogos);
        }

        // POST: Jogos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogos = await _gamesRepository.GetByIdAsync(id);

            await _gamesRepository.DeleteAsync(jogos);
            return RedirectToAction(nameof(Index));
        }
    }
}
