using System.Data;

namespace VTAuftragserfassung.Database.Connection
{
    public interface ISqlConnector
    {
        DataTable ConnectionRead(string command);
        public object ConnectionReadScalar(string command);
        void ConnectionWrite(string command);
    }
}