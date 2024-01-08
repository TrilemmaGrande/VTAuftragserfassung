using System.Data;
using System.Data.SqlClient;

namespace VTAuftragserfassung.Database.Connection
{
    public interface ISqlConnector
    {
        DataTable? ConnectionRead(string command);
        object? ConnectionReadScalar(string command);
        void ConnectionWrite(string command, SqlParameter[] parameters);
        int ConnectionWriteGetPrimaryKey(string command, SqlParameter[] parameters);
    }
}