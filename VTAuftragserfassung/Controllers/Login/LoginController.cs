using Microsoft.AspNetCore.Mvc;
using VTAuftragserfassung.Controllers.Login.Interfaces;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Login
{
    public class LoginController : Controller
    {
        #region Private Fields

        private readonly ILoginLogic _logic;

        #endregion Private Fields

        #region Public Constructors

        public LoginController(ILoginLogic logic)
        {
            _logic = logic;
        }

        #endregion Public Constructors

        #region Public Methods

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            _logic.Logout();
            return RedirectToAction("Index");
        }

        [HttpPost("/LoginVerification")]
        public bool LoginVerification([FromBody] LoginViewModel loginViewModel)
        {
            return _logic.VerifyLogin(loginViewModel);
        }

        #endregion Public Methods
    }
}