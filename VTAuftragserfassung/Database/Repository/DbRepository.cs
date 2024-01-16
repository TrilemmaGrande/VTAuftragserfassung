using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Database.Repository
{
    public class DbRepository : IDbRepository
    {
        #region Private Fields

        private readonly IDataAccess<IDatabaseObject> _dataAccess;
        private readonly IMemoryCache _memoryCache;

        #endregion Private Fields

        #region Public Constructors

        public DbRepository(IDataAccess<IDatabaseObject> dataAccess, IMemoryCache memoryCache)
        {
            _dataAccess = dataAccess;
            _memoryCache = memoryCache;
        }

        #endregion Public Constructors

        #region Public Methods

        public List<Artikel>? GetAllArticlesCached() => GetCachedModel(new Artikel());

        public List<Kunde>? GetAllCustomers() => _dataAccess.ReadAll(new Kunde());

        public List<Kunde>? GetAllCustomersCached() => GetCachedModel(new Kunde());

        public List<Gesellschafter>? GetAllShareholdersCached() => GetCachedModel(new Gesellschafter());

        public AssignmentFormViewModel? GetAssignmentFormVMByUserId(string userId)
        {
            Vertriebsmitarbeiter? salesStaff = GetUserByUserId(userId);
            List<Gesellschafter>? shareholders = GetAllShareholdersCached();
            List<Artikel>? articles = GetAllArticlesCached();
            List<Kunde>? customers = GetAllCustomersCached();
            AssignmentFormViewModel? afvm = new()
            {
                Vertriebsmitarbeiter = salesStaff,
                Gesellschafter = shareholders ?? [],
                Artikel = articles ?? [],
                Kunden = customers ?? []
            };
            return afvm;
        }

        public List<AssignmentViewModel>? GetAssignmentVMsPaginatedByUserId(string userId, Pagination? pagination)
        {
            List<Auftrag>? assignments = _dataAccess.ReadAssignmentsPaginatedByUserId(userId, pagination);
            if (assignments == null)
            {
                return null;
            }
            List<int>? assignmentPKs = assignments.Select(i => i.PK_Auftrag).ToList();
            List<Gesellschafter>? shareholders = GetAllShareholdersCached();
            List<Artikel>? articles = GetAllArticlesCached();
            List<Kunde>? customers = GetAllCustomersCached();
            List<PositionViewModel>? pvms = _dataAccess.ReadPositionVMsByAssignmentPKs(assignmentPKs) ?? [];
            List<AssignmentViewModel>? avms = assignments.Select(i => new AssignmentViewModel() { Auftrag = i }).ToList();
            if (articles == null || customers == null || shareholders == null)
            {
                return null;
            }
            foreach (var avm in avms)
            {
                avm.PositionenVM = [.. pvms.Where(i => i.Position?.FK_Auftrag == avm.Auftrag?.PK_Auftrag)];
                avm.PositionenVM.ForEach(item => item.Artikel = articles.Find(i => i.PK_Artikel == item.Position?.FK_Artikel));
                avm.Kunde = customers.Find(i => i.PK_Kunde == avm.Auftrag?.FK_Kunde);
                avm.Gesellschafter = shareholders.Find(i => i.PK_Gesellschafter == avm.Kunde?.FK_Gesellschafter);
            }
            return avms;
        }

        public Auth? GetAuthByUserPk(int userPk) => _dataAccess.ReadObjectByForeignKey(new Auth(), new Vertriebsmitarbeiter(), userPk);

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
                    TableName = "vta_Position"
                }
            };
            return pvm;
        }

        public Vertriebsmitarbeiter? GetUserByUserId(string userId) => _dataAccess.ReadUserByUserId(userId);

        public int SaveAssignmentVM(AssignmentViewModel? avm)
        {
            if (avm?.Auftrag == null)
            {
                return 0;
            }
            int pkAssignment = _dataAccess.Create(avm.Auftrag);
            if (avm.PositionenVM != null && avm.PositionenVM.Count > 0)
            {
                CreatePositions(avm.PositionenVM, pkAssignment);
            }

            UpdateCachedModel(new AssignmentViewModel());
            return pkAssignment;
        }

        public int SaveCustomer(Kunde? customer)
        {
            if (customer == null)
            {
                return 0;
            }
            int pk = _dataAccess.Create(customer);
            UpdateCachedModel(new Kunde());
            return pk;
        }

        public void Update<T>(T? model, string columnToUpdate) where T : IDatabaseObject => Update(model, new[] { columnToUpdate });

        public void Update<T>(T? model, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject
        {
            _dataAccess.Update(model, columnsToUpdate);
        }

        #endregion Public Methods

        #region Private Methods

        private void CreatePositions(List<PositionViewModel>? positions, int pkAssignment)
        {
            if (positions == null || !positions.Any())
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

            _dataAccess.CreateAll(positionList);
        }

        private List<T>? GetCachedModel<T>(T? model) where T : IDatabaseObject
        {
            if (model == null)
            {
                return null;
            }
            if (_memoryCache.TryGetValue(model.GetType().Name, out string? cachedModelJson))
            {
                return JsonSerializer.Deserialize<List<T>?>(cachedModelJson!);
            }
            return UpdateCachedModel(model);
        }

        private List<T>? UpdateCachedModel<T>(T? model) where T : IDatabaseObject
        {
            if (model == null)
            {
                return null;
            }
            List<T>? modelData = _dataAccess.ReadAll(model);
            _memoryCache.Remove(model.GetType().Name);

            _memoryCache.Set(model.GetType().Name, JsonSerializer.Serialize(modelData), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
                SlidingExpiration = TimeSpan.FromMinutes(60)
            });

            return modelData;
        }

        #endregion Private Methods
    }
}