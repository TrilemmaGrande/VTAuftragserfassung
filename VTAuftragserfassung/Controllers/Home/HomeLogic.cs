using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using VTAuftragserfassung.Database.Repository;
using VTAuftragserfassung.Models;
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
            List<AssignmentViewModel> assignmentVM = new();
            string userId = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(userId))
            {
                return assignmentVM;
            }

            List<Auftrag> assignments = _repo.GetAssignmentsByUserId(userId) ?? new();
            List<PositionViewModel> positionVM = new();
            Gesellschafter shareholder = new();
            Kunde? customer;
            List<Position> positions;
          

            foreach (var assignment in assignments)
            {

                customer = _repo.GetObjectByPrimaryKey(new Kunde(), assignment.FK_Kunde);
                if (customer != null)
                {
                    shareholder = _repo.GetObjectByPrimaryKey(new Gesellschafter(), customer.FK_Gesellschafter)!;
                }
                // change following later: JOIN Articles + Positions to get List of articles, then filter with LINQ
                positions = _repo.GetObjectListByForeignKey(new Position(), assignment, assignment.PK_Auftrag) ?? new();
                foreach (var position in positions)
                {
                    positionVM.Add(new()
                    {
                        Position = position,
                        Artikel = _repo.GetObjectByPrimaryKey(new Artikel(), position.FK_Artikel)
                    });
                }

                assignmentVM.Add(new AssignmentViewModel()
                {
                    Auftrag = assignment,
                    PositionenVM = positionVM,
                    Kunde = customer,
                    Gesellschafter = shareholder
                });
            }

            // lade Aufträge zu BenutzerId
            // lade Kunden mit Gesellschafter zu Aufträge
            // lade Positionen mit Artikeln zu Aufträge

            return assignmentVM;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion Public Methods
    }
}