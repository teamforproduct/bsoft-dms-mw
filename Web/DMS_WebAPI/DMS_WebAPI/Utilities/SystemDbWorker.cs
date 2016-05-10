using System;
using System.Linq;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using DMS_WebAPI.Models;

namespace DMS_WebAPI.Utilities
{
    public class SystemDbWorker
    {
        public LicenceInfo GetLicenceInfoByServerId(int serverId)
        {
            int? clientId = null;
            using (var dbContext = new ApplicationDbContext())
            {
                clientId = dbContext.AdminServersSet
                            .Where(x => x.Id == serverId)
                            .Select(x => x.ClientId)
                            .FirstOrDefault();
            }
            if (!clientId.HasValue)
            {
                throw new LicenceError();
            }

            return GetLicenceInfo(clientId.Value);
        }

        public LicenceInfo GetLicenceInfo(int clientId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var cl = dbContext.AspNetClientsSet
                            .Where(x => x.Id == clientId)
                            .Select(x => new
                            {
                                Name = x.Name,
                                FirstStart = x.FirstStart,
                                IsTrial = x.IsTrial,

                                IsDateLimit = x.DurationDay.HasValue,
                                DateLimit = x.DurationDay,

                                IsConcurenteLicence = x.ConcurenteNumberOfConnections.HasValue,
                                ConcurenteNumberOfConnections = x.ConcurenteNumberOfConnections,

                                IsNamedLicence = x.NamedNumberOfConnections.HasValue,
                                NamedNumberOfConnections = x.NamedNumberOfConnections,
                                NamedNumberOfConnectionsNow = x.NamedNumberOfConnections.HasValue
                                    ? dbContext.AdminServersSet.Where(y => y.ClientId == clientId)
                                        .SelectMany(y => y.UserServers).Count()
                                    : 0,

                                IsFunctionals = x.Functionals != null,
                                Functionals = x.Functionals,

                                ClientId = x.Id,
                                LicenceKey = x.LicenceKey,
                            }).FirstOrDefault();

                if (cl != null)
                {
                    var client = new DBModel.AspNetClients { Id = clientId, FirstStart = cl.FirstStart };
                    if (!client.FirstStart.HasValue)
                    {
                        client.FirstStart = DateTime.Now;
                        dbContext.AspNetClientsSet.Attach(client);
                        var entry = dbContext.Entry(client);
                        entry.Property(x => x.FirstStart).IsModified = true;
                        dbContext.SaveChanges();
                    }
                    var li = new LicenceInfo
                    {
                        ClientId = cl.ClientId,
                        Name = cl.Name,

                        FirstStart = client.FirstStart.Value,
                        IsTrial = cl.IsTrial,

                        IsDateLimit = cl.IsDateLimit,
                        DateLimit = cl.DateLimit,

                        IsConcurenteLicence = cl.IsConcurenteLicence,
                        ConcurenteNumberOfConnections = cl.ConcurenteNumberOfConnections,

                        IsNamedLicence = cl.IsNamedLicence,
                        NamedNumberOfConnections = cl.NamedNumberOfConnections,
                        NamedNumberOfConnectionsNow = cl.NamedNumberOfConnectionsNow,

                        IsFunctionals = cl.IsFunctionals,
                        Functionals = cl.Functionals,

                        LicenceKey = cl.LicenceKey,
                    };
                    return li;
                }
                throw new LicenceError();
            }
        }

        public void SaveLicenceInfo(ModifyLicenceInfo lic)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var client = new DBModel.AspNetClients
                {
                    Id = lic.ClientId,
                    FirstStart = lic.FirstStart,
                    IsTrial = lic.IsTrial,
                    NamedNumberOfConnections = lic.NamedNumberOfConnections,
                    ConcurenteNumberOfConnections = lic.ConcurenteNumberOfConnections,
                    DurationDay = lic.DurationDay,
                    Functionals = lic.Functionals
                };

                dbContext.AspNetClientsSet.Attach(client);
                var entry = dbContext.Entry(client);
                entry.Property(x => x.FirstStart).IsModified = true;
                entry.Property(x => x.IsTrial).IsModified = true;
                entry.Property(x => x.NamedNumberOfConnections).IsModified = true;
                entry.Property(x => x.ConcurenteNumberOfConnections).IsModified = true;
                entry.Property(x => x.DurationDay).IsModified = true;
                entry.Property(x => x.Functionals).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public void SaveLicenceKey(int clientId, string licenceKey)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var cl = dbContext.AspNetClientsSet.FirstOrDefault(x => x.Id == clientId);
                if (cl != null)
                {
                    cl.LicenceKey = licenceKey;
                    dbContext.SaveChanges();

                    return;
                }
                throw new LicenceError();
            }
        }
    }
}