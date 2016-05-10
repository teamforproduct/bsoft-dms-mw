using BL.CrossCutting.Helpers;
using BL.Model.AspNet.Filters;
using BL.Model.AspNet.FrontModel;
using BL.Model.AspNet.IncomingModel;
using BL.Model.Exception;
using DMS_WebAPI.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DMS_WebAPI.Utilities.AspNet
{
    public class AspNetUserServers
    {
        /// <summary>
        /// Get UserServer parameters by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FrontAspNetUserServers GetUserServer(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var items = dbContext.AspNetUserServersSet
                    .Where(x => x.Id == id)
                    .Select(x => new FrontAspNetUserServers
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        ServerId = x.ServerId,
                        UserName = x.User.UserName,
                        ServerName = x.Server.Name
                    }).FirstOrDefault();

                return items;
            }
        }

        /// <summary>
        /// List of all Users on Servers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FrontAspNetUserServers> GetUserServers(FilterAspNetUserServers filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var qry = dbContext.AspNetUserServersSet.AsQueryable();
                if (filter != null)
                {
                    if (filter.UserIds?.Any()??false)
                    {
                        qry.Where(x => filter.UserIds.Contains(x.UserId));
                    }

                    if (filter.ServerIds?.Any() ?? false)
                    {
                        qry.Where(x => filter.ServerIds.Contains(x.ServerId));
                    }
                }

                var items = qry.Select(x => new FrontAspNetUserServers
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ServerId = x.ServerId,
                    UserName = x.User.UserName,
                    ServerName = x.Server.Name
                }).ToList();

                return items;
            }
        }

        /// <summary>
        /// Add new user server
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public int AddUserServer(ModifyAspNetUserServers modal)
        {
            try
            {
                var si = new SystemInfo();
                var dbw = new SystemDbWorker();

                var lic = dbw.GetLicenceInfoByServerId(modal.ServerId);

                var regCode = si.GetRegCode(lic);

                if (lic.IsNamedLicence)
                {
                    lic.ConcurenteNumberOfConnectionsNow++;
                }

                VerifyLicence.Verify(regCode, lic);

                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new DBModel.AspNetUserServers
                    {
                        UserId = modal.UserId,
                        ServerId = modal.ServerId,
                    };
                    dbContext.AspNetUserServersSet.Add(item);
                    dbContext.SaveChanges();

                    modal.Id = item.Id;

                    return modal.Id;
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        /// <summary>
        /// Delete user from the list servers
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DictionaryRecordCouldNotBeDeleted"></exception>
        public void DeleteUserServer(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new DBModel.AspNetUserServers
                    {
                        Id = id
                    };
                    dbContext.AspNetUserServersSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }
    }
}