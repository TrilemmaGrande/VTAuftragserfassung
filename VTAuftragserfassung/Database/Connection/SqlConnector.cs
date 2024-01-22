using System.Data;
using System.Data.SqlClient;
using VTAuftragserfassung.Database.Connection.Interfaces;

namespace VTAuftragserfassung.Database.Connection
{
    public class SqlConnector : ISqlConnector
    {
        #region Private Fields

        private readonly string _connectionString;

        #endregion Private Fields

        #region Public Constructors

        public SqlConnector(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion Public Constructors

        #region Public Methods

        public DataTable? ConnectionRead(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return null;
            }
            try
            {
                using (SqlConnection sqlConn = new(_connectionString))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }

                    using (SqlCommand cmd = new(command, sqlConn))
                    {
                        using (SqlDataAdapter adapter = new(cmd))
                        {
                            DataTable ds = new();
                            adapter.Fill(ds);

                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Lesen aus Datenbank: {ex.Message}");
                return null;
            }

        }

        public object? ConnectionReadScalar(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return null;
            }
            try
            {
                using (SqlConnection sqlConn = new(_connectionString))
                {
                    if (sqlConn.State != ConnectionState.Open)
                    {
                        sqlConn.Open();
                    }

                    using (SqlCommand cmd = new(command, sqlConn))
                    {
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Lesen aus Datenbank: {ex.Message}");
                return null;
            }
        }

        public int ConnectionWrite(string command, SqlParameter[]? parameters)
        {
            int dataSetPrimaryKey = 0;
            if (string.IsNullOrEmpty(command) || parameters == null)
            {
                return 0;
            }

            using (SqlConnection sqlConn = new(_connectionString))
            {
                if (sqlConn.State != ConnectionState.Open)
                {
                    sqlConn.Open();
                }

                SqlTransaction transaction;
                transaction = sqlConn.BeginTransaction();
                using (SqlCommand cmd = new(command, sqlConn))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddRange(parameters);

                    try
                    {
                        var result = cmd.ExecuteScalar();
                        dataSetPrimaryKey = Convert.ToInt32(result);
                        if (dataSetPrimaryKey <= 0)
                        {
                            transaction.Rollback();
                            sqlConn.Close();
                            return 0;
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Schreiben in Datenbank: {ex.Message}");
                    }
                }
                sqlConn.Close();
            }
            return dataSetPrimaryKey;
        }

        public void ConnectionWrite(List<Tuple<string, SqlParameter[]?>> queryList)
        {
            using (SqlConnection sqlConn = new(_connectionString))
            {
                if (sqlConn.State != ConnectionState.Open)
                {
                    sqlConn.Open();
                }
                SqlCommand cmd = sqlConn.CreateCommand();
                SqlTransaction transaction;
                transaction = sqlConn.BeginTransaction();
                cmd.Transaction = transaction;
                try
                {
                    foreach (var cmdWithParameters in queryList)
                    {
                        string command = cmdWithParameters.Item1;
                        SqlParameter[]? parameters = cmdWithParameters.Item2;
                        if (!string.IsNullOrEmpty(command) && parameters != null)
                        {
                            cmd.CommandText = command;
                            cmd.Parameters.AddRange(parameters);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            transaction.Rollback();
                            sqlConn.Close();
                            return;
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Fehler beim Schreiben in Datenbank: {ex.Message}");
                }
                sqlConn.Close();
            }
        }

        #endregion Public Methods
    }
}