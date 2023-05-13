using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Data;
using MusicPlayer.Models;
using MusicPlayer.ViewModels;

namespace MusicPlayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var tracks = await _unitOfWork
                .GetRepository<Track>()
                .Include(x => x.Artist)
                .Include(x => x.Album)
                .Include(x => x.Playlist)
                .Where(x => x.UserId == user.Id)
                .ToArrayAsync();
            return View(tracks);
        }

        [HttpPost]
        public async Task<IActionResult> AddTrack(IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Message = await Track.AddTrack(_unitOfWork, file, user);
            
            return View();
        }

        public async Task<IActionResult> DeleteTrack(long trackId)
        {
            var user = await _userManager.GetUserAsync(User);
            await Track.DeleteTrack(_unitOfWork, trackId);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}