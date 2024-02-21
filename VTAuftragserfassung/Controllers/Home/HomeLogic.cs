using System.Text.Json;
using VTAuftragserfassung.Controllers.Home.Interfaces;
using VTAuftragserfassung.Database.Repository.Interfaces;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Enum;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Home
{
    /// <summary>
    /// [Relationship]: connects Controller with Repository
    /// [Input]: data from Controller
    /// [Output]: result-object from business logic.
    /// [Dependencies]: uses IHomeRepository Object to access backend data.
    /// [Notice]: -
    /// </summary>
    public class HomeLogic : IHomeLogic
    {
        #region Private Fields

        private readonly IHomeRepository _repo;

        #endregion Private Fields

        #region Public Constructors

        public HomeLogic(IHomeRepository repo)
        {
            _repo = repo;
        }

        #endregion Public Constructors

        #region Private Properties

        private string _userId => _repo.GetSessionUser()?.Identity?.Name ?? string.Empty;

        #endregion Private Properties

        #region Public Methods

        public int CreateAssignment(AssignmentViewModel avm)
        {
            Vertriebsmitarbeiter? salesStaff = _repo.GetUserFromSession(_userId);
            if (avm?.Auftrag == null || salesStaff?.PK_Vertriebsmitarbeiter == null)
            {
                return 0;
            }
            int salesStaffPK = salesStaff.PK_Vertriebsmitarbeiter;
            avm.Auftrag.FK_Vertriebsmitarbeiter = salesStaffPK;
            int assignmentPK = _repo.SaveAssignmentVM(avm);
            return assignmentPK;
        }

        public int CreateCustomer(Kunde? customer) => customer != null ? _repo.SaveCustomer(customer) : 0;

        public List<Gesellschafter>? GetAllShareholders() => _repo.GetAllShareholdersCached();

        public List<Artikel> GetArticlesBySearchTerm(string searchTerm)
        {
            List<Artikel>? searchTarget = _repo.GetAllArticlesCached() ?? new();
            return GetModelsBySearchterm(searchTerm, searchTarget);
        }

        public AssignmentFormViewModel? GetAssignmentFormViewModel() => !string.IsNullOrEmpty(_userId) ? _repo.GetAssignmentFormVMByUserId(_userId) : null;

        public int GetAssignmentsCount() => !string.IsNullOrEmpty(_userId) ? _repo.GetAssignmentsCount(_userId) : default;

        public List<AssignmentViewModel>? GetAssignmentViewModels(Pagination? pagination) =>
                                                    (!string.IsNullOrEmpty(_userId) && pagination != null && pagination.Page > 0)
                ? _repo.GetAssignmentVMsPaginatedByUserId(_userId, pagination)
                : null;

        public Kunde? GetCustomerByPk(int customerPK) => _repo.GetAllCustomersCached()?.Find(i => i.PK_Kunde == customerPK);

        public List<Kunde> GetCustomersBySearchTerm(string searchTerm)
        {
            List<Kunde>? searchTarget = _repo.GetAllCustomersCached() ?? new();
            return GetModelsBySearchterm(searchTerm, searchTarget);
        }

        public PositionViewModel? GetPositionViewModel(int articlePK, int positionNr)
        {
            PositionViewModel? pvm = _repo.GetNewPositionVMByArticlePK(articlePK);
            if (pvm?.Position == null)
            {
                return null;
            }
            pvm.Position.PositionsNummer = positionNr;
            return pvm;
        }

        public Gesellschafter? GetShareholderByPk(int shareholderPK) => _repo.GetAllShareholdersCached()?.Find(i => i.PK_Gesellschafter == shareholderPK);

        public string GetUserId() => _userId;

        public void UpdateAssignmentStatus(int assignmentPK, string assignmentStatus)
        {
            if (!Enum.TryParse(assignmentStatus, out Auftragsstatus status) || assignmentPK <= 0)
            {
                return;
            }
            Auftrag? assignment = new() { PK_Auftrag = assignmentPK, Auftragsstatus = status };
            List<string> columnsToUpdate = new() { "Auftragsstatus" };
            _repo.Update(assignment, columnsToUpdate.AsEnumerable<string>());
        }

        #endregion Public Methods

        #region Private Methods

        private static List<T> GetModelsBySearchterm<T>(string searchTerm, List<T> modelList)
        {
            searchTerm = searchTerm.ToLower();
            return modelList
                      .Where(model =>
                      {
                          var json = JsonSerializer.Serialize(model);
                          return json.ToLower().Contains(searchTerm);
                      }).Take(5000)
                      .ToList();
        }

        #endregion Private Methods
    }
}