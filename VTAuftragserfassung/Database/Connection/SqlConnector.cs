using System.Data;
using System.Data.SqlClient;

namespace VTAuftragserfassung.Database.Connection
{
    public class SqlConnector : ISqlConnector
    {
        #region Private Fields

        private readonly string _connectionString;

        #endregion Private Fields

        #region Public Constructors

        public SqlConnector(string connectionString) => _connectionString = connectionString;

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

        public int ConnectionWriteGetPrimaryKey(string command, SqlParameter[]? parameters)
        {
            int dataSetPrimaryKey = 0;
            if (string.IsNullOrEmpty(command) || parameters == null)
            {
                return 0;
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
                        cmd.Parameters.AddRange(parameters);
                        cmd.CommandText += "; SELECT SCOPE_IDENTITY();";

                        var result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            dataSetPrimaryKey = Convert.ToInt32(result);
                        }
                    }
                    sqlConn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Schreiben in Datenbank: {ex.Message}");
            }
            return dataSetPrimaryKey == -1 ? 0 : dataSetPrimaryKey;

        }
        public void ConnectionWrite(string command, SqlParameter[]? parameters)
        {
            if (string.IsNullOrEmpty(command) || parameters == null)
            {
                return;
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
                        cmd.Parameters.AddRange(parameters);
                        cmd.ExecuteNonQuery();
                    }
                    sqlConn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Schreiben in Datenbank: {ex.Message}");
            }
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