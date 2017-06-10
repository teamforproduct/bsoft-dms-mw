using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore.Clients;
using BL.Model.Exception;
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
                LanguageId = model.LanguageId,
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

    }
}