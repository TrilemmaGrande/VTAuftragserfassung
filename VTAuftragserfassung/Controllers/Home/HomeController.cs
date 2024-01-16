using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

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

        public IActionResult Dashboard() => View();

        public IActionResult Logout()
        {

            _logic.Logout();
            return RedirectToAction("Index", "Login");
        }

        [HttpGet("Home/GetUserId")]
        public string GetUserId()
        {
            return _logic.GetUserId();
        }

        [HttpPost("/Home/AssignmentsPartial/")]
        public PartialViewResult Assignments([FromBody] Pagination pagination)
        {
        
            List<AssignmentViewModel>? avm = _logic.GetAssignmentViewModels(pagination);
            if (avm == null)
            {
                return new();
            }
            else
            {
                return PartialView("Assignments", avm);
            }
        }

        [HttpPost("/Home/PaginationMenuPartial")]
        public PartialViewResult GetPaginationMenuPartial([FromBody] Pagination pagination)
        {
            return PartialView("PaginationMenu", pagination);
        }

        [HttpGet("/Home/NewAssignment")]
        public PartialViewResult NewAssignment()
        {
            AssignmentFormViewModel? afvm = _logic.GetAssignmentFormViewModel();
            return PartialView("AssignmentForm", afvm);
        }

        [HttpPost("/Home/SearchResultPartialArticle/")]
        public PartialViewResult SearchResultPartialArticle([FromBody] List<Artikel> modelList)
        {
            return PartialView("SearchResultArticle", modelList);
        }

        [HttpPost("/Home/SearchResultPartialCustomer/")]
        public PartialViewResult SearchResultPartialCustomer([FromBody] List<Kunde> modelList)
        {
            return PartialView("SearchResultCustomer", modelList);
        }

        [HttpPost("/Home/SearchResultPartialAssignment/")]
        public PartialViewResult SearchResultPartialAssignment([FromBody] List<AssignmentViewModel> modelList)
        {
            return PartialView("SearchResultAssignment", modelList);
        }

        [HttpGet("/Home/AddCustomerForm/")]
        public PartialViewResult AddCustomerForm()
        {
            return PartialView("CustomerForm");
        }

        [HttpGet("/Home/ShareholderDetailsPartial/{shareholderPK}")]
        public PartialViewResult GetShareholderDetailsPartial(int shareholderPK)
        {
            Gesellschafter? shareholder = _logic.GetShareholderByPK(shareholderPK);

            return PartialView("ShareholderDetails", shareholder);
        }

        [HttpGet("/Home/ShareholderFormPartial/")]
        public PartialViewResult GetShareholderFormPartial()
        {
            List<Gesellschafter>? shareholders = _logic.GetAllShareholders();
            return PartialView("ShareholderForm", shareholders);
        }

        [HttpGet("/Home/AddCustomerDetailsPartial/{customerPK}")]
        public PartialViewResult AddCustomerDetailsPartial(int customerPK)
        {
            Kunde? customer = _logic.GetCustomerByPK(customerPK);
            return PartialView("CustomerDetails", customer);
        }

        [HttpGet("/Home/AddPositionListRowFormPartial/{articlePK}")]
        public PartialViewResult AddPositionListRowFormPartial(int articlePK, int positionNr)
        {
            PositionViewModel? pvm = _logic.GetPositionViewModel(articlePK, positionNr);
            if (pvm?.Position != null)
            {
                pvm.Position.PositionsNummer = positionNr;
            }
            return PartialView("PositionListRowForm", pvm);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion Public Methods
    }
}