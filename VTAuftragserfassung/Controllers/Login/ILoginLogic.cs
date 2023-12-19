using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Controllers.Login
{
    public interface ILoginLogic
    {
        public bool VerifyLogin(LoginViewModel loginViewModel);
      
    }
}
