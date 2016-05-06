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
        public LicenceInfo GetLicenceInfo(int clientId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var cl = dbContext.AspNetClientsSet.FirstOrDefault(x => x.Id == clientId);
                if (cl != null)
                {
                    if (cl.FirstStart == null)
                    {
                        cl.FirstStart = DateTime.Now.Date;
                        dbContext.SaveChanges();
                    }
                    var li = new LicenceInfo
                    {
                        Name = cl.Name,
                        FirstStart = cl.FirstStart.Value,
                        NumberOfConnections = cl.NumberOfConnections,
                        ClientId = cl.Id,
                        DateLimit = cl.DurationDay,
                        LicType = (EnumLicenceTypes)Enum.Parse(typeof(EnumLicenceTypes), cl.LicenceType, true),
                        LicenseKey = cl.LicenceKey,
                    };
                    return li;
                }
                throw new LicenceError();
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