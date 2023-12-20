using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Controllers.Home
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHomeLogic _logic;

        public HomeController(IHomeLogic logic) => _logic = logic;

        public IActionResult Index() => View();

        public IActionResult Assignments()
        {
            List<AssignmentViewModel> avm = _logic.GetAssignmentViewModels();
            return View(avm);
        }

        public IActionResult Logout()
        {
            _logic.Logout();
            return RedirectToAction("Index", "Login");
        }
    }
}