using VTAuftragserfassung.Database.DataAccess.Services;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Database.DataAccess
{
    public class DatabaseAccess : IDataAccess<IDatabaseObject>
    {
        #region Private Fields

        private readonly IDataAccessService _dbAccess;

        #endregion Private Fields

        #region Public Constructors

        public DatabaseAccess(IDataAccessService dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public int Create<T>(T? dbModel) where T : IDatabaseObject
        {
            return _dbAccess.Create(dbModel);
        }

        public void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject
        {
            _dbAccess.CreateAll(dbModels);
        }

        #endregion Public Constructors

        #region Public Methods

        public List<T>? ReadAll<T>(T? dbModel) where T : IDatabaseObject
            => dbModel != null ? _dbAccess.ReadAll<T>($"SELECT * FROM {dbModel.TableName}") : null;

        public List<Auftrag>? ReadAssignmentsPaginatedByUserId(string userId, Pagination? pagination)
           => !string.IsNullOrEmpty(userId) && pagination != null && pagination.Page > 0 
            ? _dbAccess.ReadAll<Auftrag>($"SELECT * FROM vta_Auftrag " +
                  $"INNER JOIN vta_Vertriebsmitarbeiter ON (vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter) " +
                  $"WHERE MitarbeiterId = '{userId}' " +
                  $"ORDER BY ErstelltAm DESC, LetzteStatusAenderung DESC, PK_Auftrag DESC " +
                  $"OFFSET {pagination.Offset} ROWS " +
                  $"FETCH NEXT {pagination.LinesPerPage} ROWS ONLY") 
            : null;

        public T1? ReadObjectByForeignKey<T1, T2>(T1? dbModel, T2? foreignModel, int fk)
                    where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => dbModel != null && foreignModel != null ? ReadByCondition(dbModel, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}") : default;

        public T? ReadObjectByPrimaryKey<T>(T? dbModel, int pk) where T : IDatabaseObject
            => dbModel != null ? ReadByCondition(dbModel, "*", $"WHERE PK_{dbModel.GetType().Name} = {pk}") : default;

        public List<T1>? ReadObjectListByForeignKey<T1, T2>(T1? dbModel, T2? foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => dbModel != null && foreignModel != null ? ReadAllByCondition(dbModel, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}") : default;

        public List<PositionViewModel>? ReadPositionVMsByAssignmentPKs(List<int>? assignmentPKs)
            => assignmentPKs != null && assignmentPKs.Any() ? ReadAllByCondition(new PositionViewModel(), "*",
                    $"INNER JOIN vta_Auftrag ON ( vta_Position.FK_Auftrag = vta_Auftrag.PK_Auftrag)" +
                    $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter)" +
                    $"WHERE FK_Auftrag IN ({string.Join(",", assignmentPKs)})") : null;

        public Vertriebsmitarbeiter? ReadUserByUserId(string userId)
            => !string.IsNullOrEmpty(userId) ? ReadByCondition(new Vertriebsmitarbeiter(), "*", $"WHERE MitarbeiterId = '{userId}'") : null;

        public void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject
        {
            _dbAccess.Update(dbModel, columnsToUpdate);
        }

        #endregion Public Methods

        #region Private Methods

        private List<T>? ReadAllByCondition<T>(T? dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => dbModel != null ? _dbAccess.ReadAll<T>($"SELECT {getterColumn} FROM {dbModel.TableName} {condition}") : default;

        private T? ReadByCondition<T>(T? dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => dbModel != null ? _dbAccess.ReadSingle(dbModel, $"SELECT TOP 1 {getterColumn} FROM {dbModel.TableName} {condition}") : default;

       

        #endregion Private Methods
    }
}