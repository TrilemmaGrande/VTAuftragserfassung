using System.Security.Claims;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Database.Repository.Interfaces;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Database.Repository
{
    /// <summary>
    /// [Relationship]: connects HomeLogic with DatabaseAccess and Cache. 
    /// [Input]: ViewModels.
    /// [Output]: ViewModels, DataBaseObjects.
    /// [Dependencies]: uses IDatabaseService, ICachingService and ISessionService to access DataBaseObjects.
    /// [Notice]: joins and creates ViewModels for logic, separates ViewModels for saving.
    /// </summary>
    public class HomeRepository : IHomeRepository
    {
        private readonly IDatabaseService _database;
        private readonly ICachingService _cache;
        private readonly ISessionService _session;

        public HomeRepository(IDatabaseService database, ICachingService cache, ISessionService session)
        {
            _database = database;
            _cache = cache;
            _session = session;
        }

        #region Public Methods

        public ClaimsPrincipal? GetSessionUser() => _session.GetSessionUser() ?? default;

        public List<Artikel>? GetAllArticlesCached() => GetCachedModels(new Artikel());

        public List<Kunde>? GetAllCustomers() => _database.ReadAll(new Kunde());

        public List<Kunde>? GetAllCustomersCached() => GetCachedModels(new Kunde());

        public List<Gesellschafter>? GetAllShareholdersCached() => GetCachedModels(new Gesellschafter());

        public AssignmentFormViewModel? GetAssignmentFormVMByUserId(string userId)
        {
            Vertriebsmitarbeiter? salesStaff = GetUserFromSession(userId);
            List<Gesellschafter>? shareholders = GetAllShareholdersCached();
            List<Artikel>? articles = GetAllArticlesCached();
            List<Kunde>? customers = GetAllCustomersCached();
            AssignmentFormViewModel? afvm = new()
            {
                Vertriebsmitarbeiter = salesStaff,
                Gesellschafter = shareholders ?? new(),
                Artikel = articles ?? new(),
                Kunden = customers ?? new()
            };
            return afvm;
        }

        public bool CheckUserAssignmentMapping(string userId, int assignmentPk)
        {
            return _database.CheckUserAssignmentMapping(userId, assignmentPk);
        }

        public int GetAssignmentsCount(string userId)
        {
            return _database.CountAssignmentsByUserId(userId);
        }

        public List<AssignmentViewModel>? GetAssignmentVMsPaginatedByUserId(string userId, Pagination? pagination)
        {
            List<Auftrag>? assignments = _database.ReadAssignmentsPaginatedByUserId(userId, pagination);
            if (assignments == null)
            {
                return null;
            }
            List<int>? assignmentPKs = assignments.Select(i => i.PK_Auftrag).ToList();
            List<Gesellschafter>? shareholders = GetAllShareholdersCached();
            List<Artikel>? articles = GetAllArticlesCached();
            List<Kunde>? customers = GetAllCustomersCached();
            List<Position>? positions = _database.ReadPositionsByAssignmentPKs(assignmentPKs) ?? new();
            List<PositionViewModel>? pvms = positions.Select(p => new PositionViewModel() { Position = p }).ToList();
            List<AssignmentViewModel>? avms = assignments.Select(i => new AssignmentViewModel() { Auftrag = i }).ToList();
            if (articles == null || customers == null || shareholders == null)
            {
                return null;
            }
            foreach (var avm in avms)
            {
                avm.PositionenVM = pvms.Where(i => i.Position?.FK_Auftrag == avm.Auftrag?.PK_Auftrag).ToList();
                avm.PositionenVM.ForEach(item => item.Artikel = articles.Find(i => i.PK_Artikel == item.Position?.FK_Artikel));
                avm.Kunde = customers.Find(i => i.PK_Kunde == avm.Auftrag?.FK_Kunde);
                avm.Gesellschafter = shareholders.Find(i => i.PK_Gesellschafter == avm.Kunde?.FK_Gesellschafter);
            }
            return avms;
        }

        public PositionViewModel? GetNewPositionVMByArticlePK(int articlePK)
        {
            List<Artikel>? articles = GetAllArticlesCached();
            if (articles == null)
            {
                return null;
            }
            Artikel? article = articles.Find(i => i.PK_Artikel == articlePK);
            if (article == null)
            {
                return null;
            }
            PositionViewModel pvm = new()
            {
                Artikel = article,
                Position = new()
                {
                    FK_Artikel = articlePK,
                    Menge = 1,
                    SummePosition = article.Preis,
                }
            };
            return pvm;
        }

        public Vertriebsmitarbeiter? GetUserFromSession(string userId)
        {
            Vertriebsmitarbeiter? user = GetSessionModels(new Vertriebsmitarbeiter(), userId)?.FirstOrDefault() ?? _database.ReadUserByUserId(userId);
            if (user != null)
            {
                _session.SetSessionModels(userId, new List<Vertriebsmitarbeiter>() { user });
            }
            return user;
        }

        public int SaveAssignmentVM(AssignmentViewModel? avm)
        {
            if (avm?.Auftrag == null)
            {
                return 0;
            }
            int pkAssignment = _database.CreateSingle(avm.Auftrag);
            if (avm.PositionenVM != null && avm.PositionenVM.Count > 0)
            {
                MatchPositions(avm.PositionenVM, pkAssignment);
            }
            return pkAssignment;
        }

        public int SaveCustomer(Kunde? customer)
        {
            if (customer == null)
            {
                return 0;
            }
            int pk = _database.CreateSingle(customer);
            _cache.InvalidateCacheModels(new Kunde());
            return pk;
        }

        public void Update<T>(T? model, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject
        {
            _database.Update(model, columnsToUpdate);
        }

        #endregion Public Methods

        #region Private Methods

        private List<T>? GetCachedModels<T>(T model) where T : IDatabaseObject
        {
            return _cache.GetCachedModels(model) ?? _cache.UpdateCachedModels(_database.ReadAll(model));
        }

        private List<T>? GetSessionModels<T>(T model, string sKey) where T : IDatabaseObject
        {
            return _session.GetSessionModels(model, sKey);
        }

        private void MatchPositions(List<PositionViewModel>? positions, int pkAssignment)
        {
            if (positions == null || positions.Count == 0)
            {
                return;
            }
            List<Position>? positionList = positions
                .Where(i => i.Position != null)
                .Select(i =>
                {
                    i.Position!.FK_Auftrag = pkAssignment;
                    return i.Position;
                }).ToList();

            _database.CreateAll(positionList);
        }

        #endregion Private Methods
    }
}