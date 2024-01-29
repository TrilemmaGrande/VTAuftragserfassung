using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Home.Interfaces
{
    public interface IHomeLogic
    {
        #region Public Methods

        string GetUserId();

        int GetAssignmentsCount();

        List<AssignmentViewModel>? GetAssignmentViewModels(Pagination pagination);

        AssignmentFormViewModel? GetAssignmentFormViewModel();

        Kunde? GetCustomerByPk(int customerPK);

        PositionViewModel? GetPositionViewModel(int articlePK, int positionNr);

        int CreateAssignment(AssignmentViewModel avm);

        int CreateCustomer(Kunde? customer);

        Gesellschafter? GetShareholderByPk(int shareholderPK);

        List<Gesellschafter>? GetAllShareholders();

        void UpdateAssignmentStatus(int assignmentPK, string assignmentStatus);
        List<Artikel> GetArticlesBySearchTerm(string searchTerm);
        List<Kunde> GetCustomersBySearchTerm(string searchTerm);

        #endregion Public Methods
    }
}