using System.Data;
using System.Reflection;
using VTAuftragserfassung.Database.Connection;

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

        public List<T> GetAll<T>(T dbModel) where T : IDatabaseObject => GetAll<T>($"SELECT * FROM {dbModel.TableName}");

        public List<T> GetAllByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => GetAll<T>($"SELECT {getterColumn} FROM {dbModel.TableName} {condition}");

        public T? GetByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
          => Get(dbModel, $"SELECT TOP 1 {getterColumn} FROM {dbModel.TableName} {condition}");

        public int CountDataSets(string table, string column, string condition)
        {
            string cmd = $"SELECT COUNT{column} FROM {table} {condition}";
            return Convert.ToInt32(_conn.ConnectionReadScalar(cmd));
        }

        #endregion Public Methods
    }
}