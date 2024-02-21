using System.Reflection;
using System.Resources;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Extensions;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    /// <summary>
    /// [Relationship]: connects Database with Repositories
    /// [Input]: DataBaseObjects.
    /// [Output]: DataBaseObjects.
    /// [Dependencies]: uses IDatabaseAccess for CRUD-requests to Server and ResourceManager for query-building by .resX file.
    /// [Notice]: builds specified SQL Query strings and delegates CRUD functionality to IDatabaseAccess. 
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        #region Private Fields

        private readonly IDatabaseAccess _dbAccess;
        private readonly ResourceManager _resM;

        #endregion Private Fields

        #region Public Constructors

        public DatabaseService(IDatabaseAccess dbAccess, ResourceManager resM)
        {
            _dbAccess = dbAccess;
            _resM = resM;
        }

        #endregion Public Constructors

        #region Public Methods

        public int CountAssignmentsByUserId(string userId)
          => !string.IsNullOrEmpty(userId)
          ? CountDataSetByCondition(new Auftrag(), "*", _resM.GetQuery("WHERE_MitarbeiterId", userId) ?? string.Empty)
          : default;

        public void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject
        {
            if (dbModels == null)
            {
                return;
            }
            string cmd;
            List<KeyValuePair<T, string>> dbModelsWithCmds = new();
            foreach (var model in dbModels)
            {
                cmd = BuildInsertString(model);
                dbModelsWithCmds.Add(new KeyValuePair<T, string>(model, cmd));
            }
            _dbAccess.CreateAll(dbModelsWithCmds);
        }

        public int CreateSingle<T>(T? dbModel) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return 0;
            }
            string cmd = BuildInsertString(dbModel);
            cmd += _resM.GetQuery("SELECT_IDENTITY") ?? string.Empty;
            return _dbAccess.CreateSingle(dbModel, cmd);
        }

        public List<T>? ReadAll<T>(T? dbModel) where T : IDatabaseObject
            => dbModel != null
            ? _dbAccess.ReadAll<T>(_resM.GetQuery("SELECT_*", dbModel.TableName) ?? string.Empty)
            : null;

        public List<Auftrag>? ReadAssignmentsPaginatedByUserId(string userId, Pagination? pagination)
            => !string.IsNullOrEmpty(userId) && pagination != null && pagination.Page > 0
            ? _dbAccess.ReadAll<Auftrag>(_resM.GetQuery("SelectAssignmentsPaginatedByUserId", userId, pagination.Offset, pagination.LinesPerPage) ?? string.Empty)
            : null;

        public T1? ReadObjectByForeignKey<T1, T2>(T1? dbModel, T2? foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => dbModel != null && foreignModel != null
            ? ReadByCondition(dbModel, "*", _resM.GetQuery("WHERE_FK", foreignModel.GetType().Name, fk) ?? string.Empty)
            : default;

        public T? ReadObjectByPrimaryKey<T>(T? dbModel, int pk) where T : IDatabaseObject
            => dbModel != null
            ? ReadByCondition(dbModel, "*", _resM.GetQuery("WHERE_PK", dbModel.PrimaryKeyColumn, pk) ?? string.Empty)
            : default;

        public List<T1>? ReadObjectListByForeignKey<T1, T2>(T1? dbModel, T2? foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => dbModel != null && foreignModel != null
            ? ReadAllByCondition(dbModel, "*", _resM.GetQuery("WHERE_FK", foreignModel.GetType().Name, fk) ?? string.Empty)
            : default;

        public List<Position>? ReadPositionsByAssignmentPKs(List<int>? assignmentPKs)
            => assignmentPKs != null && assignmentPKs.Any()
            ? ReadAllByCondition(new Position(), "*", _resM.GetQuery("SelectPositionsByAssignmentPKs", string.Join(",", assignmentPKs)) ?? string.Empty)
            : null;

        public Vertriebsmitarbeiter? ReadUserByUserId(string userId)
            => !string.IsNullOrEmpty(userId)
            ? ReadByCondition(new Vertriebsmitarbeiter(), "*", _resM.GetQuery("WHERE_MitarbeiterId", userId) ?? string.Empty)
            : null;

        public void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject
        {
            if (dbModel == null || columnsToUpdate == null)
            {
                return;
            }
            string cmd = BuildUpdateString(dbModel, columnsToUpdate);
            _dbAccess.Update(dbModel, cmd);
        }

        #endregion Public Methods

        #region Private Methods

        private int CountDataSetByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => dbModel != null
            ? _dbAccess.ReadScalar(_resM.GetQuery("SELECT_COUNT_ByUserId", getterColumn, dbModel.TableName, condition.Trim('"')) ?? string.Empty)
            : default;

        private List<T>? ReadAllByCondition<T>(T? dbModel, string getterColumn, string condition) where T : IDatabaseObject
                    => dbModel != null
            ? _dbAccess.ReadAll<T>(_resM.GetQuery("SELECT", getterColumn, dbModel.TableName, condition.Trim('"')) ?? string.Empty)
            : default;

        private T? ReadByCondition<T>(T? dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => dbModel != null
            ? _dbAccess.ReadSingle(dbModel, _resM.GetQuery("SELECT_TOP_1", getterColumn, dbModel.TableName, condition.Trim('"')) ?? string.Empty)
            : default;

        private string BuildInsertString<T>(T? dbModel) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return string.Empty;
            }

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "TableName" && prop.Name != dbModel.PrimaryKeyColumn);

            string tableName = dbModel.TableName;
            string columns = string.Join(", ", properties.Where(prop => prop.Name != "PrimaryKeyColumn").Select(prop => prop.Name));
            string values = string.Join(", ", properties.Where(prop => prop.Name != "PrimaryKeyColumn").Select(prop => $"@{prop.Name}"));

            return _resM.GetQuery("INSERT", tableName, columns, values) ?? string.Empty;
        }

        private string BuildUpdateString<T>(T? dbModel, IEnumerable<string>? columnsToUpdate) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return string.Empty;
            }

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "TableName" && prop.Name != dbModel.PrimaryKeyColumn);

            int modelPk = (int)(typeof(T).GetProperty(dbModel.PrimaryKeyColumn)?.GetValue(dbModel) ?? 0);
            string tableName = dbModel.TableName;

            IEnumerable<PropertyInfo> propertiesToUpdate = columnsToUpdate != null
                ? properties.Where(prop => columnsToUpdate.Contains(prop.Name))
                : properties.Skip(1);

            string setClause = string.Join(", ", propertiesToUpdate.Select(prop => $"{prop.Name} = @{prop.Name}"));

            return _resM.GetQuery("UPDATE", tableName, setClause, dbModel.PrimaryKeyColumn, modelPk) ?? string.Empty;
        }

        #endregion Private Methods
    }
}