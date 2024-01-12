using System.Security.Claims;
using VTAuftragserfassung.Database.Repository;
using VTAuftragserfassung.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using VTAuftragserfassung.Models.DBO;

namespace VTAuftragserfassung.Controllers.Login
{
    public class LoginLogic : ILoginLogic
    {
        private readonly IDbRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginLogic(IDbRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }
        public bool VerifyLogin(LoginViewModel loginViewModel)
        {
            Vertriebsmitarbeiter? user = _repo.GetUserByUserId(loginViewModel.UserId) ?? null;
            PasswordVerificationResult result = 0;
            string hashedAuth = string.Empty;

            if (user != null)
            {
                Auth? auth = _repo.GetAuthByUserPk(user.PK_Vertriebsmitarbeiter);
                hashedAuth = auth?.HashedAuth ?? string.Empty;
            }
            else
            {
                return false;
            }
            var pwhasher = new PasswordHasher<Vertriebsmitarbeiter>();
            if (!string.IsNullOrEmpty(hashedAuth))
            {
                result = pwhasher
                    .VerifyHashedPassword(
                user!,
                hashedAuth,
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
    }
}
