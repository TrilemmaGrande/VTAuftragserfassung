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

        #endregion Private Fields

        #region Public Constructors

        public HomeLogic(IDbRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion Public Constructors

        #region Public Methods

        public List<AssignmentViewModel> GetAssignmentViewModels()
        {
            List<AssignmentViewModel> avm = [];
            string userId = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? string.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                List<PositionViewModel> pvm = _repo.GetPositionVMsByUserId(userId);
                avm = _repo.GetAssignmentVMsWithoutPositionsByUserId(userId);
                avm.ForEach(item => item.PositionenVM!.AddRange(pvm.Where(x => x.Position?.FK_Auftrag == item.Auftrag?.PK_Auftrag).ToList()));
            }
            return avm;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion Public Methods
    }
}