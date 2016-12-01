using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.AdminCore.Clients;
using BL.Model.AdminCore.WebUser;
using BL.Model.Database;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.Users;
using BL.Model.WebAPI.Filters;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Models;
using LinqKit;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    internal class WebAPIDbProcess
    {
        public WebAPIDbProcess()
        {

        }

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
            using (var dbContext = new ApplicationDbContext())
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

                return items;
            }
        }

        public IEnumerable<DatabaseModel> GetServersByAdmin(FilterAdminServers filter)
        {
            using (var dbContext = new ApplicationDbContext())
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

                return items;
            }
        }

        public IEnumerable<FrontAdminServerByUser> GetServersByUser(IContext ctx)
        {
            using (var dbContext = new ApplicationDbContext())
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

                return items;
            }
        }

        public DatabaseModel GetServerByUser(string userId, SetUserServer setUserServer)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var filterClients = new FilterAspNetUserClients { UserIds = new List<string> { userId } };
                if (setUserServer.ClientId > 0) filterClients.ClientIds = new List<int> { setUserServer.ClientId };

                var filterServers = new FilterAspNetUserServers { UserIds = new List<string> { userId } };
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

                return item;
            }
        }

        public int AddServer(ModifyAdminServer model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
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
            var sysProc = DmsResolver.Current.Get<ISystemService>();
            sysProc.InitializerDatabase(ctx);
        }

        public void UpdateServer(ModifyAdminServer model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
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
            using (var dbContext = new ApplicationDbContext())
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

                return items;
            }
        }

        public int AddClientLicence(IContext ctx, int licenceId)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetClientLicences
                    {
                        ClientId = ctx.CurrentClientId,
                        FirstStart = DateTime.UtcNow.Date,
                        IsActive = false,
                        LicenceId = licenceId
                    };
                    dbContext.AspNetClientLicencesSet.Add(item);
                    dbContext.SaveChanges();

                    return item.Id;
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public int SetClientLicenceKey(IContext ctx, SetClientLicenceKey model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
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
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetClientLicences
                    {
                        Id = model.Id,
                        IsActive = model.IsActive,
                    };
                    dbContext.AspNetClientLicencesSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(p => p.IsActive).IsModified = true;

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
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetClientLicences
                    {
                        Id = id
                    };
                    dbContext.AspNetClientLicencesSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
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

        public IEnumerable<FrontAspNetClient> GetClients(FilterAspNetClients filter)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var itemsDb = GetClientsQuery(dbContext, filter);

                var itemsRes = itemsDb;

                var items = itemsRes.Select(x => new FrontAspNetClient
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                }).ToList();

                return items;
            }
        }

        public IEnumerable<FrontAspNetClient> GetClientsByUser(IContext ctx)
        {
            using (var dbContext = new ApplicationDbContext())
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

                return items;
            }
        }

        public FrontAspNetClient GetClientByUser(string userId, int clientId = -1)
        {
            using (var dbContext = new ApplicationDbContext())
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

                return item;
            }
        }

        public string AddClient(AddClientContent model)
        {
            try
            {
                var owinContext = HttpContext.Current.Request.GetOwinContext();
                var userManager = owinContext.GetUserManager<ApplicationUserManager>();

                // определяю сервер для клиента
                // сервер может определяться более сложным образом: с учетом нагрузки, количества клиентов
                var server = GetServers(new FilterAdminServers()).FirstOrDefault();

                int serverId = server.Id;
                //if (model.Server.Id <= 0)
                //{
                //    var server = new AdminServers
                //    {
                //        Name = model.Server.Name,
                //        Address = model.Server.Address,
                //        ConnectionString = model.Server.ConnectionString,
                //        DefaultDatabase = model.Server.DefaultDatabase,
                //        DefaultSchema = model.Server.DefaultSchema,
                //        IntegrateSecurity = model.Server.IntegrateSecurity,
                //        ServerType = model.Server.ServerType.ToString(),
                //        UserName = model.Server.UserName,
                //        UserPassword = model.Server.UserPassword,
                //    };

                //    dbContext.AdminServersSet.Add(server);
                //    dbContext.SaveChanges();

                //    model.Server.Id = server.Id;
                //}


                using (var dbContext = new ApplicationDbContext())
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));

                    using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        #region Create client 1 

                        // Проверка уникальности доменного имени
                        if (dbContext.AspNetClientsSet.Any(x => x.Code.Equals(model.Domain)))
                        {
                            transaction.Dispose();
                            throw new ClientCodeAlreadyExists(model.Domain);
                        }

                        var client = new AspNetClients
                        {
                            Name = model.Domain,
                            Code = model.Domain,
                        };

                        dbContext.AspNetClientsSet.Add(client);
                        dbContext.SaveChanges();

                        model.ClientId = client.Id;

                        //pss какую лицензию здесь подставлять???
                        // PSS выяснил, что отсутствие лицензии приравнивается к дефолтной
                        //AspNetClientLicences clientLicence;
                        //if (model.LicenceId > 0)
                        //{
                        //    clientLicence = new AspNetClientLicences
                        //    {
                        //        ClientId = client.Id,
                        //        FirstStart = DateTime.UtcNow,
                        //        IsActive = false,
                        //        LicenceId = model.LicenceId.GetValueOrDefault(),
                        //        LicenceKey = null,
                        //    };

                        //    dbContext.AspNetClientLicencesSet.Add(clientLicence);
                        //    dbContext.SaveChanges();
                        //}

                        #endregion Create client 1

                        #region Create DB

                        var clientServer = new AspNetClientServers
                        {
                            ClientId = client.Id,
                            ServerId = serverId,
                        };

                        dbContext.AspNetClientServersSet.Add(clientServer);

                        #endregion Create DB

                        #region Create user                        

                        //if (!string.IsNullOrEmpty(model.Client.Code))
                        //{
                        //    email = $"Client_{client.Id}_{email}";
                        //}

                        // pss Здесь нужно дописать генерацию пароля и отправку письма с паролем и рекламой
                        var userId = AddUser(new AddWebUser
                        {
                            Email = model.Email,
                            Password = "P@ssw0rd",
                            ClientId = client.Id,
                            ServerId = serverId,
                        });

                        #endregion Create user

                        #region add user to role admin

                        // PSS 
                        var role = $"Admin_Client_{client.Id}";

                        var roleDb = roleManager.FindByName(role);

                        if (roleDb == null || string.IsNullOrEmpty(roleDb.Id))
                        {
                            roleManager.Create(new IdentityRole { Name = role });
                        }

                        userManager.AddToRole(userId, role);

                        #endregion add user to role admin

                        var ctx = new AdminContext(server);

                        transaction.Complete();

                        return "token";
                    }
                }
            }
            //catch (UserNameAlreadyExists)
            //{
            //    throw new UserNameAlreadyExists( model );
            //}
            //catch (ClientNameAlreadyExists)
            //{
            //    throw new ClientNameAlreadyExists();
            //}
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }

        }

        public int AddUserEmployee(AddDictionaryAgentEmployee model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            string userId = string.Empty;
            string userName = $"Client_{ctx.CurrentClientId}_{model.Login}";

            // Проверяю не используется ли логин
            if (ExistsUser(userName)) throw new UserNameAlreadyExists(model.Login);

            // пробую создать сотрудника
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddAgentEmployee, ctx, model);

            if (tmpItem > 0)
            {
                userId = AddUser(new BL.Model.AdminCore.WebUser.AddWebUser
                {
                    Email = model.Login,
                    // Здесь (или после подтверждения адреса) нужна генерация пароля и отправка на почту
                    Password = "P@ssw0rd",
                    // Предполагаю, что человек, который создает пользователей. создает их в тойже базе и в том же клиенте
                    ClientId = ctx.CurrentClientId,
                    ServerId = ctx.CurrentDB.Id,

                });
            }

            // обновляю сотрудника 
            tmpService.SetAgentUserUserId(ctx, new InternalDictionaryAgentUser
            {
                Id = tmpItem,
                UserId = userId
            });

            return tmpItem;

        }

        public bool ExistsUser(string userName)
        {
            var owinContext = HttpContext.Current.Request.GetOwinContext();
            var userManager = owinContext.GetUserManager<ApplicationUserManager>();
            return userManager.FindByName(userName) != null;
        }

        public string AddUser(AddWebUser model)
        {
            var owinContext = HttpContext.Current.Request.GetOwinContext();
            var userManager = owinContext.GetUserManager<ApplicationUserManager>();

            using (var dbContext = new ApplicationDbContext())
            //using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {

                if (ExistsUser(model.Email)) throw new UserNameAlreadyExists(model.Email);

                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

                IdentityResult result = userManager.Create(user, model.Password);

                if (!result.Succeeded)
                {
                    throw new DictionaryRecordCouldNotBeAdded();
                }

                var userClient = new AspNetUserClients
                {
                    UserId = user.Id,
                    ClientId = model.ClientId
                };

                dbContext.AspNetUserClientsSet.Add(userClient);
                dbContext.SaveChanges();

                var userServer = new AspNetUserServers
                {
                    ClientId = model.ClientId,
                    ServerId = model.ServerId,
                    UserId = user.Id,
                };

                dbContext.AspNetUserServersSet.Add(userServer);
                dbContext.SaveChanges();

                transaction.Complete();

                return user.Id;
            }
        }

        public int AddClient(AddAspNetClient model)
        {
            try
            {

                //TODO в transaction не может подлючиться к базе
                if (model.Server.Id <= 0)
                {
                    InitializerDatabase(model.Server);
                }


                using (var dbContext = new ApplicationDbContext())
                {
                    var owinContext = HttpContext.Current.Request.GetOwinContext();
                    var userManager = owinContext.GetUserManager<ApplicationUserManager>();
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));

                    using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        #region Create client 1 

                        if (dbContext.AspNetClientsSet.Any(x => x.Name.Equals(model.Client.Name)))
                        {
                            throw new ClientNameAlreadyExists();
                        }

                        var client = new AspNetClients
                        {
                            Name = model.Client.Name,
                            Code = model.Client.Code,
                        };

                        dbContext.AspNetClientsSet.Add(client);
                        dbContext.SaveChanges();

                        AspNetClientLicences clientLicence;
                        if (model.LicenceId > 0)
                        {
                            clientLicence = new AspNetClientLicences
                            {
                                ClientId = client.Id,
                                FirstStart = DateTime.UtcNow,
                                IsActive = false,
                                LicenceId = model.LicenceId.GetValueOrDefault(),
                                LicenceKey = null,
                            };

                            dbContext.AspNetClientLicencesSet.Add(clientLicence);
                            dbContext.SaveChanges();
                        }

                        #endregion Create client 1

                        #region Create DB

                        if (model.Server.Id <= 0)
                        {
                            var server = new AdminServers
                            {
                                Name = model.Server.Name,
                                Address = model.Server.Address,
                                ConnectionString = model.Server.ConnectionString,
                                DefaultDatabase = model.Server.DefaultDatabase,
                                DefaultSchema = model.Server.DefaultSchema,
                                IntegrateSecurity = model.Server.IntegrateSecurity,
                                ServerType = model.Server.ServerType.ToString(),
                                UserName = model.Server.UserName,
                                UserPassword = model.Server.UserPassword,
                            };

                            dbContext.AdminServersSet.Add(server);
                            dbContext.SaveChanges();

                            model.Server.Id = server.Id;
                        }

                        var clientServer = new AspNetClientServers
                        {
                            ClientId = client.Id,
                            ServerId = model.Server.Id,
                        };

                        dbContext.AspNetClientServersSet.Add(clientServer);

                        #endregion Create DB

                        #region Create user                        

                        var email = model.Admin.Email;

                        if (!string.IsNullOrEmpty(model.Client.Code))
                        {
                            email = $"Client_{client.Id}_{email}";
                        }

                        var userId = AddUser(new AddWebUser()
                        {
                            Email = email,
                            Password = model.Admin.Password,
                            ClientId = client.Id,
                            ServerId = model.Server.Id,
                        });

                        #endregion Create user

                        #region add user to role admin

                        var role = $"Admin_Client_{client.Id}";

                        var roleDb = roleManager.FindByName(role);

                        if (roleDb == null || string.IsNullOrEmpty(roleDb.Id))
                        {
                            roleManager.Create(new IdentityRole { Name = role });
                        }

                        userManager.AddToRole(userId, role);

                        #endregion add user to role admin

                        transaction.Complete();

                        return client.Id;
                    }
                }
            }
            //catch (UserNameAlreadyExists)
            //{
            //    throw new UserNameAlreadyExists();
            //}
            //catch (ClientNameAlreadyExists)
            //{
            //    throw new ClientNameAlreadyExists();
            //}
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        //public void AddUsersTemp(IEnumerable<InternalDictionaryContact> models)
        //{
        //    try
        //    {
        //        var owinContext = HttpContext.Current.Request.GetOwinContext();
        //        var userManager = owinContext.GetUserManager<ApplicationUserManager>();

        //        using (var dbContext = new ApplicationDbContext())
        //        {
        //            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));

        //            foreach (var item in models)
        //            {

        //                if (userManager.FindByName(item.Value) != null) continue;

        //                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //                {

        //                    #region Create user                        

        //                    var user = new ApplicationUser() { UserName = item.Value, Email = item.Value };

        //                    IdentityResult result = userManager.Create(user, "P@ssw0rd");

        //                    if (!result.Succeeded)
        //                    {
        //                        transaction.Dispose();
        //                        throw new DictionaryRecordCouldNotBeAdded();
        //                    }

        //                    var userClient = new AspNetUserClients
        //                    {
        //                        UserId = user.Id,
        //                        ClientId = 1,
        //                    };

        //                    dbContext.AspNetUserClientsSet.Add(userClient);
        //                    dbContext.SaveChanges();

        //                    var userServer = new AspNetUserServers
        //                    {
        //                        ClientId = 1,
        //                        ServerId = 1,
        //                        UserId = user.Id,
        //                    };

        //                    dbContext.AspNetUserServersSet.Add(userServer);
        //                    dbContext.SaveChanges();

        //                    #endregion Create user

        //                    transaction.Complete();
        //                }

        //            }
        //        }
        //    }
        //    catch (UserNameAlreadyExists)
        //    {
        //        throw new UserNameAlreadyExists();
        //    }
        //    catch (ClientNameAlreadyExists)
        //    {
        //        throw new ClientNameAlreadyExists();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DictionaryRecordCouldNotBeAdded();
        //    }
        //}

        public int AddFirstAdminClient(AddFirstAdminClient model)
        {
            try
            {
                var owinContext = HttpContext.Current.Request.GetOwinContext();
                var userManager = owinContext.GetUserManager<ApplicationUserManager>();


                using (var dbContext = new ApplicationDbContext())
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));

                    using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        #region Verification client code 

                        var client = dbContext.AspNetClientsSet.FirstOrDefault(x => x.Code == model.ClientCode && x.VerificationCode == model.VerificationCode);
                        if (client == null)
                        {
                            throw new ClientVerificationCodeIncorrect();
                        }

                        #endregion Verification client code 

                        #region Create user                        

                        var email = model.Admin.Email;

                        if (!string.IsNullOrEmpty(model.ClientCode))
                        {
                            email = $"Client_{client.Id}_{email}";
                        }

                        if (userManager.FindByName(email) != null)
                        {
                            throw new UserNameAlreadyExists(model.Admin.Email);
                        }

                        var user = new ApplicationUser() { UserName = email, Email = email };

                        IdentityResult result = userManager.Create(user, model.Admin.Password);

                        if (!result.Succeeded)
                        {
                            transaction.Dispose();
                            throw new DictionaryRecordCouldNotBeAdded();
                        }

                        var userClient = new AspNetUserClients
                        {
                            UserId = user.Id,
                            ClientId = client.Id
                        };

                        dbContext.AspNetUserClientsSet.Add(userClient);
                        dbContext.SaveChanges();

                        var serverIds = dbContext.AspNetClientServersSet.Where(x => x.ClientId == client.Id).Select(x => x.ServerId).ToList();

                        foreach (var serverId in serverIds)
                        {
                            var userServer = new AspNetUserServers
                            {
                                ClientId = client.Id,
                                ServerId = serverId,
                                UserId = user.Id,
                            };
                            dbContext.AspNetUserServersSet.Add(userServer);
                        }
                        dbContext.SaveChanges();

                        #endregion Create user

                        #region add user to role admin

                        var role = $"Admin_Client_{client.Id}";

                        var roleDb = roleManager.FindByName(role);

                        if (roleDb == null || string.IsNullOrEmpty(roleDb.Id))
                        {
                            roleManager.Create(new IdentityRole { Name = role });
                        }

                        userManager.AddToRole(user.Id, role);

                        #endregion add user to role admin

                        transaction.Complete();

                        return client.Id;
                    }
                }
            }
            catch (UserNameAlreadyExists)
            {
                throw new UserNameAlreadyExists(model.Admin.Email);
            }
            catch (ClientNameAlreadyExists)
            {
                throw new ClientNameAlreadyExists();
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void UpdateClient(ModifyAspNetClient model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetClients
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Code = model.Code,
                    };

                    dbContext.AspNetClientsSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(p => p.Name).IsModified = true;
                    entry.Property(p => p.Code).IsModified = true;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        public void DeleteClient(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetClients
                    {
                        Id = id
                    };
                    dbContext.AspNetClientsSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
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

        public IEnumerable<FrontAspNetClientServer> GetClientServers(FilterAspNetClientServers filter)
        {
            using (var dbContext = new ApplicationDbContext())
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

                return items;
            }
        }

        public int AddClientServer(ModifyAspNetClientServer model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetClientServers
                    {
                        ClientId = model.ClientId,
                        ServerId = model.ServerId,
                    };
                    dbContext.AspNetClientServersSet.Add(item);
                    dbContext.SaveChanges();

                    model.Id = item.Id;

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
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetClientServers
                    {
                        Id = id
                    };
                    dbContext.AspNetClientServersSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
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
            using (var dbContext = new ApplicationDbContext())
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

                return items;
            }
        }

        public int AddLicence(ModifyAspNetLicence model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
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
                using (var dbContext = new ApplicationDbContext())
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
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetLicences
                    {
                        Id = id
                    };
                    dbContext.AspNetLicencesSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        #endregion Licences

        #region UserClients

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
            using (var dbContext = new ApplicationDbContext())
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

                return items;
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

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    using (var dbContext = new ApplicationDbContext())
                    {
                        var item = new AspNetUserClients
                        {
                            ClientId = model.ClientId,
                            UserId = model.UserId,
                        };
                        dbContext.AspNetUserClientsSet.Add(item);
                        dbContext.SaveChanges();

                        model.Id = item.Id;

                        transaction.Complete();

                        return model.Id;
                    }
                }
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
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetUserClients
                    {
                        Id = id
                    };
                    dbContext.AspNetUserClientsSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
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
            using (var dbContext = new ApplicationDbContext())
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

                return items;
            }
        }

        public int AddUserServer(ModifyAspNetUserServer model)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
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
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new AspNetUserServers
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

        #endregion UserServers

        #region UserAgent
        public async Task RestorePasswordAgentUserAsync(RestorePasswordAgentUser model, string baseUrl, NameValueCollection query, string emailSubject, string renderPartialView)
        {
            if (query == null)
                query = new NameValueCollection();

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindByNameAsync(model.Email.UserNameFormatByClientCode(model.ClientCode));

            if (user == null)
                throw new UserNameIsNotDefined();

            if (user.IsLockout)
                throw new UserIsDeactivated(user.UserName);

            var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user.Id);



            var builder = new UriBuilder(baseUrl);
            var newQuery = HttpUtility.ParseQueryString(builder.Query);
            newQuery.Add("UserId", user.Id);
            newQuery.Add("Code", passwordResetToken);
            newQuery.Add(query);

            builder.Query = newQuery.ToString();// string.Join("&", nvc.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
            string callbackurl = builder.ToString();

            var htmlContent = callbackurl.RenderPartialViewToString(renderPartialView);

            var settings = DmsResolver.Current.Get<ISettings>();

            var client = GetClient(model.ClientCode);

            var db = GetServersByAdmin(new FilterAdminServers { ClientIds = new List<int> { client.Id } }).First();

            var ctx = new AdminContext(db);

            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            mailService.SendMessage(ctx, model.Email, emailSubject, htmlContent);
        }

        public void RestorePasswordAgentUser(RestorePasswordAgentUser model, string baseUrl, NameValueCollection query, string emailSubject, string renderPartialView)
        {
            if (query == null)
                query = new NameValueCollection();

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            ApplicationUser user = userManager.FindByEmail(model.Email.UserNameFormatByClientCode(model.ClientCode));

            if (user == null)
                throw new UserNameIsNotDefined();

            if (user.IsLockout)
                throw new UserIsDeactivated(user.UserName);

            var passwordResetToken = userManager.GeneratePasswordResetToken(user.Id);

            query.Add("UserId", user.Id);
            query.Add("Code", passwordResetToken);

            var builder = new UriBuilder(baseUrl);
            builder.Query = query.ToString();
            string callbackurl = builder.ToString();

            var htmlContent = callbackurl.RenderPartialViewToString(renderPartialView);

            var settings = DmsResolver.Current.Get<ISettings>();

            var client = GetClient(model.ClientCode);

            var db = GetServersByAdmin(new FilterAdminServers { ClientIds = new List<int> { client.Id } }).First();

            var ctx = new AdminContext(db);

            var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            mailService.SendMessage(ctx, model.Email, emailSubject, htmlContent);
        }
        #endregion
    }
}