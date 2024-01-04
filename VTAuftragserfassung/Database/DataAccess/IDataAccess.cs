using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Database.DataAccess
{
    public interface IDataAccess<IDatabaseObject>
    {
        public List<T> ReadAll<T>(T dbModel) where T : IDatabaseObject;

        int Create<T>(T dbModel) where T : DataAccess.IDatabaseObject;

        void CreateAll<T>(List<T> dbModels) where T : DataAccess.IDatabaseObject;
        List<Auftrag> ReadAssignmentsByUserId(string userId);
        List<AssignmentViewModel> ReadAssignmentsWithSalesStaffByUserId(string userId);
        Auth ReadAuthByUserPk(int userPk);
        List<PositionViewModel> ReadPositionVMsByUserId(string userId);
        Vertriebsmitarbeiter? ReadUserByUserId(string userId);
        T1? ReadObjectByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : DataAccess.IDatabaseObject
            where T2 : DataAccess.IDatabaseObject;
    }
}