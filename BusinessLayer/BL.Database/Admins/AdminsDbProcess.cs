using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Database.DatabaseContext;
using BL.Model.AdminCore;

namespace BL.Database.Admins
{
    public class AdminsDbProcess : CoreDb.CoreDb, IAdminsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public AdminsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        #region AdminAccessLevels
        public BaseAdminAccessLevel GetAdminAccessLevel(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {

                return dbContext.AdminAccessLevelsSet.Where(x => x.Id == id).Select(x => new BaseAdminAccessLevel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext ctx, FilterAdminAccessLevel filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
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
        }
        #endregion AdminAccessLevels
    }
}
