﻿using Microsoft.AspNetCore.Authentication;
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

        public List<AssignmentViewModel> GetAssignmentViewModels(int page)
        {
            return !string.IsNullOrEmpty(_userId) ? _repo.GetAssignmentVMsByUserId(_userId) : [];
        }

        public Artikel GetArticleByPK(int articlePK)
        {
            return _repo.GetAllArticlesCached().Find(i => i.PK_Artikel == articlePK)!;
        }

        public Kunde GetCustomerByPK(int customerPK)
        {
            return _repo.GetAllCustomersCached().Find(i => i.PK_Kunde == customerPK)!;
        }

        public AssignmentFormViewModel GetAssignmentFormViewModel()
        {
            return !string.IsNullOrEmpty(_userId) ? _repo.GetAssignmentFormVMByUserId(_userId) : new();
        }

        public List<Gesellschafter> GetAllShareholders()
        {
            return _repo.GetAllShareholdersCached();
        }

        public Gesellschafter GetShareholderByPK(int shareholderPK)
        {
            return _repo.GetAllShareholdersCached().Find(i => i.PK_Gesellschafter == shareholderPK)!;
        }

        public PositionViewModel GetPositionViewModel(int articlePK)
        {
            return _repo.GetNewPositionVMByArticlePK(articlePK);
        }

        public int CreateAssignment(AssignmentViewModel avm)
        {           
            Vertriebsmitarbeiter? salesStaff = _repo.GetUserByUserId(_userId);
            if (avm?.Auftrag == null || salesStaff == null)
            {
                return 0;
            }
            int salesStaffPK = salesStaff.PK_Vertriebsmitarbeiter;
            avm.Auftrag.FK_Vertriebsmitarbeiter = salesStaffPK;
            int assignmentPK = _repo.SaveAssignmentVM(avm);
            return assignmentPK;
        }

        public int CreateCustomer(Kunde customer)
        {
            return customer != null ? _repo.SaveCustomer(customer) : 0;

        }

        public void UpdateAssignmentStatus(int assignmentPK, string assignmentStatus)
        {
            if (!Enum.TryParse(assignmentStatus, out Auftragsstatus status) || assignmentPK <= 0)
            {
                return;
            }
            Auftrag assignment = new() { PK_Auftrag = assignmentPK, Auftragsstatus = status };
            _repo.Update(assignment, "Auftragsstatus");
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion Public Methods
    }
}