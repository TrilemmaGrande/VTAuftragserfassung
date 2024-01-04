using System.Data;
using System.Reflection;
using System.Text;
using VTAuftragserfassung.Database.Connection;
using VTAuftragserfassung.Models;

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

        public int CountDataSets(string table, string column, string condition)
        {
            string cmd = $"SELECT COUNT{column} FROM {table} {condition}";
            return Convert.ToInt32(_conn.ConnectionReadScalar(cmd));
        }

        public int Create<T>(T dbModel) where T : IDatabaseObject
        {
            string cmd = CreateInsertString(dbModel);
            return _conn.ConnectionWriteGetPrimaryKey(cmd);
        }

        public void CreateAll<T>(List<T> dbModels) where T : IDatabaseObject
        {
            StringBuilder cmd = new StringBuilder();
            foreach (var dbModel in dbModels)
            {
                cmd.Append(CreateInsertString(dbModel));
            }
            _conn.ConnectionWrite(cmd.ToString());
        }

        public List<T> GetAll<T>(T dbModel) where T : IDatabaseObject => GetAll<T>($"SELECT * FROM {dbModel.TableName}");

        public List<T> GetAllByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => GetAll<T>($"SELECT {getterColumn} FROM {dbModel.TableName} {condition}");

        public T? GetByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
          => Get(dbModel, $"SELECT TOP 1 {getterColumn} FROM {dbModel.TableName} {condition}");

        #endregion Public Methods

        #region Private Methods

        private string CreateInsertString<T>(T dbModel) where T : IDatabaseObject
        {
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
           .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + typeof(T).Name);

            var columns = string.Join(", ", properties.Select(prop => prop.Name).Skip(1));
            var values = string.Join(", ", properties.Select(prop => FormatValueForDatabase(prop.GetValue(dbModel))).Skip(1));

            return $"INSERT INTO {dbModel.TableName} ({columns}) VALUES ({values});";
        }

        private string FormatValueForDatabase(object value)
        {
            if (value is string)
            {
                return $"'{value}'";
            }
            else if (value is Auftragsstatus)
            {
                return ((int)value).ToString();
            }
            else if (value is DateTime)
            {
                return $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'";
            }
            else if (value is bool)
            {
                return ((bool)value) ? "1" : "0";
            }
            else
            {
                return value.ToString();
            }            
        }


        private T? Get<T>(T dbModel, string cmd) where T : IDatabaseObject
        {
            DataTable dt = _conn.ConnectionRead(cmd);
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

        private List<T> GetAll<T>(string cmd) where T : IDatabaseObject
        {
            List<T> listOfT = new();
            DataTable dt = _conn.ConnectionRead(cmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T obj = SetProperties(dr, Activator.CreateInstance<T>());
                    listOfT.Add(obj);
                }
            }
            return listOfT;
        }

        private T SetProperties<T>(DataRow dr, T obj)
        {
            List<PropertyInfo> properties = obj.GetType().GetProperties().ToList();
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