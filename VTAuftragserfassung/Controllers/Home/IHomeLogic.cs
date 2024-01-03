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

        public void CreateAssignment(AssignmentViewModel avm);

        public void Logout();
        Gesellschafter GetShareholderByPK(int shareholderPK);
        List<Gesellschafter> GetAllShareholders();

        #endregion Public Methods
    }
}