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

        public DataTable ConnectionRead(string command)
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

        public object ConnectionReadScalar(string command)
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

        public int ConnectionWriteGetPrimaryKey(string command)
        {
            int dataSetPrimaryKey;
            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                sqlConn.Open();
                using (SqlCommand cmd = new SqlCommand(command, sqlConn))
                {
                    cmd.ExecuteNonQuery();
                }
                using (SqlCommand identityCmd = new SqlCommand("SELECT SCOPE_IDENTITY()", sqlConn))
                {
                    dataSetPrimaryKey = Convert.ToInt32(identityCmd.ExecuteScalar());
                }
                sqlConn.Close();
            }
            return dataSetPrimaryKey;
        }
        public void ConnectionWrite(string command)
        {
            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                sqlConn.Open();
                using (SqlCommand cmd = new SqlCommand(command, sqlConn))
                {
                    cmd.ExecuteNonQuery();
                }
                sqlConn.Close();
            }
        }

        #endregion Public Methods
    }
}