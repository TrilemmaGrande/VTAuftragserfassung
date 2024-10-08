using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VTAuftragserfassung.Controllers.Home.Interfaces;
using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Home
{
    /// <summary>
    /// [Relationship]: connects View with Logic. 
    /// [Input]: data from View.
    /// [Output]: provides PartialViews.
    /// [Dependencies]: uses IHomeLogic Object to handle business logic.
    /// [Notice]: controller is only accessible when client is authorized.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHomeLogic _logic;

        public HomeController(IHomeLogic logic)
        {
            _logic = logic;
        }

        #region Public Methods

        public IActionResult Dashboard() => View();

        [HttpPost("/Home/AddCreatedCustomerDetailsPartial/")]
        public PartialViewResult AddCreatedCustomerDetailsPartial([FromBody] Kunde customer)
        {
            return PartialView("CustomerDetails", customer);
        }

        [HttpGet("/Home/AddCustomerDetailsPartial/{customerPK}")]
        public PartialViewResult AddCustomerDetailsPartial(int customerPK)
        {
            Kunde? customer = _logic.GetCustomerByPk(customerPK);
            return PartialView("CustomerDetails", customer);
        }

        [HttpGet("/Home/AddCustomerFormPartial/")]
        public PartialViewResult AddCustomerFormPartial()
        {
            return PartialView("CustomerForm");
        }

        [HttpGet("/Home/AddPositionListRowFormPartial/{articlePK}")]
        public PartialViewResult AddPositionListRowFormPartial(int articlePK, int positionNr)
        {
            PositionViewModel? pvm = _logic.GetPositionViewModel(articlePK, positionNr);
            return PartialView("PositionListRowForm", pvm);
        }

        [HttpGet("/Home/GetAssignmentsCount")]
        public int GetAssignmentsCount()
        {
            return _logic.GetAssignmentsCount();
        }

        [HttpPost("/Home/AssignmentsPartial/")]
        public PartialViewResult Assignments([FromBody] Pagination pagination)
        {
            List<AssignmentViewModel>? avm = _logic.GetAssignmentViewModels(pagination);
            return PartialView("Assignments", avm);
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("/Home/PaginationMenuPartial")]
        public PartialViewResult GetPaginationMenuPartial([FromBody] Pagination pagination)
        {
            return PartialView("PaginationMenu", pagination);
        }

        [HttpGet("/Home/ShareholderDetailsPartial/{shareholderPK}")]
        public PartialViewResult GetShareholderDetailsPartial(int shareholderPK)
        {
            Gesellschafter? shareholder = _logic.GetShareholderByPk(shareholderPK);

            return PartialView("ShareholderDetails", shareholder);
        }

        [HttpGet("/Home/AddShareholderFormPartial/")]
        public PartialViewResult AddShareholderFormPartial()
        {
            List<Gesellschafter>? shareholders = _logic.GetAllShareholders();
            return PartialView("ShareholderForm", shareholders);
        }

        [HttpGet("Home/GetUserId")]
        public string GetUserId()
        {
            return _logic.GetUserId();
        }

        [HttpGet("/Home/NewAssignment")]
        public PartialViewResult NewAssignment()
        {
            AssignmentFormViewModel? afvm = _logic.GetAssignmentFormViewModel();
            return PartialView("AssignmentForm", afvm);
        }


        [HttpGet("/Home/SearchResultPartialArticle/{searchTerm}")]
        public List<Artikel> SearchResultPartialArticle(string searchTerm)
        {
            return _logic.GetArticlesBySearchTerm(searchTerm);
        }


        [HttpGet("/Home/SearchResultPartialCustomer/{searchTerm}")]
        public List<Kunde> SearchResultPartialCustomer(string searchTerm)
        {
            return _logic.GetCustomersBySearchTerm(searchTerm);
        }

        [HttpPost("/Home/UpdateAssignmentStatus/{assignmentPK}")]
        public void UpdateAssignmentStatus(int assignmentPK, string assignmentStatus)
        {
            _logic.UpdateAssignmentStatus(assignmentPK, assignmentStatus);
        }

        #endregion Public Methods
    }
}