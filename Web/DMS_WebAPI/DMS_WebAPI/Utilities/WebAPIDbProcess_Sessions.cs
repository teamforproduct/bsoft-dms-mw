using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.Database.Helper;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Context;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
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
        private IQueryable<SessionLogs> GetSessionLogsQuery(ApplicationDbContext dbContext, FilterSessionsLog filter)
        {
            var qry = dbContext.SessionLogsSet.AsQueryable();

            if (filter != null)
            {
            }

            return qry;
        }


        public IEnumerable<FrontSessionLog> GetSessionLogs(FilterSessionsLog filter, UIPaging paging)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSessionLogsQuery(dbContext, filter);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontSessionLog>();

                var time = DateTime.UtcNow.AddMinutes(-2);

                var res = qry.Select(x => new FrontSessionLog
                {
                    Id = x.Id,
                    Date = x.LogDate,
                    LastUsage = x.LastUsage,
                    UserId = x.UserId,
                    UserName = x.User.UserName,
                    Message = x.Message,
                    Event = x.Event,
                    Platform = x.Platform,
                    IP = x.IP,
                    Type = ((EnumLogTypes)x.Type).ToString(),
                    Host = "*.ostrean.com",
                    Browser = x.Browser,
                    Fingerprint = x.Fingerprint,
                    IsSuccess = x.Type == 0,
                    IsActive = x.LastUsage > time
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