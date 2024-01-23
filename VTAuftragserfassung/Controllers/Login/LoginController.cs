using Microsoft.AspNetCore.Mvc;
using VTAuftragserfassung.Controllers.Login.Interfaces;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Login
{
    public class LoginController(ILoginLogic _logic) : Controller
    {
        #region Private Fields

        #endregion Private Fields

        #region Public Constructors

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
        public IActionResult LoginVerification([FromBody] LoginViewModel loginViewModel)
        {
            return _logic.VerifyLogin(loginViewModel) ? RedirectToAction("Dashboard", "Home") : RedirectToAction("Index");
        }

        #endregion Public Methods
    }
}