using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Database.Repository.Interfaces
{
    public interface IHomeRepository
    {
        public List<Gesellschafter>? GetAllShareholdersCached();

        public List<Artikel>? GetAllArticlesCached();

        public List<Kunde>? GetAllCustomers();

        public List<Kunde>? GetAllCustomersCached();

        public Vertriebsmitarbeiter? GetUserFromSession(string userId);

        public List<AssignmentViewModel>? GetAssignmentVMsPaginatedByUserId(string userId, Pagination? pagination);

        public AssignmentFormViewModel? GetAssignmentFormVMByUserId(string userId);

        public PositionViewModel? GetNewPositionVMByArticlePK(int articlePK);

        int SaveAssignmentVM(AssignmentViewModel? avm);

        int SaveCustomer(Kunde? customer);

        void Update<T>(T? model, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject;

        void Update<T>(T? model, string columnToUpdate) where T : IDatabaseObject;
    }
}