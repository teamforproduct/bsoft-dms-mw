using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Database.DatabaseContext;
using BL.Model.AdminCore;
using BL.Logic.DependencyInjection;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Users;

namespace BL.Database.Admins
{
    public class AdminsDbProcess : CoreDb.CoreDb, IAdminsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public AdminsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public AdminAccessInfo GetAdminAccesses(IContext context)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var res = new AdminAccessInfo();

                res.UserRoles = dbContext.AdminUserRolesSet.Select(x => new InternalDictionaryAdminUserRoles
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    UserId = x.UserId
                }).ToList();

                res.Roles = dbContext.AdminUserRolesSet.Select(x => new InternalDictionaryAdminRoles
                {
                    Id = x.Role.Id,
                    AccessLevel = (EnumDocumentAccesses)x.Role.AccessLevelId,
                    PositionId = x.Role.PositionId
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
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
                return qry.Distinct().Select(x => new BaseAdminUserRole
                {
                    RolePositionId = x.Role.Position.Id,
                    RolePositionName = x.Role.Position.Name,
                    RolePositionExecutorAgentName = x.Role.Position.ExecutorAgent.Name
                }).Distinct().ToList();
            }
        }

        ///// <summary>
        ///// Проверка прав на действие
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="model"></param>
        ///// <param name="isThrowExeception"></param>
        ///// <returns></returns>
        //public bool VerifyAccess(IContext context, VerifyAccess model, bool isThrowExeception = true)
        //{
        //    using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
        //    {
        //        var res = false;
        //        if (model.UserId == 0)
        //        {
        //            model.UserId = context.CurrentAgentId;
        //        }
        //        if (model.PositionsIdList == null || model.PositionsIdList.Count == 0)
        //        {
        //            model.PositionsIdList = context.CurrentPositionsIdList;
        //        }
        //        if (!string.IsNullOrEmpty(model.DocumentActionCode))
        //        {
        //            if (model.IsPositionFromContext)
        //            {
        //                model.PositionId = context.CurrentPositionId;
        //            }
        //            res = dbContext.AdminRoleActionsSet
        //                      .Any(x => x.Action.Code == model.DocumentActionCode
        //                                && x.Action.IsGrantable //TODO как отработать не грантебл
        //                                && (!x.Action.IsGrantableByRecordId || (x.RecordId == model.RecordId))
        //                                && (((model.PositionId == null) && (model.PositionsIdList.Contains(x.Role.PositionId))) || (x.Role.PositionId == model.PositionId))
        //                                && x.Role.UserRoles.Any(y => y.UserId == model.UserId)
        //                            );
        //        }
        //        else
        //        {
        //            var noAcc = model.PositionsIdList.Except(dbContext.AdminUserRolesSet.Where(x => (x.UserId == model.UserId)).Select(x => x.Role.PositionId).ToList()).ToList();
        //            res = !noAcc.Any();
        //        }
        //        if (!res && isThrowExeception)
        //        {
        //            throw new AccessIsDenied(); //TODO Сергей!!!Как красиво передать string obj, string act, int? id = null в сообщение?
        //        }
        //        return res;
        //    }
        //}

        /// <summary>
        /// Проверка соблюдения субординации
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool VerifySubordination(IContext context, VerifySubordination model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
                var pos = dictDb.GetDictionaryPositions(context, new FilterDictionaryPosition() { PositionId = new List<int> { model.TargetPosition }, SubordinatedPositions = model.SourcePositions }).FirstOrDefault();
                if (pos == null || pos.MaxSubordinationTypeId < (int)model.SubordinationType)
                {
                    return false;
                }
                return true;
            }
        }

        public Employee GetEmployee(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new Employee
                {
                    AgentId = x.Id,
                    Name = x.Name
                }).FirstOrDefault();
            }
        }

        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return null;
        }

    }
}
