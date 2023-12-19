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

        [HttpPost("/Home/GetAssignmentDetailsPartial")]
        public PartialViewResult GetAssignmentDetailsPartial([FromBody] string data)
        {
            AssignmentViewModel avm = JsonSerializer.Deserialize<AssignmentViewModel>(data)!;
            return PartialView("Partials/AssignmentDetails", avm);
        }

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