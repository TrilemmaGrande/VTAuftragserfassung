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

        public IActionResult Assignments(int page, int linesPerPage)
        {
            List<AssignmentViewModel>? avm = _logic.GetAssignmentViewModels(page, linesPerPage);
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
            AssignmentFormViewModel? afvm = _logic.GetAssignmentFormViewModel();
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

        [HttpPost("/Home/SearchResultPartialAssignment/")]
        public PartialViewResult SearchResultPartialAssignment([FromBody] List<AssignmentViewModel> modelList)
        {
            return PartialView("Partials/SearchResultAssignment", modelList);
        }

        [HttpGet("/Home/AddCustomerForm/")]
        public PartialViewResult AddCustomerForm()
        {
            return PartialView("Partials/CustomerForm");
        }

        [HttpGet("/Home/ShareholderDetailsPartial/{shareholderPK}")]
        public PartialViewResult GetShareholderDetailsPartial(int shareholderPK)
        {
            Gesellschafter? shareholder = _logic.GetShareholderByPK(shareholderPK);
            return PartialView("Partials/ShareholderDetails", shareholder);
        }

        [HttpGet("/Home/ShareholderFormPartial/")]
        public PartialViewResult GetShareholderFormPartial()
        {
            List<Gesellschafter>? shareholders = _logic.GetAllShareholders();
            return PartialView("Partials/ShareholderForm", shareholders);
        }

        [HttpGet("/Home/AddCustomerDetailsPartial/{customerPK}")]
        public PartialViewResult AddCustomerDetailsPartial(int customerPK)
        {
            Kunde? customer = _logic.GetCustomerByPK(customerPK);
            return PartialView("Partials/CustomerDetails", customer);
        }

        [HttpGet("/Home/AddPositionListRowFormPartial/{articlePK}")]
        public PartialViewResult AddPositionListRowFormPartial(int articlePK, int positionNr)
        {
            PositionViewModel? pvm = _logic.GetPositionViewModel(articlePK);
            pvm.Position!.PositionsNummer = positionNr;
            return PartialView("Partials/PositionListRowForm", pvm);
        }

        [HttpPost("/Home/CreateNewAssignment/")]
        public int CreateNewAssignment([FromBody] AssignmentViewModel avm)
        {
            return _logic.CreateAssignment(avm);
        }

        [HttpPost("/Home/CreateNewCustomer/")]
        public int CreateNewCustomer([FromBody] Kunde customer)
        {
            return _logic.CreateCustomer(customer);
        }

        [HttpPost("/Home/UpdateAssignmentStatus/{assignmentPK}")]
        public void UpdateAssignmentStatus(int assignmentPK, string assignmentStatus)
        {
            _logic.UpdateAssignmentStatus(assignmentPK, assignmentStatus);
        }

        #endregion Public Methods
    }
}