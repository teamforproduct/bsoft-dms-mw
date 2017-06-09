using BL.CrossCutting.Helpers;
using BL.Database.Common;
using BL.Database.Helper;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.DatabaseContext;
using DMS_WebAPI.DBModel;
using EntityFramework.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS_WebAPI.Utilities
{
    internal partial class WebAPIDbProcess
    {
        private IQueryable<SessionLogs> GetSessionLogsQuery(ApplicationDbContext dbContext, FilterSessionsLog filter)
        {
            var qry = dbContext.SessionLogsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.DateFrom.HasValue)
                {
                    qry = qry.Where(x => x.LogDate >= filter.DateFrom.Value);
                }

                if (filter.DateTo.HasValue)
                {
                    qry = qry.Where(x => x.LogDate <= filter.DateTo.Value);
                }

                if (!string.IsNullOrEmpty(filter.Message))
                {
                    var filterContains = PredicateBuilder.New<SessionLogs>(true);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Message)
                                .Aggregate(filterContains, (current, value) => current.And(e => e.Message.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.UserFullName))
                {
                    var filterContains = PredicateBuilder.New<SessionLogs>(true);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.UserFullName)
                                .Aggregate(filterContains, (current, value) => current.And(e => e.User.FirstName.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filter.FullTextSearchString))
                {
                    var filterContains = PredicateBuilder.New<SessionLogs>(true);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullTextSearchString)
                                .Aggregate(filterContains, (current, value) => current.And(e => 
                                (e.Message + " " + e.Event + " " + e.User.UserName + " " + e.User.FirstName + " "+ e.IP + " "+ e.Fingerprint + " " + e.Platform + " " + e.Browser + " ").Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.Types?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<SessionLogs>(false);
                    filterContains = filter.Types.Aggregate(filterContains, (current, value) => current.Or(e => e.Type == (int)value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.IPExact))
                {
                    qry = qry.Where(x => x.IP == filter.IPExact);
                }

                if (!string.IsNullOrEmpty(filter.UserId))
                {
                    qry = qry.Where(x => x.UserId == filter.UserId);
                }

                if (filter.IsActive.HasValue && filter.IsActive.Value)
                {
                    var time = DateTime.UtcNow.AddMinutes(-2);
                    qry = qry.Where(x => x.LastUsage > time);
                }

                if (filter.IsEnabled.HasValue)
                {
                    qry = qry.Where(x => x.Enabled == filter.IsEnabled.Value);
                }

            }

            return qry;
        }


        public IEnumerable<FrontSessionLog> GetSessionLogs(FilterSessionsLog filter, UIPaging paging)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSessionLogsQuery(dbContext, filter);

                qry = qry.OrderByDescending(x => x.LogDate).ThenBy(x => x.Id);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontSessionLog>();

                var time = DateTime.UtcNow.AddMinutes(-2);

                var res = qry.Select(x => new FrontSessionLog
                {
                    Id = x.Id,
                    Date = x.LogDate,
                    LastUsage = x.LastUsage,
                    Message = x.Message,
                    Event = x.Event,
                    Platform = x.Platform,
                    IP = x.IP,
                    Type = ((EnumLogTypes)x.Type).ToString(),
                    Host = "*.ostrean.com",
                    Browser = x.Browser,
                    Fingerprint = x.Fingerprint,
                    IsActive = x.LastUsage.HasValue ? x.LastUsage.Value > time : false,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public int AddSessionLog(AddSessionLog model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {

                    var item = new SessionLogs
                    {
                        LogDate = model.Date,
                        LastUsage = model.LastUsage,
                        Enabled = true,
                        Type = (int)model.Type,
                        UserId = model.UserId,
                        Event = model.Event,
                        Browser = model.Browser,
                        Message = model.Message,
                        Platform = model.Platform,
                        Fingerprint = model.Fingerprint,
                        IP = model.IP,
                    };

                    dbContext.SessionLogsSet.Add(item);
                    dbContext.SaveChanges();

                    transaction.Complete();
                    return item.Id;
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }


        public void SetSessionLogLastUsage(DateTime lastUsage, FilterSessionsLog filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSessionLogsQuery(dbContext, filter);

                qry.Update(x => new SessionLogs { LastUsage = lastUsage });

                transaction.Complete();
            }
        }

        public void SetSessionLogEnabled(bool enabled, FilterSessionsLog filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSessionLogsQuery(dbContext, filter);

                qry.Update(x => new SessionLogs { Enabled = enabled });

                transaction.Complete();
            }
        }

        public void DeleteSessionLogs(FilterSessionsLog filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSessionLogsQuery(dbContext, filter);

                qry.Delete();

                transaction.Complete();
            }
        }

    }
}