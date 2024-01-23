using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Login.Interfaces
{
    public interface ILoginLogic
    {
        void Logout();
        #region Public Methods

        bool VerifyLogin(LoginViewModel loginViewModel);

        #endregion Public Methods
    }
}