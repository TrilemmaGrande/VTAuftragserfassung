using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.Repository.Interfaces;
using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Database.Repository
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IDataAccess<IDatabaseObject> _dataAccess;

        public LoginRepository(IDataAccess<IDatabaseObject> dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public Auth? GetAuthByUserPk(int userPk) => _dataAccess.ReadObjectByForeignKey(new Auth(), new Vertriebsmitarbeiter(), userPk);

        public Vertriebsmitarbeiter? GetUserByUserId(string userId) => _dataAccess.ReadUserByUserId(userId);

    }
}
