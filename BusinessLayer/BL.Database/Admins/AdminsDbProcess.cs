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

                res.UserRoles = dbContext.AdminUserRolesSet.Select(x => new InternalDictionaryAdminUserRoles
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    UserId = x.UserId
                }).ToList();

                res.Roles = dbContext.AdminRolesSet.Select(x => new InternalDictionaryAdminRoles
                {
                    Id = x.Id

                }).ToList();

                res.PositionRoles = dbContext.AdminPositionRolesSet.Select(x => new InternalDictionaryAdminPositionRoles
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

                res.ActionAccess = dbContext.AdminRoleActionsSet.Select(x => new InternalDictionaryAdminRoleActions
                {
                    Id = x.Id,
                    RecordId = x.RecordId,
                    RoleId = x.RoleId,
                    ActionId = x.ActionId
                }).ToList();

                return res;
            }
        }

        public IEnumerable<BaseAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();

                if (filter.UserRoleId?.Count > 0)
                {
                    qry = qry.Where(x => filter.UserRoleId.Contains(x.Id));
                }
                if (filter.UserId?.Count > 0)
                {
                    qry = qry.Where(x => filter.UserId.Contains(x.UserId));
                }
                if (filter.RoleId?.Count > 0)
                {
                    qry = qry.Where(x => filter.UserId.Contains(x.RoleId));
                }
                return qry.Distinct().SelectMany(x => x.Role.PositionRoles).Select(x => new BaseAdminUserRole
                {
                    RolePositionId = x.PositionId,
                    RolePositionName = x.Position.Name,
                    RolePositionExecutorAgentName = x.Position.ExecutorAgent.Name
                }).Distinct().ToList();
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
                var pos = dictDb.GetDictionaryPositions(context, new FilterDictionaryPosition() { IDs = new List<int> { model.TargetPosition }, SubordinatedPositions = model.SourcePositions }).FirstOrDefault();
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
                return dbContext.DictionaryAgentUsersSet.Where(x => x.UserId.Equals(userId)).Select(x => new Employee
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
