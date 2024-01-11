using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Controllers.Home
{
    public interface IHomeLogic
    {
        #region Public Methods

        public List<AssignmentViewModel> GetAssignmentViewModels();

        public AssignmentFormViewModel GetAssignmentFormViewModel();

        public Artikel GetArticleByPK(int articlePK);

        public Kunde GetCustomerByPK(int customerPK);

        public PositionViewModel GetPositionViewModel(int articlePK);

        public int CreateAssignment(AssignmentViewModel avm);

        int CreateCustomer(Kunde customer);

        public void Logout();

        Gesellschafter GetShareholderByPK(int shareholderPK);

        List<Gesellschafter> GetAllShareholders();
        void UpdateAssignmentStatus(int assignmentPK, string assignmentStatus);

        #endregion Public Methods
    }
}