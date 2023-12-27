using Microsoft.AspNetCore.Mvc;
using VTAuftragserfassung.Models.ViewModels;

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

        [HttpPost("/LoginVerification")]
        public IActionResult LoginVerification([FromBody] LoginViewModel loginViewModel)
        {
            return _logic.VerifyLogin(loginViewModel) ? RedirectToAction("Dashboard", "Home") : RedirectToAction("Index");
        }

        #endregion Public Methods
    }
}