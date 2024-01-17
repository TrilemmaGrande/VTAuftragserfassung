namespace VTAuftragserfassung.Database.DataAccess
{
    public interface IDatabaseObject
    {
        public string TableName { get; }
        public string PrimaryKeyColumn { get; set; }
    }
}
