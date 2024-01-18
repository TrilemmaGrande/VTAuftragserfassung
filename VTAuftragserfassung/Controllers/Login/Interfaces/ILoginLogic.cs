using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Login.Interfaces
{
    public interface ILoginLogic
    {
        #region Public Methods

        bool VerifyLogin(LoginViewModel loginViewModel);

        #endregion Public Methods
    }
}