using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Login.Interfaces
{
    public interface ILoginLogic
    {
        public bool VerifyLogin(LoginViewModel loginViewModel);

    }
}
