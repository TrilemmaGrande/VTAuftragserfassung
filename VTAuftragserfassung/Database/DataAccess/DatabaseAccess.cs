using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using VTAuftragserfassung.Database.Connection;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Database.DataAccess
{
    public class DatabaseAccess : IDataAccess<IDatabaseObject>
    {
        #region Private Fields

        private readonly ISqlConnector _conn;

        #endregion Private Fields

        #region Public Constructors

        public DatabaseAccess(ISqlConnector conn)
        {
            _conn = conn;
        }

        #endregion Public Constructors

        #region Public Methods

        public int Create<T>(T? dbModel) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return 0;
            }

            string cmd = CreateInsertString(dbModel);
            SqlParameter[]? parameters = GenerateParameters(dbModel);

            return _conn.ConnectionWriteGetPrimaryKey(cmd, parameters!);
        }

        public void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject
        {
            if (dbModels == null || dbModels.Count == 0)
            {
                return;
            }

            foreach (var dbModel in dbModels)
            {
                string cmd = CreateInsertString(dbModel);
                SqlParameter[]? parameters = GenerateParameters(dbModel);

                _conn.ConnectionWrite(cmd, parameters);
            }
        }

        public List<T>? ReadAll<T>(T? dbModel) where T : IDatabaseObject
            => dbModel != null ? ReadAll<T>($"SELECT * FROM {dbModel.TableName}") : null;

        public List<Auftrag>? ReadAssignmentsPaginatedByUserId(string userId, Pagination? pagination)
           => !string.IsNullOrEmpty(userId) && pagination != null && pagination.Page > 0 ? ReadAllByCondition(new Auftrag(), "*",
                  $"INNER JOIN vta_Vertriebsmitarbeiter ON (vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter) " +
                  $"WHERE MitarbeiterId = '{userId}' " +
                  $"ORDER BY ErstelltAm DESC, LetzteStatusAenderung DESC, PK_Auftrag DESC " +
                  $"OFFSET {pagination.Offset} ROWS " +
                  $"FETCH NEXT {pagination.LinesPerPage} ROWS ONLY") : null;

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
            if (dbModel == null)
            {
                return;
            }
            string cmd = CreateUpdateString(dbModel, columnsToUpdate);
            SqlParameter[]? parameters = GenerateParameters(dbModel);

            _conn.ConnectionWrite(cmd, parameters);
        }

        #endregion Public Methods

        #region Private Methods

        private string CreateInsertString<T>(T? dbModel) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return string.Empty;
            }
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + typeof(T).Name);

            string tableName = properties.First().GetValue(dbModel)?.ToString()?.Trim('\'') ?? "";
            string columns = string.Join(", ", properties.Skip(1).Select(prop => prop.Name));
            string values = string.Join(", ", properties.Skip(1).Select(prop => $"@{prop.Name}"));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values});";
        }

        private string CreateUpdateString<T>(T? dbModel, IEnumerable<string>? columnsToUpdate) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return string.Empty;
            }

            string modelName = typeof(T).Name;

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + modelName);

            int modelPk = (int)(typeof(T).GetProperty("PK_" + modelName)?.GetValue(dbModel) ?? 0);
            string tableName = properties.First().GetValue(dbModel)?.ToString()?.Trim('\'') ?? "";

            IEnumerable<PropertyInfo> propertiesToUpdate = columnsToUpdate != null
                ? properties.Where(prop => columnsToUpdate.Contains(prop.Name))
                : properties.Skip(1);

            string setClause = string.Join(", ", propertiesToUpdate.Select(prop => $"{prop.Name} = @{prop.Name}"));

            return $"UPDATE {tableName} SET {setClause} WHERE PK_{modelName} = {modelPk};";
        }

        private Func<PropertyInfo, SqlParameter> FormatPropertyForDatabase<T>(T? dbModel) where T : IDatabaseObject
        {
            return prop =>
            {
                string paramName = $"@{prop.Name}";
                object? propValue = prop.GetValue(dbModel);

                if (propValue == null || propValue == DBNull.Value)
                {
                    return new SqlParameter(paramName, DBNull.Value);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    return new SqlParameter(paramName, (bool)propValue ? 1 : 0);
                }
                else if (prop.PropertyType.IsEnum)
                {
                    return new SqlParameter(paramName, (int)propValue);
                }
                else
                {
                    return new SqlParameter(paramName, propValue);
                }
            };
        }

        private SqlParameter[]? GenerateParameters<T>(T? dbModel) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return null;
            }

            var parameters = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + typeof(T).Name)
                .Skip(1)
                .Select(FormatPropertyForDatabase(dbModel))
                .ToArray();

            return parameters;
        }

        private T? Read<T>(T? dbModel, string cmd) where T : IDatabaseObject
        {
            if (dbModel == null || string.IsNullOrEmpty(cmd))
            {
                return default;
            }
            DataTable? dt = _conn.ConnectionRead(cmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                PropertyInfo[] properties = dbModel.GetType().GetProperties();
                DataRow dr = dt.Rows[0];

                T obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in dr.Table.Columns)
                {
                    foreach (PropertyInfo property in properties)
                    {
                        if (column.ColumnName == property.Name)
                        {
                            property.SetValue(obj, dr[column.ColumnName]);
                        }
                    }
                }
                return obj;
            }
            return default;
        }

        private List<T>? ReadAll<T>(string cmd) where T : IDatabaseObject
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return default;
            }
            List<T> listOfT = [];
            DataTable? dt = _conn.ConnectionRead(cmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T? obj = SetProperties(dr, Activator.CreateInstance<T>());
                    if (obj != null)
                    {
                        listOfT.Add(obj);
                    }
                }
                return listOfT;
            }
            return default;
        }

        private List<T>? ReadAllByCondition<T>(T? dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => dbModel != null ? ReadAll<T>($"SELECT {getterColumn} FROM {dbModel.TableName} {condition}") : default;

        private T? ReadByCondition<T>(T? dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => dbModel != null ? Read(dbModel, $"SELECT TOP 1 {getterColumn} FROM {dbModel.TableName} {condition}") : default;

        private T? SetProperties<T>(DataRow? dr, T? obj)
        {
            List<PropertyInfo>? properties = obj?.GetType().GetProperties().ToList();
            if (properties == null || properties.Count == 0 || dr == null)
            {
                return default;
            }
            properties.ForEach(property =>
            {
                if (property.PropertyType.IsClass && property.PropertyType.Namespace!.Contains(GetType().Namespace!.Split('.')[0]))
                {
                    property.SetValue(obj, SetProperties(dr, Activator.CreateInstance(property.PropertyType)));
                }
                else
                {
                    foreach (DataColumn column in dr.Table.Columns)
                    {
                        if (column.ColumnName == property.Name)
                        {
                            property.SetValue(obj, dr[column.ColumnName]);
                        }
                    }
                }
            });
            return obj;
        }

        #endregion Private Methods
    }
}