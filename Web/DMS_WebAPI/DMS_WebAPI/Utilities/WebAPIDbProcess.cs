using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Common;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DatabaseContext;
using DMS_WebAPI.DBModel;
using EntityFramework.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIDbProcess
    {
        public WebAPIDbProcess()
        {

        }

        #region UserClients

        public IEnumerable<FrontAspNetUserClient> GetUserClientList(FilterAspNetUserClient filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserClientQuery(dbContext, filter);

                var res = qry.Select(x => new FrontAspNetUserClient
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    UserId = x.UserId,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        private IQueryable<AspNetUserClients> GetUserClientQuery(ApplicationDbContext dbContext, FilterAspNetUserClient filter)
        {
            var qry = dbContext.AspNetUserClientsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.UserServerIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserClients>(false);
                    filterContains = filter.UserServerIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ClientIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserClients>(false);
                    filterContains = filter.ClientIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.UserIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserClients>(false);
                    filterContains = filter.UserIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.UserId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public int AddUserClient(SetUserClient model)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AspNetUserClients
                {
                    UserId = model.UserId,
                    ClientId = model.ClientId,
                };
                dbContext.AspNetUserClientsSet.Add(item);
                dbContext.SaveChanges();

                transaction.Complete();
                return item.Id;
            }
        }

        public void DeleteUserClient(FilterAspNetUserClient filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserClientQuery(dbContext, filter);
                dbContext.AspNetUserClientsSet.RemoveRange(qry);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        #endregion

        #region UserFingerprint

        private IQueryable<AspNetUserFingerprints> GetUserFingerprintQuery(ApplicationDbContext dbContext, FilterAspNetUserFingerprint filter)
        {
            var qry = dbContext.AspNetUserFingerprintsSet.AsQueryable();

            if (filter != null)
            {

                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserFingerprints>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserFingerprints>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.UserIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserFingerprints>(false);
                    filterContains = filter.UserIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.UserId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.BrowserExact))
                {
                    qry = qry.Where(x => filter.BrowserExact.Equals(x.Browser, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(filter.PlatformExact))
                {
                    qry = qry.Where(x => filter.PlatformExact.Equals(x.Platform, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(filter.FingerprintExact))
                {
                    qry = qry.Where(x => filter.FingerprintExact.Equals(x.Fingerprint, StringComparison.OrdinalIgnoreCase));
                }

                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive);
                }

            }

            return qry;
        }
        public IEnumerable<FrontAspNetUserFingerprint> GetUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserFingerprintQuery(dbContext, filter);

                var items = qry.Select(x => new FrontAspNetUserFingerprint
                {
                    Id = x.Id,
                    Fingerprint = x.Fingerprint.Substring(0, 7) + "...",
                    Name = x.Name,
                    Browser = x.Browser,
                    Platform = x.Platform,
                    IsActive = x.IsActive,

                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public bool ExistsUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserFingerprintQuery(dbContext, filter);

                var res = qry.Any();

                transaction.Complete();
                return res;
            }
        }

        public int AddUserFingerprint(AddAspNetUserFingerprint model)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AspNetUserFingerprints
                {
                    UserId = model.UserId,
                    Browser = model.Browser,
                    Platform = model.Platform,
                    Fingerprint = model.Fingerprint,
                    IsActive = true,// model.IsActive,
                    LastChangeDate = DateTime.UtcNow,
                    Name = model.Name,
                };
                dbContext.AspNetUserFingerprintsSet.Add(item);
                dbContext.SaveChanges();

                transaction.Complete();
                return item.Id;
            }
        }

        public void UpdateUserFingerprint(ModifyAspNetUserFingerprint model)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AspNetUserFingerprints
                {
                    Id = model.Id,
                    Name = model.Name,
                    IsActive = true,//model.IsActive,
                    LastChangeDate = DateTime.UtcNow,

                };
                dbContext.AspNetUserFingerprintsSet.Attach(item);

                var entry = dbContext.Entry(item);
                entry.Property(p => p.Name).IsModified = true;
                entry.Property(p => p.IsActive).IsModified = true;
                entry.Property(p => p.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteUserFingerprints(FilterAspNetUserFingerprint filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserFingerprintQuery(dbContext, filter);
                qry.Delete();
                transaction.Complete();
            }
        }


        #endregion

        #region UserContexts

        private IQueryable<AspNetUserContexts> GetUserContextQuery(ApplicationDbContext dbContext, FilterAspNetUserContext filter)
        {
            var qry = dbContext.AspNetUserContextsSet.AsQueryable();

            if (filter != null)
            {

                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserContexts>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserContexts>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.UserIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserContexts>(false);
                    filterContains = filter.UserIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.UserId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ClientIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserContexts>(false);
                    filterContains = filter.ClientIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TokenExact))
                {
                    qry = qry.Where(x => filter.TokenExact.Equals(x.Token, StringComparison.OrdinalIgnoreCase));
                }

                if (filter.LastUsegeDateLess.HasValue)
                {
                    qry = qry.Where(x => x.LastChangeDate < filter.LastUsegeDateLess.Value);
                }

            }

            return qry;
        }

        public IEnumerable<AspNetUserContexts> GetUserContexts(FilterAspNetUserContext filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserContextQuery(dbContext, filter);

                var items = qry.ToList();
                transaction.Complete();
                return items;
            }
        }

        public bool ExistsUserContexts(FilterAspNetUserContext filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserContextQuery(dbContext, filter);

                var res = qry.Any();

                transaction.Complete();
                return res;
            }
        }

        public int AddUserContext(AspNetUserContexts item)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AspNetUserContextsSet.Add(item);
                dbContext.SaveChanges();

                transaction.Complete();
                return item.Id;
            }
        }

        public void UpdateUserContext(AspNetUserContexts item)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AspNetUserContextsSet.Attach(item);
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateUserContextLastChangeDate(string token, DateTime date)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserContextQuery(dbContext, new FilterAspNetUserContext { TokenExact = token });

                qry.Update(x => new AspNetUserContexts { LastChangeDate = date });

                transaction.Complete();
            }

        }

        public void DeleteUserContext(string token)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AspNetUserContextsSet.Where(x => x.Token == token).Delete();
                transaction.Complete();
            }
        }

        public void DeleteUserContexts(FilterAspNetUserContext filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserContextQuery(dbContext, filter);
                qry.Delete();
                transaction.Complete();
            }
        }




        #endregion

        public IEnumerable<ListItem> GetControlQuestions(string language)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemControlQuestionsSet.AsQueryable();

                qry = qry.OrderBy(x => x.Id);

                var items = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    Name = x.Name,

                }).ToList();
                transaction.Complete();

                var tmpService = DmsResolver.Current.Get<ILanguages>();
                items.ForEach(x => x.Name = tmpService.GetTranslation(language, x.Name));

                return items;
            }
        }

    }
}