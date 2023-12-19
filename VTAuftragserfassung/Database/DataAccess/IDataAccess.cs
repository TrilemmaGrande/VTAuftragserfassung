namespace VTAuftragserfassung.Database.DataAccess
{
    public interface IDataAccess<IDatabaseObject>
    {
        public List<T> GetAll<T>(T dbModel) where T : IDatabaseObject;

        public List<T> GetByCondition<T>(T dbModel, string getColumn, string condition) where T : IDatabaseObject;

        public int CountDataSets(string table, string column, string condition);
    }
}