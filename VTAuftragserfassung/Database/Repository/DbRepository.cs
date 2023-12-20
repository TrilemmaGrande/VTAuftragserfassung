using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Database.Repository
{
    public class DbRepository : IDbRepository
    {
        #region Public Fields

        public readonly IDataAccess<IDatabaseObject> _dataAccess;

        #endregion Public Fields

        #region Public Constructors

        public DbRepository(IDataAccess<IDatabaseObject> dataAccess)
        {
            this._dataAccess = dataAccess;
        }

        #endregion Public Constructors

        #region Public Methods

        public List<IDatabaseObject> GetAll(IDatabaseObject dbModel) => _dataAccess.GetAll(dbModel);

        public List<Auftrag> GetAssignmentsByUserId(string userId) => _dataAccess.GetByCondition(new Auftrag(), "*", $" " +
            $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter) WHERE MitarbeiterId = '{userId}'").ToList();

        public Vertriebsmitarbeiter? GetUserByUserId(string userId) => _dataAccess.GetByCondition(new Vertriebsmitarbeiter(), "*", $"WHERE MitarbeiterId = '{userId}'").FirstOrDefault();

        public Auth? GetAuthByUserPk(int pk_vertriebsmitarbeiter) => _dataAccess.GetByCondition(new Auth(), "*", $"WHERE FK_Vertriebsmitarbeiter = {pk_vertriebsmitarbeiter}").FirstOrDefault();

        public T? GetObjectByPrimaryKey<T>(T model, int pk) where T : IDatabaseObject => _dataAccess.GetByCondition(model, "*", $"WHERE PK_{model.GetType().Name} = {pk}").FirstOrDefault()!;

        public T1? GetObjectByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
          where T1 : IDatabaseObject
          where T2 : IDatabaseObject
           => _dataAccess.GetByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}")!.FirstOrDefault()!;

        public List<T1>? GetObjectListByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => _dataAccess.GetByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        #endregion Public Methods
    }
}