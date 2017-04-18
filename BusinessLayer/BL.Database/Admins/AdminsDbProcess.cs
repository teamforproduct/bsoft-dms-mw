using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Helpers.CashService;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.DBModel.Document;
using BL.Database.Dictionaries;
using BL.Database.Helper;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Common;
using BL.Model.Constants;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Users;
using EntityFramework.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Database.Admins
{
    public class AdminsDbProcess : CoreDb.CoreDb//, IAdminsDbProcess
    {
        private readonly ICacheService _cacheService;

        public AdminsDbProcess(ICacheService casheService)
        {
            _cacheService = casheService;
        }

        #region [+] General ...
        public AdminAccessInfo GetAdminAccesses(IContext ctx)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = new AdminAccessInfo();

                res.UserRoles = dbContext.AdminUserRolesSet.Where(x => x.Role.ClientId == ctx.CurrentClientId).Select(x => new InternalAdminUserRole
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    AgentId = x.PositionExecutor.AgentId
                }).ToList();

                res.Roles = dbContext.AdminRolesSet.Where(x => x.ClientId == ctx.CurrentClientId).Select(x => new InternalAdminRole
                {
                    Id = x.Id

                }).ToList();

                res.PositionRoles = dbContext.AdminPositionRolesSet.Where(x => x.Role.ClientId == ctx.CurrentClientId).Select(x => new InternalAdminPositionRole
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
                    PermissionId = x.PermissionId,
                    ObjectId = (EnumObjects)x.ObjectId,
                }).ToList();

                res.RolePermissions = dbContext.AdminRolePermissionsSet.Where(x => x.Role.ClientId == ctx.CurrentClientId).Select(x => new InternalAdminRolePermission
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    PermissionId = x.PermissionId
                }).ToList();

                // pss Таблица подлежит удалению - пустая
                //res.ActionAccess = dbContext.AdminRoleActionsSet.Where(x => x.Role.ClientId == ctx.CurrentClientId).Select(x => new InternalAdminRoleAction
                //{
                //    Id = x.Id,
                //    RecordId = x.RecordId,
                //    RoleId = x.RoleId,
                //    ActionId = x.ActionId
                //}).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontUserAssignments> GetAvailablePositions(IContext ctx, int agentId, List<int> PositionIDs)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryPositionExecutorsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).AsQueryable();

                var now = DateTime.UtcNow;
                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

                if (PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);

                    filterContains = PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                qry = qry.Where(x => x.AgentId == agentId && x.Position.ExecutorAgentId.HasValue && x.IsActive && now >= x.StartDate && now <= x.EndDate);

                qry = qry.OrderBy(x => x.PositionExecutorTypeId).ThenBy(x => x.Position.Order);

                var res = qry.Select(x => new FrontUserAssignments
                {
                    RolePositionId = x.PositionId,
                    RolePositionName = x.Position.Name,
                    RolePositionExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : null),
                    RolePositionExecutorTypeId = x.PositionExecutorType.Id,
                    RolePositionExecutorTypeName = x.PositionExecutorType.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate > maxDateTime ? (DateTime?)null : x.EndDate,
                    DepartmentName = x.Position.Department.Name,
                }).ToList();

                try
                {
                    var lastPositionChose =
                        dbContext.DictionaryAgentUsersSet.Where(x => x.Id == ctx.CurrentAgentId)
                        .Select(x => x.LastPositionChose).FirstOrDefault()
                        .Split(',').Select(n => Convert.ToInt32(n)).ToArray();

                    res.Where(x => x.RolePositionId.HasValue && lastPositionChose.Contains(x.RolePositionId.Value)).ToList().ForEach(x => x.IsLastChosen = true);
                }
                catch { };

                var positions = res.Select(s => s.RolePositionId).ToList();
                var filterAccessPositionContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterAccessPositionContains = positions.Aggregate(filterAccessPositionContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());
                var accessQry = dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(filterAccessPositionContains)
                                .GroupBy(x => x.PositionId)
                                .Select(x => new
                                {
                                    PositionId = x.Key,
                                    CountNewEvents = x.Sum(y => y.CountNewEvents),
                                    CountWaits = x.Sum(y => y.CountWaits),
                                    OverDueCountWaits = x.Sum(y => y.OverDueCountWaits),
                                    MinDueDate = x.Min(y => y.MinDueDate),
                                });
                var access = accessQry.ToList();
                res.ForEach(x =>
                {
                    var stat = access.Where(y => y.PositionId == x.RolePositionId).FirstOrDefault();
                    x.NewEventsCount = stat?.CountNewEvents;
                    x.ControlsCount = stat?.CountWaits;
                    x.OverdueControlsCount = stat?.OverDueCountWaits;
                    x.MinDueDate = stat?.MinDueDate;
                });


                //var filterNewEventTargetPositionContains = PredicateBuilder.New<DBModel.Document.DocumentEvents>(false);
                //filterNewEventTargetPositionContains = positions.Aggregate(filterNewEventTargetPositionContains,
                //    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                //var neweventQry = dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)   //TODO include doc access
                //                .Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId)
                //                .Where(filterNewEventTargetPositionContains)
                //                .GroupBy(g => g.TargetPositionId)
                //                .Select(s => new { PosID = s.Key, EvnCnt = s.Count() });
                //var newevnt = neweventQry.ToList();

                //var filterOnEventPositionsContains = PredicateBuilder.New<DocumentWaits>(false);
                //filterOnEventPositionsContains = positions.Aggregate(filterOnEventPositionsContains,
                //    (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value /*|| e.OnEvent.SourcePositionId == value*/).Expand());

                //var waitQry = dbContext.DocumentWaitsSet.Where(x => x.ClientId == ctx.CurrentClientId)   //TODO include doc access
                //                .Where(x => !x.OffEventId.HasValue)
                //                .Where(filterOnEventPositionsContains)
                //                .GroupBy(y => new
                //                {
                //                    y.OnEvent.SourcePositionId, y.OnEvent.TargetPositionId,
                //                   // IsOverDue = !y.OffEventId.HasValue && y.DueDate.HasValue && y.DueDate.Value <= DateTime.UtcNow,
                //                   // DueDate = DbFunctions.TruncateTime(y.DueDate),
                //                })
                //                .Select(x => new
                //                {
                //                    Pos = x.Key,
                //                    ControlsCount = x.Count(),
                //                    OverdueControlsCount = x.Where(y => y.DueDate.HasValue && y.DueDate.Value < DateTime.UtcNow).Count(),
                //                    MinDueDate = x.Where(y => y.DueDate.HasValue).Min(y => y.DueDate),
                //                })
                //                ;
                //var wait = waitQry.ToList();

                ////res.Join(newevnt, r => r.RolePositionId, e => e.PosID, (r, e) => { r.NewEventsCount = e.EvnCnt; r.ControlsCount = 1; r.OverdueControlsCount = 1; r.MinDueDate = DateTime.UtcNow; return r; }).ToList();
                //res.ForEach(x =>
                //{
                //    x.NewEventsCount = newevnt.Where(y => y.PosID == x.RolePositionId).Select(y => y.EvnCnt).FirstOrDefault();
                //    var t = wait.Where(y => y.Pos.SourcePositionId == x.RolePositionId || y.Pos.TargetPositionId == x.RolePositionId).GroupBy(y => 1)
                //        .Select(y => new
                //        {
                //            MinDueDate = y.Min(z => z.MinDueDate),
                //            ControlsCount = y.Sum(z => z.ControlsCount),
                //            OverdueControlsCount = y.Sum(z => z.OverdueControlsCount)
                //        }).FirstOrDefault();
                //    x.MinDueDate = t?.MinDueDate;
                //    x.ControlsCount = t?.ControlsCount ?? 0;
                //    x.OverdueControlsCount = t?.OverdueControlsCount ?? 0;
                //});

                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontUserAssignmentsAvailable> GetAvailablePositionsList(IContext ctx, int agentId, List<int> PositionIDs)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryPositionExecutorsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).AsQueryable();

                var now = DateTime.UtcNow;

                if (PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);

                    filterContains = PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                qry = qry.Where(x => x.AgentId == agentId && x.Position.ExecutorAgentId.HasValue && x.IsActive && now >= x.StartDate && now <= x.EndDate);

                var res = qry.Select(x => new FrontUserAssignmentsAvailable
                {
                    PositionId = x.PositionId,
                    PositionName = x.Position.Name,
                    DepartmentName = x.Position.Department.Name,
                    ExecutorName = (x.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.Personal ? string.Empty : x.Position.ExecutorAgent.Name),
                    ImageByteArray = (x.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.Personal ? new byte[] { } : x.Position.ExecutorAgent.Image),
                    ExecutorTypeId = x.PositionExecutorType.Id,
                    ExecutorTypeDescription = x.PositionExecutorType.Description,
                }).ToList();

                //IsLastChosen
                try
                {
                    var lastPositionChose =
                        dbContext.DictionaryAgentUsersSet.Where(x => x.Id == agentId)
                        .Select(x => x.LastPositionChose).FirstOrDefault()
                        .Split(',').Select(n => Convert.ToInt32(n)).ToArray();

                    res.Where(x => lastPositionChose.Contains(x.PositionId)).ToList().ForEach(x => x.IsLastChosen = true);
                }
                catch { };

                var positions = res.Select(s => s.PositionId).ToList();
                var filterAccessPositionContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterAccessPositionContains = positions.Aggregate(filterAccessPositionContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());
                var accessQry = dbContext.DocumentAccessesSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(filterAccessPositionContains)
                                .GroupBy(x => x.PositionId)
                                .Select(x => new
                                {
                                    PositionId = x.Key,
                                    CountNewEvents = x.Sum(y => y.CountNewEvents),
                                    CountWaits = x.Sum(y => y.CountWaits),
                                    OverDueCountWaits = x.Sum(y => y.OverDueCountWaits),
                                    MinDueDate = x.Min(y => y.MinDueDate),
                                });
                var access = accessQry.ToList();
                res.ForEach(x =>
                {
                    var stat = access.Where(y => y.PositionId == x.PositionId).FirstOrDefault();
                    x.NewEventsCount = stat?.CountNewEvents;
                    x.ControlsCount = stat?.CountWaits;
                    x.OverdueControlsCount = stat?.OverDueCountWaits;
                    x.MinDueDate = stat?.MinDueDate;
                });

                //var filterNewEventTargetPositionContains = PredicateBuilder.New<DBModel.Document.DocumentEvents>(false);
                //filterNewEventTargetPositionContains = positionList.Aggregate(filterNewEventTargetPositionContains,
                //    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                //var neweventQry = dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)   //TODO include doc access
                //                .Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId)
                //                .Where(filterNewEventTargetPositionContains)
                //                .GroupBy(g => g.TargetPositionId)
                //                .Select(s => new { PosID = s.Key, EvnCnt = s.Count() });
                //var newevnt = neweventQry.ToList();

                //var filterOnEventPositionsContains = PredicateBuilder.New<DocumentWaits>(false);
                //filterOnEventPositionsContains = positionList.Aggregate(filterOnEventPositionsContains,
                //    (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value /*|| e.OnEvent.SourcePositionId == value*/).Expand());

                //var waitQry = dbContext.DocumentWaitsSet.Where(x => x.ClientId == ctx.CurrentClientId)   //TODO include doc access
                //                .Where(x => !x.OffEventId.HasValue)
                //                .Where(filterOnEventPositionsContains)
                //                .GroupBy(y => new
                //                {
                //                    y.OnEvent.SourcePositionId,
                //                    y.OnEvent.TargetPositionId,
                //                    // IsOverDue = !y.OffEventId.HasValue && y.DueDate.HasValue && y.DueDate.Value <= DateTime.UtcNow,
                //                    // DueDate = DbFunctions.TruncateTime(y.DueDate),
                //                })
                //                .Select(x => new
                //                {
                //                    Pos = x.Key,
                //                    ControlsCount = x.Count(),
                //                    OverdueControlsCount = x.Where(y => y.DueDate.HasValue && y.DueDate.Value < DateTime.UtcNow).Count(),
                //                    MinDueDate = x.Where(y => y.DueDate.HasValue).Min(y => y.DueDate),
                //                })
                //                ;
                //var wait = waitQry.ToList();

                ////res.Join(newevnt, r => r.RolePositionId, e => e.PosID, (r, e) => { r.NewEventsCount = e.EvnCnt; r.ControlsCount = 1; r.OverdueControlsCount = 1; r.MinDueDate = DateTime.UtcNow; return r; }).ToList();
                //res.ForEach(x =>
                //{
                //    x.NewEventsCount = newevnt.Where(y => y.PosID == x.PositionId).Select(y => y.EvnCnt).FirstOrDefault();
                //    var t = wait.Where(y => y.Pos.SourcePositionId == x.PositionId || y.Pos.TargetPositionId == x.PositionId).GroupBy(y => 1)
                //        .Select(y => new
                //        {
                //            MinDueDate = y.Min(z => z.MinDueDate),
                //            ControlsCount = y.Sum(z => z.ControlsCount),
                //            OverdueControlsCount = y.Sum(z => z.OverdueControlsCount)
                //        }).FirstOrDefault();
                //    x.MinDueDate = t?.MinDueDate;
                //    x.ControlsCount = t?.ControlsCount ?? 0;
                //    x.OverdueControlsCount = t?.OverdueControlsCount ?? 0;
                //});

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return null;
        }

        public Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext ctx)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dateNow = DateTime.UtcNow;
                var qry = dbContext.DictionaryPositionExecutorsSet
                    .Where(x => dateNow >= x.StartDate && dateNow <= x.EndDate && x.AgentId == ctx.CurrentAgentId);
                var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
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
        /// <param name="ctx"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool VerifySubordination(IContext ctx, VerifySubordination model)
        {
            if (model.SourcePositions.Contains(model.TargetPosition))
                return true;

            using (var transaction = Transactions.GetTransaction())
            {
                var dictDb = DmsResolver.Current.Get<DictionariesDbProcess>();
                var pos = dictDb.GetPositions(ctx, new FilterDictionaryPosition() { IDs = new List<int> { model.TargetPosition }, SubordinatedPositions = model.SourcePositions })
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

        public Employee GetEmployeeForContext(IContext ctx, string userId)
        {
            using (var dbContext = new DmsContext(ctx))// Тут контекст должен создаваться каждый раз т.к метод вызывается в случаях, когда рабочий контекст запроса еще не создан
            using (var transaction = Transactions.GetTransaction())
            {
                var now = DateTime.UtcNow;

                // для авторизации 
                var res = dbContext.DictionaryAgentUsersSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).Where(x => x.UserId.Equals(userId))
                    .Select(x => new Employee
                    {
                        AgentId = x.Id,
                        Name = x.Agent.Name,
                        LanguageId = x.Agent.AgentUser.LanguageId,
                        IsActive = x.Agent.AgentEmployee.IsActive,
                        PositionExecutorsCount = x.Agent.AgentEmployee.PositionExecutors.Where(y => y.AgentId == x.Id & y.IsActive == true & now >= y.StartDate & now <= y.EndDate).Count(), //IS THAT CORRECT?? 
                    }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        #endregion


        #region [+] Roles ...

        public int AddRoleType(IContext ctx, InternalAdminRoleType model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbRoleType(ctx, model);
                dbContext.AdminRolesTypesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public int AddRole(IContext ctx, InternalAdminRole model)
        {
            int res;
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbRole(ctx, model);
                dbContext.AdminRolesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.AdminRoles, EnumOperationType.AddNew);
                transaction.Complete();
                res = dbModel.Id;
            }
            _cacheService.RefreshKey(ctx, SettingConstants.ADMIN_ROLE_CASHE_KEY);
            return res;
        }

        public void UpdateRole(IContext ctx, InternalAdminRole model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                AdminRoles dbModel = AdminModelConverter.GetDbRole(ctx, model);
                dbContext.AdminRolesSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.AdminRoles, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
            _cacheService.RefreshKey(ctx, SettingConstants.ADMIN_ROLE_CASHE_KEY);
        }

        public void DeleteRoles(IContext ctx, FilterAdminRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(ctx, dbContext, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.AdminRoles, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
            _cacheService.RefreshKey(ctx, SettingConstants.ADMIN_ROLE_CASHE_KEY);
        }

        public InternalAdminRole GetInternalRole(IContext ctx, FilterAdminRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(ctx, dbContext, filter);

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

        public IEnumerable<FrontMainRoles> GetMainRoles(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(ctx, dbContext, filter as FilterAdminRole);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainRoles>();

                var res = qry.Select(x => new FrontMainRoles
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDefault = x.RoleTypeId.HasValue,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetListRoles(IContext ctx, FilterAdminRole filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(ctx, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<ListItem>();

                var res = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }
        //Func<IContext, IBaseFilter, UISorting, List<int>> IdsFunc,
        public List<int> GetRoleIDs(IContext ctx, IBaseFilter filter, UISorting sorting)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(ctx, dbContext, filter as FilterAdminRole);

                qry = qry.OrderBy(x => x.Name);

                //if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<int>();

                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontAdminRole> GetRoles(IContext ctx, FilterAdminRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(ctx, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontAdminRole
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsEditable = x.RoleType == null

                    //RoleCode = x.RoleType.Code,
                    //RoleName = x.RoleType.Name
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public string GetRoleTypeCode(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.AdminRolesSet.
                    Where(x => x.ClientId == ctx.CurrentClientId).
                    Where(x => x.Id == id).
                    AsQueryable();

                var res = qry.Select(x => x.RoleType.Code).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int GetRoleByCode(IContext ctx, Roles item)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                // Для заводских ролей отношение к типам ролей 1:1
                var qry = dbContext.AdminRolesSet.
                    Where(x => x.ClientId == ctx.CurrentClientId).
                    Where(x => x.RoleType.Code == item.ToString()).
                    AsQueryable();

                var res = qry.Select(x => x.Id).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public bool ExistsRole(IContext ctx, FilterAdminRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new FrontAdminRole
                {
                    Id = x.Id
                }).FirstOrDefault();
                transaction.Complete();
                return res != null;
            }
        }

        private IQueryable<AdminRoles> GetRolesQuery(IContext ctx, DmsContext dbContext, FilterAdminRole filter)
        {
            var qry = dbContext.AdminRolesSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminPositionRoles>(false);
                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(x => x.PositionRoles.AsQueryable().Any(filterContains));
                }


                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRoles>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRoles>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Список классификаторов
                if (filter.RoleTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRoles>(false);

                    filterContains = filter.RoleTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RoleTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<AdminRoles>(false);

                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

                if (!string.IsNullOrEmpty(filter.Description))
                {
                    var filterContains = PredicateBuilder.New<AdminRoles>(false);

                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Description).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Description.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }
        #endregion


        #region [+] PositionRole ...
        public int AddPositionRole(IContext ctx, InternalAdminPositionRole model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                AdminPositionRoles dbModel = AdminModelConverter.GetDbPositionRole(ctx, model);
                dbContext.AdminPositionRolesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.AdminPositionRoles, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void DeletePositionRoles(IContext ctx, FilterAdminPositionRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(ctx, dbContext, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.AdminPositionRoles, EnumOperationType.Delete);
                qry.Delete();
                transaction.Complete();
            }
        }

        public InternalAdminPositionRole GetInternalPositionRole(IContext ctx, FilterAdminPositionRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(ctx, dbContext, filter);

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

        public FrontAdminPositionRole GetPositionRole(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.AdminRolesSet.Where(x => x.Id == id).Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

                var res = qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    RoleName = x.Name,
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalAdminPositionRole> GetInternalPositionRoles(IContext ctx, FilterAdminPositionRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(ctx, dbContext, filter);

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

        public IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext ctx, FilterAdminPositionRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    RoleName = x.Role.Name,
                    PositionId = x.PositionId,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<FrontAdminPositionRole> GetPositionRolesDIP(IContext ctx, FilterAdminPositionRoleDIP filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var roleFilter = new FilterAdminRole();

                if (filter.IsChecked ?? false)
                {
                    roleFilter.PositionIDs = filter.PositionIDs;
                }


                var qry = GetRolesQuery(ctx, dbContext, roleFilter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    RoleId = x.Id,
                    RoleName = x.Name,
                    IsChecked = x.PositionRoles.Where(y => y.RoleId == x.Id).Where(y => filter.PositionIDs.Contains(y.PositionId)).Any(),
                    IsDefault = x.PositionRoles.Where(y => y.RoleId == x.Id).Where(y => y.Role.RoleTypeId.HasValue).Any(),
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public bool ExistsPositionRole(IContext ctx, FilterAdminPositionRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(ctx, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<AdminPositionRoles> GetAdminPositionRoleQuery(IContext ctx, DmsContext dbContext, FilterAdminPositionRole filter)
        {
            var qry = dbContext.AdminPositionRolesSet.Where(x => x.Role.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminPositionRoles>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminPositionRoles>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminPositionRoles>(false);

                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.RoleIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminPositionRoles>(false);

                    filterContains = filter.RoleIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RoleId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }
        #endregion

        #region [+] UserRole ...
        public void AddUserRoles(IContext ctx, IEnumerable<InternalAdminUserRole> models)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModels = AdminModelConverter.GetDbUserRoles(ctx, models);
                var list = dbContext.AdminUserRolesSet.AddRange(dbModels);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, list.Select(x => x.Id).ToList(), EnumObjects.AdminUserRoles, EnumOperationType.AddNew);
                transaction.Complete();
            }
            _cacheService.RefreshKey(ctx, SettingConstants.USER_ROLE_CASHE_KEY);
        }

        public void DeleteUserRoles(IContext ctx, FilterAdminUserRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserRolesQuery(ctx, dbContext, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.AdminUserRoles, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
            _cacheService.RefreshKey(ctx, SettingConstants.USER_ROLE_CASHE_KEY);
        }

        public IEnumerable<InternalAdminUserRole> GetInternalUserRoles(IContext ctx, FilterAdminUserRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

                var qry = GetUserRolesQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new InternalAdminUserRole
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    PositionExecutorId = x.PositionExecutorId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,

                    RoleTypeId = x.Role.RoleTypeId,
                    PositionId = x.PositionExecutor.PositionId,
                    StartDate = x.PositionExecutor.StartDate,
                    EndDate = x.PositionExecutor.EndDate > maxDateTime ? (DateTime?)null : x.PositionExecutor.EndDate,
                    AgentId = x.PositionExecutor.AgentId,

                }).ToList();

                transaction.Complete();

                return res;
            }
        }


        public List<int> GetRolesByUsers(IContext ctx, FilterAdminUserRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserRolesQuery(ctx, dbContext, filter);

                var res = qry.Select(x => x.RoleId).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<FrontAdminUserRole> GetUserRoles(IContext ctx, FilterAdminUserRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserRolesQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new FrontAdminUserRole
                {
                    Id = x.Id,
                    RoleId = x.Id,
                    RolePositionId = x.PositionExecutor.PositionId,
                    PositionExecutorId = x.PositionExecutorId,
                    UserId = x.PositionExecutor.AgentId,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public bool ExistsUserRole(IContext ctx, FilterAdminUserRole filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserRolesQuery(ctx, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<AdminUserRoles> GetUserRolesQuery(IContext ctx, DmsContext dbContext, FilterAdminUserRole filter)
        {
            var qry = dbContext.AdminUserRolesSet.Where(x => x.Role.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminUserRoles>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminUserRoles>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.UserIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminUserRoles>(false);

                    filterContains = filter.UserIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionExecutor.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.RoleIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminUserRoles>(false);

                    filterContains = filter.RoleIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RoleId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminUserRoles>(false);

                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionExecutor.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionExecutorIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminUserRoles>(false);

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
                    qry = qry.Where(x => x.PositionExecutor.StartDate <= (filter.EndDate ?? DateTime.UtcNow));
                }

                if (filter.EndDate.HasValue)
                {
                    qry = qry.Where(x => x.PositionExecutor.EndDate >= (filter.StartDate ?? DateTime.UtcNow));
                }
            }

            return qry;
        }

        #endregion

        #region [+] DepartmentAdmin ...

        public int AddDepartmentAdmin(IContext ctx, InternalDepartmentAdmin model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbEmployeeDepartments(ctx, model);
                dbContext.AdminEmployeeDepartmentsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }

        }

        public void DeleteDepartmentAdmins(IContext ctx, FilterAdminEmployeeDepartments filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetEmployeeDepartmentsQuery(ctx, dbContext, filter);
                qry.Delete();

                transaction.Complete();
            }
        }

        public IEnumerable<FrontAdminEmployeeDepartments> GetDepartmentAdmins(IContext ctx, int departmentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetEmployeeDepartmentsQuery(ctx, dbContext, new FilterAdminEmployeeDepartments { DepartmentIDs = new List<int> { departmentId } });

                var res = qry.Select(x => new FrontAdminEmployeeDepartments
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.Agent.Name,
                    DepartmentId = x.DepartmentId,
                }).ToList();
                transaction.Complete();
                return res;
            }

        }

        private IQueryable<AdminEmployeeDepartments> GetEmployeeDepartmentsQuery(IContext ctx, DmsContext dbContext, FilterAdminEmployeeDepartments filter)
        {
            var qry = dbContext.AdminEmployeeDepartmentsSet.Where(x => x.Employee.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminEmployeeDepartments>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminEmployeeDepartments>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DepartmentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminEmployeeDepartments>(false);

                    filterContains = filter.DepartmentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }


        #endregion

        #region [+] Subordination ...
        public int AddSubordination(IContext ctx, InternalAdminSubordination model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                AdminSubordinations dbModel = AdminModelConverter.GetDbSubordination(ctx, model);
                dbContext.AdminSubordinationsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void AddSubordinations(IContext ctx, List<InternalAdminSubordination> list)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var items = AdminModelConverter.GetDbSubordinations(ctx, list);
                dbContext.AdminSubordinationsSet.AddRange(items);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateSubordination(IContext ctx, InternalAdminSubordination model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                AdminSubordinations dbModel = AdminModelConverter.GetDbSubordination(ctx, model);
                dbContext.AdminSubordinationsSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteSubordinations(IContext ctx, FilterAdminSubordination filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(ctx, dbContext, filter);
                qry.Delete();
                transaction.Complete();
            }
        }

        public InternalAdminSubordination GetInternalSubordination(IContext ctx, FilterAdminSubordination filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new InternalAdminSubordination
                {
                    Id = x.Id,
                    SourcePositionId = x.SourcePositionId,
                    TargetPositionId = x.TargetPositionId,
                    SubordinationTypeId = x.SubordinationTypeId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontAdminSubordination> GetSubordinations(IContext ctx, FilterAdminSubordination filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(ctx, dbContext, filter);

                //qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontAdminSubordination
                {
                    Id = x.Id,
                    SourcePositionId = x.SourcePositionId,
                    SourcePositionName = x.SourcePosition.Name,
                    TargetPositionId = x.TargetPositionId,
                    TargetPositionName = x.TargetPosition.Name,
                    SubordinationTypeId = (EnumSubordinationTypes)x.SubordinationTypeId,
                    SubordinationTypeName = x.SubordinationType.Name
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<InternalAdminSubordination> GetInternalSubordinations(IContext ctx, FilterAdminSubordination filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(ctx, dbContext, filter);

                //qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new InternalAdminSubordination
                {
                    Id = x.Id,
                    SourcePositionId = x.SourcePositionId,
                    TargetPositionId = x.TargetPositionId,
                    SubordinationTypeId = x.SubordinationTypeId,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public List<int> GetSubordinationTargetIDs(IContext ctx, FilterAdminSubordination filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(ctx, dbContext, filter);

                //qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.TargetPositionId).ToList();

                transaction.Complete();

                return res;
            }
        }

        public bool ExistsSubordination(IContext ctx, FilterAdminSubordination filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(ctx, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<AdminSubordinations> GetSubordinationsQuery(IContext ctx, DmsContext dbContext, FilterAdminSubordination filter)
        {
            var qry = dbContext.AdminSubordinationsSet.Where(x => x.SourcePosition.Department.Company.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminSubordinations>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminSubordinations>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.SourcePositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminSubordinations>(false);

                    filterContains = filter.SourcePositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetPositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminSubordinations>(false);

                    filterContains = filter.TargetPositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminSubordinations>(false);

                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value || e.TargetPositionId == value).Expand());

                    qry = qry.Where(filterContains);

                }

                if (filter.SubordinationTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminSubordinations>(false);

                    filterContains = filter.SubordinationTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SubordinationTypeId == (int)value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        #endregion

        #region [+] RegistrationJournalPositions ...
        public int AddRegistrationJournalPosition(IContext ctx, InternalRegistrationJournalPosition model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                AdminRegistrationJournalPositions dbModel = AdminModelConverter.GetDbRegistrationJournalPosition(ctx, model);
                dbContext.AdminRegistrationJournalPositionsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.AdminRegistrationJournalPositions, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void AddRegistrationJournalPositions(IContext ctx, List<InternalRegistrationJournalPosition> list)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var items = AdminModelConverter.GetDbRegistrationJournalPositions(ctx, list);
                var res = dbContext.AdminRegistrationJournalPositionsSet.AddRange(items);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, res.Select(x => x.Id).ToList(), EnumObjects.AdminRegistrationJournalPositions, EnumOperationType.AddNew);
                transaction.Complete();
            }
        }

        public void DeleteRegistrationJournalPositions(IContext ctx, FilterAdminRegistrationJournalPosition filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalPositionQuery(ctx, dbContext, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.AdminRegistrationJournalPositions, EnumOperationType.Delete);
                qry.Delete();
                transaction.Complete();
            }
        }

        public InternalRegistrationJournalPosition GetInternalRegistrationJournalPosition(IContext ctx, FilterAdminRegistrationJournalPosition filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalPositionQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new InternalRegistrationJournalPosition
                {
                    Id = x.Id,
                    PositionId = x.PositionId,
                    RegistrationJournalId = x.RegJournalId,
                    RegJournalAccessTypeId = x.RegJournalAccessTypeId,
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalRegistrationJournalPosition> GetInternalRegistrationJournalPositions(IContext ctx, FilterAdminRegistrationJournalPosition filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalPositionQuery(ctx, dbContext, filter);

                //qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new InternalRegistrationJournalPosition
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

        public bool ExistsRegistrationJournalPosition(IContext ctx, FilterAdminRegistrationJournalPosition filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalPositionQuery(ctx, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<AdminRegistrationJournalPositions> GetRegistrationJournalPositionQuery(IContext ctx, DmsContext dbContext, FilterAdminRegistrationJournalPosition filter)
        {
            var qry = dbContext.AdminRegistrationJournalPositionsSet.Where(x => x.RegistrationJournal.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRegistrationJournalPositions>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRegistrationJournalPositions>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRegistrationJournalPositions>(false);

                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.RegistrationJournalIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRegistrationJournalPositions>(false);

                    filterContains = filter.RegistrationJournalIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RegJournalId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.RegistrationJournalAccessTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRegistrationJournalPositions>(false);

                    filterContains = filter.RegistrationJournalAccessTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RegJournalAccessTypeId == (int)value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        #endregion

        public int AddRolePermission(IContext ctx, InternalAdminRolePermission model)
        {
            int res;
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbRolePermission(ctx, model);
                dbContext.AdminRolePermissionsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                res = dbModel.Id;
            }
            _cacheService.RefreshKey(ctx, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY);
            return res;
        }

        public void AddRolePermissions(IContext ctx, IEnumerable<AdminRolePermissions> models)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                //var dbModels = AdminModelConverter.GetDbRolePermissions(ctx, models);
                dbContext.AdminRolePermissionsSet.AddRange(models);
                dbContext.SaveChanges();
                transaction.Complete();
            }
            _cacheService.RefreshKey(ctx, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY);
        }


        public void DeleteRolePermissions(IContext ctx, FilterAdminRolePermissions filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolePermissionsQuery(ctx, dbContext, filter);
                qry.Delete();
                transaction.Complete();
            }
            _cacheService.RefreshKey(ctx, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY);
        }


        private IQueryable<AdminRolePermissions> GetRolePermissionsQuery(IContext ctx, DmsContext dbContext, FilterAdminRolePermissions filter)
        { //TODO change to cashe
            var qry = dbContext.AdminRolePermissionsSet.Where(x => x.Role.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRolePermissions>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRolePermissions>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.RoleIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRolePermissions>(false);

                    filterContains = filter.RoleIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RoleId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PermissionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminRolePermissions>(false);

                    filterContains = filter.PermissionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PermissionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }

        public bool ExistsRolePermissions(IContext ctx, FilterAdminRolePermissions filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolePermissionsQuery(ctx, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }


        private void VerifyAdminSecurityCash(IContext ctx)
        {
            var admCtx = new AdminContext(ctx);
            var dbCtx = admCtx.DbContext as DmsContext;
            if (!_cacheService.Exists(ctx, SettingConstants.PERMISSION_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(ctx, SettingConstants.PERMISSION_CASHE_KEY, () =>
                {

                    return dbCtx.SystemPermissionsSet.Select(x => new InternalSystemPermission
                    {
                        Id = x.Id,
                        AccessTypeId = x.AccessTypeId,
                        FeatureCode = x.Feature.Code,
                        FeatureId = x.FeatureId,
                        FeatureOrder = x.Feature.Order,
                        ModuleCode = x.Module.Code,
                        ModuleId = x.ModuleId,
                        ModuleOrder = x.Module.Order,
                        AccessTypeCode = x.AccessType.Code,
                        AccessTypeOrder = x.AccessType.Order
                    }).ToList();

                });
            }

            if (!_cacheService.Exists(ctx, SettingConstants.ACTION_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(ctx, SettingConstants.ACTION_CASHE_KEY, () =>
                {

                    return dbCtx.SystemActionsSet.Select(x => new InternalSystemAction
                    {
                        Id = x.Id,
                        Description = x.Description,
                        PermissionId = x.PermissionId,
                        Code = x.Code,
                        Category = x.Category,
                        ObjectId = (EnumObjects)x.ObjectId
                    }).ToList();
                });
            }

            if (!_cacheService.Exists(ctx, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(ctx, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY, () =>
                {

                    return dbCtx.AdminRolePermissionsSet.Select(x => new InternalAdminRolePermission
                    {
                        Id = x.Id,
                        RoleId = x.RoleId,
                        PermissionId = x.PermissionId,
                        LastChangeDate = x.LastChangeDate,
                        LastChangeUserId = x.LastChangeUserId
                    }).ToList();

                });
            }

            if (!_cacheService.Exists(ctx, SettingConstants.ADMIN_ROLE_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(ctx, SettingConstants.ADMIN_ROLE_CASHE_KEY, () =>
                {

                    return dbCtx.AdminRolesSet.Select(x => new InternalAdminRole
                    {
                        Id = x.Id,
                        LastChangeDate = x.LastChangeDate,
                        LastChangeUserId = x.LastChangeUserId,
                        Name = x.Name,
                        Description = x.Description,
                        RoleTypeId = x.RoleTypeId
                    }).ToList();

                });
            }
            if (!_cacheService.Exists(ctx, SettingConstants.USER_ROLE_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(ctx, SettingConstants.USER_ROLE_CASHE_KEY, () =>
                {

                    return dbCtx.AdminUserRolesSet.Select(x => new InternalAdminUserRole
                    {
                        Id = x.Id,
                        RoleId = x.RoleId,
                        PositionExecutorId = x.PositionExecutorId
                    }).ToList();

                });
            }
            if (!_cacheService.Exists(ctx, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(ctx, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY, () =>
                {

                    return dbCtx.DictionaryPositionExecutorsSet.Select(x => new InternalDictionaryPositionExecutor
                    {
                        Id = x.Id,
                        Description = x.Description,
                        AccessLevelId = x.AccessLevelId,
                        EndDate = x.EndDate,
                        AgentId = x.AgentId,
                        PositionId = x.PositionId,
                        IsActive = x.IsActive,
                        StartDate = x.StartDate,
                        PositionExecutorTypeId = x.PositionExecutorTypeId
                    }).ToList();

                });
            }
        }

        private List<InternalSystemPermission> GetPermissionsAccessQuery(IContext ctx, FilterPermissionsAccess filter)
        {
            if (filter == null)
                return null;
            VerifyAdminSecurityCash(ctx);

            var permiss = _cacheService.GetData(ctx, SettingConstants.PERMISSION_CASHE_KEY) as List<InternalSystemPermission>;
            var adminPermiss = _cacheService.GetData(ctx, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY) as List<InternalAdminRolePermission>;
            var adminRole = _cacheService.GetData(ctx, SettingConstants.ADMIN_ROLE_CASHE_KEY) as List<InternalAdminRole>;
            var userRole = _cacheService.GetData(ctx, SettingConstants.USER_ROLE_CASHE_KEY) as List<InternalAdminUserRole>;
            var dictPos = _cacheService.GetData(ctx, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY) as List<InternalDictionaryPositionExecutor>;

            if (permiss == null || adminPermiss == null || adminRole == null || userRole == null || dictPos == null) throw new KeyNotFoundException();

            var fltRoles = userRole.AsQueryable();
            if (filter.RoleIDs != null && filter.RoleIDs.Any())
            {
                fltRoles = fltRoles.Where(x => filter.RoleIDs.Contains(x.RoleId));
            }

            var now = DateTime.UtcNow;

            var dictFlt = dictPos.Where(x => x.AgentId == filter.UserId && x.IsActive && now >= x.StartDate && now <= x.EndDate);
            if (filter.PositionsIdList != null && filter.PositionsIdList.Any())
            {
                dictFlt = dictFlt.Where(x => filter.PositionsIdList.Contains(x.PositionId));
            }

            var fltPermiss = permiss.AsQueryable();
            if (filter.PermissionIDs != null && filter.PermissionIDs.Any())
            {
                fltPermiss = fltPermiss.Where(x => filter.PermissionIDs.Contains(x.Id));
            }
            if (filter.ModuleId.HasValue)
            {
                fltPermiss = fltPermiss.Where(x => x.ModuleId == filter.ModuleId.Value);
            }

            var qry = fltPermiss.Join(adminPermiss, p => p.Id, a => a.PermissionId, (p, a) => new { perm = p, admPerm = a })
                .Join(adminRole, p => p.admPerm.RoleId, r => r.Id, (a, r) => new { a.perm, a.admPerm, admRole = r })
                .Join(fltRoles, a => a.admRole.Id, u => u.RoleId, (a, u) => new { a.perm, a.admPerm, a.admRole, usrRole = u })
                .Join(dictFlt, a => a.usrRole.PositionExecutorId, d => d.Id, (a, d) => new { a.perm, a.admPerm, a.admRole, a.usrRole, dPos = d });

            if (filter.ActionId.HasValue)
            {
                var act = _cacheService.GetData(ctx, SettingConstants.ACTION_CASHE_KEY) as List<InternalSystemAction>;
                if (act == null) throw new KeyNotFoundException();
                var fltAct = act.Where(x => x.Id == filter.ActionId.Value && x.PermissionId.HasValue).Select(x => x.PermissionId).Distinct().ToList();
                qry.Where(x => fltAct.Contains(x.perm.Id));
            }

            return qry.Select(x => x.perm).Distinct().ToList();
        }

        public bool ExistsPermissionsAccess(IContext ctx, FilterPermissionsAccess filter)
        {

            var qry = GetPermissionsAccessQuery(ctx, filter);

            return qry.Any();
        }

        public IEnumerable<FrontPermission> GetUserPermissionsAccess(IContext ctx, FilterPermissionsAccess filter)
        {
            var qry = GetPermissionsAccessQuery(ctx, filter);

            qry = qry.OrderBy(x => x.FeatureOrder).ThenBy(x => x.AccessTypeOrder).ToList();

            return qry.Select(x => new FrontPermission
            {
                Module = x.ModuleCode,
                Feature = x.FeatureCode,
                AccessType = x.AccessTypeCode
            }).ToList();

        }

        public IEnumerable<FrontModule> GetRolePermissions(IContext ctx, FilterAdminRolePermissionsDIP filter)
        {
            var permissions = GetInternalPermissionsByRole(ctx, filter);

            var res = permissions.GroupBy(x => new { x.ModuleCode, x.ModuleName, x.ModuleOrder })
                 .OrderBy(x => x.Key.ModuleOrder)
                 .Select(x => new FrontModule()
                 {
                     Module = x.Key.ModuleCode,
                     Name = x.Key.ModuleName,
                     Features = x.GroupBy(y => new { y.FeatureCode, y.FeatureName, y.FeatureOrder, y.ModuleCode })
                     .OrderBy(y => y.Key.FeatureOrder)
                     .Select(y => new FrontFeature
                     {
                         Feature = y.Key.FeatureCode,
                         Module = y.Key.ModuleCode,
                         Name = y.Key.FeatureName,
                         Order = y.Key.FeatureOrder,
                         Read = new FrontPermissionValue
                         {
                             Module = y.Any(z => z.AccessTypeId == EnumAccessTypes.R) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.R).FirstOrDefault().ModuleCode : string.Empty,
                             Feature = y.Any(z => z.AccessTypeId == EnumAccessTypes.R) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.R).FirstOrDefault().FeatureCode : string.Empty,
                             AccessType = y.Any(z => z.AccessTypeId == EnumAccessTypes.R) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.R).FirstOrDefault().AccessTypeCode : string.Empty,
                             Value = y.Any(z => z.AccessTypeId == EnumAccessTypes.R) ? (y.Any(z => z.AccessTypeId == EnumAccessTypes.R && z.IsChecked) ? EnumAccessTypesValue.Cheched : EnumAccessTypesValue.Uncheched) : EnumAccessTypesValue.Undefined
                         },
                         Create = new FrontPermissionValue
                         {
                             Module = y.Any(z => z.AccessTypeId == EnumAccessTypes.C) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.C).FirstOrDefault().ModuleCode : string.Empty,
                             Feature = y.Any(z => z.AccessTypeId == EnumAccessTypes.C) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.C).FirstOrDefault().FeatureCode : string.Empty,
                             AccessType = y.Any(z => z.AccessTypeId == EnumAccessTypes.C) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.C).FirstOrDefault().AccessTypeCode : string.Empty,
                             Value = y.Any(z => z.AccessTypeId == EnumAccessTypes.C) ? (y.Any(z => z.AccessTypeId == EnumAccessTypes.C && z.IsChecked) ? EnumAccessTypesValue.Cheched : EnumAccessTypesValue.Uncheched) : EnumAccessTypesValue.Undefined
                         },
                         Update = new FrontPermissionValue
                         {

                             Module = y.Any(z => z.AccessTypeId == EnumAccessTypes.U) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.U).FirstOrDefault().ModuleCode : string.Empty,
                             Feature = y.Any(z => z.AccessTypeId == EnumAccessTypes.U) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.U).FirstOrDefault().FeatureCode : string.Empty,
                             AccessType = y.Any(z => z.AccessTypeId == EnumAccessTypes.U) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.U).FirstOrDefault().AccessTypeCode : string.Empty,
                             Value = y.Any(z => z.AccessTypeId == EnumAccessTypes.U) ? (y.Any(z => z.AccessTypeId == EnumAccessTypes.U && z.IsChecked) ? EnumAccessTypesValue.Cheched : EnumAccessTypesValue.Uncheched) : EnumAccessTypesValue.Undefined
                         },
                         Delete = new FrontPermissionValue
                         {
                             Module = y.Any(z => z.AccessTypeId == EnumAccessTypes.D) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.D).FirstOrDefault().ModuleCode : string.Empty,
                             Feature = y.Any(z => z.AccessTypeId == EnumAccessTypes.D) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.D).FirstOrDefault().FeatureCode : string.Empty,
                             AccessType = y.Any(z => z.AccessTypeId == EnumAccessTypes.D) ? y.Where(z => z.AccessTypeId == EnumAccessTypes.D).FirstOrDefault().AccessTypeCode : string.Empty,
                             Value = y.Any(z => z.AccessTypeId == EnumAccessTypes.D) ? (y.Any(z => z.AccessTypeId == EnumAccessTypes.D && z.IsChecked) ? EnumAccessTypesValue.Cheched : EnumAccessTypesValue.Uncheched) : EnumAccessTypesValue.Undefined
                         },
                     }).ToList()
                 });


            return res;
        }

        private IEnumerable<InternalPermissions> GetInternalPermissionsByRole(IContext ctx, FilterAdminRolePermissionsDIP filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.SystemPermissionsSet.AsQueryable();

                if (!string.IsNullOrEmpty(filter.Module))
                {
                    qry = qry.Where(x => x.Module.Code == filter.Module);
                }

                if (!string.IsNullOrEmpty(filter.Feature))
                {
                    qry = qry.Where(x => x.Feature.Code == filter.Feature);
                }

                if (filter.IsChecked)
                {
                    qry = qry.Where(x => x.RolePermissions.Any(y => y.RoleId == filter.RoleId));
                }

                qry = qry.OrderBy(x => x.Module.Order).ThenBy(x => x.Feature.Order).ThenBy(x => x.AccessType.Order);

                var res = qry.Select(x => new InternalPermissions
                {
                    Id = x.Id,

                    AccessTypeId = (EnumAccessTypes)x.AccessTypeId,
                    AccessTypeCode = x.AccessType.Code,
                    AccessTypeName = x.AccessType.Name,
                    AccessTypeOrder = x.AccessType.Order,

                    ModuleId = x.Feature.Module.Id,
                    ModuleCode = x.Feature.Module.Code,
                    ModuleName = x.Feature.Module.Name,
                    ModuleOrder = x.Feature.Module.Order,

                    FeatureId = x.FeatureId,
                    FeatureCode = x.Feature.Code,
                    FeatureName = x.Feature.Name,
                    FeatureOrder = x.Feature.Order,

                    IsChecked = x.RolePermissions.Any(y => y.RoleId == filter.RoleId),
                }).ToList();

                transaction.Complete();
                return res;
            }
        }



        #region [+] AddNewClient ...

        public List<InternalAdminRolePermission> GetRolePermissionsForAdmin(IContext ctx)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.SystemPermissionsSet.Select(x => new InternalAdminRolePermission { PermissionId = x.Id }).ToList();

                transaction.Complete();

                return res;
            }
        }

        #endregion

    }
}
