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

        private List<T> GetAll<T>(T dbModel, string cmd) where T : IDatabaseObject
        {
            List<T> listOfT = new();
            DataTable dt = _conn.ConnectionRead(cmd);
            if (dt != null && dt.Rows.Count > 0)
            {
                PropertyInfo[] properties = dbModel.GetType().GetProperties();
                foreach (DataRow dr in dt.Rows)
                {
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
                    listOfT.Add(obj);
                }
            }
            return listOfT;
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

        public List<T> GetAll<T>(T dbModel) where T : IDatabaseObject => GetAll(dbModel, $"SELECT * FROM {dbModel.TableName}");

        public List<T> GetAllByCondition<T>(T dbModel, string getColumn, string condition) where T : IDatabaseObject
            => GetAll(dbModel, $"SELECT {getColumn} FROM {dbModel.TableName} {condition}");

        public T? GetByCondition<T>(T dbModel, string getColumn, string condition) where T : IDatabaseObject
          => Get(dbModel, $"SELECT TOP 1 {getColumn} FROM {dbModel.TableName} {condition}");

        public int CountDataSets(string table, string column, string condition)
        {
            string cmd = $"SELECT COUNT{column} FROM {table} {condition}";
            return Convert.ToInt32(_conn.ConnectionReadScalar(cmd));
        }

        #endregion Public Methods
    }
}