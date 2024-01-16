namespace VTAuftragserfassung.Database.DataAccess.Services
{
    public interface ICachingService
    {
        #region Public Methods

        List<T>? GetCachedModel<T>(T? model) where T : IDatabaseObject;

        public List<T>? UpdateCachedModel<T>(List<T>? newModelData) where T : IDatabaseObject;

        #endregion Public Methods
    }
}