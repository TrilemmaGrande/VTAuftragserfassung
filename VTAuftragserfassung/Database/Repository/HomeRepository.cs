﻿using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Database.Repository.Interfaces;
using VTAuftragserfassung.Models.DBO;
using VTAuftragserfassung.Models.Shared;
using VTAuftragserfassung.Models.ViewModel;

namespace VTAuftragserfassung.Database.Repository
{
    public class HomeRepository : IHomeRepository
    {
        #region Private Fields

        private readonly ICachingService _caching;
        private readonly ISessionService _session;
        private readonly IDataAccess<IDatabaseObject> _dataAccess;

        #endregion Private Fields

        #region Public Constructors

        public HomeRepository(IDataAccess<IDatabaseObject> dataAccess, ICachingService caching, ISessionService session)
        {
            _dataAccess = dataAccess;
            _caching = caching;
            _session = session;
        }

        #endregion Public Constructors

        #region Public Methods

        public List<Artikel>? GetAllArticlesCached() => GetCachedModels(new Artikel());

        public List<Kunde>? GetAllCustomers() => _dataAccess.ReadAll(new Kunde());

        public List<Kunde>? GetAllCustomersCached() => GetCachedModels(new Kunde());

        public List<Gesellschafter>? GetAllShareholdersCached() => GetCachedModels(new Gesellschafter());

        public Vertriebsmitarbeiter? GetUserFromSession(string userId)
        {
            Vertriebsmitarbeiter? user = GetSessionModels(new Vertriebsmitarbeiter(), userId)?.FirstOrDefault() ?? _dataAccess.ReadUserByUserId(userId);
            if (user != null)
            {
                _session.SetSessionModels(userId, new List<Vertriebsmitarbeiter>() { user });
            }
            return user;
        }

        public AssignmentFormViewModel? GetAssignmentFormVMByUserId(string userId)
        {
            Vertriebsmitarbeiter? salesStaff = GetUserFromSession(userId);
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
            List<Position>? positions = _dataAccess.ReadPositionsByAssignmentPKs(assignmentPKs) ?? [];
            List<PositionViewModel>? pvms = positions?.Select(p => new PositionViewModel() { Position = p }).ToList() ?? [];
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

      

        public int SaveAssignmentVM(AssignmentViewModel? avm)
        {
            if (avm?.Auftrag == null)
            {
                return 0;
            }
            int pkAssignment = _dataAccess.Create(avm.Auftrag);
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
            int pk = _dataAccess.Create(customer);
            _caching.InvalidateCacheModels(new Kunde());
            return pk;
        }

        public void Update<T>(T? model, string columnToUpdate) where T : IDatabaseObject => Update(model, new[] { columnToUpdate });

        public void Update<T>(T? model, IEnumerable<string>? columnsToUpdate = null) where T : IDatabaseObject
        {
            _dataAccess.Update(model, columnsToUpdate);
        }

        #endregion Public Methods

        #region Private Methods

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

            _dataAccess.CreateAll(positionList);
        }

        private List<T>? GetCachedModels<T>(T model) where T : IDatabaseObject
        {
            return _caching.GetCachedModels(model) ?? _caching.UpdateCachedModels(_dataAccess.ReadAll(model));
        }

        private List<T>? GetSessionModels<T>(T model, string sKey) where T : IDatabaseObject
        {
            return _session.GetSessionModels(model, sKey);
        }

        #endregion Private Methods
    }
}