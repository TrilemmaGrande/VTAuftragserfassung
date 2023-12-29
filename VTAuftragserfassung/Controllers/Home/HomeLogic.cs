using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using VTAuftragserfassung.Database.Repository;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Controllers.Home
{
    public class HomeLogic : IHomeLogic
    {
        #region Private Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbRepository _repo;
        private readonly string _userId = string.Empty;

        #endregion Private Fields

        #region Public Constructors

        public HomeLogic(IDbRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? string.Empty;
        }

        #endregion Public Constructors

        #region Public Methods

        public List<AssignmentViewModel> GetAssignmentViewModels()
        {
            return !string.IsNullOrEmpty(_userId) ? _repo.GetAssignmentVMsByUserId(_userId) : [];
        }

        public AssignmentFormViewModel GetAssignmentFormViewModel()
        {
            return !string.IsNullOrEmpty(_userId) ? _repo.GetAssignmentFormVMByUserId(_userId) : new();
        }

        public PositionViewModel GetPositionViewModel(int articlePK)
        {           
            return _repo.GetPositionVMByArticlePK(articlePK);
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion Public Methods
    }
}