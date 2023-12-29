using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Controllers.Home
{
    public interface IHomeLogic
    {
        #region Public Methods

        public List<AssignmentViewModel> GetAssignmentViewModels();

        public AssignmentFormViewModel GetAssignmentFormViewModel();

        public PositionViewModel GetPositionViewModel(int articlePK);

        public void Logout();

        #endregion Public Methods
    }
}