﻿using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Database.DatabaseContext;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Database.DBModel.Admin;
using LinqKit;
using System.Transactions;
using BL.CrossCutting.Helpers;

namespace BL.Database.Admins
{
    public class LanguagesDbProcess : CoreDb.CoreDb, ILanguagesDbProcess
    {
        public LanguagesDbProcess()
        {
        }

        public AdminLanguageInfo GetAdminLanguage(IContext context)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = new AdminLanguageInfo();

                res.Languages = dbContext.AdminLanguagesSet.Select(x => new InternalAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).ToList();

                res.LanguageValues = dbContext.AdminLanguageValuesSet.Select(x => new InternalAdminLanguageValue
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    Label = x.Label,
                    Value = x.Value
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        #region AdminLanguages
        private IQueryable<AdminLanguages> GetAdminLanguagesQuery(DmsContext dbContext, FilterAdminLanguage filter)
        {
            var qry = dbContext.AdminLanguagesSet.AsQueryable();

            if (filter.IDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminLanguages>();
                filterContains = filter.IDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (!string.IsNullOrEmpty(filter.Code))
            {
                qry = qry.Where(x => filter.Code.Equals(x.Code));
            }

            return qry;
        }

        public IEnumerable<FrontAdminLanguage> GetAdminLanguages(IContext context, FilterAdminLanguage filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminLanguagesQuery(dbContext, filter);

                var res = qry.Select(x => new FrontAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontAdminUserLanguage> GetAdminUserLanguages(IContext context, int userId, FilterAdminLanguage filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminLanguagesQuery(dbContext, filter);

                var res = qry.Select(x => new FrontAdminUserLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault,
                    IsChecked = x.AgentUsers.Where(y => y.LanguageId == x.Id).Any(),
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public InternalAdminLanguage GetInternalAdminLanguage(IContext context, FilterAdminLanguage filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminLanguagesQuery(dbContext, filter);
                var res =  qry.Select(x => new InternalAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int AddAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AdminLanguages
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsDefault = model.IsDefault
                };
                dbContext.AdminLanguagesSet.Add(item);
                dbContext.SaveChanges();
                model.Id = item.Id;
                transaction.Complete();
                return item.Id;
            }
        }

        public void UpdateAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AdminLanguages
                {
                    Id = model.Id,
                    Code = model.Code,
                    Name = model.Name,
                    IsDefault = model.IsDefault
                };
                dbContext.AdminLanguagesSet.Attach(item);
                var entity = dbContext.Entry(item);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.IsDefault).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        public void DeleteAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AdminLanguagesSet.RemoveRange(dbContext.AdminLanguagesSet.Where(x => x.Id == model.Id));
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        #endregion AdminLanguages

        #region AdminLanguageValues

        private IQueryable<AdminLanguageValues> GetAdminLanguageValuesQuery(IContext context, DmsContext dbContext, FilterAdminLanguageValue filter)
        {
            var qry = dbContext.AdminLanguageValuesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter.LanguageValueId?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminLanguageValues>();
                filterContains = filter.LanguageValueId.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.LanguageId.HasValue)
            {
                if (filter.LanguageId > 0)
                {
                    qry = qry.Where(x => x.Id == filter.LanguageId);
                }
                else
                {
                    qry = qry.Where(x => x.Language.IsDefault);
                }
            }

            if (!string.IsNullOrEmpty(filter.Label))
            {
                qry = qry.Where(x => filter.Label.Equals(x.Label));
            }

            if (filter.Labels?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminLanguageValues>();
                filterContains = filter.Labels.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Label == value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;
        }

        public IEnumerable<FrontAdminLanguageValue> GetAdminLanguageValues(IContext context, FilterAdminLanguageValue filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminLanguageValuesQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontAdminLanguageValue
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    Label = x.Label,
                    Value = x.Value
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public InternalAdminLanguageValue GetInternalAdminLanguageValue(IContext context, FilterAdminLanguageValue filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminLanguageValuesQuery(context, dbContext, filter);

                var res = qry.Select(x => new InternalAdminLanguageValue
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    Label = x.Label,
                    Value = x.Value
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int AddAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AdminLanguageValues
                {
                    LanguageId = model.LanguageId,
                    Label = model.Label,
                    Value = model.Value,
                    ClientId = context.CurrentClientId
                };
                dbContext.AdminLanguageValuesSet.Add(item);
                dbContext.SaveChanges();
                model.Id = item.Id;
                transaction.Complete();
                return item.Id;
            }
        }

        public void AddAdminLanguageValues(IContext context, List<AdminLanguageValues> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AdminLanguageValuesSet.AddRange(list);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AdminLanguageValues
                {
                    Id = model.Id,
                    LanguageId = model.LanguageId,
                    Label = model.Label,
                    Value = model.Value
                };
                dbContext.AdminLanguageValuesSet.Attach(item);
                var entity = dbContext.Entry(item);

                entity.Property(x => x.LanguageId).IsModified = true;
                entity.Property(x => x.Label).IsModified = true;
                entity.Property(x => x.Value).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        public void DeleteAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AdminLanguageValuesSet.RemoveRange(dbContext.AdminLanguageValuesSet.Where(x => x.Id == model.Id));
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteAllAdminLanguageValues(IContext context)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AdminLanguageValuesSet.RemoveRange(dbContext.AdminLanguageValuesSet);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        #endregion AdminLanguageValues
    }
}
