using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using VTAuftragserfassung.Database.Connection;

namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public class DataAccessService : IDataAccessService
    {
        #region Private Fields

        private readonly ISqlConnector _conn;

        #endregion Private Fields

        #region Public Constructors

        public DataAccessService(ISqlConnector conn)
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

            StringBuilder queryBuilder = new();

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + typeof(T).Name);

            string tableName = properties.First().GetValue(dbModel)?.ToString()?.Trim('\'') ?? "";
            string columns = string.Join(", ", properties.Skip(1).Select(prop => prop.Name));
            string values = string.Join(", ", properties.Skip(1).Select(prop => $"@{prop.Name}"));

            queryBuilder.Append("INSERT INTO ")
                .Append(tableName)
                .Append(" (")
                .Append(columns)
                .Append(") VALUES (")
                .Append(values)
                .Append(");");

            return queryBuilder.ToString();
        }

        private string CreateUpdateString<T>(T? dbModel, IEnumerable<string>? columnsToUpdate) where T : IDatabaseObject
        {
            if (dbModel == null)
            {
                return string.Empty;
            }

            string modelName = typeof(T).Name;
            StringBuilder queryBuilder = new();

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + modelName);

            int modelPk = (int)(typeof(T).GetProperty("PK_" + modelName)?.GetValue(dbModel) ?? 0);
            string tableName = properties.First().GetValue(dbModel)?.ToString()?.Trim('\'') ?? "";

            IEnumerable<PropertyInfo> propertiesToUpdate = columnsToUpdate != null
                ? properties.Where(prop => columnsToUpdate.Contains(prop.Name))
                : properties.Skip(1);

            string setClause = string.Join(", ", propertiesToUpdate.Select(prop => $"{prop.Name} = @{prop.Name}"));

            queryBuilder.Append("UPDATE ")
              .Append(tableName)
              .Append(" SET ")
              .Append(setClause)
              .Append(" WHERE PK_")
              .Append(modelName)
              .Append(" = ")
              .Append(modelPk);

            return queryBuilder.ToString();
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