using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;

namespace BL.Database.Admins
{
    public class AdminsDbProcess : CoreDb.CoreDb, IAdminsDbProcess
    {
        #region AdminAccessLevels
        public BaseAdminAccessLevel GetAdminAccessLevel(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.AdminAccessLevelsSet.Where(x => x.Id == id).Select(x => new BaseAdminAccessLevel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
            }).FirstOrDefault();
        }

        public IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.AdminAccessLevelsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseAdminAccessLevel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
            }).ToList();
        }
        #endregion AdminAccessLevels
    }
}
