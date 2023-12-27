using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Database.Repository
{
    public interface IDbRepository
    {
        public List<Gesellschafter> GetAllShareholders();

        public List<Artikel> GetAllArticles();

        public List<Kunde> GetAllCustomers();

        public Vertriebsmitarbeiter? GetUserByUserId(string userId);

        public Auth? GetAuthByUserPk(int pk_vertriebsmitarbeiter);

        public List<Auftrag>? GetAssignmentsByUserId(string userId);

        public List<AssignmentViewModel> GetAssignmentVMsByUserId(string userId);

        public List<PositionViewModel> GetPositionVMsByUserId(string userId);

        public T? GetObjectByPrimaryKey<T>(T model, int pk) where T : IDatabaseObject;

        public T1? GetObjectByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk) where T1 : IDatabaseObject where T2 : IDatabaseObject;

        public List<T1>? GetObjectListByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk) where T1 : IDatabaseObject where T2 : IDatabaseObject;
    }
}