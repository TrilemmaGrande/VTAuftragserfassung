﻿using Microsoft.Extensions.Caching.Memory;
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

        public Auth? GetAuthByUserPk(int userPk) => _dataAccess.ReadObjectByForeignKey(new Auth(), new Vertriebsmitarbeiter(), userPk);

        public Vertriebsmitarbeiter? GetUserByUserId(string userId) => _dataAccess.ReadUserByUserId(userId);

        public List<Artikel> GetAllArticlesCached() => GetCachedModel(new Artikel());

        public List<Kunde> GetAllCustomers() => _dataAccess.ReadAll(new Kunde());

        public List<Kunde> GetAllCustomersCached() => GetCachedModel(new Kunde());

        public List<Gesellschafter> GetAllShareholdersCached() => GetCachedModel(new Gesellschafter());

        public List<AssignmentViewModel> GetAssignmentVMsByUserId(string userId)
        {
            List<Auftrag> assignments = _dataAccess.ReadAssignmentsByUserId(userId);
            List<Gesellschafter> shareholders = GetAllShareholdersCached();
            List<Artikel> articles = GetAllArticlesCached();
            List<Kunde> customers = GetAllCustomersCached();
            List<PositionViewModel> pvms = _dataAccess.ReadPositionVMsByUserId(userId);

            List<AssignmentViewModel> avms = assignments.Select(i => new AssignmentViewModel() { Auftrag = i }).ToList();
            foreach (var avm in avms)
            {
                avm.PositionenVM!.AddRange(pvms.Where(i => i.Position?.FK_Auftrag == avm.Auftrag?.PK_Auftrag));
                avm.PositionenVM!.ForEach(item => item.Artikel = articles.Find(i => i.PK_Artikel == item.Position?.FK_Artikel));
                avm.Kunde = customers.Find(i => i.PK_Kunde == avm.Auftrag?.FK_Kunde);
                avm.Gesellschafter = shareholders.Find(i => i.PK_Gesellschafter == avm.Kunde?.FK_Gesellschafter);
            }
            return avms;
        }

        public AssignmentFormViewModel GetAssignmentFormVMByUserId(string userId)
        {
            Vertriebsmitarbeiter salesStaff = GetUserByUserId(userId) ?? new();
            List<Gesellschafter> shareholders = GetAllShareholdersCached() ?? [];
            List<Artikel> articles = GetAllArticlesCached() ?? [];
            List<Kunde> customers = GetAllCustomers() ?? [];
            AssignmentFormViewModel afvm = new()
            {
                Vertriebsmitarbeiter = salesStaff,
                Gesellschafter = shareholders,
                Artikel = articles,
                Kunden = customers
            };
            return afvm;
        }

        public PositionViewModel GetNewPositionVMByArticlePK(int articlePK)
        {
            List<Artikel> articles = GetAllArticlesCached();
            Artikel article = articles?.Find(i => i.PK_Artikel == articlePK)!;
            PositionViewModel pvm = new();
            pvm.Artikel = article;
            pvm.Position = new()
            {
                FK_Artikel = articlePK,
                Menge = 1,
                SummePosition = article.Preis,
                TableName = "vta_Position"
            };
            return pvm;
        }

        public int SaveCustomer(Kunde customer) => _dataAccess.Create(customer);

        public int SaveAssignmentVM(AssignmentViewModel avm)
        {
            int pkAssignment = _dataAccess.Create(avm.Auftrag);
            List<Position> positions = [];
            avm.PositionenVM.ForEach(i =>
            {
                i.Position.FK_Auftrag = pkAssignment;
                positions.Add(i.Position);
            });
            _dataAccess.CreateAll(positions);
            return pkAssignment;
        }

        #endregion Public Methods

        #region Private Methods

        private List<T> GetCachedModel<T>(T model) where T : IDatabaseObject
        {
            if (_memoryCache.TryGetValue(model?.GetType().Name!, out List<T>? cachedModel))
            {
                return cachedModel ?? [];
            }
            List<T> modelData = _dataAccess.ReadAll(model!);
            _memoryCache.Set(model!.GetType().Name, modelData);
            return modelData;
        }

        #endregion Private Methods
    }
}