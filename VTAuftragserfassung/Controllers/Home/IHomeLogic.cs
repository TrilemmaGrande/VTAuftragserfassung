using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Controllers.Home
{
    public interface IHomeLogic
    {
        #region Public Methods

        public List<AssignmentViewModel> GetAssignmentViewModels();
        public AssignmentFormViewModel GetAssignmentFormViewModel();

        public void Logout();

        #endregion Public Methods
    }
}