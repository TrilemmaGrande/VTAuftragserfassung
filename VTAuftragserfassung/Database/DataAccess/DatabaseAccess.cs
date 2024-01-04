using System.Data;
using System.Reflection;
using System.Text;
using VTAuftragserfassung.Database.Connection;
using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

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

        public Auth ReadAuthByUserPk(int userPk) => ReadByCondition(new Auth(), "*", $"WHERE FK_Vertriebsmitarbeiter = {userPk}")!;

        public List<T> ReadAll<T>(T dbModel) where T : IDatabaseObject => ReadAll<T>($"SELECT * FROM {dbModel.TableName}");

        public List<Auftrag> ReadAssignmentsByUserId(string userId)
            => ReadAllByCondition(new Auftrag(), "*",
                    $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter) " +
                    $"WHERE MitarbeiterId = '{userId}'");

        public List<AssignmentViewModel> ReadAssignmentsWithSalesStaffByUserId(string userId)
            => ReadAllByCondition(new AssignmentViewModel(), "vta_Auftrag.*, vta_Vertriebsmitarbeiter.MitarbeiterId",
                    $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter)" +
                    $"WHERE MitarbeiterId = '{userId}'" +
                    $"ORDER BY vta_Auftrag.ErstelltAm");
        public T1? ReadObjectByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => ReadByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        public T? ReadObjectByPrimaryKey<T>(T model, int pk) where T : IDatabaseObject => ReadByCondition(model, "*", $"WHERE PK_{model.GetType().Name} = {pk}");

        public List<T1>? ReadObjectListByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => ReadAllByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        public List<PositionViewModel> ReadPositionVMsByUserId(string userId) => ReadAllByCondition(new PositionViewModel(), "*",
                                    $"INNER JOIN vta_Auftrag ON ( vta_Position.FK_Auftrag = vta_Auftrag.PK_Auftrag)" +
            $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter)" +
            $"WHERE MitarbeiterId = '{userId}'");

        public Vertriebsmitarbeiter? ReadUserByUserId(string userId) => ReadByCondition(new Vertriebsmitarbeiter(), "*", $"WHERE MitarbeiterId = '{userId}'");

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

        private T? Read<T>(T dbModel, string cmd) where T : IDatabaseObject
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

        private List<T> ReadAll<T>(string cmd) where T : IDatabaseObject
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

        private List<T> ReadAllByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
        => ReadAll<T>($"SELECT {getterColumn} FROM {dbModel.TableName} {condition}");

        private T? ReadByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
          => Read(dbModel, $"SELECT TOP 1 {getterColumn} FROM {dbModel.TableName} {condition}");

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