using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VTAuftragserfassung.Models;
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


        [HttpPost("/Home/SearchResultPartialArticle/")]
        public PartialViewResult SearchResultPartialArticle([FromBody] List<Artikel> modelList)
        {
            return PartialView("Partials/SearchResultArticle", modelList);
        }

        [HttpPost("/Home/SearchResultPartialCustomer/")]
        public PartialViewResult SearchResultPartialCustomer([FromBody] List<Kunde> modelList)
        {
            return PartialView("Partials/SearchResultCustomer", modelList);
        }

        [HttpGet("/Home/AddCustomerForm/")]
        public PartialViewResult AddCustomerForm()
        {
            return PartialView("Partials/CustomerForm");
        }


        [HttpGet("/Home/AddCustomerDetailsPartial/{customerPK}")]
        public PartialViewResult AddCustomerDetailsPartial(int customerPK)
        {
            Kunde customer = _logic.GetCustomerByPK(customerPK);
            return PartialView("Partials/CustomerDetails", customer);
        }

        [HttpGet("/Home/AddPositionListRowFormPartial/{articlePK}")]
        public PartialViewResult AddPositionListRowFormPartial(int articlePK)
        {
            PositionViewModel pvm = _logic.GetPositionViewModel(articlePK);
            return PartialView("Partials/PositionListRowForm", pvm);
        }

        [HttpPost("/Home/CrateNewAssignment/")]
        public void CreateNewAssignment(AssignmentViewModel avm)
        {
            _logic.CreateAssignment(avm);
        }

        #endregion Public Methods
    }
}