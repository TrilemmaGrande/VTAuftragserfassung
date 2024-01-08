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

        public SqlConnector(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion Public Constructors

        #region Public Methods

        public DataTable? ConnectionRead(string command)
        {
            try
            {

                using (SqlConnection sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = new SqlCommand(command, sqlConn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable ds = new DataTable();
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
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = new SqlCommand(command, sqlConn))
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

        public int ConnectionWriteGetPrimaryKey(string command, SqlParameter[] parameters)
        {
            int dataSetPrimaryKey = -1;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();

                    using (SqlCommand cmd = new SqlCommand(command, sqlConn))
                    {
                        cmd.Parameters.AddRange(parameters);
                        cmd.CommandText += "; SELECT SCOPE_IDENTITY();";

                        var result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            dataSetPrimaryKey = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Schreiben in Datenbank: {ex.Message}");
            }

            return dataSetPrimaryKey;
        }
        public void ConnectionWrite(string command, SqlParameter[] parameters)
        {
            try
            {


                using (SqlConnection sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();
                    using (SqlCommand cmd = new SqlCommand(command, sqlConn))
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

        #endregion Public Methods
    }
}