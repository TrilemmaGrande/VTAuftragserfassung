using System.Security.Claims;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Database.Repository.Interfaces
{
    public interface IHomeRepository
    {
        bool CheckUserAssignmentMapping(string userId, int assignmentPk);
        #region Public Methods

        List<Artikel>? GetAllArticlesCached();

        List<Kunde>? GetAllCustomers();

        List<Kunde>? GetAllCustomersCached();

        List<Gesellschafter>? GetAllShareholdersCached();

        AssignmentFormViewModel? GetAssignmentFormVMByUserId(string userId);

        int GetAssignmentsCount(string userId);

        List<AssignmentViewModel>? GetAssignmentVMsPaginatedByUserId(string userId, Pagination? pagination);

        PositionViewModel? GetNewPositionVMByArticlePK(int articlePK);

        ClaimsPrincipal? GetSessionUser();

        Vertriebsmitarbeiter? GetUserFromSession(string userId);

        int SaveAssignmentVM(AssignmentViewModel? avm);

        int SaveCustomer(Kunde? customer);

        void Update<T>(T? model, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject;

        #endregion Public Methods
    }
}