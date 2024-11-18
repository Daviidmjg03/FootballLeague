using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectLigaNosWeb.Data;
using ProjectLigaNosWeb.Helpers;
using ProjectLigaNosWeb.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Controllers
{
    [Authorize(Roles = "Admin, Clube")]
    public class ClubsController : Controller
    {
        private readonly IClubsRepository _clubesRepository;
        private readonly DataContext _context;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        public ClubsController(IClubsRepository clubesRepository, IStatisticsRepository statisticsRepository, IUserHelper userHelper, IBlobHelper blobHelper, IConverterHelper converterHelper , DataContext context)
        {
            _clubesRepository = clubesRepository;
            _statisticsRepository = statisticsRepository;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_clubesRepository.GetAll().OrderBy(c => c.Name));
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClubNotFound");
            }

            var clubes = await _clubesRepository.GetByIdAsync(id.Value);
            if (clubes == null)
            {
                return new NotFoundViewResult("ClubNotFound");
            }

            return View(clubes);
        }

        // GET: Clubes/Create
        [Authorize(Roles = "Admin, Clube")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clubes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ClubViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool clubExists = await _clubesRepository
                    .GetAll()
                    .AnyAsync(c => c.Name == model.Name || c.Acroyn == model.Acroyn);

                if (clubExists)
                {
                    ModelState.AddModelError(string.Empty, "A club with the same name or abbreviation already exists.");
                    return View(model); 
                }

                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "clubs");
                }

                var clubes = _converterHelper.ToClubs(model, imageId, true);
                clubes.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                await _clubesRepository.CreateAsync(clubes);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // GET: Clubes/Edit/5
        [Authorize(Roles = "Admin, Clube")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClubNotFound");
            }

            var clubes = await _clubesRepository.GetByIdAsync(id.Value);
            if (clubes == null)
            {
                return new NotFoundViewResult("ClubNotFound");
            }

            var model = _converterHelper.ToClubeViewModel(clubes);

            return View(model);
        }


        // POST: Clubes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClubViewModel model)
        {
            if (id != model.Id)
            {
                return new NotFoundViewResult("ClubNotFound");
            }

            if (ModelState.IsValid)
            {
                var club = await _clubesRepository.GetByIdAsync(id);
                if (club == null)
                {
                    return new NotFoundViewResult("ClubNotFound");
                }

                club.Name = model.Name;
                club.Acroyn = model.Acroyn;
                club.DateFund = model.DateFund;
                club.City = model.City;
                club.Country = model.Country;
                club.CapacityStadium = model.CapacityStadium;
                club.President = model.President;
                club.NationalTitles = model.NationalTitles;
                club.InternationalTitles = model.InternationalTitles;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    club.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "clubs");
                }

                await _clubesRepository.UpdateAsync(club);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Clubes/Delete/5
        [Authorize(Roles = "Admin, Clube")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _clubesRepository.GetByIdAsync(id.Value);
            if (club == null)
            {
                return NotFound();
            }

            var hasPlayers = await _context.Players.AnyAsync(p => p.ClubId == club.Id);

            if (hasPlayers)
            {
                var players = await _context.Players.Where(p => p.ClubId == club.Id).ToListAsync();
                _context.Players.RemoveRange(players);

            
            }

            await _clubesRepository.DeleteAsync(club);

            return RedirectToAction(nameof(Index));
        }

        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var club = await _clubesRepository.GetByIdAsync(id);

            if (club == null)
            {
                return NotFound();
            }

            var hasPlayers = await _context.Players.AnyAsync(p => p.ClubId == club.Id);

            if (hasPlayers)
            {

                var players = await _context.Players.Where(p => p.ClubId == club.Id).ToListAsync();
                _context.Players.RemoveRange(players);

              
            }

            await _clubesRepository.DeleteAsync(club);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult ClubNotFound()
        {
            return View();
        }
    }
}

