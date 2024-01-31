using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VTAuftragserfassung.Controllers.Login.Interfaces;
using VTAuftragserfassung.Database.Repository.Interfaces;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Controllers.Login
{
    /// <summary>
    /// [Relationship]: connects Controller with Repository
    /// [Input]: data from Controller
    /// [Output]: result-object from business logic.
    /// [Dependencies]: uses ILoginRepository Object to access backend data.
    /// [Notice]: -
    /// </summary>
    public class LoginLogic : ILoginLogic
    {
        private readonly ILoginRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginLogic(ILoginRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Public Methods

        public void Logout() =>
       _httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        public bool VerifyLogin(LoginViewModel loginViewModel)
        {
            Vertriebsmitarbeiter? user = _repo.GetUserByUserId(loginViewModel.UserId) ?? null;
            if (user == null)
            {
                return false;
            }
            Auth? auth = _repo.GetAuthByUserPk(user.PK_Vertriebsmitarbeiter);

            PasswordVerificationResult result;

            var pwhasher = new PasswordHasher<Vertriebsmitarbeiter>();
            if (!string.IsNullOrEmpty(auth?.HashedAuth))
            {
                result = pwhasher
                    .VerifyHashedPassword(
                user,
                auth.HashedAuth,
                loginViewModel.Auth);
            }
            // delete below later
            else
            {
                string hashedAuth2 = pwhasher.HashPassword(user, loginViewModel.Auth);
                result = pwhasher
                  .VerifyHashedPassword(
                user!,
                hashedAuth2,
                loginViewModel.Auth);
            }
            // delete above later

            var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user!.MitarbeiterId),
                    new Claim(ClaimTypes.Role, "User")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            _httpContextAccessor.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return result != 0;
        }

        #endregion Public Methods
    }
}