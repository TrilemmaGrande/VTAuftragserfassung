namespace VTAuftragserfassung.Database.DataAccess.Interfaces
{
    public interface IDatabaseObject
    {
        #region Public Properties

        string PrimaryKeyColumn { get; set; }
        string TableName { get; }

        #endregion Public Properties
    }
}