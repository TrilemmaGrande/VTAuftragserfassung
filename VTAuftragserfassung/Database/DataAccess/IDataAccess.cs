using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Database.DataAccess
{
    public interface IDataAccess<IDatabaseObject>
    {
        #region Public Methods

        int Create<T>(T? dbModel) where T : DataAccess.IDatabaseObject;

        void CreateAll<T>(List<T>? dbModels) where T : DataAccess.IDatabaseObject;

        public List<T>? ReadAll<T>(T? dbModel) where T : IDatabaseObject;

        List<Auftrag>? ReadAssignmentsByUserId(string userId);

        T1? ReadObjectByForeignKey<T1, T2>(T1? model, T2? foreignModel, int fk)
            where T1 : DataAccess.IDatabaseObject
            where T2 : DataAccess.IDatabaseObject;

        List<PositionViewModel>? ReadPositionVMsByUserId(string userId);

        Vertriebsmitarbeiter? ReadUserByUserId(string userId);
        void Update<T>(T? dbModel, IEnumerable<string>? columnsToUpdate = null) where T : DataAccess.IDatabaseObject;

        #endregion Public Methods
    }
}