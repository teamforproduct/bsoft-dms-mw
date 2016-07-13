using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Database.DatabaseContext;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Users;
using LinqKit;
using BL.Database.DBModel.Admin;
using BL.Model.AdminCore.InternalModel;
using BL.Database.Common;

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

                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminUserRoles>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Role.UserRoles.AsQueryable().Any(filterContains));
                }
                if (filter.UserIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminUserRoles>();
                    filterContains = filter.UserIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.UserId == value).Expand());

                    qry = qry.Where(x => x.Role.UserRoles.AsQueryable().Any(filterContains));
                }
                if (filter.RoleIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminPositionRoles>();
                    filterContains = filter.RoleIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RoleId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                var res = qry.Select(x => new FrontAdminUserRole
                {
                    RolePositionId = x.PositionId,
                    RolePositionName = x.Position.Name,
                    RolePositionExecutorAgentName = x.Position.ExecutorAgent.Name
                }).Distinct().ToList();

                var roleList = res.Select(s => s.RolePositionId).ToList();

                var filterNewEventTargetPositionContains = PredicateBuilder.False<DBModel.Document.DocumentEvents>();
                filterNewEventTargetPositionContains = roleList.Aggregate(filterNewEventTargetPositionContains,
                    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                var neweventQry = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                .Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId)
                                .Where(filterNewEventTargetPositionContains)
                                .GroupBy(g => g.TargetPositionId)
                                .Select(s => new { PosID = s.Key, EvnCnt = s.Count() });
                var newevnt = neweventQry.ToList();

                res.Join(newevnt, r => r.RolePositionId, e => e.PosID, (r, e) => { r.NewEventsCount = e.EvnCnt; return r; }).ToList();

                //TODO
                //foreach (var rn in res.Join(newevnt, r => r.RolePositionId, e => e.PosID, (r, e) => new { rs = r, ne = e }))
                //{
                //    rn.rs.NewEventsCount = rn.ne.EvnCnt;
                //}

                return res;
            }
        }
        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return null;
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

        #region [+] PositionRole ...
        public int AddPositionRole(IContext context, InternalAdminPositionRole model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminPositionRoles dbModel = AdminModelConverter.GetDbPositionRole(model);
                dbContext.AdminPositionRolesSet.Add(dbModel);
                // pss CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }
        public void UpdatePositionRole(IContext context, InternalAdminPositionRole model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminPositionRoles dbModel = AdminModelConverter.GetDbPositionRole(model);
                dbContext.AdminPositionRolesSet.Attach(dbModel);
                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Update);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        public void DeletePositionRole(IContext context, InternalAdminPositionRole model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = dbContext.AdminPositionRolesSet.FirstOrDefault(x => x.Id == model.Id);
                if (dbModel != null)
                {
                    dbContext.AdminPositionRolesSet.Remove(dbModel);
                    //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalAdminPositionRole GetInternalPositionRole(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminPositionRolesSet.AsQueryable();

                qry = GetWherePositionRole(ref qry, filter);

                return qry.Select(x => new InternalAdminPositionRole
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    RoleId = x.RoleId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
            }
        }



        public IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminPositionRolesSet.AsQueryable();

                qry = GetWherePositionRole(ref qry, filter);

                //qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    PositionId = x.Position.Id,
                    PositionName = x.Position.Name,
                    RoleId = x.Role.Id,
                    RoleName = x.Role.Name
                }).ToList();
            }
        }

        public bool ExistsPositionRole(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminPositionRolesSet.AsQueryable();

                qry = GetWherePositionRole(ref qry, filter);

                var res = qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        private static IQueryable<AdminPositionRoles> GetWherePositionRole(ref IQueryable<AdminPositionRoles> qry, FilterAdminPositionRole filter)
        {
            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminPositionRoles>();

                filterContains = filter.IDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminPositionRoles>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.PositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminPositionRoles>();

                filterContains = filter.PositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.RoleIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminPositionRoles>();

                filterContains = filter.RoleIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;
        }

        #endregion

    }
}
