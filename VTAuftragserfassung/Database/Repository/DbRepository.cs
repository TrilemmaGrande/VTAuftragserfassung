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

        public List<Auftrag> GetAssignmentsByUserId(string userId) => _dataAccess.GetAllByCondition(new Auftrag(), "*",
            $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter) WHERE MitarbeiterId = '{userId}'");

        public List<AssignmentViewModel> GetAssignmentVMsWithoutPositionsByUserId(string userId)
            => _dataAccess.GetAllByCondition(new AssignmentViewModel(), "vta_Auftrag.*, vta_Kunde.*, vta_Gesellschafter.*, vta_Vertriebsmitarbeiter.MitarbeiterId",
            $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter)" +
            $"INNER JOIN vta_Kunde ON ( vta_Auftrag.FK_Kunde = vta_Kunde.PK_Kunde) " +
            $"INNER JOIN vta_Gesellschafter ON ( vta_Kunde.FK_Gesellschafter = vta_Gesellschafter.PK_Gesellschafter)" +
            $"WHERE MitarbeiterId = '{userId}'");

        public List<PositionViewModel> GetPositionVMsByUserId(string userId) => _dataAccess.GetAllByCondition(new PositionViewModel(), "*",
            $"INNER JOIN vta_Auftrag ON ( vta_Position.FK_Auftrag = vta_Auftrag.PK_Auftrag)" +
            $"INNER JOIN vta_Artikel ON ( vta_Position.FK_Artikel = vta_Artikel.PK_Artikel)" +
            $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter)" +
            $"WHERE MitarbeiterId = '{userId}'");

        public Vertriebsmitarbeiter? GetUserByUserId(string userId) => _dataAccess.GetByCondition(new Vertriebsmitarbeiter(), "*", $"WHERE MitarbeiterId = '{userId}'");

        public Auth? GetAuthByUserPk(int pk_vertriebsmitarbeiter) => _dataAccess.GetByCondition(new Auth(), "*", $"WHERE FK_Vertriebsmitarbeiter = {pk_vertriebsmitarbeiter}");

        public T? GetObjectByPrimaryKey<T>(T model, int pk) where T : IDatabaseObject => _dataAccess.GetByCondition(model, "*", $"WHERE PK_{model.GetType().Name} = {pk}");

        public T1? GetObjectByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => _dataAccess.GetByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        public List<T1>? GetObjectListByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => _dataAccess.GetAllByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        #endregion Public Methods
    }
}