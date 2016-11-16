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
using System;
using BL.Database.DBModel.Dictionary;
using BL.Model.AdminCore.Actions;
using BL.Model.Common;
using System.Transactions;
using BL.Model.Tree;
using BL.Model.DictionaryCore.FrontModel;
using BL.Database.DBModel.System;
using BL.Model.DictionaryCore.IncomingModel;
using EntityFramework.Extensions;
using BL.Model.SystemCore.InternalModel;

namespace BL.Database.Admins
{
    public class AdminsDbProcess : CoreDb.CoreDb, IAdminsDbProcess
    {

        private List<InternalDepartmentAdmin> DepartmentAdminsSet = new List<InternalDepartmentAdmin>();

        public AdminsDbProcess()
        {
        }

        #region [+] General ...
        public AdminAccessInfo GetAdminAccesses(IContext context)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var res = new AdminAccessInfo();

                res.UserRoles = dbContext.AdminUserRolesSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalAdminUserRole
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    UserId = x.UserId ?? x.PositionExecutor.AgentId
                }).ToList();

                res.Roles = dbContext.AdminRolesSet.Where(x => x.ClientId == context.CurrentClientId).Select(x => new InternalAdminRole
                {
                    Id = x.Id

                }).ToList();

                res.PositionRoles = dbContext.AdminPositionRolesSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalAdminPositionRole
                {
                    PositionId = x.PositionId,
                    Id = x.Id,
                    RoleId = x.RoleId
                }).ToList();

                res.Actions = dbContext.SystemActionsSet.Select(x => new InternalSystemAction
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    GrantId = x.GrantId,
                    IsGrantable = x.IsGrantable,
                    IsGrantableByRecordId = x.IsGrantableByRecordId,
                    IsVisible = x.IsVisible,
                    IsVisibleInMenu = x.IsVisibleInMenu,
                    ObjectId = (EnumObjects)x.ObjectId,
                }).ToList();

                res.ActionAccess = dbContext.AdminRoleActionsSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalAdminRoleAction
                {
                    Id = x.Id,
                    RecordId = x.RecordId,
                    RoleId = x.RoleId,
                    ActionId = x.ActionId
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontAvailablePositions> GetAvailablePositions(IContext ctx , int agentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.DictionaryPositionExecutorsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).AsQueryable();

                var now = DateTime.UtcNow;
                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

                qry = qry.Where(x => x.AgentId == agentId & x.IsActive == true & now >= x.StartDate & now <=x.EndDate);

                qry = qry.OrderBy(x => x.PositionExecutorTypeId).ThenBy(x => x.Position.Order);

                var res = qry.Select(x => new FrontAvailablePositions
                {
                    RolePositionId = x.PositionId,
                    RolePositionName = x.Position.Name,
                    RolePositionExecutorAgentName = x.Position.ExecutorAgent.Name ?? "##l@Message:PositionIsVacant@l##",
                    RolePositionExecutorTypeName = x.PositionExecutorType.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate > maxDateTime ? (DateTime?)null : x.EndDate,
                }).ToList();

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
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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
                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return null;
        }

        public Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var dateNow = DateTime.UtcNow;
                var qry = dbContext.DictionaryPositionExecutorsSet
                    .Where(x => dateNow >= x.StartDate && dateNow <= x.EndDate && x.AgentId == context.CurrentAgentId);
                var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(filterContains);
                var res = qry.GroupBy(x => x.PositionId)
                        .Select(x => new { x.Key, maxAccessLevel = x.Max(y => y.AccessLevelId) })
                        .ToDictionary(x => x.Key, z => z.maxAccessLevel);
                transaction.Complete();
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
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
                var pos = dictDb.GetPositions(context, new FilterDictionaryPosition() { IDs = new List<int> { model.TargetPosition }, SubordinatedPositions = model.SourcePositions })
                    .Select(x => new { MaxSubordinationTypeId = x.MaxSubordinationTypeId })
                    .FirstOrDefault();
                if (pos?.MaxSubordinationTypeId == null || pos.MaxSubordinationTypeId < (int)model.SubordinationType)
                {
                    return false;
                }
                transaction.Complete();
                return true;
            }
        }

        public Employee GetUserForContext(IContext ctx, string userId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var now = DateTime.UtcNow;

                // для авторизации 
                var res = dbContext.DictionaryAgentUsersSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Where(x => x.UserId.Equals(userId))
                    .Select(x => new Employee
                    {
                        AgentId = x.Id,
                        Name = x.Agent.Name,
                        LanguageId = x.Agent.AgentUser.LanguageId ?? -1,
                        IsActive = x.IsActive & x.Agent.AgentEmployee.IsActive,
                        PositionExecutorsCount = x.Agent.AgentEmployee.PositionExecutors.Where(y => y.AgentId == x.Id & y.IsActive == true & now >= y.StartDate & now <= y.EndDate).Count(),
                    }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        #endregion


        #region [+] Roles ...

        public int AddRoleType(IContext context, InternalAdminRoleType model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                AdminRoleTypes dbModel = AdminModelConverter.GetDbRoleType(context, model);
                dbContext.AdminRolesTypesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public int AddNamedRole(IContext context, string code, string name, IEnumerable<InternalAdminRoleAction> roleActions)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                // Классификатор роли
                var roleType = AddRoleType(context, new InternalAdminRoleType() { Code = code, Name = name });

                // Новая роль со ссылкой на классификатор ролей.
                var roleId = AddRole(context, new InternalAdminRole() { RoleTypeId = roleType, Name = name });

                var ra = new List<AdminRoleActions>();

                // Указание ид роли для предложенных действий
                foreach (var item in roleActions)
                {
                    ra.Add(new AdminRoleActions() { ActionId = item.ActionId, RoleId = roleId });
                }

                // Запись списка соответствий роль-действие
                dbContext.AdminRoleActionsSet.AddRange(ra);
                dbContext.SaveChanges();
                transaction.Complete();
                return roleId;
            }
        }

        public int AddRole(IContext context, InternalAdminRole model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                AdminRoles dbModel = AdminModelConverter.GetDbRole(context, model);
                dbContext.AdminRolesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }
        public void UpdateRole(IContext context, InternalAdminRole model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                AdminRoles dbModel = AdminModelConverter.GetDbRole(context, model);
                dbContext.AdminRolesSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public void DeleteRole(IContext context, InternalAdminRole model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                dbContext.AdminRoleActionsSet.Where(x => x.RoleId == model.Id).Delete(); ;
 
                var dbModel = dbContext.AdminRolesSet.FirstOrDefault(x => x.Id == model.Id);
                dbContext.AdminRolesSet.Remove(dbModel);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalAdminRole GetInternalRole(IContext context, FilterAdminRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRolesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereRole(ref qry, filter);

                var res = qry.Select(x => new InternalAdminRole
                {
                    Id = x.Id,
                    Name = x.Name,
                    RoleTypeId = x.RoleTypeId,
                    Description = x.Description,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontAdminRole> GetRoles(IContext context, FilterAdminRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRolesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();
                qry = GetWhereRole(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontAdminRole
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    
                    //RoleCode = x.RoleType.Code,
                    //RoleName = x.RoleType.Name
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public bool ExistsRole(IContext context, FilterAdminRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRolesSet.AsQueryable();

                qry = GetWhereRole(ref qry, filter);

                var res = qry.Select(x => new FrontAdminRole
                {
                    Id = x.Id
                }).FirstOrDefault();
                transaction.Complete();
                return res != null;
            }
        }

        private static IQueryable<AdminRoles> GetWhereRole(ref IQueryable<AdminRoles> qry, FilterAdminRole filter)
        {

            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRoles>();

                filterContains = filter.IDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.True<AdminRoles>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.And(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }


            // Список классификаторов
            if (filter.RoleTypeIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRoles>();

                filterContains = filter.RoleTypeIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.RoleTypeId == value).Expand());

                qry = qry.Where(filterContains);
            }

            // Поиск по наименованию
            if (!string.IsNullOrEmpty(filter.Name))
            {
                var filterContains = PredicateBuilder.False<AdminRoles>();

                filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Name == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (!string.IsNullOrEmpty(filter.NameExact))
            {
                qry = qry.Where(x => x.Name == filter.NameExact);
            }

            if (!string.IsNullOrEmpty(filter.Description))
            {
                var filterContains = PredicateBuilder.False<AdminRoles>();

                filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Description).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Description == value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;
        }
        #endregion

        #region [+] RoleAction ...
        public int AddRoleAction(IContext context, InternalAdminRoleAction model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                AdminRoleActions dbModel = AdminModelConverter.GetDbRoleAction(context, model);
                dbContext.AdminRoleActionsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void AddRoleActions(IContext context, IEnumerable<InternalAdminRoleAction> models)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModels = AdminModelConverter.GetDbRoleActions(context, models);
                dbContext.AdminRoleActionsSet.AddRange(dbModels);
                dbContext.SaveChanges();
            }
        }

        public void UpdateRoleAction(IContext context, InternalAdminRoleAction model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                AdminRoleActions dbModel = AdminModelConverter.GetDbRoleAction(context, model);
                dbContext.AdminRoleActionsSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public void DeleteRoleAction(IContext context, InternalAdminRoleAction model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var dbModel = dbContext.AdminRoleActionsSet.FirstOrDefault(x => x.Id == model.Id);
                dbContext.AdminRoleActionsSet.Remove(dbModel);
                dbContext.SaveChanges();
            }
        }

        public void DeleteRoleActions(IContext context, FilterAdminRoleAction filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRoleActionsSet.AsQueryable();
                qry = GetWhereRoleAction(ref qry, filter);
                qry.Delete();
                //dbContext.AdminRoleActionsSet.Remove(dbModel);
                //dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalAdminRoleAction GetInternalRoleAction(IContext context, FilterAdminRoleAction filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRoleActionsSet.AsQueryable();
                qry = GetWhereRoleAction(ref qry, filter);
                var res = qry.Select(x => new InternalAdminRoleAction
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    ActionId = x.ActionId,
                    RecordId = x.RecordId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontAdminRoleAction> GetRoleActions(IContext context, FilterAdminRoleAction filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRoleActionsSet.AsQueryable();

                qry = GetWhereRoleAction(ref qry, filter);

                //qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontAdminRoleAction
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    RoleName = x.Role.Name,
                    ActionId = x.ActionId,
                    ActionDescription = x.Action.Description,
                    RecordId = x.RecordId,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public bool ExistsRoleAction(IContext context, FilterAdminRoleAction filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRoleActionsSet.AsQueryable();

                qry = GetWhereRoleAction(ref qry, filter);

                var res = qry.Select(x => new FrontAdminRoleAction
                {
                    Id = x.Id
                }).FirstOrDefault();
                transaction.Complete();
                return res != null;
            }
        }

        private static IQueryable<AdminRoleActions> GetWhereRoleAction(ref IQueryable<AdminRoleActions> qry, FilterAdminRoleAction filter)
        {
            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRoleActions>();

                filterContains = filter.IDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.True<AdminRoleActions>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.And(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.RoleIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRoleActions>();

                filterContains = filter.RoleIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.RoleId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.ActionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRoleActions>();

                filterContains = filter.ActionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.ActionId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.RecordIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRoleActions>();

                filterContains = filter.RecordIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.RecordId == value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;
        }

        public List<int> GetActionsByRoles(IContext context, FilterAdminRoleAction filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRoleActionsSet.AsQueryable();

                qry = GetWhereRoleAction(ref qry, filter);

                var res = qry.Select(x => x.ActionId).ToList();
                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+] PositionRole ...
        public int AddPositionRole(IContext context, InternalAdminPositionRole model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminPositionRoles dbModel = AdminModelConverter.GetDbPositionRole(context, model);
                dbContext.AdminPositionRolesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }
        public void UpdatePositionRole(IContext context, InternalAdminPositionRole model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminPositionRoles dbModel = AdminModelConverter.GetDbPositionRole(context, model);
                dbContext.AdminPositionRolesSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeletePositionRole(IContext context, InternalAdminPositionRole model)
        {
            using (var dbContext = new DmsContext(context))
            {
                // по полям RoleId и PositionId соблюдается уникальность, поэтому запись идентифицируется правильно (всегда одна)
                var dbModel = dbContext.AdminPositionRolesSet.Where(x => x.RoleId == model.RoleId).Where(x => x.PositionId == model.PositionId).FirstOrDefault();
                dbContext.AdminPositionRolesSet.Remove(dbModel);
                dbContext.SaveChanges();
            }
        }

        public void DeletePositionRoles(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminPositionRolesSet.AsQueryable();
                qry = GetWherePositionRole(ref qry, filter);
                dbContext.AdminPositionRolesSet.RemoveRange(qry);
                dbContext.SaveChanges();
            }
        }

        public InternalAdminPositionRole GetInternalPositionRole(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminPositionRolesSet.AsQueryable();

                qry = GetWherePositionRole(ref qry, filter);

                var res = qry.Select(x => new InternalAdminPositionRole
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    RoleId = x.RoleId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public List<int> GetRolesByPositions(IContext context, List<int> positionIDs)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminPositionRolesSet.AsQueryable();

                var filterContains = PredicateBuilder.False<AdminPositionRoles>();

                filterContains = positionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(filterContains);
                var res = qry.Select(x => x.RoleId).ToList();
                transaction.Complete();
                return res;

            }

        }

        public FrontAdminPositionRole GetPositionRole(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRolesSet.Where(x => x.Id == id).Where(x => x.ClientId == context.CurrentClientId).AsQueryable();


                return qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    RoleName = x.Name,
                }).FirstOrDefault();
            }
        }

        public IEnumerable<InternalAdminPositionRole> GetInternalPositionRoles(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminPositionRolesSet.AsQueryable();

                qry = GetWherePositionRole(ref qry, filter);

                var res = qry.Select(x => new InternalAdminPositionRole
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    PositionId = x.PositionId,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminPositionRolesSet.AsQueryable();

                qry = GetWherePositionRole(ref qry, filter);

                return qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    RoleName = x.Role.Name,
                    PositionId = x.PositionId,
                }).ToList();
            }
        }


        public IEnumerable<FrontAdminPositionRole> GetPositionRolesDIP(IContext context, FilterAdminRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRolesSet.AsQueryable();

                if (filter.PositionIDs?.Count > 0)
                {
                    if (filter.IsChecked == true)
                    {
                        List<int> roles = GetRolesByPositions(context, filter.PositionIDs);

                        if (filter.IDs == null) filter.IDs = new List<int>();

                        filter.IDs.AddRange(roles);
                    }
                }

                if (filter.LinkIDs?.Count > 0)
                {
                    List<int> roles = GetRolesByPositions(context, filter.LinkIDs);

                    filter.IDs.AddRange(roles);
                }

                qry = GetWhereRole(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    RoleId = x.Id,
                    RoleName = x.Name,
                    IsChecked = x.PositionRoles.Where(y => y.RoleId == x.Id).Where(y => filter.PositionIDs.Contains(y.PositionId)).Any()
                }).ToList();
            }
        }

        public bool ExistsPositionRole(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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

        private IQueryable<AdminPositionRoles> GetAdminPositionRoleQuery(DmsContext dbContext, FilterAdminPositionRole filter)
        {
            var qry = dbContext.AdminPositionRolesSet.AsQueryable();

            qry = GetWherePositionRole(ref qry, filter);

            return qry;
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
                var filterContains = PredicateBuilder.True<AdminPositionRoles>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.And(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.PositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminPositionRoles>();

                filterContains = filter.PositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.RoleIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminPositionRoles>();

                filterContains = filter.RoleIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.RoleId == value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;
        }

        #endregion

        #region [+] UserRole ...
        public int AddUserRole(IContext context, InternalAdminUserRole model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminUserRoles dbModel = AdminModelConverter.GetDbUserRole(context, model);
                dbContext.AdminUserRolesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void AddUserRoles(IContext context, IEnumerable<InternalAdminUserRole> models)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModels = AdminModelConverter.GetDbUserRoles(context, models);
                dbContext.AdminUserRolesSet.AddRange(dbModels);
                dbContext.SaveChanges();
            }
        }

        public void UpdateUserRole(IContext context, InternalAdminUserRole model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminUserRoles dbModel = AdminModelConverter.GetDbUserRole(context, model);
                dbContext.AdminUserRolesSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteUserRole(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = dbContext.AdminUserRolesSet.FirstOrDefault(x => x.Id == id);
                dbContext.AdminUserRolesSet.Remove(dbModel);
                dbContext.SaveChanges();
            }
        }
        public void DeleteUserRoles(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();
                qry = GetWhereUserRole(ref qry, filter);
                if (qry.Count() == 0) return;
                dbContext.AdminUserRolesSet.RemoveRange(qry);
                dbContext.SaveChanges();
            }
        }

        public InternalAdminUserRole GetInternalUserRole(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();

                qry = GetWhereUserRole(ref qry, filter);

                return qry.Select(x => new InternalAdminUserRole
                {
                    Id = x.Id,
                    UserId = x.UserId ?? x.PositionExecutor.AgentId,
                    RoleId = x.RoleId,
                    //PositionId = x.PositionExecutor.PositionId,
                    PositionExecutorId = x.PositionExecutorId,
                    //StartDate = x.PositionExecutor.StartDate,
                    //EndDate = x.PositionExecutor.EndDate == DateTime.MaxValue ? (DateTime?)null : x.PositionExecutor.EndDate,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
            }
        }

        public IEnumerable<InternalAdminUserRole> GetInternalUserRoles(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();

                qry = GetWhereUserRole(ref qry, filter);

                return qry.Select(x => new InternalAdminUserRole
                {
                    Id = x.Id,
                    UserId = x.UserId ?? x.PositionExecutor.AgentId,
                    RoleId = x.RoleId,
                    //PositionId = x.PositionExecutor.PositionId,
                    PositionExecutorId = x.PositionExecutorId,
                    //StartDate = x.PositionExecutor.StartDate,
                    //EndDate = x.PositionExecutor.EndDate == DateTime.MaxValue ? (DateTime?)null : x.PositionExecutor.EndDate,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();
            }
        }


        public List<int> GetRolesByUsers(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();

                qry = GetWhereUserRole(ref qry, filter);

                return qry.Select(x => x.RoleId).ToList();
            }
        }

        public IEnumerable<FrontAdminUserRole> GetUserRoles(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();

                qry = GetWhereUserRole(ref qry, filter);

                return qry.Select(x => new FrontAdminUserRole
                {
                    Id = x.Id,
                    RoleId = x.Id,
                    RolePositionId = x.PositionExecutor.PositionId,
                    UserId = x.UserId,
                }).ToList();

            }
        }

        //public IEnumerable<FrontAdminUserRole> GetUserRolesDIP(IContext context, FilterAdminRole filter)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
        //    {
        //        var qry = dbContext.AdminRolesSet.AsQueryable();

        //        qry = GetWhereRole(ref qry, filter);

        //        qry = qry.OrderBy(x => x.Name);

        //        return qry.Select(x => new FrontAdminUserRole
        //        {
        //            Id = x.Id,
        //            //UserId = x.UserId,
        //            RoleId = x.Id,
        //            RoleName = x.Name,
        //            IsChecked = x.UserRoles.
        //            Where(y => y.RoleId == x.Id).
        //            Where(y => filter.UserIDs.Contains(y.UserId ?? y.PositionExecutor.AgentId)).
        //            Where(y => y.PositionExecutor.StartDate <= (filter.EndDate ?? DateTime.MaxValue) && y.PositionExecutor.EndDate >= (filter.StartDate ?? DateTime.MinValue)).
        //            Any()
        //        }).ToList();

        //    }
        //}

        //public IEnumerable<FrontDIPUserRolesRoles> GetUserRolesDIPUserRoles(IContext context, FilterAdminUserRole filter)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    {
        //        var qry = dbContext.AdminUserRolesSet.AsQueryable();

        //        qry = GetWhereUserRole(ref qry, filter);

        //        string objId = ((int)EnumObjects.AdminUserRoles).ToString();

        //        return qry.Select(x => new FrontDIPUserRolesRoles
        //        {
        //            Id = x.Id,
        //            Name = x.Role.Name,
        //            SearchText = string.Concat(x.Role.Name, " ", x.StartDate, " ", x.EndDate),
        //            ObjectId = (int)EnumObjects.AdminUserRoles,
        //            TreeId = string.Concat(x.Id.ToString(), "_", objId),
        //            TreeParentId = string.Concat(x.PositionExecutors.PositionId.ToString(), "_", (int)EnumObjects.DictionaryPositionExecutors),
        //            IsActive = true,
        //            IsList = true,
        //            StartDate = x.StartDate,
        //            EndDate = x.EndDate,
        //            PositionId = x.PositionExecutors.PositionId,
        //        }).ToList();
        //    }
        //}

        public bool ExistsUserRole(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();

                qry = GetWhereUserRole(ref qry, filter);

                var res = qry.Select(x => new FrontAdminUserRole
                {
                    Id = x.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        private static IQueryable<AdminUserRoles> GetWhereUserRole(ref IQueryable<AdminUserRoles> qry, FilterAdminUserRole filter)
        {
            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminUserRoles>();

                filterContains = filter.IDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.True<AdminUserRoles>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.And(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.UserIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminUserRoles>();

                filterContains = filter.UserIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.UserId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.RoleIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminUserRoles>();

                filterContains = filter.RoleIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.RoleId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.PositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminUserRoles>();

                filterContains = filter.PositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionExecutor.PositionId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.PositionExecutorIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminUserRoles>();

                filterContains = filter.PositionExecutorIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionExecutorId == value).Expand());

                qry = qry.Where(filterContains);
            }

            //if (filter.Period?.IsActive == true)
            //{
            //    qry = qry.Where(x => x.StartDate >= filter.Period.DateBeg);
            //    qry = qry.Where(x => x.EndDate <= filter.Period.DateEnd);
            //}

            if (filter.StartDate.HasValue)
            {
                // PSS ссылка может отсутствовать
                qry = qry.Where(x => x.PositionExecutor.StartDate <= (filter.EndDate ?? DateTime.UtcNow));
            }

            if (filter.EndDate.HasValue)
            {
                // PSS ссылка может отсутствовать
                qry = qry.Where(x => x.PositionExecutor.EndDate >= (filter.StartDate ?? DateTime.UtcNow));
            }

            return qry;
        }

        #endregion

        #region [+] DepartmentAdmin ...

        public int AddDepartmentAdmin(IContext context, InternalDepartmentAdmin model)
        {
            DepartmentAdminsSet.Add(model);
            return model.EmployeeId;
        }

        public void DeleteDepartmentAdmin(IContext context, InternalDepartmentAdmin model)
        {
            var da = DepartmentAdminsSet.Where(x => x.DepartmentId == model.DepartmentId & x.EmployeeId == model.EmployeeId);
            foreach (var item in da)
            {
                DepartmentAdminsSet.Remove(item);
                break;
            }
        }

        public IEnumerable<FrontDictionaryAgentEmployee> GetDepartmentAdmins(IContext context, int departmentId)
        {
            return DepartmentAdminsSet.Where(x => x.DepartmentId == departmentId).Select(x => new FrontDictionaryAgentEmployee()
            {
                Id = x.EmployeeId,
                Name = x.EmployeeName
            });
        }
        #endregion

        #region [+] Subordination ...
        public int AddSubordination(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminSubordinations dbModel = AdminModelConverter.GetDbSubordination(context, model);
                dbContext.AdminSubordinationsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void AddSubordinations(IContext context, List<InternalAdminSubordination> list)
        {
            using (var dbContext = new DmsContext(context))
            {
                var items = AdminModelConverter.GetDbSubordinations(context, list);
                dbContext.AdminSubordinationsSet.AddRange(items);
                dbContext.SaveChanges();
            }
        }

        public void UpdateSubordination(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminSubordinations dbModel = AdminModelConverter.GetDbSubordination(context, model);
                dbContext.AdminSubordinationsSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        public void DeleteSubordination(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminSubordinations dbModel = null;
                if (model.Id == 0)
                {
                    dbModel = dbContext.AdminSubordinationsSet.
                        FirstOrDefault(x => x.SourcePositionId == model.SourcePositionId && x.TargetPositionId == model.TargetPositionId && x.SubordinationTypeId == model.SubordinationTypeId);
                }
                else
                {
                    dbModel = dbContext.AdminSubordinationsSet.FirstOrDefault(x => x.Id == model.Id);
                }
                dbContext.AdminSubordinationsSet.Remove(dbModel);
                dbContext.SaveChanges();
            }
        }

        public void DeleteSubordinationsBySourcePositionId(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var list = dbContext.AdminSubordinationsSet.Where(x => x.SourcePositionId == model.SourcePositionId);
                dbContext.AdminSubordinationsSet.RemoveRange(list);
                dbContext.SaveChanges();
            }
        }

        public void DeleteSubordinationsByTargetPositionId(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var list = dbContext.AdminSubordinationsSet.Where(x => x.TargetPositionId == model.TargetPositionId);
                dbContext.AdminSubordinationsSet.RemoveRange(list);
                dbContext.SaveChanges();
            }
        }

        public void DeleteSubordinations(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminSubordinationsSet.AsQueryable();
                qry = GetWhereSubordination(ref qry, filter);
                dbContext.AdminSubordinationsSet.RemoveRange(qry);
                dbContext.SaveChanges();
            }
        }

        public InternalAdminSubordination GetInternalSubordination(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminSubordinationsSet.AsQueryable();

                qry = GetWhereSubordination(ref qry, filter);

                return qry.Select(x => new InternalAdminSubordination
                {
                    Id = x.Id,
                    SourcePositionId = x.SourcePositionId,
                    TargetPositionId = x.TargetPositionId,
                    SubordinationTypeId = x.SubordinationTypeId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
            }
        }


        public IEnumerable<FrontAdminSubordination> GetSubordinations(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context))
            //using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminSubordinationsSet.AsQueryable();

                qry = GetWhereSubordination(ref qry, filter);

                //qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new FrontAdminSubordination
                {
                    Id = x.Id,
                    SourcePositionId = x.SourcePositionId,
                    SourcePositionName = x.SourcePosition.Name,
                    TargetPositionId = x.TargetPositionId,
                    TargetPositionName = x.TargetPosition.Name,
                    SubordinationTypeId = (EnumSubordinationTypes)x.SubordinationTypeId,
                    SubordinationTypeName = x.SubordinationType.Name
                }).ToList();
            }
        }

        public IEnumerable<InternalAdminSubordination> GetInternalSubordinations(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context))
            //using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminSubordinationsSet.AsQueryable();

                qry = GetWhereSubordination(ref qry, filter);

                //qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new InternalAdminSubordination
                {
                    Id = x.Id,
                    SourcePositionId = x.SourcePositionId,
                    TargetPositionId = x.TargetPositionId,
                    SubordinationTypeId = x.SubordinationTypeId,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();
            }
        }

        public List<int> GetSubordinationTargetIDs(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context))
            //using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminSubordinationsSet.AsQueryable();

                qry = GetWhereSubordination(ref qry, filter);

                //qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => x.TargetPositionId).ToList();
            }
        }

        public bool ExistsSubordination(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context))
            //using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminSubordinationsSet.AsQueryable();

                qry = GetWhereSubordination(ref qry, filter);

                var res = qry.Select(x => new FrontAdminSubordination
                {
                    Id = x.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        private static IQueryable<AdminSubordinations> GetWhereSubordination(ref IQueryable<AdminSubordinations> qry, FilterAdminSubordination filter)
        {
            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminSubordinations>();

                filterContains = filter.IDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.True<AdminSubordinations>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.And(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.SourcePositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminSubordinations>();

                filterContains = filter.SourcePositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourcePositionId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.TargetPositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminSubordinations>();

                filterContains = filter.TargetPositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.PositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminSubordinations>();

                filterContains = filter.PositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourcePositionId == value || e.TargetPositionId == value).Expand());

                qry = qry.Where(filterContains);

            }

            if (filter.SubordinationTypeIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminSubordinations>();

                filterContains = filter.SubordinationTypeIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SubordinationTypeId == (int)value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;
        }

        #endregion

        #region [+] RegistrationJournalPositions ...
        public int AddRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminRegistrationJournalPositions dbModel = AdminModelConverter.GetDbRegistrationJournalPosition(context, model);
                dbContext.AdminRegistrationJournalPositionsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void AddRegistrationJournalPositions(IContext context, List<InternalRegistrationJournalPosition> list)
        {
            using (var dbContext = new DmsContext(context))
            {
                var items = AdminModelConverter.GetDbRegistrationJournalPositions(context, list);
                dbContext.AdminRegistrationJournalPositionsSet.AddRange(items);
                dbContext.SaveChanges();
            }
        }

        public void UpdateRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminRegistrationJournalPositions dbModel = AdminModelConverter.GetDbRegistrationJournalPosition(context, model);
                dbContext.AdminRegistrationJournalPositionsSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model)
        {
            using (var dbContext = new DmsContext(context))
            {
                AdminRegistrationJournalPositions dbModel = null;
                if (model.Id == 0)
                {
                    dbModel = dbContext.AdminRegistrationJournalPositionsSet.
                        FirstOrDefault(x => x.PositionId == model.PositionId && x.RegJournalId == model.RegistrationJournalId && x.RegJournalAccessTypeId == model.RegJournalAccessTypeId);
                }
                else
                {
                    dbModel = dbContext.AdminRegistrationJournalPositionsSet.FirstOrDefault(x => x.Id == model.Id);
                }
                dbContext.AdminRegistrationJournalPositionsSet.Remove(dbModel);
                dbContext.SaveChanges();
            }
        }

        public void DeleteRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();
                qry = GetWhereRegistrationJournalPosition(ref qry, filter);
                dbContext.AdminRegistrationJournalPositionsSet.RemoveRange(qry);
                dbContext.SaveChanges();
            }
        }

        public InternalRegistrationJournalPosition GetInternalRegistrationJournalPosition(IContext context, FilterAdminRegistrationJournalPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();

                qry = GetWhereRegistrationJournalPosition(ref qry, filter);

                return qry.Select(x => new InternalRegistrationJournalPosition
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    RegistrationJournalId = x.RegJournalId,
                    RegJournalAccessTypeId = x.RegJournalAccessTypeId,
                }).FirstOrDefault();
            }
        }

        public IEnumerable<InternalRegistrationJournalPosition> GetInternalRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();

                qry = GetWhereRegistrationJournalPosition(ref qry, filter);

                //qry = qry.OrderBy(x => x.Name);

                var res= qry.Select(x => new InternalRegistrationJournalPosition
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    RegistrationJournalId = x.RegJournalId,
                    RegJournalAccessTypeId = x.RegJournalAccessTypeId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }


        //public IEnumerable<FrontAdminRegistrationJournalPosition> GetRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    //using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
        //    {
        //        var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();

        //        qry = GetWhereRegistrationJournalPosition(ref qry, filter);

        //        //qry = qry.OrderBy(x => x.Name);

        //        return qry.Select(x => new FrontAdminRegistrationJournalPosition
        //        {
        //            Id = x.Id,
        //            SourcePositionId = x.SourcePositionId,
        //            SourcePositionName = x.SourcePosition.Name,
        //            TargetPositionId = x.TargetPositionId,
        //            TargetPositionName = x.TargetPosition.Name,
        //            RegistrationJournalPositionTypeId = (EnumRegistrationJournalPositionTypes)x.RegistrationJournalPositionTypeId,
        //            RegistrationJournalPositionTypeName = x.RegistrationJournalPositionType.Name
        //        }).ToList();
        //    }
        //}

        

        //public List<int> GetRegistrationJournalPositionTargetIDs(IContext context, FilterAdminRegistrationJournalPosition filter)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    //using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
        //    {
        //        var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();

        //        qry = GetWhereRegistrationJournalPosition(ref qry, filter);

        //        //qry = qry.OrderBy(x => x.Name);

        //        return qry.Select(x => x.TargetPositionId).ToList();
        //    }
        //}

        public bool ExistsRegistrationJournalPosition(IContext context, FilterAdminRegistrationJournalPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            //using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();

                qry = GetWhereRegistrationJournalPosition(ref qry, filter);

                return qry.Any(); ;
            }
        }

        private static IQueryable<AdminRegistrationJournalPositions> GetWhereRegistrationJournalPosition(ref IQueryable<AdminRegistrationJournalPositions> qry, FilterAdminRegistrationJournalPosition filter)
        {
            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRegistrationJournalPositions>();

                filterContains = filter.IDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(filterContains);
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.True<AdminRegistrationJournalPositions>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.And(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.PositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRegistrationJournalPositions>();

                filterContains = filter.PositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.RegistrationJournalIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRegistrationJournalPositions>();

                filterContains = filter.RegistrationJournalIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.RegJournalId == value).Expand());

                qry = qry.Where(filterContains);
            }


            if (filter.RegistrationJournalAccessTypeIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminRegistrationJournalPositions>();

                filterContains = filter.RegistrationJournalAccessTypeIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.RegJournalAccessTypeId == (int)value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;
        }

        #endregion

        #region [+] MainMenu ...
        public IEnumerable<TreeItem> GetMainMenu(IContext context)
        {

            TreeItem itemDictDMS = new TreeItem { Id = 20, Name = "Документооборот" };

            itemDictDMS.Childs = new List<ITreeItem> {
                            new TreeItem { Id = 30, Name = "Типы документов", Description = "document-types" },
                            new TreeItem { Id = 31, Name = "Журналы регистрации", Description = "journals" },
                            new TreeItem { Id = 32, Name = "Тематики документов", Description = "" },
                            new TreeItem { Id = 33, Name = "Шаблоны документов", Description = "" },
                            };

            TreeItem itemDict = new TreeItem { Id = 4, Name = "Справочники" };

            itemDict.Childs = new List<ITreeItem> {
                itemDictDMS,
                new TreeItem { Id = 22, Name = "Физлица", Description = "agent-persons" },
                new TreeItem { Id = 23, Name = "Банки", Description = "agent-banks" },
                new TreeItem { Id = 24, Name = "Юрлица", Description = "agent-companies" },
                new TreeItem { Id = 25, Name = "-" },
                new TreeItem { Id = 26, Name = "Теги", Description = "tags" },
                new TreeItem { Id = 27, Name = "Клиентские справочники", Description = "" }
            };

            List<TreeItem> menus = new List<TreeItem> {
                    new TreeItem {Id = 1, Name = "Сотрудники", Description = "agent-employees"},
                    new TreeItem {Id = 2, Name = "Роли"},
                    new TreeItem {Id = 3, Name = "Структура"},
                    itemDict,
                    new TreeItem {Id = 5, Name = "Документы", Description = "docs"},
                    new TreeItem {Id = 6, Name = "События", Description = "events"},
                    new TreeItem {Id = 7, Name = "Файлы", Description = "attachments"},
                    new TreeItem {Id = 8, Name = "Ожидания", Description = "documentWaits"}
            };

            return menus;
        }

        #endregion

        #region [+] AddNewClient ...

        public List<InternalAdminRoleAction> GetRoleActionsForAdmin(IContext context)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return dbContext.SystemActionsSet.Select(x => new InternalAdminRoleAction { ActionId = x.Id }).ToList();
            }
        }

        #endregion
    }
}


