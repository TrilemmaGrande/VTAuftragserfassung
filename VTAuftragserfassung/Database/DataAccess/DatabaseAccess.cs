﻿using System.Data;
using System.Data.SqlClient;
using System.Reflection;
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
            SqlParameter[] parameters = GenerateParameters(dbModel);

            return _conn.ConnectionWriteGetPrimaryKey(cmd, parameters);
        }

        public void CreateAll<T>(List<T> dbModels) where T : IDatabaseObject
        {
            foreach (var dbModel in dbModels)
            {
                string cmd = CreateInsertString(dbModel);
                SqlParameter[] parameters = GenerateParameters(dbModel);

                _conn.ConnectionWrite(cmd, parameters);
            }
        }

  
        public List<T>? ReadAll<T>(T dbModel) where T : IDatabaseObject
            => ReadAll<T>($"SELECT * FROM {dbModel.TableName}");

        public T1? ReadObjectByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => ReadByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        public T? ReadObjectByPrimaryKey<T>(T model, int pk) where T : IDatabaseObject
            => ReadByCondition(model, "*", $"WHERE PK_{model.GetType().Name} = {pk}");

        public List<T1>? ReadObjectListByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => ReadAllByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        public List<Auftrag>? ReadAssignmentsByUserId(string userId)
            => ReadAllByCondition(new Auftrag(), "*",
                    $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter) " +
                    $"WHERE MitarbeiterId = '{userId}'");

        public List<PositionViewModel>? ReadPositionVMsByUserId(string userId)
                                    => ReadAllByCondition(new PositionViewModel(), "*",
                    $"INNER JOIN vta_Auftrag ON ( vta_Position.FK_Auftrag = vta_Auftrag.PK_Auftrag)" +
                    $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter)" +
                    $"WHERE MitarbeiterId = '{userId}'");

        public Vertriebsmitarbeiter? ReadUserByUserId(string userId) => ReadByCondition(new Vertriebsmitarbeiter(), "*", $"WHERE MitarbeiterId = '{userId}'");

        public void Update<T>(T dbModel, IEnumerable<string> columnsToUpdate = null) where T : IDatabaseObject
        {
            string cmd = CreateUpdateString(dbModel, columnsToUpdate);
            SqlParameter[] parameters = GenerateParameters(dbModel);

            _conn.ConnectionWrite(cmd, parameters);
        }


        #endregion Public Methods

        #region Private Methods

        private string CreateInsertString<T>(T dbModel) where T : IDatabaseObject
        {
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + typeof(T).Name);

            string tableName = properties.First().GetValue(dbModel)?.ToString()?.Trim('\'') ?? "";
            string columns = string.Join(", ", properties.Skip(1).Select(prop => prop.Name));
            string values = string.Join(", ", properties.Skip(1).Select(prop => $"@{prop.Name}"));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values});";
        }

        private string CreateUpdateString<T>(T dbModel, IEnumerable<string> columnsToUpdate) where T : IDatabaseObject
        {
            string modelName = typeof(T).Name;

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + modelName);

            int modelPk = (int)typeof(T).GetProperty("PK_" + modelName).GetValue(dbModel);
            string tableName = properties.First().GetValue(dbModel)?.ToString()?.Trim('\'') ?? "";

            IEnumerable<PropertyInfo> propertiesToUpdate = columnsToUpdate != null
                ? properties.Where(prop => columnsToUpdate.Contains(prop.Name))
                : properties.Skip(1);

            string setClause = string.Join(", ", propertiesToUpdate.Select(prop => $"{prop.Name} = @{prop.Name}"));

            return $"UPDATE {tableName} SET {setClause} WHERE PK_{modelName} = {modelPk};";
        }

        private SqlParameter[] GenerateParameters<T>(T dbModel) where T : IDatabaseObject
        {
            var parameters = typeof(T).GetProperties()
                .Where(prop => prop.GetValue(dbModel) != null && prop.Name != "PK_" + typeof(T).Name)
                .Skip(1)
                .Select(FormatPropertyForDatabase(dbModel))
                .ToArray();

            return parameters;
        }

        private Func<PropertyInfo, SqlParameter> FormatPropertyForDatabase<T>(T dbModel) where T : IDatabaseObject
        {
            return prop =>
            {
                var paramName = $"@{prop.Name}";
                var propValue = prop.GetValue(dbModel);

                if (prop.PropertyType == typeof(bool))
                {
                    return new SqlParameter(paramName, (bool)propValue ? 1 : 0);
                }
                else if (prop.PropertyType.IsEnum)
                {
                    return new SqlParameter(paramName, (int)propValue);
                }
                else
                {
                    return new SqlParameter(paramName, propValue ?? DBNull.Value);
                }
            };
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

        private List<T>? ReadAll<T>(string cmd) where T : IDatabaseObject
        {
            List<T> listOfT = [];
            DataTable dt = _conn.ConnectionRead(cmd);
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

        private T? SetProperties<T>(DataRow dr, T obj)
        {
            List<PropertyInfo>? properties = obj == null ? null : obj!.GetType().GetProperties().ToList();
            if (properties == null || properties.Count == 0)
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

        private T? ReadByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => Read(dbModel, $"SELECT TOP 1 {getterColumn} FROM {dbModel.TableName} {condition}");

        private List<T>? ReadAllByCondition<T>(T dbModel, string getterColumn, string condition) where T : IDatabaseObject
            => ReadAll<T>($"SELECT {getterColumn} FROM {dbModel.TableName} {condition}");

        #endregion Private Methods
    }
}