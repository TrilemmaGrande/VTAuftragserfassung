using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Login
{
    public interface ILoginLogic
    {
        public bool VerifyLogin(LoginViewModel loginViewModel);
      
    }
}
