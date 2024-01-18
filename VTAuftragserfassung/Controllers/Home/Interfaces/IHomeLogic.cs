using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Home.Interfaces
{
    public interface IHomeLogic
    {
        #region Public Methods

        string GetUserId();

        List<AssignmentViewModel>? GetAssignmentViewModels(Pagination pagination);

        AssignmentFormViewModel? GetAssignmentFormViewModel();

        Artikel? GetArticleByPK(int articlePK);

        Kunde? GetCustomerByPK(int customerPK);

        PositionViewModel? GetPositionViewModel(int articlePK, int positionNr);

        int CreateAssignment(AssignmentViewModel avm);

        int CreateCustomer(Kunde? customer);

        void Logout();

        Gesellschafter? GetShareholderByPK(int shareholderPK);

        List<Gesellschafter>? GetAllShareholders();

        void UpdateAssignmentStatus(int assignmentPK, string assignmentStatus);

        #endregion Public Methods
    }
}