﻿using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.Common;
using BL.Model.Database;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using EntityFramework.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BL.Database.DatabaseContext;
using Ninject;
using Ninject.Parameters;

namespace DMS_WebAPI.Utilities
{
    internal class WebAPIDbProcess
    {
        public WebAPIDbProcess()
        {

        }

        //        private TransactionScope GetTransaction() => new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });

        #region Servers

        private IQueryable<AdminServers> GetServersQuery(ApplicationDbContext dbContext, FilterAdminServers filter)
        {
            var qry = dbContext.AdminServersSet.AsQueryable();

            if (filter != null)
            {
                if (filter.ClientIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetClientServers>();
                    filterContains = filter.ClientIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(x => x.ClientServers.AsQueryable().Any(filterContains));
                }

                if (filter.ServerIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminServers>();
                    filterContains = filter.ServerIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ServerTypes?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminServers>();
                    filterContains = filter.ServerTypes.Select(x => x.ToString()).ToList().Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ServerType == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public FrontAdminServer GetServer(int id)
        {
            return GetServers(new FilterAdminServers { ServerIds = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAdminServer> GetServers(FilterAdminServers filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetServersQuery(dbContext, filter);

                var items = itemsDb.Select(x => new FrontAdminServer
                {
                    Id = x.Id,
                    Address = x.Address,
                    Name = x.Name,
                    ServerTypeName = x.ServerType,
                    //ServerType = (DatabaseType)Enum.Parse(typeof(DatabaseType), x.ServerType),
                    DefaultDatabase = x.DefaultDatabase,
                    IntegrateSecurity = x.IntegrateSecurity,
                    UserName = x.UserName,
                    UserPassword = x.UserPassword,
                    ConnectionString = x.ConnectionString,
                    DefaultSchema = x.DefaultSchema,
                }).ToList();

                items.ForEach(x => { x.ServerType = (EnumDatabaseType)Enum.Parse(typeof(EnumDatabaseType), x.ServerTypeName); });
                transaction.Complete();
                return items;
            }
        }

        public IEnumerable<DatabaseModel> GetServersByAdmin(FilterAdminServers filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetServersQuery(dbContext, filter);

                var itemsRes = (from server in itemsDb
                                join client in dbContext.AspNetClientServersSet on server.Id equals client.ServerId
                                select new
                                {
                                    Server = server,
                                    ClientId = client.ClientId
                                }).ToList();

                var items = itemsRes.Select(x => new DatabaseModel
                {
                    Id = x.Server.Id,
                    Address = x.Server.Address,
                    Name = x.Server.Name,
                    ServerType = (EnumDatabaseType)Enum.Parse(typeof(EnumDatabaseType), x.Server.ServerType),
                    DefaultDatabase = x.Server.DefaultDatabase,
                    IntegrateSecurity = x.Server.IntegrateSecurity,
                    UserName = x.Server.UserName,
                    UserPassword = x.Server.UserPassword,
                    ConnectionString = x.Server.ConnectionString,
                    DefaultSchema = x.Server.DefaultSchema,
                    ClientId = x.ClientId
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public IEnumerable<FrontAdminServerByUser> GetServersByUser(IContext ctx)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var userClients = GetUserClientsQuery(dbContext, new FilterAspNetUserClients { UserIds = new List<string> { ctx.CurrentEmployee.UserId }, ClientCode = ctx.CurrentEmployee.ClientCode })
                                    .Select(x => x.ClientId);

                var userServers = GetUserServersQuery(dbContext, new FilterAspNetUserServers { UserIds = new List<string> { ctx.CurrentEmployee.UserId } });

                userServers = userServers.Where(x => userClients.Contains(x.ClientId));

                var items = userServers.Select(x => new FrontAdminServerByUser
                {
                    Id = x.Server.Id,
                    Name = x.Server.Name,
                    ClientId = x.Client.Id,
                    ClientName = x.Client.Name
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public DatabaseModel GetServerByUser(string userId, SetUserServer setUserServer)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var filterClients = new FilterAspNetUserClients();
                var filterServers = new FilterAspNetUserServers();

                if (!string.IsNullOrEmpty(userId))
                {
                    filterClients.UserIds = new List<string> { userId };
                    filterServers.UserIds = new List<string> { userId };
                }
                if (setUserServer.ClientId > 0) filterClients.ClientIds = new List<int> { setUserServer.ClientId };
                filterClients.ClientCode = setUserServer.ClientCode;

                if (setUserServer.ServerId > 0) filterServers.ServerIds = new List<int> { setUserServer.ServerId };

                var userClients = GetUserClientsQuery(dbContext, filterClients).Select(x => x.ClientId);

                var userServers = GetUserServersQuery(dbContext, filterServers);

                userServers = userServers.Where(x => userClients.Contains(x.ClientId));

                var item = userServers.Select(x => new DatabaseModel
                {
                    Id = x.Server.Id,
                    Address = x.Server.Address,
                    Name = x.Server.Name,
                    ServerTypeName = x.Server.ServerType,
                    //ServerType = (DatabaseType)Enum.Parse(typeof(DatabaseType), x.Server.ServerType),
                    DefaultDatabase = x.Server.DefaultDatabase,
                    IntegrateSecurity = x.Server.IntegrateSecurity,
                    UserName = x.Server.UserName,
                    UserPassword = x.Server.UserPassword,
                    ConnectionString = x.Server.ConnectionString,
                    DefaultSchema = x.Server.DefaultSchema,
                }).FirstOrDefault();

                item.ServerType = (EnumDatabaseType)Enum.Parse(typeof(EnumDatabaseType), item.ServerTypeName);
                transaction.Complete();
                return item;
            }
        }

        public int AddServer(ModifyAdminServer model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AdminServers
                    {
                        Address = model.Address,
                        Name = model.Name,
                        ServerType = model.ServerType.ToString(),
                        DefaultDatabase = model.DefaultDatabase,
                        IntegrateSecurity = model.IntegrateSecurity,
                        UserName = model.UserName,
                        UserPassword = model.UserPassword,
                        ConnectionString = model.ConnectionString,
                        DefaultSchema = model.DefaultSchema,
                    };
                    dbContext.AdminServersSet.Add(item);
                    dbContext.SaveChanges();

                    model.Id = item.Id;
                    transaction.Complete();
                    return model.Id;
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void InitializerDatabase(ModifyAdminServer model)
        {
            var db = new DatabaseModel
            {
                Address = model.Address,
                Name = model.Name,
                ServerType = model.ServerType,
                DefaultDatabase = model.DefaultDatabase,
                IntegrateSecurity = model.IntegrateSecurity,
                UserName = model.UserName,
                UserPassword = model.UserPassword,
                ConnectionString = model.ConnectionString,
                DefaultSchema = model.DefaultSchema,
                ClientId = model.ClientId
            };
            var ctx = new AdminContext(db);
            ctx.DbContext = DmsResolver.Current.Kernel.Get<DmsContext>(new ConstructorArgument("dbModel", ctx.CurrentDB));
            var sysProc = DmsResolver.Current.Get<ISystemService>();
            sysProc.InitializerDatabase(ctx);
        }

        public void UpdateServer(ModifyAdminServer model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AdminServers
                    {
                        Id = model.Id,
                        Address = model.Address,
                        Name = model.Name,
                        ServerType = model.ServerType.ToString(),
                        DefaultDatabase = model.DefaultDatabase,
                        IntegrateSecurity = model.IntegrateSecurity,
                        UserName = model.UserName,
                        UserPassword = model.UserPassword,
                        ConnectionString = model.ConnectionString,
                        DefaultSchema = model.DefaultSchema,
                    };
                    dbContext.AdminServersSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(p => p.Address).IsModified = true;
                    entry.Property(p => p.Name).IsModified = true;
                    entry.Property(p => p.ServerType).IsModified = true;
                    entry.Property(p => p.DefaultDatabase).IsModified = true;
                    entry.Property(p => p.IntegrateSecurity).IsModified = true;
                    entry.Property(p => p.UserName).IsModified = true;
                    entry.Property(p => p.UserPassword).IsModified = true;
                    entry.Property(p => p.ConnectionString).IsModified = true;
                    entry.Property(p => p.DefaultSchema).IsModified = true;
                    transaction.Complete();
                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteServer(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AdminServers
                    {
                        Id = id
                    };
                    dbContext.AdminServersSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        #endregion Servers

        #region ClientLicences

        private IQueryable<AspNetClientLicences> GetClientLicencesQuery(ApplicationDbContext dbContext, FilterAspNetClientLicences filter)
        {
            var qry = dbContext.AspNetClientLicencesSet.AsQueryable();

            if (filter != null)
            {
                if (filter.ClientLicenceIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetClientLicences>();
                    filterContains = filter.ClientLicenceIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ClientIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetClientLicences>();
                    filterContains = filter.ClientIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsNowUsed.HasValue)
                {
                    if (filter.IsNowUsed.Value)
                    {
                        qry = qry.GroupBy(x => x.ClientId)
                                .Select(x => x
                                    .OrderByDescending(y => y.IsActive)
                                    .ThenByDescending(y => y.LicenceKey != null)
                                    .ThenByDescending(y => y.FirstStart)
                                    .FirstOrDefault())
                                .Where(x => x != null);
                    }
                }
            }

            return qry;
        }

        public FrontAspNetClientLicence GetClientLicence(int id)
        {
            return GetClientLicences(new FilterAspNetClientLicences { ClientLicenceIds = new List<int> { id } }).FirstOrDefault();
        }

        public FrontAspNetClientLicence GetClientLicenceActive(int clientId)
        {
            var lic = GetClientLicences(new FilterAspNetClientLicences { ClientIds = new List<int> { clientId }, IsNowUsed = true }).FirstOrDefault();
            if (lic == null)
            {
                lic = new FrontAspNetClientLicence
                {
                    Id = 0,
                    ClientId = clientId,
                    FirstStart = DateTime.MinValue,
                    IsActive = false,
                    LicenceId = 0,
                    IsConcurenteLicence = false,
                    IsDateLimit = false,
                    IsFunctionals = false,
                    IsNamedLicence = false,
                    LicenceKey = null
                };
            }
            return lic;
        }

        public IEnumerable<FrontAspNetClientLicence> GetClientLicences(FilterAspNetClientLicences filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetClientLicencesQuery(dbContext, filter);

                var itemsRes = itemsDb;

                var items = itemsRes.Select(x => new FrontAspNetClientLicence
                {
                    Id = x.Id,

                    LicenceId = x.LicenceId,
                    ClientId = x.ClientId,
                    ClientName = x.Client.Name,

                    FirstStart = x.FirstStart,

                    IsDateLimit = x.Licence.DurationDay.HasValue,
                    DateLimit = x.Licence.DurationDay,

                    IsConcurenteLicence = x.Licence.ConcurenteNumberOfConnections.HasValue,
                    ConcurenteNumberOfConnections = x.Licence.ConcurenteNumberOfConnections,

                    IsNamedLicence = x.Licence.NamedNumberOfConnections.HasValue,
                    NamedNumberOfConnections = x.Licence.NamedNumberOfConnections,

                    IsFunctionals = x.Licence.Functionals != null,
                    Functionals = x.Licence.Functionals,

                    LicenceKey = x.LicenceKey,

                    IsActive = x.IsActive,
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public int AddClientLicence(IContext ctx, int licenceId)
        {
            return AddClientLicence(ctx.CurrentClientId, licenceId);
        }

        public int AddClientLicence(int clientId, int licenceId)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AspNetClientLicences
                {
                    ClientId = clientId,
                    FirstStart = DateTime.UtcNow.Date,
                    IsActive = false,
                    LicenceId = licenceId
                };
                dbContext.AspNetClientLicencesSet.Add(item);
                dbContext.SaveChanges();
                transaction.Complete();
                return item.Id;
            }
        }

        public int SetClientLicenceKey(IContext ctx, SetClientLicenceKey model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetClientLicences
                    {
                        Id = model.ClientLicenceId,
                        LicenceKey = model.LicenceKey,
                        IsActive = true,
                    };

                    dbContext.AspNetClientLicencesSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(p => p.LicenceKey).IsModified = true;
                    entry.Property(p => p.IsActive).IsModified = true;

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

        public void UpdateClientLicence(ModifyAspNetClientLicence model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetClientLicences
                    {
                        Id = model.Id,
                        IsActive = model.IsActive,
                    };
                    dbContext.AspNetClientLicencesSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(p => p.IsActive).IsModified = true;
                    transaction.Complete();
                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteClientLicence(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetClientLicences
                    {
                        Id = id
                    };
                    dbContext.AspNetClientLicencesSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        #endregion ClientLicences

        #region Clients

        private IQueryable<AspNetClients> GetClientsQuery(ApplicationDbContext dbContext, FilterAspNetClients filter)
        {
            var qry = dbContext.AspNetClientsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.ClientIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetClients>();
                    filterContains = filter.ClientIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Code))
                {
                    qry = qry.Where(x => filter.Code.Equals(x.Code));
                }

                if (!string.IsNullOrEmpty(filter.VerificationCode))
                {
                    qry = qry.Where(x => filter.VerificationCode.Equals(x.VerificationCode));
                }

            }

            return qry;
        }

        public FrontAspNetClient GetClient(int id)
        {
            return GetClients(new FilterAspNetClients { ClientIds = new List<int> { id } }).FirstOrDefault();
        }
        public FrontAspNetClient GetClient(string clientCode)
        {
            return GetClients(new FilterAspNetClients { Code = clientCode }).FirstOrDefault();
        }

        public int GetClientId(string clientCode)
        {
            var res = GetClients(new FilterAspNetClients { Code = clientCode }).FirstOrDefault();

            if (res == null) throw new ClientIsNotFound();

            return res.Id;
        }

        public string GetClientCode(int clientId)
        {
            var res = GetClients(new FilterAspNetClients { ClientIds = new List<int> { clientId } }).FirstOrDefault();

            if (res == null) throw new ClientIsNotFound();

            return res.Code;
        }

        public IEnumerable<FrontAspNetClient> GetClients(FilterAspNetClients filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetClientsQuery(dbContext, filter);

                var items = itemsDb.Select(x => new FrontAspNetClient
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public bool ExistsClients(FilterAspNetClients filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var res = GetClientsQuery(dbContext, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontAspNetClient> GetClientsByUser(IContext ctx)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var userClients = GetUserClientsQuery(dbContext, new FilterAspNetUserClients { UserIds = new List<string> { ctx.CurrentEmployee.UserId }, ClientCode = ctx.CurrentEmployee.ClientCode })
                                    .AsQueryable();

                var itemsRes = from userClient in userClients
                               join client in dbContext.AspNetClientsSet on userClient.ClientId equals client.Id
                               select new
                               {
                                   Client = client
                               };


                var items = itemsRes.Select(x => new FrontAspNetClient
                {
                    Id = x.Client.Id,
                    Name = x.Client.Name,
                    Code = x.Client.Code,
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public FrontAspNetClient GetClientByUser(string userId, int clientId = -1)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var filter = new FilterAspNetUserClients { UserIds = new List<string> { userId } };

                if (clientId > 0)
                {
                    filter.ClientIds = new List<int> { clientId };
                }

                var userClients = GetUserClientsQuery(dbContext, filter).AsQueryable();


                var itemsRes = from userClient in userClients
                               join client in dbContext.AspNetClientsSet on userClient.ClientId equals client.Id
                               select new
                               {
                                   Client = client,
                               };


                var item = itemsRes.Select(x => new FrontAspNetClient
                {
                    Id = x.Client.Id,
                    Name = x.Client.Name,
                    Code = x.Client.Code,
                }).FirstOrDefault();
                transaction.Complete();
                return item;
            }
        }

        public int AddClient(ModifyAspNetClient model)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var client = GetDbClient(model);

                dbContext.AspNetClientsSet.Add(client);
                dbContext.SaveChanges();

                model.Id = client.Id;
                transaction.Complete();
                return model.Id;
            }
        }

        private static AspNetClients GetDbClient(ModifyAspNetClient model)
        {
            return new AspNetClients
            {
                Id = model.Id,
                Name = model.Name,
                Code = model.Code,
            };
        }

        public void UpdateClient(ModifyAspNetClient model)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var item = GetDbClient(model);

                dbContext.AspNetClientsSet.Attach(item);

                var entry = dbContext.Entry(item);
                entry.Property(p => p.Name).IsModified = true;
                entry.Property(p => p.Code).IsModified = true;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteClient(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetClients
                    {
                        Id = id
                    };
                    dbContext.AspNetClientsSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        #endregion Clients

        #region ClientServers

        private IQueryable<AspNetClientServers> GetClientServersQuery(ApplicationDbContext dbContext, FilterAspNetClientServers filter)
        {
            var qry = dbContext.AspNetClientServersSet.AsQueryable();

            if (filter != null)
            {
                if (filter.ClientServerIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetClientServers>();
                    filterContains = filter.ClientServerIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ClientIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetClientServers>();
                    filterContains = filter.ClientIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ServerIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetClientServers>();
                    filterContains = filter.ServerIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ServerId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public FrontAspNetClientServer GetClientServer(int id)
        {
            return GetClientServers(new FilterAspNetClientServers { ClientServerIds = new List<int> { id } }).FirstOrDefault();
        }

        public int GetServerIdByClientId(int clientId)
        {
            var clientServer = GetClientServers(new FilterAspNetClientServers { ClientIds = new List<int> { clientId } }).FirstOrDefault();

            if (clientServer == null)
            {
                var clientCode = GetClientCode(clientId);
                throw new ClientServerIsNotSet(clientCode);
            }

            return clientServer.ServerId;
        }

        public IEnumerable<FrontAspNetClientServer> GetClientServers(FilterAspNetClientServers filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetClientServersQuery(dbContext, filter);

                var itemsRes = itemsDb;

                var items = itemsRes.Select(x => new FrontAspNetClientServer
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ServerId = x.ServerId,
                    ClientName = x.Client.Name,
                    ServerName = x.Server.Name
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public int AddClientServer(ModifyAspNetClientServer model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetClientServers
                    {
                        ClientId = model.ClientId,
                        ServerId = model.ServerId,
                    };
                    dbContext.AspNetClientServersSet.Add(item);
                    dbContext.SaveChanges();

                    model.Id = item.Id;
                    transaction.Complete();
                    return model.Id;
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteClientServer(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetClientServers
                    {
                        Id = id
                    };
                    dbContext.AspNetClientServersSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        #endregion ClientServers

        #region Licences

        private IQueryable<AspNetLicences> GetLicencesQuery(ApplicationDbContext dbContext, FilterAspNetLicences filter)
        {
            var qry = dbContext.AspNetLicencesSet.AsQueryable();

            if (filter != null)
            {
                if (filter.LicenceIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetLicences>();
                    filterContains = filter.LicenceIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public FrontAspNetLicence GetLicence(int id)
        {
            return GetLicences(new FilterAspNetLicences { LicenceIds = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAspNetLicence> GetLicences(FilterAspNetLicences filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetLicencesQuery(dbContext, filter);

                var itemsRes = itemsDb;

                var items = itemsRes.Select(x => new FrontAspNetLicence
                {
                    Id = x.Id,

                    Name = x.Name,
                    Description = x.Description,

                    IsDateLimit = x.DurationDay.HasValue,
                    DateLimit = x.DurationDay,

                    IsConcurenteLicence = x.ConcurenteNumberOfConnections.HasValue,
                    ConcurenteNumberOfConnections = x.ConcurenteNumberOfConnections,

                    IsNamedLicence = x.NamedNumberOfConnections.HasValue,
                    NamedNumberOfConnections = x.NamedNumberOfConnections,

                    IsFunctionals = x.Functionals != null,
                    Functionals = x.Functionals,
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public int AddLicence(ModifyAspNetLicence model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetLicences
                    {
                        Name = model.Name,
                        Description = model.Description,
                        NamedNumberOfConnections = model.NamedNumberOfConnections,
                        ConcurenteNumberOfConnections = model.ConcurenteNumberOfConnections,
                        DurationDay = model.DurationDay,
                        Functionals = model.Functionals,
                    };
                    dbContext.AspNetLicencesSet.Add(item);
                    dbContext.SaveChanges();

                    model.Id = item.Id;
                    transaction.Complete();
                    return model.Id;
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void UpdateLicence(ModifyAspNetLicence model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetLicences
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Description = model.Description,
                        NamedNumberOfConnections = model.NamedNumberOfConnections,
                        ConcurenteNumberOfConnections = model.ConcurenteNumberOfConnections,
                        DurationDay = model.DurationDay,
                        Functionals = model.Functionals,
                    };
                    dbContext.AspNetLicencesSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(p => p.Name).IsModified = true;
                    entry.Property(p => p.Description).IsModified = true;
                    entry.Property(p => p.NamedNumberOfConnections).IsModified = true;
                    entry.Property(p => p.ConcurenteNumberOfConnections).IsModified = true;
                    entry.Property(p => p.DurationDay).IsModified = true;
                    entry.Property(p => p.Functionals).IsModified = true;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteLicence(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetLicences
                    {
                        Id = id
                    };
                    dbContext.AspNetLicencesSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        #endregion Licences

        #region [+] UserClients ...

        private IQueryable<AspNetUserClients> GetUserClientsQuery(ApplicationDbContext dbContext, FilterAspNetUserClients filter)
        {
            var qry = dbContext.AspNetUserClientsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.UserClientIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserClients>();
                    filterContains = filter.UserClientIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ClientIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserClients>();
                    filterContains = filter.ClientIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.UserIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserClients>();
                    filterContains = filter.UserIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.UserId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.ClientCode))
                {
                    qry = qry.Where(x => filter.ClientCode.Equals(x.Client.Code));
                }
            }

            return qry;
        }

        public FrontAspNetUserClient GetUserClient(int id)
        {
            return GetUserClients(new FilterAspNetUserClients { UserClientIds = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAspNetUserClient> GetUserClients(FilterAspNetUserClients filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetUserClientsQuery(dbContext, filter);

                var itemsRes = itemsDb;

                var items = itemsRes.Select(x => new FrontAspNetUserClient
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    UserId = x.UserId,
                    ClientName = x.Client.Name,
                    UserName = x.User.UserName
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        private int AddUserClient(string userId, int clientId)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = new AspNetUserClients
                {
                    UserId = userId,
                    ClientId = clientId
                };
                dbContext.AspNetUserClientsSet.Add(dbModel);
                dbContext.SaveChanges();
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public int AddUserClient(ModifyAspNetUserClient model)
        {
            try
            {
                var si = new SystemInfo();

                LicenceInfo lic = GetClientLicenceActive(model.ClientId);

                if (lic == null)
                {
                    throw new LicenceError();
                }

                var regCode = si.GetRegCode(lic);

                if (lic.IsNamedLicence)
                {
                    lic.NamedNumberOfConnectionsNow++;
                }

                new Licences().Verify(regCode, lic, null, true);

                model.Id = AddUserClient(model.UserId, model.ClientId);

                return model.Id;
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteUserClient(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetUserClients
                    {
                        Id = id
                    };
                    dbContext.AspNetUserClientsSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteUserClients(FilterAspNetUserClients filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserClientsQuery(dbContext, filter);
                dbContext.AspNetUserClientsSet.RemoveRange(qry);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        #endregion UserClients

        #region UserServers

        private IQueryable<AspNetUserServers> GetUserServersQuery(ApplicationDbContext dbContext, FilterAspNetUserServers filter)
        {
            var qry = dbContext.AspNetUserServersSet.AsQueryable();

            if (filter != null)
            {
                if (filter.UserServerIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserServers>();
                    filterContains = filter.UserServerIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ClientIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserServers>();
                    filterContains = filter.ClientIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.UserIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserServers>();
                    filterContains = filter.UserIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.UserId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ServerIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserServers>();
                    filterContains = filter.ServerIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ServerId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public FrontAspNetUserServer GetUserServer(int id)
        {
            return GetUserServers(new FilterAspNetUserServers { UserServerIds = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAspNetUserServer> GetUserServers(FilterAspNetUserServers filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetUserServersQuery(dbContext, filter);

                var itemsRes = itemsDb;

                var items = itemsRes.Select(x => new FrontAspNetUserServer
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    UserId = x.UserId,
                    ServerId = x.ServerId,
                    ClientName = x.Client.Name,
                    UserName = x.User.UserName,
                    ServerName = x.Server.Name
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public int AddUserServer(ModifyAspNetUserServer model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetUserServers
                    {
                        ClientId = model.ClientId,
                        UserId = model.UserId,
                        ServerId = model.ServerId,
                    };
                    dbContext.AspNetUserServersSet.Add(item);
                    dbContext.SaveChanges();

                    model.Id = item.Id;
                    transaction.Complete();
                    return model.Id;
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteUserServer(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
                {
                    var item = new AspNetUserServers
                    {
                        Id = id
                    };
                    dbContext.AspNetUserServersSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteUserServers(FilterAspNetUserServers filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserServersQuery(dbContext, filter);
                dbContext.AspNetUserServersSet.RemoveRange(qry);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        #endregion UserServers

        #region UserFingerprint

        private IQueryable<AspNetUserFingerprints> GetUserFingerprintQuery(ApplicationDbContext dbContext, FilterAspNetUserFingerprint filter)
        {
            var qry = dbContext.AspNetUserFingerprintsSet.AsQueryable();

            if (filter != null)
            {

                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserFingerprints>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<AspNetUserFingerprints>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.UserIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserFingerprints>();
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
                    Fingerprint = x.Fingerprint.Substring(1, 8) + "...",
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

        public void DeleteUserFingerprint(int id)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AspNetUserFingerprints
                {
                    Id = id
                };
                dbContext.AspNetUserFingerprintsSet.Attach(item);

                dbContext.Entry(item).State = EntityState.Deleted;

                dbContext.SaveChanges();
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
                    var filterContains = PredicateBuilder.False<AspNetUserContexts>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<AspNetUserContexts>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.UserIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AspNetUserContexts>();
                    filterContains = filter.UserIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.UserId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TokenExact))
                {
                    qry = qry.Where(x => filter.TokenExact.Equals(x.Token, StringComparison.OrdinalIgnoreCase));
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

        public void DeleteUserContext(string  token)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AspNetUserContextsSet.Where(x=>x.Token == token).Delete();
                transaction.Complete();
            }
        }


        #endregion

        public IEnumerable<ListItem> GetControlQuestions()
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
                items.ForEach(x => x.Name = tmpService.GetTranslation(x.Name));

                return items;
            }
        }

    }
}