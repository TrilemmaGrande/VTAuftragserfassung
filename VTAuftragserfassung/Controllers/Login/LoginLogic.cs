using System.Security.Claims;
using VTAuftragserfassung.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Database.Repository.Interfaces;
using VTAuftragserfassung.Controllers.Login.Interfaces;

namespace VTAuftragserfassung.Controllers.Login
{
    public class LoginLogic(ILoginRepository _repo, IHttpContextAccessor _httpContextAccessor)
        : ILoginLogic
    {
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