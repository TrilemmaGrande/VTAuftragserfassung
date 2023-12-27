using Microsoft.Extensions.Caching.Memory;
using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Models;
using VTAuftragserfassung.Models.ViewModels;

namespace VTAuftragserfassung.Database.Repository
{
    public class DbRepository : IDbRepository
    {
        #region Public Fields

        public readonly IDataAccess<IDatabaseObject> _dataAccess;
        public readonly IMemoryCache _memoryCache;

        #endregion Public Fields

        #region Public Constructors

        public DbRepository(IDataAccess<IDatabaseObject> dataAccess, IMemoryCache memoryCache)
        {
            _dataAccess = dataAccess;
            _memoryCache = memoryCache;
        }

        #endregion Public Constructors

        #region Public Methods


        private List<T> GetCachedModel<T>(T model) where T : IDatabaseObject
        {
            if (_memoryCache.TryGetValue(model.GetType().Name, out List<T> cachedModel))
            {
                return cachedModel;
            }
            List<T> modelData = _dataAccess.GetAll(model);
            _memoryCache.Set(model.GetType().Name, modelData);
            return modelData;
        }

        public List<Gesellschafter> GetAllShareholders() => GetCachedModel(new Gesellschafter());

        public List<Artikel> GetAllArticles() => GetCachedModel(new Artikel());

        public List<Kunde> GetAllCustomers() => _dataAccess.GetAll(new Kunde());

        public List<Auftrag> GetAssignmentsByUserId(string userId) => _dataAccess.GetAllByCondition(new Auftrag(), "*",
            $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter) WHERE MitarbeiterId = '{userId}'");

        public List<AssignmentViewModel> GetAssignmentVMsByUserId(string userId)
        {
            List<Gesellschafter> shareholders = GetAllShareholders();
            List<Artikel> articles = GetAllArticles();
            List<AssignmentViewModel> avms = _dataAccess.GetAllByCondition(new AssignmentViewModel(), "vta_Auftrag.*, vta_Kunde.*, vta_Vertriebsmitarbeiter.MitarbeiterId",
                    $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter)" +
                    $"INNER JOIN vta_Kunde ON ( vta_Auftrag.FK_Kunde = vta_Kunde.PK_Kunde) " +
                    $"WHERE MitarbeiterId = '{userId}'" +
                    $"ORDER BY ErstelltAm");
            List<PositionViewModel> pvms = GetPositionVMsByUserId(userId);

            foreach (var avm in avms)
            {
                avm.PositionenVM!.AddRange(pvms.Where(i => i.Position!.FK_Auftrag == avm.Auftrag!.PK_Auftrag));
                avm.PositionenVM!.ForEach(item => item.Artikel = articles.Find(i => i.PK_Artikel == item.Position.FK_Artikel));
                avm.Gesellschafter = shareholders.Find(i => i.PK_Gesellschafter == avm.Kunde.FK_Gesellschafter);                
            }
            return avms;
        }

        public List<PositionViewModel> GetPositionVMsByUserId(string userId) => _dataAccess.GetAllByCondition(new PositionViewModel(), "*",
            $"INNER JOIN vta_Auftrag ON ( vta_Position.FK_Auftrag = vta_Auftrag.PK_Auftrag)" +
            $"INNER JOIN vta_Vertriebsmitarbeiter ON ( vta_Auftrag.FK_Vertriebsmitarbeiter = vta_Vertriebsmitarbeiter.PK_Vertriebsmitarbeiter)" +
            $"WHERE MitarbeiterId = '{userId}'");

        public Vertriebsmitarbeiter? GetUserByUserId(string userId) => _dataAccess.GetByCondition(new Vertriebsmitarbeiter(), "*", $"WHERE MitarbeiterId = '{userId}'");

        public Auth? GetAuthByUserPk(int pk_vertriebsmitarbeiter) => _dataAccess.GetByCondition(new Auth(), "*", $"WHERE FK_Vertriebsmitarbeiter = {pk_vertriebsmitarbeiter}");

        public T? GetObjectByPrimaryKey<T>(T model, int pk) where T : IDatabaseObject => _dataAccess.GetByCondition(model, "*", $"WHERE PK_{model.GetType().Name} = {pk}");

        public T1? GetObjectByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => _dataAccess.GetByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        public List<T1>? GetObjectListByForeignKey<T1, T2>(T1 model, T2 foreignModel, int fk)
            where T1 : IDatabaseObject
            where T2 : IDatabaseObject
            => _dataAccess.GetAllByCondition(model, "*", $"Where FK_{foreignModel!.GetType().Name} = {fk}");

        #endregion Public Methods
    }
}