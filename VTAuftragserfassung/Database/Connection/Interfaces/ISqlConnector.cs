using System.Data;
using System.Data.SqlClient;

namespace VTAuftragserfassung.Database.Connection.Interfaces
{
    public interface ISqlConnector
    {
        DataTable? ConnectionRead(string command);
        object? ConnectionReadScalar(string command);
        void ConnectionWrite(List<Tuple<string, SqlParameter[]?>> queryList);
        int ConnectionWrite(string command, SqlParameter[]? parameters);
    }
}