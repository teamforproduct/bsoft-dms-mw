using BL.Model.Database;
using BL.Model.Database.FrontModel;
using BL.Model.Database.IncomingModel;
using BL.Model.Exception;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Represend functionality to configure available servers
    /// </summary>
    public class Servers
    {
        /// <summary>
        /// Get list of the available servers to display for user.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FrontServer> GetServersByUser(string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var items = dbContext.AspNetUserServersSet
                    .Where(x => userId.Equals(x.UserId, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(x => x.Server)
                    .Select(x => x.Key)
                    .Select(x => new FrontServer
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ClientName = x.ClientId.HasValue ? x.Client.Name : string.Empty
                    }).ToList();

                return items;
            }
            return GetServers().ToList().Select(x => new FrontServer { Id = x.Id, Name = x.Name, ClientName = x.ClientName });
        }

        /// <summary>
        /// Get all server parameters by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DatabaseModel GetServer(int id)
        {
            return GetServers().FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// List of all aceptable servers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DatabaseModel> GetServers()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var items = dbContext.AdminServersSet.ToList().Select(x => new DatabaseModel
                {
                    Id = x.Id,
                    Address = x.Address,
                    Name = x.Name,
                    ServerType = (DatabaseType)Enum.Parse(typeof(DatabaseType), x.ServerType),
                    DefaultDatabase = x.DefaultDatabase,
                    IntegrateSecurity = x.IntegrateSecurity,
                    UserName = x.UserName,
                    UserPassword = x.UserPassword,
                    ConnectionString = x.ConnectionString,
                    DefaultSchema = x.DefaultSchema,
                    ClientName = x.ClientId.HasValue ? x.Client.Name : string.Empty,
                    ClientId = x.ClientId.HasValue ? x.ClientId.Value : 0
                }).ToList();

                return items;
            }
        }

        /// <summary>
        /// Add new server to list of aceptable servers
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public int AddServer(ModifyServer modal)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AdminServers
                    {
                        Address = modal.Address,
                        Name = modal.Name,
                        ServerType = modal.ServerType.ToString(),
                        DefaultDatabase = modal.DefaultSchema,
                        IntegrateSecurity = modal.IntegrateSecurity,
                        UserName = modal.UserName,
                        UserPassword = modal.UserPassword,
                        ConnectionString = modal.ConnectionString,
                        DefaultSchema = modal.DefaultSchema,
                        ClientId = modal.ClientId
                    };
                    dbContext.AdminServersSet.Add(item);
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
        /// Modify server parameters
        /// </summary>
        /// <param name="modal"></param>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public void UpdateServer(ModifyServer modal)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AdminServers
                    {
                        Id = modal.Id,
                        Address = modal.Address,
                        Name = modal.Name,
                        ServerType = modal.ServerType.ToString(),
                        DefaultDatabase = modal.DefaultSchema,
                        IntegrateSecurity = modal.IntegrateSecurity,
                        UserName = modal.UserName,
                        UserPassword = modal.UserPassword,
                        ConnectionString = modal.ConnectionString,
                        DefaultSchema = modal.DefaultSchema,
                        ClientId = modal.ClientId
                    };
                    dbContext.AdminServersSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Modified;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        /// <summary>
        /// Delete server from the list
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DictionaryRecordCouldNotBeDeleted"></exception>
        public void DeleteServer(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AdminServers
                    {
                        Id = id
                    };
                    dbContext.AdminServersSet.Attach(item);

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