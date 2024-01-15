using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Home
{
    public interface IHomeLogic
    {
        #region Public Methods
        public string GetUserId();
        public List<AssignmentViewModel>? GetAssignmentViewModels(Pagination pagination);

        public AssignmentFormViewModel? GetAssignmentFormViewModel();

        public Artikel? GetArticleByPK(int articlePK);

        public Kunde? GetCustomerByPK(int customerPK);

        public PositionViewModel? GetPositionViewModel(int articlePK, int positionNr);

        public int CreateAssignment(AssignmentViewModel? avm);

        int CreateCustomer(Kunde? customer);

        public void Logout();

        Gesellschafter? GetShareholderByPK(int shareholderPK);

        List<Gesellschafter>? GetAllShareholders();
        void UpdateAssignmentStatus(int assignmentPK, string assignmentStatus);

        #endregion Public Methods
    }
}