using System;
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

        public IEnumerable<BaseAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();

                if (filter.Id?.Count > 0)
                {
                    qry = qry.Where(x => filter.Id.Contains(x.Id));
                }
                if (filter.UserId?.Count > 0)
                {
                    qry = qry.Where(x => filter.UserId.Contains(x.UserId));
                }
                if (filter.RoleId?.Count > 0)
                {
                    qry = qry.Where(x => filter.UserId.Contains(x.RoleId));
                }
                return qry.Distinct().Select(x => new BaseAdminUserRole
                {
                    RolePositionId = x.Role.Position.Id,
                    RolePositionName = x.Role.Position.Name,
                    RolePositionExecutorAgentName = x.Role.Position.ExecutorAgent.Name
                }).Distinct().ToList();
            }
        }

        public bool VerifyAccess(IContext context, VerifyAccess acc)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.AdminRoleActionsSet
                          .Where(x => x.Action.Code == acc.ActionCode
                                    && x.Action.Object.Code == acc.ObjectCode
                                    && x.Action.IsGrantable
                                    && (!x.Action.IsGrantableByRecordId || (x.RecordId == acc.RecordId))
                                    && x.Role.UserRoles.Any(y => y.UserId == acc.UserId)
                                ).AsQueryable();

                return dbContext.AdminRoleActionsSet
                          .Any(x => x.Action.Code == acc.ActionCode
                                    && x.Action.Object.Code == acc.ObjectCode
                                    && x.Action.IsGrantable
                                    && (!x.Action.IsGrantableByRecordId || (x.RecordId == acc.RecordId))
                                    && x.Role.UserRoles.Any(y => y.UserId == acc.UserId)
                                );
            }
        }
    }
}
