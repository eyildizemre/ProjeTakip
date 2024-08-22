using Microsoft.AspNetCore.Mvc;
using ProjeTakip.DataAccess.Repository.IRepository;
using ProjeTakip.Models;

namespace ProjeTakipUygulaması.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeamController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var teams = _unitOfWork.Teams.GetAll();
            return View(teams);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Team team)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Teams.Add(team);
                _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var team = _unitOfWork.Teams.GetFirstOrDefault(t => t.TeamId == id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Team team)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Teams.Update(team);
                _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }
    }
}
