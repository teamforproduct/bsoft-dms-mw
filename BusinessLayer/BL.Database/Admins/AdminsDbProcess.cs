using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Database.DatabaseContext;
using BL.Model.AdminCore;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Users;

namespace BL.Database.Admins
{
    public class AdminsDbProcess : CoreDb.CoreDb, IAdminsDbProcess
    {
        public AdminsDbProcess()
        {
        }

        public AdminAccessInfo GetAdminAccesses(IContext context)
        {
            using (var dbContext = new DmsContext(context))
            {
                var res = new AdminAccessInfo();

                res.UserRoles = dbContext.AdminUserRolesSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalDictionaryAdminUserRoles
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    UserId = x.UserId
                }).ToList();

                res.Roles = dbContext.AdminRolesSet.Where(x => x.ClientId == context.CurrentClientId).Select(x => new InternalDictionaryAdminRoles
                {
                    Id = x.Id

                }).ToList();

                res.PositionRoles = dbContext.AdminPositionRolesSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalDictionaryAdminPositionRoles
                {
                    PositionId = x.PositionId,
                    Id = x.Id,
                    RoleId = x.RoleId
                }).ToList();

                res.Actions = dbContext.SystemActionsSet.Select(x => new InternalDictionarySystemActions
                {
                    Id = x.Id,
                    Code = x.Code,
                    GrantId = x.GrantId,
                    IsGrantable = x.IsGrantable,
                    IsGrantableByRecordId = x.IsGrantableByRecordId,
                    IsVisible = x.IsVisible,
                    Object = (EnumObjects)x.ObjectId
                }).ToList();

                res.ActionAccess = dbContext.AdminRoleActionsSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalDictionaryAdminRoleActions
                {
                    Id = x.Id,
                    RecordId = x.RecordId,
                    RoleId = x.RoleId,
                    ActionId = x.ActionId
                }).ToList();

                return res;
            }
        }

        public IEnumerable<FrontAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.AdminPositionRolesSet.Where(x => x.Role.ClientId == ctx.CurrentClientId).AsQueryable();

                if (filter.UserRoleId?.Count > 0)
                {
                    qry = qry.Where(x => x.Role.UserRoles.Any(y=> filter.UserRoleId.Contains(y.Id)));
                }
                if (filter.UserId?.Count > 0)
                {
                    qry = qry.Where(x => x.Role.UserRoles.Any(y => filter.UserId.Contains(y.UserId)));
                }
                if (filter.RoleId?.Count > 0)
                {
                    qry = qry.Where(x => filter.RoleId.Contains(x.RoleId));
                }

                var res = qry.Select(x => new FrontAdminUserRole
                {
                    RolePositionId = x.PositionId,
                    RolePositionName = x.Position.Name,
                    RolePositionExecutorAgentName = x.Position.ExecutorAgent.Name
                }).Distinct().ToList();

                var roleList = res.Select(s => s.RolePositionId).ToList();

                var newevnt = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                    .Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId
                                             && roleList.Contains(x.TargetPositionId.Value))
                                        .GroupBy(g => g.TargetPositionId)
                                        .Select(s => new { PosID = s.Key, EvnCnt = s.Count() }).ToList();

                res.Join(newevnt, r => r.RolePositionId, e => e.PosID, (r, e) => { r.NewEventsCount = e.EvnCnt; return r; });

                //TODO
                //foreach (var rn in res.Join(newevnt, r => r.RolePositionId, e => e.PosID, (r, e) => new { rs = r, ne = e }))
                //{
                //    rn.rs.NewEventsCount = rn.ne.EvnCnt;
                //}

                return res;
            }
        }

        /// <summary>
        /// Проверка соблюдения субординации
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool VerifySubordination(IContext context, VerifySubordination model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
                var pos = dictDb.GetPositions(context, new FilterDictionaryPosition() { IDs = new List<int> { model.TargetPosition }, SubordinatedPositions = model.SourcePositions })
                    .Select(x => new { MaxSubordinationTypeId = x.MaxSubordinationTypeId })
                    .FirstOrDefault();
                if (pos == null || pos.MaxSubordinationTypeId < (int)model.SubordinationType)
                {
                    return false;
                }
                return true;
            }
        }

        public Employee GetEmployee(IContext ctx, string userId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return dbContext.DictionaryAgentUsersSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Where(x => x.UserId.Equals(userId))
                    .Select(x => new Employee
                    {
                        AgentId = x.Id,
                        Name = x.Agent.Name,
                        LanguageId = x.Agent.LanguageId ?? 0
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return null;
        }
    }
}
