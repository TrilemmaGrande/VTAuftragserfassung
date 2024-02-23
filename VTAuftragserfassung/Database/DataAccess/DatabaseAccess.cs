using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using VTAuftragserfassung.Database.Connection.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;

namespace VTAuftragserfassung.Database.DataAccess
{
    /// <summary>
    /// [Relationship]: connects Database-Connection with DatabaseService.
    /// [Input]: DataBaseObjects, SQL Query as String, SQL Parameters as String.
    /// [Output]: DataBaseObjects.
    /// [Dependencies]: uses ISqlConnector to establish connection and IDatabaseObject for consistency.
    /// [Notice]: performs CRUD operations and ORM functionality, maps Parameterized Strings. 
    /// </summary>
    public class DatabaseAccess : IDatabaseAccess
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

        public void CreateAll<T>(List<KeyValuePair<T, string>> dbModelsWithCmds) where T : IDatabaseObject
        {
            if (dbModelsWithCmds == null || dbModelsWithCmds.Count == 0)
            {
                return;
            }

            List<Tuple<string, SqlParameter[]?>> queryList = new();
            foreach (var dbModelWithCmd in dbModelsWithCmds)
            {
                SqlParameter[]? parameters = GenerateParameters(dbModelWithCmd.Key);
                queryList.Add(new Tuple<string, SqlParameter[]?>(dbModelWithCmd.Value, parameters));
            }
            _conn.ConnectionWrite(queryList);
        }

        public int CreateSingle<T>(T? dbModel, string cmd) where T : IDatabaseObject
        {
            if (dbModel == null || string.IsNullOrEmpty(cmd))
            {
                return 0;
            }
            SqlParameter[]? parameters = GenerateParameters(dbModel);

            return _conn.ConnectionWrite(cmd, parameters!);
        }

        public List<T>? ReadAll<T>(string cmd) where T : IDatabaseObject
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return default;
            }
            List<T> listOfT = new();
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

        public int ReadScalar(string cmd) {
            return !string.IsNullOrEmpty(cmd) ? Convert.ToInt32(_conn.ConnectionReadScalar(cmd)) : default;
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

        public void Update<T>(T? dbModel, string cmd) where T : IDatabaseObject
        {
            if (dbModel == null || string.IsNullOrEmpty(cmd))
            {
                return;
            }
            SqlParameter[]? parameters = GenerateParameters(dbModel);

            _conn.ConnectionWrite(cmd, parameters, true);
        }

        #endregion Public Methods

        #region Private Methods

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