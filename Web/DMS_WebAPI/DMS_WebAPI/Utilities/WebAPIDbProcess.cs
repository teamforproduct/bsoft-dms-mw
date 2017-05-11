using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.AdminCore.Clients;
using BL.Model.Common;
using BL.Model.Context;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore.InternalModel;
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
    internal class WebAPIDbProcess
    {
        public WebAPIDbProcess()
        {

        }

        //        private TransactionScope GetTransaction() => new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });

        #region [+] Settings ...

        public int MergeSetting(InternalGeneralSetting model)
        {
            var res = 0;
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var cset = dbContext.SystemSettingsSet.FirstOrDefault(x => x.Key == model.Key);
                if (cset == null)
                {
                    var nsett = new SystemSettings
                    {
                        Key = model.Key,
                        Value = model.Value,
                        ValueTypeId = (int)model.ValueType,
                        Name = model.Name,
                        Description = model.Description,
                        Order = model.Order,
                    };
                    dbContext.SystemSettingsSet.Add(nsett);
                    dbContext.SaveChanges();
                    res = nsett.Id;
                }
                else
                {
                    cset.Value = model.Value;

                    if (model.ValueType > 0)
                    {
                        cset.ValueTypeId = (int)model.ValueType;
                    }

                    dbContext.SaveChanges();
                    res = cset.Id;
                }
                transaction.Complete();
                return res;
            }
        }

        public string GetSettingValue(string key)
        {
            var res = string.Empty;
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                res = dbContext.SystemSettingsSet.Where(x => x.Key == key)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalGeneralSetting> GetSystemSettingsInternal()
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemSettingsSet.AsQueryable();

                qry = qry.OrderBy(x => x.Order);

                var res = qry.Select(x => new InternalGeneralSetting
                {
                    Key = x.Key,
                    Name = x.Name,
                    Value = x.Value,
                    ValueType = (EnumValueTypes)x.ValueTypeId,
                    Description = x.Description,
                    Order = x.Order
                }).ToList();

                transaction.Complete();

                return res;
            }
        }


        #endregion

        #region Servers

        private IQueryable<AdminServers> GetServersQuery(ApplicationDbContext dbContext, FilterAdminServers filter)
        {
            var qry = dbContext.AdminServersSet.AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.ClientCode))
                {
                    qry = qry.Where(x => x.ClientServers.AsQueryable().Any(y => y.Client.Code == filter.ClientCode));
                }

                if (filter.ClientIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetClientServers>(false);
                    filterContains = filter.ClientIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(x => x.ClientServers.AsQueryable().Any(filterContains));
                }

                if (filter.ServerIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminServers>(false);
                    filterContains = filter.ServerIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ServerTypes?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminServers>(false);
                    filterContains = filter.ServerTypes.Select(x => x.ToString()).ToList().Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ServerType == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.ServerNameExact))
                {
                    qry = qry.Where(x => x.Name == filter.ServerNameExact);
                }
            }

            return qry;
        }

        public FrontAdminServer GetServer(int id)
        {
            return GetServers(new FilterAdminServers { ServerIDs = new List<int> { id } }).FirstOrDefault();
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

        public IEnumerable<DatabaseModelForAdminContext> GetServersByAdminContext(FilterAdminServers filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var servers = GetServersQuery(dbContext, filter);

                // перемножаю серверы на клиентов
                var itemsRes = (from server in servers
                                join clientServer in dbContext.AspNetClientServersSet on server.Id equals clientServer.ServerId
                                select new
                                {
                                    Server = server,
                                    ClientId = clientServer.ClientId,
                                    ClientCode = clientServer.Client.Code
                                }).ToList();

                var items = itemsRes.Select(x => new DatabaseModelForAdminContext
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
                    ClientId = x.ClientId,
                    ClientCode = x.ClientCode
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public DatabaseModelForAdminContext GetClientServer(string clientCode)
        {
            var clientId = GetClientId(clientCode);
            return GetClientServer(clientId);
        }

        public DatabaseModelForAdminContext GetClientServer(int clientId)
        {
            // тут подразумевается, что клиентские данные могут рассполагатся только на одном из серверов   
            var serv = GetServers(new FilterAdminServers { ClientIDs = new List<int> { clientId } }).FirstOrDefault();
            if (serv == null) throw new ServerIsNotFound();
            //!!!!!!!!!!!!!!!!!!!!!!
            var res = new DatabaseModelForAdminContext(serv);
            res.ClientId = clientId;
            res.ClientCode = GetClientCode(clientId);
            return res;
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
            var db = new DatabaseModelForAdminContext
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
                ClientId = model.ClientId,
                ClientCode = GetClientCode(model.ClientId),
            };
            var ctx = new AdminContext(db);
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
                    var filterContains = PredicateBuilder.New<AspNetClientLicences>(false);
                    filterContains = filter.ClientLicenceIds.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ClientIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetClientLicences>(false);
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
            return AddClientLicence(ctx.Client.Id, licenceId);
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

        public void UpdateClientLicence(ModifyAspNetClientLicence model)
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

        public void DeleteClientLicence(FilterAspNetClientLicences filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetClientLicencesQuery(dbContext, filter);
                qry.Delete();

                transaction.Complete();
            }
        }

        #endregion ClientLicences

        #region ClientRequests

        private IQueryable<AspNetClientRequests> GetClientRequestsQuery(ApplicationDbContext dbContext, FilterAspNetClientRequests filter)
        {
            var qry = dbContext.AspNetClientRequestsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetClientRequests>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.CodeExact))
                {
                    qry = qry.Where(x => filter.CodeExact.Equals(x.ClientCode));
                }

                if (!string.IsNullOrEmpty(filter.HashCodeExact))
                {
                    qry = qry.Where(x => filter.HashCodeExact.Equals(x.HashCode));
                }

                if (!string.IsNullOrEmpty(filter.SMSCodeExact))
                {
                    qry = qry.Where(x => filter.SMSCodeExact.Equals(x.SMSCode));
                }

                if (filter.DateCreateLess.HasValue)
                {
                    qry = qry.Where(x => x.CreateDate < filter.DateCreateLess.Value);
                }

            }

            return qry;
        }

        public FrontAspNetClientRequest GetClientRequest(int id)
        {
            return GetClientRequests(new FilterAspNetClientRequests { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAspNetClientRequest> GetClientRequests(FilterAspNetClientRequests filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var itemsDb = GetClientRequestsQuery(dbContext, filter);

                var items = itemsDb.Select(x => new FrontAspNetClientRequest
                {
                    Id = x.Id,
                    ClientCode = x.ClientCode,
                    ClientName = x.ClientName,
                    Language = x.Language,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    PhoneNumber = x.PhoneNumber,
                    HashCode = x.HashCode,
                    SMSCode = x.SMSCode,
                }).ToList();
                transaction.Complete();
                return items;
            }
        }

        public bool ExistsClientRequests(FilterAspNetClientRequests filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var res = GetClientRequestsQuery(dbContext, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        public int AddClientRequest(AddClientSaaS model)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var client = GetDbClientRequest(model);

                dbContext.AspNetClientRequestsSet.Add(client);
                dbContext.SaveChanges();

                transaction.Complete();
                return client.Id;
            }
        }

        public void DeleteClientRequest(FilterAspNetClientRequests filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetClientRequestsQuery(dbContext, filter);
                qry.Delete();

                transaction.Complete();
            }
        }

        #endregion

        #region Clients

        private IQueryable<AspNetClients> GetClientsQuery(ApplicationDbContext dbContext, FilterAspNetClients filter)
        {
            var qry = dbContext.AspNetClientsSet.AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetClients>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
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
            return GetClients(new FilterAspNetClients { IDs = new List<int> { id } }).FirstOrDefault();
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
            var res = GetClients(new FilterAspNetClients { IDs = new List<int> { clientId } }).FirstOrDefault();

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

        //public IEnumerable<FrontAspNetClient> GetClientsByUser(IContext ctx)
        //{
        //    using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
        //    {
        //        var userClients = GetUserClientsQuery(dbContext, new FilterAspNetUserClients { UserIds = new List<string> { ctx.CurrentEmployee.UserId }, ClientCode = ctx.CurrentEmployee.ClientCode })
        //                            .AsQueryable();

        //        var itemsRes = from userClient in userClients
        //                       join client in dbContext.AspNetClientsSet on userClient.ClientId equals client.Id
        //                       select new
        //                       {
        //                           Client = client
        //                       };


        //        var items = itemsRes.Select(x => new FrontAspNetClient
        //        {
        //            Id = x.Client.Id,
        //            Name = x.Client.Name,
        //            Code = x.Client.Code,
        //        }).ToList();
        //        transaction.Complete();
        //        return items;
        //    }
        //}

        //public FrontAspNetClient GetClientByUser(string userId, int clientId = -1)
        //{
        //    using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
        //    {
        //        var filter = new FilterAspNetUserClients { UserIds = new List<string> { userId } };

        //        if (clientId > 0)
        //        {
        //            filter.ClientIds = new List<int> { clientId };
        //        }

        //        var userClients = GetUserClientsQuery(dbContext, filter).AsQueryable();


        //        var itemsRes = from userClient in userClients
        //                       join client in dbContext.AspNetClientsSet on userClient.ClientId equals client.Id
        //                       select new
        //                       {
        //                           Client = client,
        //                       };


        //        var item = itemsRes.Select(x => new FrontAspNetClient
        //        {
        //            Id = x.Client.Id,
        //            Name = x.Client.Name,
        //            Code = x.Client.Code,
        //        }).FirstOrDefault();
        //        transaction.Complete();
        //        return item;
        //    }
        //}

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

        private static AspNetClientRequests GetDbClientRequest(AddClientSaaS model)
        {
            return new AspNetClientRequests
            {
                ClientCode = model.ClientCode,
                ClientName = model.ClientName,
                Language = model.Language,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                HashCode = model.HashCode,
                SMSCode = model.SMSCode,
                CreateDate = DateTime.UtcNow,
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

        #endregion Clients

        #region Licences

        private IQueryable<AspNetLicences> GetLicencesQuery(ApplicationDbContext dbContext, FilterAspNetLicences filter)
        {
            var qry = dbContext.AspNetLicencesSet.AsQueryable();

            if (filter != null)
            {
                if (filter.LicenceIds?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetLicences>(false);
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

        #region ClientServers

        public IEnumerable<FrontAspNetClientServer> GetClientServerList(FilterAspNetClientServer filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetClientServerQuery(dbContext, filter);

                var res = qry.Select(x => new FrontAspNetClientServer
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ServerId = x.ServerId,
                    ClientName = x.Client.Name,
                    ServerName = x.Server.Name,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        private IQueryable<AspNetClientServers> GetClientServerQuery(ApplicationDbContext dbContext, FilterAspNetClientServer filter)
        {
            var qry = dbContext.AspNetClientServersSet.AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetClientServers>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ClientIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetClientServers>(false);
                    filterContains = filter.ClientIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ServerIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetClientServers>(false);
                    filterContains = filter.ServerIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ServerId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public int AddClientServer(SetClientServer model)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AspNetClientServers
                {
                    ServerId = model.ServerId,
                    ClientId = model.ClientId,
                };
                dbContext.AspNetClientServersSet.Add(item);
                dbContext.SaveChanges();

                transaction.Complete();
                return item.Id;
            }
        }

        public void DeleteClientServer(FilterAspNetClientServer filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetClientServerQuery(dbContext, filter);
                dbContext.AspNetClientServersSet.RemoveRange(qry);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        #endregion

        #region UserClientServers

        public IEnumerable<FrontAspNetUserClientServer> GetUserClientServerList(FilterAspNetUserClientServer filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserClientServerQuery(dbContext, filter);

                var res = qry.Select(x => new FrontAspNetUserClientServer
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    UserId = x.UserId,
                    ServerId = x.ServerId,
                    ClientName = x.Client.Name,
                    UserName = x.User.UserName,
                    ServerName = x.Server.Name,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        private IQueryable<AspNetUserClientServer> GetUserClientServerQuery(ApplicationDbContext dbContext, FilterAspNetUserClientServer filter)
        {
            var qry = dbContext.AspNetUserClientServerSet.AsQueryable();

            if (filter != null)
            {
                if (filter.UserServerIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserClientServer>(false);
                    filterContains = filter.UserServerIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ClientIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserClientServer>(false);
                    filterContains = filter.ClientIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ClientId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.UserIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserClientServer>(false);
                    filterContains = filter.UserIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.UserId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ServerIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AspNetUserClientServer>(false);
                    filterContains = filter.ServerIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ServerId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public int AddUserClientServer(SetUserClientServer model)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var item = new AspNetUserClientServer
                {
                    UserId = model.UserId,
                    ServerId = model.ServerId,
                    ClientId = model.ClientId,
                };
                dbContext.AspNetUserClientServerSet.Add(item);
                dbContext.SaveChanges();

                transaction.Complete();
                return item.Id;
            }
        }

        public void DeleteUserClientServer(FilterAspNetUserClientServer filter)
        {
            using (var dbContext = new ApplicationDbContext()) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserClientServerQuery(dbContext, filter);
                dbContext.AspNetUserClientServerSet.RemoveRange(qry);
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