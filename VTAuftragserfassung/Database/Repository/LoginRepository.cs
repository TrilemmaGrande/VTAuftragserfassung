using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.Repository.Interfaces;
using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Database.Repository
{
    /// <summary>
    /// [Relationship]: connects LoginLogic with DatabaseAccess. 
    /// [Input]: - 
    /// [Output]: DataBaseObjects.
    /// [Dependencies]: uses IDataAccess to access DataBaseObjects.
    /// [Notice]: this repository delegates verification, provides separation of concern.
    /// </summary>
    public class LoginRepository : ILoginRepository
    {
        private readonly IDatabaseService _database;

        public LoginRepository(IDatabaseService database)
        {
            _database = database;
        }

        public Auth? GetAuthByUserPk(int userPk) => _database.ReadObjectByForeignKey(new Auth(), new Vertriebsmitarbeiter(), userPk);

        public Vertriebsmitarbeiter? GetUserByUserId(string userId) => _database.ReadUserByUserId(userId);
    }
}