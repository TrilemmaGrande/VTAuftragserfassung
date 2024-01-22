using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Resources;
using VTAuftragserfassung.Database.Connection.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Extensions;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public class DataAccessService(ISqlConnector _conn, ResourceManager _resM) : IDataAccessService
    {

        #region Public Methods

        public int ReadScalar(string cmd) => Convert.ToInt32(_conn.ConnectionReadScalar(cmd));

        public void CreateAll<T>(List<T>? dbModels) where T : IDatabaseObject
        {
            if (dbModels == null || dbModels.Count == 0)
            {
                return;
            }

            List<Tuple<string, SqlParameter[]?>> queryList = new();
            foreach (var dbModel in dbModels)
            {
                string cmd = CreateInsertString(dbModel);
                SqlParameter[]? parameters = GenerateParameters(dbModel);
                queryList.Add(new Tuple<string, SqlParameter[]?>(cmd, parameters));
            }
            _conn.ConnectionWrite(queryList);
        }

        public int CreateSingle<T>(T? dbModel) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return 0;
            }

            string cmd = CreateInsertString(dbModel);
            cmd += _resM.GetQuery("SELECT_IDENTITY") ?? string.Empty;
            SqlParameter[]? parameters = GenerateParameters(dbModel);

            return _conn.ConnectionWrite(cmd, parameters!);
        }

        public List<T>? ReadAll<T>(string cmd) where T : IDatabaseObject
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

        public T? ReadSingle<T>(T? dbModel, string cmd) where T : IDatabaseObject
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

        public void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return;
            }
            string cmd = CreateUpdateString(dbModel, columnsToUpdate);
            cmd += _resM.GetQuery("SELECT_IDENTITY") ?? string.Empty;
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
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "TableName" && prop.Name != dbModel.PrimaryKeyColumn);

            string tableName = dbModel.TableName;
            string columns = string.Join(", ", properties.Where(prop => prop.Name != "PrimaryKeyColumn").Select(prop => prop.Name));
            string values = string.Join(", ", properties.Where(prop => prop.Name != "PrimaryKeyColumn").Select(prop => $"@{prop.Name}"));

            return _resM.GetQuery("INSERT", tableName, columns, values) ?? string.Empty;
        }

        private string CreateUpdateString<T>(T? dbModel, IEnumerable<string>? columnsToUpdate) where T : IDatabaseObject
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
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PrimaryKeyColumn" && prop.Name != "TableName" && prop.Name != dbModel.PrimaryKeyColumn)
                .Where(prop => prop.Name != dbModel.TableName)
                .Select(FormatPropertyForDatabase(dbModel))
                .ToArray();

            return parameters;
        }

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