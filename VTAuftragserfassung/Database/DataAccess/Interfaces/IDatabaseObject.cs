namespace VTAuftragserfassung.Database.DataAccess.Interfaces
{
    public interface IDatabaseObject
    {
        public string TableName { get; }
        public string PrimaryKeyColumn { get; set; }
    }
}
