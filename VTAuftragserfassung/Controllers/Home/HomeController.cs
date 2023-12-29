using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Controllers.Home
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Private Fields

        private readonly IHomeLogic _logic;

        #endregion Private Fields

        #region Public Constructors

        public HomeController(IHomeLogic logic) => _logic = logic;

        #endregion Public Constructors

        #region Public Methods

        public IActionResult Assignments()
        {
            List<AssignmentViewModel> avm = _logic.GetAssignmentViewModels();
            return View(avm);
        }

        public IActionResult Dashboard() => View();
        public IActionResult Logout()
        {
            _logic.Logout();
            return RedirectToAction("Index", "Login");
        }

        [HttpGet("/Home/NewAssignment")]
        public PartialViewResult NewAssignment()
        {
            AssignmentFormViewModel afvm = _logic.GetAssignmentFormViewModel();
            return PartialView("Partials/AssignmentForm", afvm);
        }


        [HttpGet("/Home/AddPositionListRowFormPartial/{articlePK}")]
        public PartialViewResult AddPositionListRowFormPartial(int articlePK)
        {
            PositionViewModel pvm = _logic.GetPositionViewModel(articlePK);
            return PartialView("Partials/PositionListRowForm", pvm);
        }
        #endregion Public Methods
    }
}