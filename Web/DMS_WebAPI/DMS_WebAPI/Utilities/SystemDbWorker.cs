using System;
using System.Linq;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using DMS_WebAPI.Models;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

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

        public LicenceInfo GetLicenceInfo(int clientId, bool isRereadLicense = true)
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

                    if (isRereadLicense && string.IsNullOrEmpty(li.LicenceKey))
                    {
                        SetLicence(li.Name);

                        return GetLicenceInfo(clientId, false);
                    }
                    return li;
                }
                throw new LicenceError();
            }
        }

        public void SaveLicenceInfoByName(ModifyLicenceInfo lic)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var client = dbContext.AspNetClientsSet.FirstOrDefault(x => x.Name.Equals(lic.Name));

                client.IsTrial = lic.IsTrial;
                client.NamedNumberOfConnections = lic.NamedNumberOfConnections;
                client.ConcurenteNumberOfConnections = lic.ConcurenteNumberOfConnections;
                client.DurationDay = lic.DurationDay;
                client.Functionals = lic.Functionals;
                client.LicenceKey = lic.LicenceKey;
                dbContext.SaveChanges();
            }
        }

        public void SetLicence(string clientName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/lic.ini")))
                {
                    // Read the stream to a string, and write the string to the console.
                    string lines = sr.ReadToEnd();
                    var lics = JsonConvert.DeserializeObject<List<LicenceInfo>>(lines);

                    var lic = lics.FirstOrDefault(x => x.Name.Equals(clientName));

                    if (lic == null)
                        throw new LicenceError();

                    SaveLicenceInfoByName(new ModifyLicenceInfo
                    {
                        Name = lic.Name,
                        DurationDay = lic.DateLimit,
                        NamedNumberOfConnections = lic.NamedNumberOfConnections,
                        ConcurenteNumberOfConnections = lic.ConcurenteNumberOfConnections,
                        Functionals = lic.Functionals,
                        LicenceKey = lic.LicenceKey
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}