using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Database.Repository
{
    public interface IDbRepository
    {
        public List<Gesellschafter> GetAllShareholdersCached();

        public List<Artikel> GetAllArticlesCached();

        public List<Kunde> GetAllCustomers();

        public List<Kunde> GetAllCustomersCached();

        public Vertriebsmitarbeiter? GetUserByUserId(string userId);

        public Auth? GetAuthByUserPk(int userPk);

        public List<AssignmentViewModel> GetAssignmentVMsByUserId(string userId);

        public AssignmentFormViewModel GetAssignmentFormVMByUserId(string userId);

        public PositionViewModel GetNewPositionVMByArticlePK(int articlePK);

        int SaveAssignmentVM(AssignmentViewModel avm);

        int SaveCustomer(Kunde customer);
    }
}