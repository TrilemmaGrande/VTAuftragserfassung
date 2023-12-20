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
            string userId = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(userId))
            {
                return new();
            }
            List<AssignmentViewModel> assignmentVMs = _repo.GetAssignmentVMsWithoutPositionsByUserId(userId);
            List<PositionViewModel> positionVMs = _repo.GetPositionVMsByUserId(userId);
            foreach (var assignmentVM in assignmentVMs)
            {
                assignmentVM.PositionenVM = positionVMs.Where(i => i.Position.FK_Auftrag == assignmentVM.Auftrag.PK_Auftrag).ToList();
            }

            // lade Aufträge zu BenutzerId
            // lade Kunden mit Gesellschafter zu Aufträge
            // lade Positionen mit Artikeln zu Aufträge

            return assignmentVMs;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion Public Methods
    }
}