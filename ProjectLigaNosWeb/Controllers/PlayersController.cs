using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectLigaNosWeb.Data;
using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Models;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace ProjectLigaNosWeb.Controllers
{
    [Authorize(Roles = "Clube")]
    public class PlayersController : Controller
    {
        private readonly IPlayersRepository _playersRepository;
        private readonly IClubsRepository _clubsRepository; 


        public PlayersController(IPlayersRepository playersRepository, IClubsRepository clubsRepository)
        {
            _playersRepository = playersRepository;
            _clubsRepository = clubsRepository;
        }
        public async Task<IActionResult> Index()
        {
            var players = await _playersRepository.GetAllAsync(); 

            var clubs = await _clubsRepository.GetAllAsync();

            var playerViewModels = players.Select(player => new PlayerViewModel
            {
                Id = player.Id,
                Name = player.Name,
                DateBirth = player.DateBirth,
                Position = player.Position,
                ShirtNum = player.ShirtNum,
                Nacionality = player.Nacionality,
                ClubId = player.ClubId,
                ClubName = clubs.FirstOrDefault(c => c.Id == player.ClubId)?.Name 
            }).ToList();

            return View(playerViewModels); 
        }


        public async Task<IActionResult> Details(int id)
        {
            var player = await _playersRepository.GetByIdAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            var club = await _clubsRepository.GetByIdAsync(player.ClubId);

            var playerViewModel = new PlayerViewModel
            {
                Id = player.Id,
                Name = player.Name,
                DateBirth = player.DateBirth,
                Position = player.Position,
                ShirtNum = player.ShirtNum,
                Nacionality = player.Nacionality,
                ClubId = player.ClubId,
                ClubName = club?.Name 
            };

            return View(playerViewModel);
        }

        // GET: Jogadores/Create
        public async Task<IActionResult> Create()
        {
            var clubs = await _clubsRepository.GetAllAsync();

            var model = new PlayerViewModel
            {
                Clubs = clubs.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),

                DateBirth = new DateTime(2024, 1, 1)
            };

            return View(model);
        }

        // POST: Jogadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Jogadores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                if (model.ProfilePicture != null)
                {
                    // Definir diretório para salvar as imagens
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/players");
                    Directory.CreateDirectory(uploadsFolder); // Certifique-se de que o diretório existe

                    // Criar um nome único para o arquivo
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(fileStream);
                    }
                }

                var player = new Players
                {
                    Name = model.Name,
                    DateBirth = model.DateBirth,
                    Position = model.Position,
                    ShirtNum = model.ShirtNum,
                    Nacionality = model.Nacionality,
                    ClubId = model.ClubId,
                    ProfilePicturePath = uniqueFileName != null ? $"/images/players/{uniqueFileName}" : null
                };

                await _playersRepository.CreateAsync(player);
                return RedirectToAction(nameof(Index));
            }

            model.Clubs = (await _clubsRepository.GetAllAsync()).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(model);
        }


        // GET: Jogadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogador = await _playersRepository.GetByIdAsync(id.Value);
            if (jogador == null)
            {
                return NotFound();
            }

            if (jogador.DateBirth == DateTime.MinValue) 
            {
                jogador.DateBirth = new DateTime(2024, 1, 1);
            }

            return View(jogador);
        }


        // POST: Jogadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Jogadores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlayerViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var player = await _playersRepository.GetByIdAsync(id);
                if (player == null)
                {
                    return NotFound();
                }

                string uniqueFileName = player.ProfilePicturePath;

                if (model.ProfilePicture != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/players");
                    Directory.CreateDirectory(uploadsFolder);

                    // Apagar a foto antiga, se houver
                    if (!string.IsNullOrEmpty(player.ProfilePicturePath))
                    {
                        var oldFilePath = Path.Combine(uploadsFolder, Path.GetFileName(player.ProfilePicturePath));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Criar um nome único para o novo arquivo
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(fileStream);
                    }
                }

                player.Name = model.Name;
                player.DateBirth = model.DateBirth;
                player.Position = model.Position;
                player.ShirtNum = model.ShirtNum;
                player.Nacionality = model.Nacionality;
                player.ClubId = model.ClubId;
                player.ProfilePicturePath = uniqueFileName != null ? $"/images/players/{uniqueFileName}" : player.ProfilePicturePath;

                await _playersRepository.UpdateAsync(player);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }



        // GET: Jogadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogador = await _playersRepository.GetByIdAsync(id.Value);

            if (jogador == null)
            {
                return NotFound();
            }

            if (jogador.ClubId != null)
            {
                ModelState.AddModelError("", "You cannot delete this player because they are associated with a club.");
                return View(jogador); 
            }

            return View(jogador);
        }


        // POST: Jogadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogador = await _playersRepository.GetByIdAsync(id);

            if (jogador.ClubId != null)
            {
                ModelState.AddModelError("", "You cannot delete this player because they are associated with a club.");
                return View(jogador); 
            }

            await _playersRepository.DeleteAsync(jogador);
            return RedirectToAction(nameof(Index)); 
        }
    }
}

