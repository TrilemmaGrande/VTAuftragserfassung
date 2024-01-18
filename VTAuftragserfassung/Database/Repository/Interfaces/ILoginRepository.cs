using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Database.Repository.Interfaces
{
    public interface ILoginRepository
    {
        #region Public Methods

        Auth? GetAuthByUserPk(int userPk);

        Vertriebsmitarbeiter? GetUserByUserId(string userId);

        #endregion Public Methods
    }
}