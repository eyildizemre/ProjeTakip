using Microsoft.AspNetCore.Mvc;
using ProjeTakip.DataAccess.Repository.IRepository;

namespace ProjeTakipUygulaması.Areas.TeamLead.Controllers
{
    [Area("TeamLead")]
    public class TeamLeadController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeamLeadController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
