using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Database.Repository.Interfaces
{
    public interface IHomeRepository
    {
        #region Public Methods

        List<Artikel>? GetAllArticlesCached();

        List<Kunde>? GetAllCustomers();

        List<Kunde>? GetAllCustomersCached();

        List<Gesellschafter>? GetAllShareholdersCached();

        AssignmentFormViewModel? GetAssignmentFormVMByUserId(string userId);

        List<AssignmentViewModel>? GetAssignmentVMsPaginatedByUserId(string userId, Pagination? pagination);

        PositionViewModel? GetNewPositionVMByArticlePK(int articlePK);

        Vertriebsmitarbeiter? GetUserFromSession(string userId);

        int SaveAssignmentVM(AssignmentViewModel? avm);

        int SaveCustomer(Kunde? customer);

        void Update<T>(T? model, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject;

        void Update<T>(T? model, string columnToUpdate) where T : IDatabaseObject;

        #endregion Public Methods
    }
}