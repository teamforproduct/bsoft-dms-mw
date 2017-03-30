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
        public AdminAccessInfo GetAdminAccesses(IContext context)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = new AdminAccessInfo();

                res.UserRoles = dbContext.AdminUserRolesSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalAdminUserRole
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    AgentId = x.PositionExecutor.AgentId
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
                    PermissionId = x.PermissionId,
                    ObjectId = (EnumObjects)x.ObjectId,
                }).ToList();

                res.RolePermissions = dbContext.AdminRolePermissionsSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalAdminRolePermission
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    PermissionId = x.PermissionId
                }).ToList();

                // pss Таблица подлежит удалению - пустая
                //res.ActionAccess = dbContext.AdminRoleActionsSet.Where(x => x.Role.ClientId == context.CurrentClientId).Select(x => new InternalAdminRoleAction
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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryPositionExecutorsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).AsQueryable();

                var now = DateTime.UtcNow;
                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

                if (PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();

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
                var filterAccessPositionContains = PredicateBuilder.False<DocumentAccesses>();
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


                //var filterNewEventTargetPositionContains = PredicateBuilder.False<DBModel.Document.DocumentEvents>();
                //filterNewEventTargetPositionContains = positions.Aggregate(filterNewEventTargetPositionContains,
                //    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                //var neweventQry = dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)   //TODO include doc access
                //                .Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId)
                //                .Where(filterNewEventTargetPositionContains)
                //                .GroupBy(g => g.TargetPositionId)
                //                .Select(s => new { PosID = s.Key, EvnCnt = s.Count() });
                //var newevnt = neweventQry.ToList();

                //var filterOnEventPositionsContains = PredicateBuilder.False<DocumentWaits>();
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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryPositionExecutorsSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).AsQueryable();

                var now = DateTime.UtcNow;
                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

                if (PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();

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
                var filterAccessPositionContains = PredicateBuilder.False<DocumentAccesses>();
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

                //var filterNewEventTargetPositionContains = PredicateBuilder.False<DBModel.Document.DocumentEvents>();
                //filterNewEventTargetPositionContains = positionList.Aggregate(filterNewEventTargetPositionContains,
                //    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                //var neweventQry = dbContext.DocumentEventsSet.Where(x => x.ClientId == ctx.CurrentClientId)   //TODO include doc access
                //                .Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId)
                //                .Where(filterNewEventTargetPositionContains)
                //                .GroupBy(g => g.TargetPositionId)
                //                .Select(s => new { PosID = s.Key, EvnCnt = s.Count() });
                //var newevnt = neweventQry.ToList();

                //var filterOnEventPositionsContains = PredicateBuilder.False<DocumentWaits>();
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

        public Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
            if (model.SourcePositions.Contains(model.TargetPosition))
                return true;
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dictDb = DmsResolver.Current.Get<DictionariesDbProcess>();
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

        public Employee GetEmployeeForContext(IContext ctx, string userId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbRoleType(context, model);
                dbContext.AdminRolesTypesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public int AddRole(IContext context, InternalAdminRole model)
        {
            int res;
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbRole(context, model);
                dbContext.AdminRolesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                res = dbModel.Id;
            }
            _cacheService.RefreshKey(context, SettingConstants.ADMIN_ROLE_CASHE_KEY);
            return res;
        }

        public void UpdateRole(IContext context, InternalAdminRole model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                AdminRoles dbModel = AdminModelConverter.GetDbRole(context, model);
                dbContext.AdminRolesSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
            _cacheService.RefreshKey(context, SettingConstants.ADMIN_ROLE_CASHE_KEY);
        }

        public void DeleteRole(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.AdminRolePermissionsSet.Where(x => x.RoleId == id).Delete();
                dbContext.AdminPositionRolesSet.Where(x => x.RoleId == id).Delete();
                dbContext.AdminUserRolesSet.Where(x => x.RoleId == id).Delete();

                dbContext.AdminRolesSet.Where(x => x.Id == id).Delete();
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalAdminRole GetInternalRole(IContext context, FilterAdminRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(context, dbContext, filter);

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

        public IEnumerable<ListItem> GetMainRoles(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(context, dbContext, filter as FilterAdminRole);

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

        public IEnumerable<ListItem> GetListRoles(IContext context, FilterAdminRole filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(context, dbContext, filter);

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
        public List<int> GetRoleIDs(IContext context, IBaseFilter filter, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(context, dbContext, filter as FilterAdminRole);

                qry = qry.OrderBy(x => x.Name);

                //if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<int>();

                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontAdminRole> GetRoles(IContext context, FilterAdminRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(context, dbContext, filter);

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

        public string GetRoleTypeCode(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.AdminRolesSet.
                    Where(x => x.ClientId == context.CurrentClientId).
                    Where(x => x.Id == id).
                    AsQueryable();

                var res = qry.Select(x => x.RoleType.Code).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int GetRoleByCode(IContext context, Roles item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                // Для заводских ролей отношение к типам ролей 1:1
                var qry = dbContext.AdminRolesSet.
                    Where(x => x.ClientId == context.CurrentClientId).
                    Where(x => x.RoleType.Code == item.ToString()).
                    AsQueryable();

                var res = qry.Select(x => x.Id).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public bool ExistsRole(IContext context, FilterAdminRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolesQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontAdminRole
                {
                    Id = x.Id
                }).FirstOrDefault();
                transaction.Complete();
                return res != null;
            }
        }

        private IQueryable<AdminRoles> GetRolesQuery(IContext context, DmsContext dbContext, FilterAdminRole filter)
        {
            var qry = dbContext.AdminRolesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminPositionRoles>();
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
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

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
                        (current, value) => current.Or(e => e.Description.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }
        #endregion


        #region [+] PositionRole ...
        public int AddPositionRole(IContext context, InternalAdminPositionRole model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                AdminPositionRoles dbModel = AdminModelConverter.GetDbPositionRole(context, model);
                dbContext.AdminPositionRolesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }
        public void DeletePositionRole(IContext context, InternalAdminPositionRole model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                // по полям RoleId и PositionId соблюдается уникальность, поэтому запись идентифицируется правильно (всегда одна)
                var dbModel = dbContext.AdminPositionRolesSet.Where(x => x.RoleId == model.RoleId).Where(x => x.PositionId == model.PositionId).FirstOrDefault();
                dbContext.AdminPositionRolesSet.Remove(dbModel);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeletePositionRoles(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(context, dbContext, filter);
                dbContext.AdminPositionRolesSet.RemoveRange(qry);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalAdminPositionRole GetInternalPositionRole(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(context, dbContext, filter);

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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.AdminRolesSet.Where(x => x.Id == id).Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                var res = qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    RoleName = x.Name,
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalAdminPositionRole> GetInternalPositionRoles(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(context, dbContext, filter);

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
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(context, dbContext, filter);

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

        public IEnumerable<FrontAdminPositionRole> GetPositionRolesDIP(IContext context, FilterAdminPositionRoleDIP filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var roleFilter = new FilterAdminRole();

                if (filter.IsChecked ?? false)
                {
                    roleFilter.PositionIDs = filter.PositionIDs;
                }


                var qry = GetRolesQuery(context, dbContext, roleFilter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontAdminPositionRole
                {
                    Id = x.Id,
                    RoleId = x.Id,
                    RoleName = x.Name,
                    IsChecked = x.PositionRoles.Where(y => y.RoleId == x.Id).Where(y => filter.PositionIDs.Contains(y.PositionId)).Any()
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public bool ExistsPositionRole(IContext context, FilterAdminPositionRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAdminPositionRoleQuery(context, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<AdminPositionRoles> GetAdminPositionRoleQuery(IContext context, DmsContext dbContext, FilterAdminPositionRole filter)
        {
            var qry = dbContext.AdminPositionRolesSet.AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
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
            }

            return qry;
        }
        #endregion

        #region [+] UserRole ...
        public int AddUserRole(IContext context, InternalAdminUserRole model)
        {
            int res;
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbUserRole(context, model);
                dbContext.AdminUserRolesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                res = dbModel.Id;
            }
            _cacheService.RefreshKey(context, SettingConstants.USER_ROLE_CASHE_KEY);
            return res;
        }

        public void AddUserRoles(IContext context, IEnumerable<InternalAdminUserRole> models)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModels = AdminModelConverter.GetDbUserRoles(context, models);
                dbContext.AdminUserRolesSet.AddRange(dbModels);
                dbContext.SaveChanges();
                transaction.Complete();
            }
            _cacheService.RefreshKey(context, SettingConstants.USER_ROLE_CASHE_KEY);
        }

        public void UpdateUserRole(IContext context, InternalAdminUserRole model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                AdminUserRoles dbModel = AdminModelConverter.GetDbUserRole(context, model);
                dbContext.AdminUserRolesSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
            _cacheService.RefreshKey(context, SettingConstants.USER_ROLE_CASHE_KEY);
        }

        public void DeleteUserRole(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = dbContext.AdminUserRolesSet.FirstOrDefault(x => x.Id == id);
                dbContext.AdminUserRolesSet.Remove(dbModel);
                dbContext.SaveChanges();
                transaction.Complete();
            }
            _cacheService.RefreshKey(context, SettingConstants.USER_ROLE_CASHE_KEY);
        }
        public void DeleteUserRoles(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserRolesQuery(context, dbContext, filter);
                qry.Delete();
                //if (qry.Count() == 0) return;
                //dbContext.AdminUserRolesSet.RemoveRange(qry);
                //dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalAdminUserRole> GetInternalUserRoles(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

                var qry = GetUserRolesQuery(context, dbContext, filter);

                var res = qry.Select(x => new InternalAdminUserRole
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    PositionExecutorId = x.PositionExecutorId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,

                    PositionId = x.PositionExecutor.PositionId,
                    StartDate = x.PositionExecutor.StartDate,
                    EndDate = x.PositionExecutor.EndDate > maxDateTime ? (DateTime?)null : x.PositionExecutor.EndDate,
                    AgentId = x.PositionExecutor.AgentId,

                }).ToList();

                transaction.Complete();

                return res;
            }
        }


        public List<int> GetRolesByUsers(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserRolesQuery(context, dbContext, filter);

                var res = qry.Select(x => x.RoleId).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<FrontAdminUserRole> GetUserRoles(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserRolesQuery(context, dbContext, filter);

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

        public bool ExistsUserRole(IContext context, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetUserRolesQuery(context, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<AdminUserRoles> GetUserRolesQuery(IContext context, DmsContext dbContext, FilterAdminUserRole filter)
        {
            var qry = dbContext.AdminUserRolesSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
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
                        (current, value) => current.Or(e => e.PositionExecutor.AgentId == value).Expand());

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

        public int AddDepartmentAdmin(IContext context, InternalDepartmentAdmin model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbEmployeeDepartments(context, model);
                dbContext.AdminEmployeeDepartmentsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }

        }

        public void DeleteDepartmentAdmin(IContext context, FilterAdminEmployeeDepartments filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetEmployeeDepartmentsQuery(context, dbContext, filter);
                qry.Delete();

                transaction.Complete();
            }
        }

        public IEnumerable<FrontAdminEmployeeDepartments> GetDepartmentAdmins(IContext context, int departmentId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetEmployeeDepartmentsQuery(context, dbContext, new FilterAdminEmployeeDepartments { DepartmentIDs = new List<int> { departmentId } });

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

        private IQueryable<AdminEmployeeDepartments> GetEmployeeDepartmentsQuery(IContext context, DmsContext dbContext, FilterAdminEmployeeDepartments filter)
        {
            var qry = dbContext.AdminEmployeeDepartmentsSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminEmployeeDepartments>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<AdminEmployeeDepartments>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DepartmentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminEmployeeDepartments>();

                    filterContains = filter.DepartmentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }


        #endregion

        #region [+] Subordination ...
        public int AddSubordination(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                AdminSubordinations dbModel = AdminModelConverter.GetDbSubordination(context, model);
                dbContext.AdminSubordinationsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void AddSubordinations(IContext context, List<InternalAdminSubordination> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var items = AdminModelConverter.GetDbSubordinations(context, list);
                dbContext.AdminSubordinationsSet.AddRange(items);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateSubordination(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                AdminSubordinations dbModel = AdminModelConverter.GetDbSubordination(context, model);
                dbContext.AdminSubordinationsSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public void DeleteSubordination(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public void DeleteSubordinationsBySourcePositionId(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var list = dbContext.AdminSubordinationsSet.Where(x => x.SourcePositionId == model.SourcePositionId);
                dbContext.AdminSubordinationsSet.RemoveRange(list);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteSubordinationsByTargetPositionId(IContext context, InternalAdminSubordination model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var list = dbContext.AdminSubordinationsSet.Where(x => x.TargetPositionId == model.TargetPositionId);
                dbContext.AdminSubordinationsSet.RemoveRange(list);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteSubordinations(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(context, dbContext, filter);
                qry.Delete();
                //var e = qry.ToList();
                //dbContext.AdminSubordinationsSet.RemoveRange(qry);
                //dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalAdminSubordination GetInternalSubordination(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(context, dbContext, filter);

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


        public IEnumerable<FrontAdminSubordination> GetSubordinations(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(context, dbContext, filter);

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

        public IEnumerable<InternalAdminSubordination> GetInternalSubordinations(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(context, dbContext, filter);

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

        public List<int> GetSubordinationTargetIDs(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(context, dbContext, filter);

                //qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.TargetPositionId).ToList();

                transaction.Complete();

                return res;
            }
        }

        public bool ExistsSubordination(IContext context, FilterAdminSubordination filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetSubordinationsQuery(context, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<AdminSubordinations> GetSubordinationsQuery(IContext context, DmsContext dbContext, FilterAdminSubordination filter)
        {
            var qry = dbContext.AdminSubordinationsSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
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
            }

            return qry;
        }

        #endregion

        #region [+] RegistrationJournalPositions ...
        public int AddRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                AdminRegistrationJournalPositions dbModel = AdminModelConverter.GetDbRegistrationJournalPosition(context, model);
                dbContext.AdminRegistrationJournalPositionsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void AddRegistrationJournalPositions(IContext context, List<InternalRegistrationJournalPosition> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var items = AdminModelConverter.GetDbRegistrationJournalPositions(context, list);
                dbContext.AdminRegistrationJournalPositionsSet.AddRange(items);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                AdminRegistrationJournalPositions dbModel = AdminModelConverter.GetDbRegistrationJournalPosition(context, model);
                dbContext.AdminRegistrationJournalPositionsSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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
                transaction.Complete();
            }
        }

        public void DeleteRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalPositionQuery(context, dbContext, filter);
                dbContext.AdminRegistrationJournalPositionsSet.RemoveRange(qry);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalRegistrationJournalPosition GetInternalRegistrationJournalPosition(IContext context, FilterAdminRegistrationJournalPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalPositionQuery(context, dbContext, filter);

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

        public IEnumerable<InternalRegistrationJournalPosition> GetInternalRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalPositionQuery(context, dbContext, filter);

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


        //public IEnumerable<FrontAdminRegistrationJournalPosition> GetRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    //using (var transaction = Transactions.GetTransaction())
        //    {
        //        var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();

        //        qry = GetWhereRegistrationJournalPosition(qry, filter);

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
        //    //using (var transaction = Transactions.GetTransaction())
        //    {
        //        var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();

        //        qry = GetWhereRegistrationJournalPosition(qry, filter);

        //        //qry = qry.OrderBy(x => x.Name);

        //        return qry.Select(x => x.TargetPositionId).ToList();
        //    }
        //}

        public bool ExistsRegistrationJournalPosition(IContext context, FilterAdminRegistrationJournalPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalPositionQuery(context, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }

        private IQueryable<AdminRegistrationJournalPositions> GetRegistrationJournalPositionQuery(IContext context, DmsContext dbContext, FilterAdminRegistrationJournalPosition filter)
        {
            var qry = dbContext.AdminRegistrationJournalPositionsSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
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
            }

            return qry;
        }

        #endregion

        public int AddRolePermission(IContext context, InternalAdminRolePermission model)
        {
            int res;
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = AdminModelConverter.GetDbRolePermission(context, model);
                dbContext.AdminRolePermissionsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                res = dbModel.Id;
            }
            _cacheService.RefreshKey(context, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY);
            return res;
        }

        public void AddRolePermissions(IContext context, IEnumerable<InternalAdminRolePermission> models)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModels = AdminModelConverter.GetDbRolePermissions(context, models);
                dbContext.AdminRolePermissionsSet.AddRange(dbModels);
                dbContext.SaveChanges();
                transaction.Complete();
            }
            _cacheService.RefreshKey(context, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY);
        }


        public void DeleteRolePermission(IContext context, InternalAdminRolePermission model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                AdminRolePermissions dbModel = null;
                if (model.Id == 0)
                {
                    dbModel = dbContext.AdminRolePermissionsSet.
                        FirstOrDefault(x => x.RoleId == model.RoleId && x.PermissionId == model.PermissionId);
                }
                else
                {
                    dbModel = dbContext.AdminRolePermissionsSet.FirstOrDefault(x => x.Id == model.Id);
                }
                dbContext.AdminRolePermissionsSet.Remove(dbModel);
                dbContext.SaveChanges();
                transaction.Complete();
            }
            _cacheService.RefreshKey(context, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY);
        }


        private IQueryable<AdminRolePermissions> GetRolePermissionsQuery(IContext context, DmsContext dbContext, FilterAdminRolePermissions filter)
        { //TODO change to cashe
            var qry = dbContext.AdminRolePermissionsSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminRolePermissions>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<AdminRolePermissions>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.RoleIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminRolePermissions>();

                    filterContains = filter.RoleIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RoleId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PermissionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminRolePermissions>();

                    filterContains = filter.PermissionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PermissionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }

        public bool ExistsRolePermissions(IContext context, FilterAdminRolePermissions filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRolePermissionsQuery(context, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();

                return res;
            }
        }


        private void VerifyAdminSecurityCash(IContext context)
        {
            var admCtx = new AdminContext(context);
            if (!_cacheService.Exists(context, SettingConstants.PERMISSION_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(context, SettingConstants.PERMISSION_CASHE_KEY, () =>
                {
                    using (var dbCtx = new DmsContext(admCtx))
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
                    }
                });
            }

            if (!_cacheService.Exists(context, SettingConstants.ACTION_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(context, SettingConstants.ACTION_CASHE_KEY, () =>
                {
                    using (var dbCtx = new DmsContext(admCtx))
                    {
                        return dbCtx.SystemActionsSet.Select(x => new InternalSystemAction
                        {
                            Id = x.Id,
                            Description = x.Description,
                            PermissionId = x.PermissionId,
                            Code = x.Code,
                            API = x.API,
                            Category = x.Category,
                            GrantId = x.GrantId,
                            IsVisible = x.IsVisible,
                            IsGrantable = x.IsGrantable,
                            IsGrantableByRecordId = x.IsGrantableByRecordId,
                            IsVisibleInMenu = x.IsVisibleInMenu,
                            ObjectId = (EnumObjects)x.ObjectId
                        }).ToList();
                    }
                });
            }

            if (!_cacheService.Exists(context, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(context, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY, () =>
                {
                    using (var dbCtx = new DmsContext(admCtx))
                    {
                        return dbCtx.AdminRolePermissionsSet.Select(x => new InternalAdminRolePermission
                        {
                            Id = x.Id,
                            RoleId = x.RoleId,
                            PermissionId = x.PermissionId,
                            LastChangeDate = x.LastChangeDate,
                            LastChangeUserId = x.LastChangeUserId
                        }).ToList();
                    }
                });
            }

            if (!_cacheService.Exists(context, SettingConstants.ADMIN_ROLE_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(context, SettingConstants.ADMIN_ROLE_CASHE_KEY, () =>
                {
                    using (var dbCtx = new DmsContext(admCtx))
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
                    }
                });
            }
            if (!_cacheService.Exists(context, SettingConstants.USER_ROLE_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(context, SettingConstants.USER_ROLE_CASHE_KEY, () =>
                {
                    using (var dbCtx = new DmsContext(admCtx))
                    {
                        return dbCtx.AdminUserRolesSet.Select(x => new InternalAdminUserRole
                        {
                            Id = x.Id,
                            RoleId = x.RoleId,
                            PositionExecutorId = x.PositionExecutorId
                        }).ToList();
                    }
                });
            }
            if (!_cacheService.Exists(context, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY))
            {
                _cacheService.AddOrUpdateCasheData(context, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY, () =>
                {
                    using (var dbCtx = new DmsContext(admCtx))
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
                    }
                });
            }
        }

        private List<InternalSystemPermission> GetPermissionsAccessQuery(IContext context, FilterPermissionsAccess filter)
        {
            if (filter == null)
                return null;
            VerifyAdminSecurityCash(context);

            var permiss = _cacheService.GetData(context, SettingConstants.PERMISSION_CASHE_KEY) as List<InternalSystemPermission>;
            var adminPermiss = _cacheService.GetData(context, SettingConstants.PERMISSION_ADMIN_ROLE_CASHE_KEY) as List<InternalAdminRolePermission>;
            var adminRole = _cacheService.GetData(context, SettingConstants.ADMIN_ROLE_CASHE_KEY) as List<InternalAdminRole>;
            var userRole = _cacheService.GetData(context, SettingConstants.USER_ROLE_CASHE_KEY) as List<InternalAdminUserRole>;
            var dictPos = _cacheService.GetData(context, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY) as List<InternalDictionaryPositionExecutor>;

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
                var act = _cacheService.GetData(context, SettingConstants.ACTION_CASHE_KEY) as List<InternalSystemAction>;
                if (act == null) throw new KeyNotFoundException();
                var fltAct = act.Where(x => x.Id == filter.ActionId.Value && x.PermissionId.HasValue).Select(x => x.PermissionId).Distinct().ToList();
                qry.Where(x => fltAct.Contains(x.perm.Id));
            }

            return qry.Select(x => x.perm).Distinct().ToList();
        }

        public bool ExistsPermissionsAccess(IContext context, FilterPermissionsAccess filter)
        {

            var qry = GetPermissionsAccessQuery(context, filter);

            return qry.Any();
        }

        public IEnumerable<FrontPermission> GetUserPermissionsAccess(IContext context, FilterPermissionsAccess filter)
        {
            var qry = GetPermissionsAccessQuery(context, filter);

            qry = qry.OrderBy(x => x.FeatureOrder).ThenBy(x => x.AccessTypeOrder).ToList();

            return qry.Select(x => new FrontPermission
            {
                Module = x.ModuleCode,
                Feature = x.FeatureCode,
                AccessType = x.AccessTypeCode
            }).ToList();

        }

        public IEnumerable<FrontModule> GetRolePermissions(IContext context, FilterAdminRolePermissionsDIP filter)
        {
            var permissions = GetInternalPermissionsByRole(context, filter);

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

        private IEnumerable<InternalPermissions> GetInternalPermissionsByRole(IContext context, FilterAdminRolePermissionsDIP filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
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

        public List<InternalAdminRolePermission> GetRolePermissionsForAdmin(IContext context)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.SystemPermissionsSet.Select(x => new InternalAdminRolePermission { PermissionId = x.Id }).ToList();

                transaction.Complete();

                return res;
            }
        }

        #endregion


    }
}


