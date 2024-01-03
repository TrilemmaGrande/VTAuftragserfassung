using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Database.Repository
{
    public interface IDbRepository
    {
        public List<Gesellschafter> GetAllShareholdersCached();

        public List<Artikel> GetAllArticlesCached();

        public List<Kunde> GetAllCustomers();

        public List<Kunde> GetAllCustomersCached();

        public Vertriebsmitarbeiter? GetUserByUserId(string userId);

        public Auth? GetAuthByUserPk(int pk_vertriebsmitarbeiter);

        public List<Auftrag>? GetAssignmentsByUserId(string userId);

        public List<AssignmentViewModel> GetAssignmentVMsByUserId(string userId);

        public AssignmentFormViewModel GetAssignmentFormVMByUserId(string userId);

        public List<PositionViewModel> GetPositionVMsByUserId(string userId);

        public PositionViewModel GetNewPositionVMByArticlePK(int articlePK);

        public T? GetObjectByPrimaryKey<T>(T model, int pk) where T : IDatabaseObject;

        public T1? GetObjectByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk) where T1 : IDatabaseObject where T2 : IDatabaseObject;

        public List<T1>? GetObjectListByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk) where T1 : IDatabaseObject where T2 : IDatabaseObject;
    }
}