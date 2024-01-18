using System.Resources;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;

namespace VTAuftragserfassung.Database.DataAccess
{
    public class DatabaseAccess : IDataAccess<IDatabaseObject>
    {
        #region Private Fields

        private readonly IDataAccessService _dbAccess;
        private readonly ResourceManager _resM;

        #endregion Private Fields

        #region Public Constructors

        public DatabaseAccess(IDataAccessService dbAccess, ResourceManager resM)
        {
            _resM = resM;
            _dbAccess = dbAccess;
        }

        public int Create<T>(T? dbModel) where T : IDatabaseObject
        {
            return _dbAccess.CreateSingle(dbModel);
        }

        public void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject
        {
            _dbAccess.CreateAll(dbModels);
        }

        #endregion Public Constructors

        #region Public Methods

        public List<T>? ReadAll<T>(T? dbModel) where T : IDatabaseObject
            => dbModel != null ? _dbAccess.ReadAll<T>(string.Format(_resM.GetString("SELECT_*") ?? string.Empty, dbModel.TableName)) : null;

        public List<Auftrag>? ReadAssignmentsPaginatedByUserId(string userId, Pagination? pagination)
           => !string.IsNullOrEmpty(userId) && pagination != null && pagination.Page > 0
            ? _dbAccess.ReadAll<Auftrag>(string.Format(_resM.GetString("SelectAssignmentsPaginatedByUserId") ?? string.Empty, userId, pagination.Offset, pagination.LinesPerPage))
            : null;

        public T1? ReadObjectByForeignKey<T1, T2>(T1? dbModel, T2? foreignModel, int fk)
                    where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => dbModel != null && foreignModel != null ? ReadByCondition(dbModel, "*", string.Format(_resM.GetString("WHERE_FK") ?? string.Empty, foreignModel.GetType().Name, fk)) : default;

        public T? ReadObjectByPrimaryKey<T>(T? dbModel, int pk) where T : IDatabaseObject
            => dbModel != null ? ReadByCondition(dbModel, "*", $"WHERE {dbModel.PrimaryKeyColumn} = {pk}") : default;

        public List<T1>? ReadObjectListByForeignKey<T1, T2>(T1? dbModel, T2? foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => dbModel != null && foreignModel != null ? ReadAllByCondition(dbModel, "*", string.Format(_resM.GetString("WHERE_FK") ?? string.Empty, foreignModel.GetType().Name, fk)) : default;

        public List<Position>? ReadPositionsByAssignmentPKs(List<int>? assignmentPKs)
            => assignmentPKs != null && assignmentPKs.Any() ? ReadAllByCondition(new Position(), "*",
                string.Format(_resM.GetString("SelectPositionsByAssignmentPKs") ?? string.Empty, string.Join(",", assignmentPKs))) : null;

        public Vertriebsmitarbeiter? ReadUserByUserId(string userId)
            => !string.IsNullOrEmpty(userId) ? ReadByCondition(new Vertriebsmitarbeiter(), "*", string.Format(_resM.GetString("WHERE_MitarbeiterId") ?? string.Empty, userId)) : null;

        public void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject
        {
            _dbAccess.Update(dbModel, columnsToUpdate);
        }

        #endregion Public Methods

        #region Private Methods

        private List<T>? ReadAllByCondition<T>(T? dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => dbModel != null ? _dbAccess.ReadAll<T>(string.Format(_resM.GetString("SELECT") ?? string.Empty, getterColumn, dbModel.TableName, condition.Trim('"'))) : default;

        private T? ReadByCondition<T>(T? dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => dbModel != null ? _dbAccess.ReadSingle(dbModel, string.Format(_resM.GetString("SELECT_TOP_1") ?? string.Empty, getterColumn, dbModel.TableName, condition.Trim('"'))) : default;

       

        #endregion Private Methods
    }
}