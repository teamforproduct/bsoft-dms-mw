﻿using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.Database;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using BL.Model.Tree;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Logic.TreeBuilder;
using BL.Model.DictionaryCore.FrontModel;
using BL.CrossCutting.Extensions;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using System.Transactions;
using BL.Database.Common;
using BL.Model.SystemCore;
using BL.Database.SystemDb;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.Filters;
using BL.CrossCutting.Helpers;

namespace BL.Logic.AdminCore
{
    public class AdminService : IAdminService
    {
        private readonly IAdminsDbProcess _adminDb;
        private readonly IDictionariesDbProcess _dictDb;
        private readonly ISystemDbProcess _systemDb;
        private readonly ICommandService _commandService;

        private const int _MINUTES_TO_UPDATE_INFO = 5;

        private Dictionary<string, StoreInfo> accList;

        public AdminService(IAdminsDbProcess adminDb, IDictionariesDbProcess dictDb, ISystemDbProcess systemDb, ICommandService commandService)
        {
            _adminDb = adminDb;
            _dictDb = dictDb;
            _systemDb = systemDb;
            _commandService = commandService;
            accList = new Dictionary<string, StoreInfo>();
        }

        public object ExecuteAction(EnumAdminActions act, IContext context, object param)
        {
            var cmd = AdminCommandFactory.GetAdminCommand(act, context, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        #region [+] General ...

        private AdminAccessInfo GetAccInfo(IContext context)
        {
            var key = CommonSystemUtilities.GetServerKey(context);
            if (accList.ContainsKey(key))
            {
                var so = accList[key];
                if ((DateTime.UtcNow - so.LastUsage).TotalMinutes > _MINUTES_TO_UPDATE_INFO)
                {
                    var lst = _adminDb.GetAdminAccesses(context);
                    so.StoreObject = lst;
                    so.LastUsage = DateTime.UtcNow;
                    return lst;
                }
                return so.StoreObject as AdminAccessInfo;
            }
            var nlst = _adminDb.GetAdminAccesses(context);
            var nso = new StoreInfo
            {
                LastUsage = DateTime.UtcNow,
                StoreObject = nlst
            };
            accList.Add(key, nso);
            return nlst;
        }
        public Employee GetEmployeeForContext(IContext context, string userId)
        {
            return _adminDb.GetEmployeeForContext(context, userId);
        }

        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return _adminDb.GetPositionsByUser(employee);
        }

        public Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context)
        {
            return _adminDb.GetCurrentPositionsAccessLevel(context);
        }

        /// <summary>
        /// GetAvailablePositions вызывается каждые 36 секунд из фронта
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<FrontAvailablePositions> GetAvailablePositions(IContext context)
        {
            var res = _adminDb.GetAvailablePositions(context, context.CurrentAgentId);

            // Если контекст полностью сформирован, тогда проверяю на вшивость
            if (context.IsFormed)
            {
                // Решено тут синхронизировать context.CurrentPositionsIdList и генерировать исключения если AvailablePositions конфликтуют с CurrentPositionsIdList
                if (res.Count() == 0) throw new UserNotExecuteAnyPosition(context.CurrentEmployee.Name);

                // Проверяю содержатся ли выбранные должности в списке доступных
                foreach (var positionId in context.CurrentPositionsIdList)
                {
                    if (!res.Any(x => x.RolePositionId == positionId)) throw new UserNotExecuteCheckPosition();
                }
            }

            return res;
        }

        public IEnumerable<FrontSystemAction> GetUserActions(IContext ctx)
        {

            var positionExecutors = _dictDb.GetInternalPositionExecutors(ctx, new FilterDictionaryPositionExecutor
            {
                AgentIDs = new List<int> { ctx.CurrentAgentId },
                PositionIDs = ctx.CurrentPositionsIdList,
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
            });

            var roles = _adminDb.GetInternalUserRoles(ctx, new FilterAdminUserRole
            {
                PositionExecutorIDs = positionExecutors.Select(x => x.Id).ToList(),
            });

            var actions = _adminDb.GetActionsByRoles(ctx, new FilterAdminRoleAction
            {
                RoleIDs = roles.Select(x => x.RoleId).ToList()
            });

            var res = _systemDb.GetSystemActions(ctx, new FilterSystemAction
            {
                IDs = actions,
                IsGrantable = true,
                IsVisible = true,
                IsGrantableByRecordId = false,
            });


            return res;
        }
        #endregion

        #region [+] Verify ...

        /// <summary>
        /// Проверка доступа к должностям для текущего пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isThrowExeception"></param>
        /// <param name="context"></param>
        public bool VerifyAccess(IContext context, VerifyAccess model, bool isThrowExeception = true)
        {
            if (context is AdminContext) return true;//Full access to admin. ADMIN IS COOL!!! 

            var data = GetAccInfo(context);
            var res = false;
            if (model.UserId == 0)
            {
                model.UserId = context.CurrentAgentId;
            }
            if (model.PositionsIdList == null || model.PositionsIdList.Count == 0)
            {
                model.PositionsIdList = context.CurrentPositionsIdList;
            }

            if (model.DocumentActionId.HasValue)
            {
                if (model.IsPositionFromContext)
                {
                    model.PositionId = context.CurrentPositionId;
                }

                var actionId = model.DocumentActionId;

                while (!res && actionId.HasValue)
                {
                    var qry = data.ActionAccess
                        .Join(data.Actions, aa => aa.ActionId, ac => ac.Id, (aa, ac) => new { ActAccess = aa, Act = ac })
                        .Join(data.PositionRoles, aa => aa.ActAccess.RoleId, r => r.RoleId, (aa, r) => new { aa.ActAccess, aa.Act, Role = r });
                    //var t = qry.Where(x => x.Act.Id == actionId.Value
                    //                        && data.UserRoles.Where(s => s.RoleId == x.Role.RoleId).Any(y => y.AgentId == model.UserId)
                    //                        //&& x.Role.PositionId == 5516
                    //                        ).ToList();
                    // test it really good!
                    res = qry.Any(x => x.Act.Id == actionId.Value
                        && data.UserRoles.Where(s => s.RoleId == x.Role.RoleId).Any(y => y.AgentId == model.UserId)
                        && (((model.PositionId == null) && (model.PositionsIdList.Contains(x.Role.PositionId))) || (x.Role.PositionId == model.PositionId))
                        && (!x.Act.IsGrantable || (x.Act.IsGrantable && (!x.Act.IsGrantableByRecordId || x.ActAccess.RecordId == 0 || x.ActAccess.RecordId == model.RecordId)))
                        );
                    if (!res)
                    {
                        actionId = data.Actions.Where(x => x.Id == actionId.Value).Select(x => x.GrantId).FirstOrDefault();
                    }
                }
            }
            else
            {
                var qry = data.UserRoles.Join(data.PositionRoles, ur => ur.RoleId, r => r.RoleId, (u, r) => new { URole = u, PR = r });

                res = !model.PositionsIdList.Except(qry.Where(x => x.URole.AgentId == model.UserId).Select(x => x.PR.PositionId)).Any();
            }
            if (!res && isThrowExeception)
            {
                if (model.DocumentActionId == null)
                { throw new AccessIsDenied(); }
                else
                {
                    string actionName = string.Empty;
                    var a = data.Actions.Where(x => x.Id == model.DocumentActionId).FirstOrDefault();

                    if (a != null)
                    {
                        actionName = a.Description;
                    }

                    throw new ActionIsDenied(actionName); //TODO Сергей!!!Как красиво передать string obj, string act, int? id = null в сообщение?
                }
            }
            return res;

        }

        public bool VerifyAccess(IContext context, EnumDocumentActions action, bool isPositionFromContext = true, bool isThrowExeception = true)
        {
            return VerifyAccess(context, new VerifyAccess { DocumentActionId = (int)action, IsPositionFromContext = isPositionFromContext }, isThrowExeception);
        }

        public bool VerifyAccess(IContext context, EnumDictionaryActions action, bool isPositionFromContext = true, bool isThrowExeception = true)
        {
            return VerifyAccess(context, new VerifyAccess { DocumentActionId = (int)action, IsPositionFromContext = isPositionFromContext }, isThrowExeception);
        }

        public bool VerifyAccess(IContext context, EnumEncryptionActions action, bool isPositionFromContext = true, bool isThrowExeception = true)
        {
            return VerifyAccess(context, new VerifyAccess { DocumentActionId = (int)action, IsPositionFromContext = isPositionFromContext }, isThrowExeception);
        }



        public bool VerifyAccess(IContext context, EnumAdminActions action, bool isPositionFromContext = true, bool isThrowExeception = true)
        {
            return VerifyAccess(context, new VerifyAccess { DocumentActionId = (int)action, IsPositionFromContext = isPositionFromContext }, isThrowExeception);
        }

        public bool VerifyAccess(IContext context, EnumSystemActions action, bool isPositionFromContext = true, bool isThrowExeception = true)
        {
            return VerifyAccess(context, new VerifyAccess { DocumentActionId = (int)action, IsPositionFromContext = isPositionFromContext }, isThrowExeception);
        }

        public bool VerifySubordination(IContext context, VerifySubordination model)
        {
            return _adminDb.VerifySubordination(context, model);
        }

        #endregion`

        #region [+] Role ...
        //public FrontAdminPositionRole GetAdminRole(IContext context, int id)
        //{
        //    return _adminDb.GetRole(context, new FilterAdminPositionRole() { IDs = new List<int> { id } }).FirstOrDefault();
        //}

        public IEnumerable<FrontAdminRole> GetAdminRoles(IContext context, FilterAdminRole filter)
        {
            return _adminDb.GetRoles(context, filter);
        }

        public int AddNamedRole(IContext context, string code, string name, IEnumerable<InternalAdminRoleAction> roleActions)
        {
            int roleId = 0;

            using (var transaction = Transactions.GetTransaction())
            {
                var roleType = new InternalAdminRoleType() { Code = code, Name = name };
                CommonDocumentUtilities.SetLastChange(context, roleType);

                // Классификатор роли
                var roleTypeId = _adminDb.AddRoleType(context, roleType);

                var role = new InternalAdminRole() { RoleTypeId = roleTypeId, Name = name };
                CommonDocumentUtilities.SetLastChange(context, role);

                // Новая роль со ссылкой на классификатор ролей.
                roleId = _adminDb.AddRole(context, role);

                var ra = new List<InternalAdminRoleAction>();

                // Указание ид роли для предложенных действий
                foreach (var item in roleActions)
                {
                    ra.Add(new InternalAdminRoleAction() { ActionId = item.ActionId, RoleId = roleId });
                }

                CommonDocumentUtilities.SetLastChange(context, ra);

                _adminDb.AddRoleActions(context, ra);

                transaction.Complete();
            }
            return roleId;
        }
        #endregion

        #region [+] RoleAction ...
        public IEnumerable<FrontAdminRoleAction> GetRoleActions(IContext context, FilterAdminRoleAction filter)
        {
            return _adminDb.GetRoleActions(context, filter);
        }

        #endregion

        #region [+] PositionRoles ...
        public IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminRole filter)
        {
            return _adminDb.GetPositionRolesDIP(context, filter);
        }

        public FrontAdminPositionRole GetPositionRole(IContext context, int id)
        {
            return _adminDb.GetPositionRole(context, id);
        }
        #endregion

        #region [+] UserRoles ...

        public IEnumerable<FrontAdminUserRole> GetUserRoles(IContext context, FilterAdminUserRole filter)
        {
            return _adminDb.GetUserRoles(context, filter);
        }

        public IEnumerable<ITreeItem> GetUserRolesDIP(IContext context, int userId, FilterDIPAdminUserRole filter)
        {
            var flatTree = new List<TreeItem>();

            var executorFilter = new FilterDictionaryPositionExecutor
            { AgentIDs = new List<int> { userId }, StartDate = filter.StartDate, EndDate = filter.EndDate };

            // Если указана должность, нохожу все исполнения должности в указанный период
            if (filter.PositionId != null)
            {
                executorFilter.PositionIDs = new List<int> { filter.PositionId ?? -1 };
            }

            if (filter.PositionExecutorId != null)
            {
                executorFilter.IDs = new List<int> { filter.PositionExecutorId ?? -1 };
            }

            // определяю назначения с ролями должности, которые исполняет сотрудник на указанном отрезке времени
            var executors = _dictDb.GetPositionExecutorsDIPUserRoles(context, executorFilter);

            if (executors.Count() > 0)
            {
                var userRoles = _adminDb.GetInternalUserRoles(context, new FilterAdminUserRole { PositionExecutorIDs = executors.Select(x => x.Id).ToList() });

                foreach (var executor in executors)
                {
                    var e = new FrontDIPUserRolesExecutor()
                    {
                        Id = executor.Id,
                        Name = executor.PositionName,
                        StartDate = executor.StartDate,
                        EndDate = executor.EndDate,
                        ExecutorTypeName = executor.PositionExecutorTypeName,
                        IsActive = executor.IsActive ?? true,
                        ObjectId = (int)EnumObjects.DictionaryPositionExecutors,
                    };

                    var roles = new List<TreeItem>();

                    foreach (var role in executor.PositionRoles)
                    {
                        var r = new FrontDIPUserRolesRoles()
                        {
                            Id = userRoles.Where(x => x.RoleId == role.RoleId & x.PositionExecutorId == e.Id).Select(x => x.Id).FirstOrDefault(),
                            RoleId = role.RoleId ?? -1,
                            PositionExecutorId = e.Id,
                            Name = role.RoleName,
                            IsActive = true,
                            ObjectId = (int)EnumObjects.AdminRoles,
                            IsChecked = userRoles.Where(x => x.RoleId == role.RoleId & x.PositionExecutorId == e.Id).Any(),
                        };

                        if ((filter?.IsChecked == true) & !r.IsChecked) continue;
                        roles.Add(r);
                    }

                    // Если есть отмеченные роли, добавляю должность и роли
                    if (roles?.Count > 0)
                    {
                        flatTree.Add(e);
                        flatTree.AddRange(roles);
                    }
                }

            }
            return flatTree;
        }

        public void AddAllPositionRoleForUser(IContext context, ModifyDictionaryPositionExecutor positionExecutor)
        {
            var positionRoles = _adminDb.GetInternalPositionRoles(context, new FilterAdminPositionRole() { PositionIDs = new List<int> { positionExecutor.PositionId } });

            if (positionRoles.Count() == 0) return;

            var userRoles = new List<InternalAdminUserRole>();

            userRoles = positionRoles.Select(x => new InternalAdminUserRole()
            {
                //UserId = positionExecutor.AgentId,
                RoleId = x.RoleId,
                //PositionId = positionExecutor.PositionId,
                PositionExecutorId = positionExecutor.Id,
                //StartDate = positionExecutor.StartDate,
                //EndDate = positionExecutor.EndDate ?? DateTime.MaxValue,
            }).ToList();

            CommonDocumentUtilities.SetLastChange(context, userRoles);

            _adminDb.AddUserRoles(context, userRoles);
        }

        #endregion

        #region [+] DepartmentAdmin ...
        public IEnumerable<FrontDictionaryAgentEmployee> GetDepartmentAdmins(IContext context, int departmentId)
        {
            return _adminDb.GetDepartmentAdmins(context, departmentId);
        }
        #endregion

        #region [+] Subordination ...
        public IEnumerable<FrontAdminSubordination> GetAdminSubordinations(IContext context, FilterAdminSubordination filter)
        {
            return _adminDb.GetSubordinations(context, filter);
        }

        public IEnumerable<ITreeItem> GetSubordinationsDIP(IContext context, int positionId, FilterAdminSubordinationTree filter)
        {

            int levelCount = filter?.LevelCount ?? 0;
            IEnumerable<TreeItem> positions = null;
            IEnumerable<TreeItem> departments = null;
            IEnumerable<TreeItem> companies = null;

            if (levelCount >= 3 || levelCount == 0)
            {
                //List<int> targetPositions = null;

                //if (filter.IsChecked)
                //{
                //    targetPositions = _adminDb.GetSubordinationTargetIDs(context,
                //        new FilterAdminSubordination()
                //        {
                //            SourcePositionIDs = new List<int> { positionId }
                //        });
                //}

                positions = _dictDb.GetPositionsForDIPSubordinations(context, positionId, new FilterDictionaryPosition()
                {
                    IsActive = filter.IsActive,
                    //IDs = targetPositions
                });
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForDIPSubordinations(context, positionId, new FilterDictionaryDepartment()
                {
                    IsActive = filter.IsActive,
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                companies = _dictDb.GetAgentClientCompaniesForDIPSubordinations(context, positionId, new FilterDictionaryAgentClientCompany()
                {
                    IsActive = filter.IsActive
                });
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (positions != null) flatList.AddRange(positions);
            if (departments != null) flatList.AddRange(departments);

            // для растановки галочек дерево нельзя сужать
            var res = Tree.Get(flatList, new FilterTree() { RemoveEmptyBranchesByObject = new List<EnumObjects> { EnumObjects.DictionaryPositions } });

            // растановка галочек для групп(компании, отделы)
            SetCheckForDepartmentsAndCompaniesDIPSubordinations(res);

            if (filter.IsChecked ?? false)
            {
                var safeList = new List<string>();

                FormSafeListDIPSubordinations(res, safeList, filter);

                if (safeList.Count > 0)
                {
                    flatList.RemoveAll(r => !safeList.Contains(r.TreeId));
                    filter.IsChecked = null;
                    res = Tree.Get(flatList, filter);
                }
                else
                {
                    res = null;
                }
            }
            else
            {
                res = Tree.Get(flatList, filter);
            }

            return Tree.GetList(res);
        }

        private void FormSafeListDIPSubordinations(List<TreeItem> tree, List<string> safeList, FilterTree filter)
        {
            if (tree != null)
            {
                //var existsNameFilter = !string.IsNullOrEmpty(filter.Name);

                //string[] arrName = null;

                //if (existsNameFilter)
                //{ arrName = CommonFilterUtilites.GetWhereExpressions(filter.Name.ToLower()); }

                foreach (var item in tree)
                {
                    var addToSafeList = true;

                    var pos = (FrontDIPSubordinationsBase)item;

                    //if (existsNameFilter & addToSafeList)
                    //{
                    //    // Поиск присходит по специальному полю для поиска
                    //    addToSafeList = (item.SearchText.ToLower().ContainsArray(arrName));
                    //}

                    if (addToSafeList)
                    {
                        addToSafeList = pos.IsExecution > 0 || pos.IsInforming > 0;
                    }

                    if (addToSafeList) safeList.AddRange(item.Path.Split('/'));

                    FormSafeListDIPSubordinations((List<TreeItem>)item.Childs, safeList, filter);
                }
            }
        }

        private void SetCheckForDepartmentsAndCompaniesDIPSubordinations(List<TreeItem> tree)
        {
            if (tree == null) return;

            foreach (var item in tree)
            {
                if (item.IsList ?? true) continue;

                SetCheckForDepartmentsAndCompaniesDIPSubordinations((List<TreeItem>)item.Childs);

                var child = (FrontDIPSubordinationsBase)item;

                int allCount = 0;
                int infCount = 0;
                int excCount = 0;
                bool infGr = false;
                bool excGr = false;

                GetCheckCountDIPSubordinations((List<TreeItem>)child.Childs, out allCount, out infCount, out excCount, out infGr, out excGr);

                if (infGr) { child.IsInforming = 2; }
                else { child.IsInforming = (allCount == infCount) ? 1 : (infCount == 0) ? 0 : 2; }

                if (excGr) { child.IsExecution = 2; }
                else { child.IsExecution = (allCount == excCount) ? 1 : (excCount == 0) ? 0 : 2; }

            }
        }

        private void GetCheckCountDIPSubordinations(List<TreeItem> tree, out int AllCount, out int InfCount, out int ExcCount, out bool InfGr, out bool ExcGr)
        {
            int allCount = 0;
            int infCount = 0;
            int excCount = 0;
            bool infGr = false;
            bool excGr = false;

            foreach (var item in tree)
            {
                var child = (FrontDIPSubordinationsBase)item;

                allCount++;
                if (child.IsInforming > 0) infCount++;
                if (child.IsExecution > 0) excCount++;

                if (child.IsInforming == 2) infGr = true;
                if (child.IsExecution == 2) excGr = true;

            }

            AllCount = allCount;
            InfCount = infCount;
            ExcCount = excCount;
            InfGr = infGr;
            ExcGr = excGr;
        }

        //foreach (var item in tree)
        //{
        //    if (item.IsList) continue;

        //    SetCheckForDepartmentsAndCompanies((List<TreeItem>)item.Childs);

        //    int posCount = 0;
        //    int posInfCount = 0;
        //    int posExcCount = 0;

        //    foreach (var c in item.Childs)
        //    {
        //        var child = (FrontDIPSubordinationsBase)c;

        //        posCount++;
        //        if (child.IsInforming == 1) posInfCount++;
        //        if (child.IsExecution == 1) posExcCount++;
        //    }

        //    (item as FrontDIPSubordinationsBase).IsExecution = (posCount == posExcCount) ? 1 : (posExcCount == 0) ? 0 : 2;
        //    (item as FrontDIPSubordinationsBase).IsInforming = (posCount == posInfCount) ? 1 : (posInfCount == 0) ? 0 : 2;

        //}




        #endregion

        #region [+] RegistrationJournalPosition ...


        public IEnumerable<FrontDIPRegistrationJournalPositions> GetPositionsByJournalDIP(IContext context, int journalId, FilterDictionaryPosition filter)
        {
            var journalPositions = _adminDb.GetInternalRegistrationJournalPositions(context, new FilterAdminRegistrationJournalPosition()
            { RegistrationJournalIDs = new List<int> { journalId } });

            var posList = journalPositions.Select(x => x.PositionId).ToList();

            if (posList.Count == 0) return null;

            if (filter == null) filter = new FilterDictionaryPosition();

            if (filter.IDs?.Count > 0)
            { filter.IDs.AddRange(posList); }
            else
            { filter.IDs = posList; }

            var positions = _dictDb.GetPositionsForDIPRegistrationJournals(context, journalId, filter);

            return positions;
        }

        public IEnumerable<ITreeItem> GetRegistrationJournalPositionsDIP(IContext context, int positionId, FilterTree filter)
        {

            int levelCount = filter?.LevelCount ?? 0;
            IEnumerable<TreeItem> journals = null;
            IEnumerable<TreeItem> departments = null;
            IEnumerable<TreeItem> companies = null;

            if (levelCount >= 3 || levelCount == 0)
            {
                journals = _dictDb.GetRegistrationJournalsForDIPRJournalPositions(context, positionId, new FilterDictionaryRegistrationJournal()
                {
                    IsActive = filter.IsActive,
                });
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForDIPRJournalPositions(context, positionId, new FilterDictionaryDepartment()
                {
                    //IDs = journals.Select(x=>x.d)
                    IsActive = filter.IsActive,
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                companies = _dictDb.GetAgentClientCompaniesForDIPRJournalPositions(context, positionId, new FilterDictionaryAgentClientCompany()
                {
                    IsActive = filter.IsActive
                });
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (journals != null) flatList.AddRange(journals);
            if (departments != null) flatList.AddRange(departments);

            // для растановки галочек дерево нельзя сужать
            var res = Tree.Get(flatList, new FilterTree() { RemoveEmptyBranchesByObject = new List<EnumObjects> { EnumObjects.DictionaryRegistrationJournals } });

            // растановка галочек для групп(компании, отделы)
            SetCheckForDepartmentsAndCompaniesDIPRJournalPositions(res);

            if (filter.IsChecked ?? false)
            {
                var safeList = new List<string>();

                FormSafeListDIPRJournalPositions(res, safeList, filter);

                if (safeList.Count > 0)
                {
                    flatList.RemoveAll(r => !safeList.Contains(r.TreeId));
                    filter.IsChecked = null;
                    res = Tree.Get(flatList, filter);
                }
                else
                {
                    res = null;
                }
            }
            else
            {
                res = Tree.Get(flatList, filter);
            }

            return Tree.GetList(res);
        }

        private void FormSafeListDIPRJournalPositions(List<TreeItem> tree, List<string> safeList, FilterTree filter)
        {
            if (tree != null)
            {
                //var existsNameFilter = !string.IsNullOrEmpty(filter.Name);

                //string[] arrName = null;

                //if (existsNameFilter)
                //{ arrName = CommonFilterUtilites.GetWhereExpressions(filter.Name.ToLower()); }

                foreach (var item in tree)
                {
                    var addToSafeList = true;

                    var pos = (FrontDIPRegistrationJournalPositionsBase)item;

                    //if (existsNameFilter & addToSafeList)
                    //{
                    //    // Поиск присходит по специальному полю для поиска
                    //    addToSafeList = (item.SearchText.ToLower().ContainsArray(arrName));
                    //}

                    if (addToSafeList)
                    {
                        addToSafeList = pos.IsViewing > 0 || pos.IsRegistration > 0;
                    }

                    if (addToSafeList) safeList.AddRange(item.Path.Split('/'));

                    FormSafeListDIPRJournalPositions((List<TreeItem>)item.Childs, safeList, filter);
                }
            }
        }

        private void SetCheckForDepartmentsAndCompaniesDIPRJournalPositions(List<TreeItem> tree)
        {
            if (tree == null) return;

            foreach (var item in tree)
            {
                if (item.IsList ?? true) continue;

                SetCheckForDepartmentsAndCompaniesDIPRJournalPositions((List<TreeItem>)item.Childs);

                var child = (FrontDIPRegistrationJournalPositionsBase)item;

                int allCount = 0;
                int viewCount = 0;
                int regCount = 0;
                bool viewGr = false;
                bool regGr = false;

                GetCheckCountDIPRJournalPositions((List<TreeItem>)child.Childs, out allCount, out viewCount, out regCount, out viewGr, out regGr);

                if (viewGr) { child.IsViewing = 2; }
                else { child.IsViewing = (allCount == viewCount) ? 1 : (viewCount == 0) ? 0 : 2; }

                if (regGr) { child.IsRegistration = 2; }
                else { child.IsRegistration = (allCount == regCount) ? 1 : (regCount == 0) ? 0 : 2; }

            }
        }

        private void GetCheckCountDIPRJournalPositions(List<TreeItem> tree, out int AllCount, out int ViewCount, out int RegCount, out bool ViewGr, out bool RegGr)
        {
            int allCount = 0;
            int vCount = 0;
            int rCount = 0;
            bool vGr = false;
            bool rGr = false;

            foreach (var item in tree)
            {
                var child = (FrontDIPRegistrationJournalPositionsBase)item;

                allCount++;
                if (child.IsViewing > 0) vCount++;
                if (child.IsRegistration > 0) rCount++;

                if (child.IsViewing == 2) vGr = true;
                if (child.IsRegistration == 2) rGr = true;

            }

            AllCount = allCount;
            ViewCount = vCount;
            RegCount = rCount;
            ViewGr = vGr;
            RegGr = rGr;
        }
        #endregion

    }
}
