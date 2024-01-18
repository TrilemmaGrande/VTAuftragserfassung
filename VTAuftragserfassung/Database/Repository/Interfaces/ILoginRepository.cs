using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Database.Repository.Interfaces
{
    public interface ILoginRepository
    {
        Auth? GetAuthByUserPk(int userPk);
        Vertriebsmitarbeiter? GetUserByUserId(string userId);
    }
}
