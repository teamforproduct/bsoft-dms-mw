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

namespace BL.Database.Admins
{
    public class LanguagesDbProcess : CoreDb.CoreDb, ILanguagesDbProcess
    {
        public LanguagesDbProcess()
        {
        }

        public AdminLanguageInfo GetAdminLanguage(IContext context)
        {
            using (var dbContext = new DmsContext(context))
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

                return res;
            }
        }

        #region AdminLanguages

        public IEnumerable<FrontAdminLanguage> GetAdminLanguages(IContext context, FilterAdminLanguage filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminLanguagesSet.AsQueryable();

                if (filter.LanguageId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LanguageId.Contains(x.Id));
                }

                return qry.Select(x => new FrontAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).ToList();
            }
        }

        public InternalAdminLanguage GetInternalAdminLanguage(IContext context, FilterAdminLanguage filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminLanguagesSet.AsQueryable();

                if (filter.LanguageId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LanguageId.Contains(x.Id));
                }

                if (!string.IsNullOrEmpty(filter.Code))
                {
                    qry = qry.Where(x => filter.Code.Equals(x.Code));
                }

                return qry.Select(x => new InternalAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).FirstOrDefault();
            }
        }

        public int AddAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(context))
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
                return item.Id;
            }
        }

        public void UpdateAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(context))
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
            }
        }


        public void DeleteAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(context))
            {

                var item = dbContext.AdminLanguagesSet.FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.AdminLanguagesSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion AdminLanguages

        #region AdminLanguageValues

        public IEnumerable<FrontAdminLanguageValue> GetAdminLanguageValues(IContext context, FilterAdminLanguageValue filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminLanguageValuesSet.AsQueryable();

                if (filter.LanguageValueId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LanguageValueId.Contains(x.Id));
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

                if (filter.Labels != null)
                {
                    if (filter.Labels.Count > 0)
                        qry = qry.Where(x => filter.Labels.Contains(x.Label));
                    else
                        qry = qry.Where(x => x.Id == 0);
                }

                return qry.Select(x => new FrontAdminLanguageValue
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    Label = x.Label,
                    Value = x.Value
                }).ToList();
            }
        }

        public InternalAdminLanguageValue GetInternalAdminLanguageValue(IContext context, FilterAdminLanguageValue filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminLanguageValuesSet.AsQueryable();

                if (filter.LanguageValueId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LanguageValueId.Contains(x.Id));
                }

                if (filter.LanguageId.HasValue)
                {
                    qry = qry.Where(x => x.Id == filter.LanguageId);
                }

                if (!string.IsNullOrEmpty(filter.Label))
                {
                    qry = qry.Where(x => filter.Label.Equals(x.Label));
                }

                return qry.Select(x => new InternalAdminLanguageValue
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    Label = x.Label,
                    Value = x.Value
                }).FirstOrDefault();
            }
        }

        public int AddAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = new AdminLanguageValues
                {
                    LanguageId = model.LanguageId,
                    Label = model.Label,
                    Value = model.Value
                };
                dbContext.AdminLanguageValuesSet.Add(item);
                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void UpdateAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(context))
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
            }
        }


        public void DeleteAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(context))
            {

                var item = dbContext.AdminLanguageValuesSet.FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.AdminLanguageValuesSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion AdminLanguageValues
    }
}