using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Admin;
using BL.Database.Dictionaries;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemCore;
using BL.Logic.TreeBuilder;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.Tree;
using BL.Model.Users;
using BL.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.AdminCore
{
    public class AdminService : IAdminService
    {
        private readonly AdminsDbProcess _adminDb;
        private readonly DictionariesDbProcess _dictDb;
        private readonly ICommandService _commandService;

        public AdminService(AdminsDbProcess adminDb, DictionariesDbProcess dictDb, ICommandService commandService)
        {
            _adminDb = adminDb;
            _dictDb = dictDb;
            _commandService = commandService;
        }

        public object ExecuteAction(EnumActions act, IContext context, object param)
        {
            var cmd = AdminCommandFactory.GetAdminCommand(act, context, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        #region [+] General ...


        public Employee GetEmployeeForContext(IContext context, string userId)
        {
            return _adminDb.GetEmployeeForContext(context, userId);
        }

        public void ChangeLoginAgentUser(IContext context, ChangeLoginAgentUser model)
        {
            var user = new InternalDictionaryAgentUser { Id = model.Id, UserName = model.NewEmail };
            CommonDocumentUtilities.SetLastChange(context, user);
            _dictDb.SetAgentUserUserName(context, user);
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
        public IEnumerable<FrontUserAssignments> GetAvailablePositions(IContext context, List<int> PositionIDs = null)
        {
            var res = _adminDb.GetAvailablePositions(context, context.CurrentAgentId, PositionIDs);

            // Если контекст полностью сформирован, тогда проверяю на вшивость
            if (context.IsFormed)
            {
                // Решено тут синхронизировать context.CurrentPositionsIdList и генерировать исключения если AvailablePositions конфликтуют с CurrentPositionsIdList
                if (res.Count() == 0) throw new EmployeeNotExecuteAnyPosition(context.Employee.Name);

                // Проверяю содержатся ли выбранные должности в списке доступных
                foreach (var positionId in context.CurrentPositionsIdList)
                {
                    if (!res.Any(x => x.RolePositionId == positionId)) throw new EmployeeNotExecuteCheckPosition();
                }
            }

            return res;
        }

        // Диалог выбора должностей
        public IEnumerable<FrontUserAssignmentsAvailableGroup> GetAvailablePositionsDialog(IContext context, List<int> PositionIDs = null)
        {
            var list = _adminDb.GetAvailablePositionsList(context, context.CurrentAgentId, PositionIDs);

            var res = list.GroupBy(x => new { x.ExecutorTypeId, x.ExecutorTypeDescription })
                .OrderBy(x => x.Key.ExecutorTypeId)
                .Select(x => new FrontUserAssignmentsAvailableGroup
                {
                    ExecutorTypeId = x.Key.ExecutorTypeId,
                    ExecutorTypeDescription = x.Key.ExecutorTypeDescription,
                    Assignments = x.OrderBy(y => y.PositionName).ToList(),
                });

            return res;
        }

        #region [+] Role ...
        //public FrontAdminPositionRole GetAdminRole(IContext context, int id)
        //{
        //    return _adminDb.GetRole(context, new FilterAdminPositionRole() { IDs = new List<int> { id } }).FirstOrDefault();
        //}

        public IEnumerable<FrontMainRoles> GetMainRoles(IContext context, FullTextSearch ftSearch, FilterAdminRole filter, UIPaging paging)
        {
            return FTS.Get(context, Modules.Role, ftSearch, filter, paging, null, _adminDb.GetMainRoles, _adminDb.GetRoleIDs);
        }

        public IEnumerable<ListItem> GetListRoles(IContext context, FilterAdminRole filter, UIPaging paging)
        {
            return _adminDb.GetListRoles(context, filter, paging);
        }

        public FrontAdminRole GetAdminRole(IContext context, int id)
        {
            return _adminDb.GetRoles(context, new FilterAdminRole { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAdminRole> GetAdminRoles(IContext context, FilterAdminRole filter)
        {
            return _adminDb.GetRoles(context, filter);
        }

        public int AddNamedRole(IContext context, Roles roleTypeId)
        {
            int roleId = 0;

            using (var transaction = Transactions.GetTransaction())
            {
                var languageService = DmsResolver.Current.Get<ILanguages>();
                var name = languageService.GetTranslation(context.User.LanguageId, Labels.Get("Roles", roleTypeId.ToString()));

                // предполагаю, что классификатор ролей загружен при инсталяции приложения
                var role = new InternalAdminRole() { RoleTypeId = (int)roleTypeId, Name = name };
                CommonDocumentUtilities.SetLastChange(context, role);

                // Новая роль со ссылкой на классификатор ролей.
                roleId = _adminDb.AddRole(context, role);

                DmsDbImportData.InitPermissions();

                var rp = new List<AdminRolePermissions>();

                // Указание ид роли для предложенных действий
                foreach (var item in DmsDbImportData.GetAdminRolePermissions().Where(x => x.RoleId == (int)roleTypeId))
                {
                    rp.Add(new AdminRolePermissions() { PermissionId = item.PermissionId, RoleId = roleId, LastChangeDate = DateTime.UtcNow, LastChangeUserId = (int)EnumSystemUsers.AdminUser });
                }

                _adminDb.AddRolePermissions(context, rp);

                transaction.Complete();
            }
            return roleId;
        }
        #endregion


        #endregion

        #region [+] PositionRoles ...
        public IEnumerable<FrontAdminPositionRole> GetPositionRolesDIP(IContext context, FilterAdminPositionRoleDIP filter)
        {
            // Тонкий момент, проверяю не является ли сотрудник локальным администратором.
            // Если не локальный значит, надеюсь, что глобальный и разрешаю давать все роли для должности
            var employeeDepartments = GetInternalEmployeeDepartments(context, context.Employee.Id);

            if (employeeDepartments != null)
            {
                if (filter == null) filter = new FilterAdminPositionRoleDIP();

                filter.WithoutFeatures = new List<int> {
                        DmsDbImportData.GetFeatureId(Modules.Department,Features.Admins ), //Добавление локальных администраторов, 
                        DmsDbImportData.GetFeatureId(Modules.Auditlog,Features.Info ), //Просмотр полной истории подключений,
                        DmsDbImportData.GetFeatureId(Modules.Position,Features.DocumentAccesses ), // Управление доступом к документу 
                    };

            }

            return _adminDb.GetPositionRolesDIP(context, filter);
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
                // роли у назначения
                var userRoles = _adminDb.GetInternalUserRoles(context, new FilterAdminUserRole { PositionExecutorIDs = executors.Select(x => x.AssignmentId).ToList() });

                // создаю структуру из TreeItem для постоения дерева и накладываю параметры из userRoles
                foreach (var executor in executors)
                {
                    // дерево - группа
                    var e = new FrontDIPUserRolesExecutor()
                    {
                        Id = executor.AssignmentId,
                        PositionId = executor.PositionId ?? -1,
                        Name = executor.PositionName,
                        StartDate = executor.StartDate,
                        EndDate = executor.EndDate,
                        ExecutorName = executor.ExecutorName,
                        ExecutorTypeSuffix = executor.PositionExecutorSuffix,
                        DepartmentCodeName = executor.DepartmentCodeName,
                        IsActive = executor.IsActive ?? true,
                        ObjectId = (int)EnumObjects.DictionaryPositionExecutors,
                    };

                    // дерево - листья
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
                            IsDefault = userRoles.Where(x => x.RoleId == role.RoleId & x.RoleTypeId.HasValue).Any(),
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

        public void AddAllPositionRoleForUser(IContext context, InternalDictionaryPositionExecutor positionExecutor)
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
        public IEnumerable<FrontAdminEmployeeDepartments> GetDepartmentAdmins(IContext context, int departmentId)
        {
            return _adminDb.GetDepartmentAdmins(context, departmentId);
        }

        public List<int> GetInternalEmployeeDepartments(IContext context, int employeeId, List<int> depertmentsIDs = null)
        {
            // проверяю есть ли для сотрудника отделы для администрирования
            var deps = _adminDb.GetInternalDepartmentAdmins(context, new FilterAdminEmployeeDepartments { EmployeeIDs = new List<int> { employeeId } });

            if (deps?.Count() > 0)
            {
                if (depertmentsIDs?.Count() > 0)
                {
                    deps = deps.Where(x => depertmentsIDs.Contains(x.DepartmentId));
                }

                return deps.Select(x => x.DepartmentId).ToList();
            }
            // Если нет отделов возвращаю null - в администраторах не числится
            else return null;

        }
        #endregion

        #region [+] Subordination ...

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
                    IsActive = true,
                    //IDs = targetPositions
                });
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForDIPSubordinations(context, positionId, new FilterDictionaryDepartment()
                {
                    IsActive = true,
                    ExcludeDepartmentsWithoutPositions = true,
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                companies = _dictDb.GetAgentOrgsForDIPSubordinations(context, positionId, new FilterDictionaryAgentOrg()
                {
                    IsActive = true
                });
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (positions != null) flatList.AddRange(positions);
            if (departments != null) flatList.AddRange(departments);

            // для растановки галочек дерево нельзя сужать
            var res = Tree.Get(flatList, new FilterTree());

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
                if (item.IsLeaf ?? true) continue;

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

        #endregion

        #region [+] RegistrationJournalPosition ...

        // Таб Доступ в журналах
        public IEnumerable<ITreeItem> GetPositionsByJournalDIP(IContext context, int journalId, FilterTree filter)
        {

            int levelCount = filter?.LevelCount ?? 0;
            IEnumerable<TreeItem> positions = null;
            IEnumerable<TreeItem> departments = null;
            IEnumerable<TreeItem> companies = null;

            if (levelCount >= 3 || levelCount == 0)
            {
                positions = _dictDb.GetPositionsForDIPJournalAccess(context, journalId, new FilterDictionaryPosition()
                {
                    IsActive = true,
                });
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForDIPJournalAccess(context, journalId, new FilterDictionaryDepartment()
                {
                    IsActive = true,
                    ExcludeDepartmentsWithoutPositions = true,
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                companies = _dictDb.GetAgentOrgsForDIPJournalAccess(context, journalId, new FilterDictionaryAgentOrg()
                {
                    IsActive = true
                });
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (positions != null) flatList.AddRange(positions);
            if (departments != null) flatList.AddRange(departments);

            // для растановки галочек дерево нельзя сужать
            var res = Tree.Get(flatList, new FilterTree());

            // растановка галочек для групп(компании, отделы)
            SetCheckForDepartmentsAndCompaniesDIPJournalAccess(res);

            if (filter.IsChecked ?? false)
            {
                var safeList = new List<string>();

                FormSafeListDIPJournalAccess(res, safeList, filter);

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


            //var journalPositions = _adminDb.GetInternalRegistrationJournalPositions(context, new FilterAdminRegistrationJournalPosition()
            //{ RegistrationJournalIDs = new List<int> { journalId } });

            //var posList = journalPositions.Select(x => x.PositionId).ToList();

            //if (posList.Count == 0) return null;

            //if (filter == null) filter = new FilterDictionaryPosition();

            //if (filter.IDs?.Count > 0)
            //{ filter.IDs.AddRange(posList); }
            //else
            //{ filter.IDs = posList; }

            //var positions = _dictDb.GetPositionsForDIPRegistrationJournals(context, journalId, filter);

            //return positions;
        }

        private void FormSafeListDIPJournalAccess(List<TreeItem> tree, List<string> safeList, FilterTree filter)
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

                    var pos = (FrontDIPJournalAccessBase)item;

                    //if (existsNameFilter & addToSafeList)
                    //{
                    //    // Поиск присходит по специальному полю для поиска
                    //    addToSafeList = (item.SearchText.ToLower().ContainsArray(arrName));
                    //}

                    if (addToSafeList)
                    {
                        addToSafeList = pos.IsView > 0 || pos.IsRegistration > 0;
                    }

                    if (addToSafeList) safeList.AddRange(item.Path.Split('/'));

                    FormSafeListDIPJournalAccess((List<TreeItem>)item.Childs, safeList, filter);
                }
            }
        }

        private void SetCheckForDepartmentsAndCompaniesDIPJournalAccess(List<TreeItem> tree)
        {
            if (tree == null) return;

            foreach (var item in tree)
            {
                if (item.IsLeaf ?? true) continue;

                SetCheckForDepartmentsAndCompaniesDIPJournalAccess((List<TreeItem>)item.Childs);

                var child = (FrontDIPJournalAccessBase)item;

                int allCount = 0;
                int vwCount = 0;
                int rgCount = 0;
                bool vwGr = false;
                bool rgGr = false;

                GetCheckCountDIPJournalAccess((List<TreeItem>)child.Childs, out allCount, out vwCount, out rgCount, out vwGr, out rgGr);

                if (vwGr) { child.IsView = 2; }
                else { child.IsView = (allCount == vwCount) ? 1 : (vwCount == 0) ? 0 : 2; }

                if (rgGr) { child.IsRegistration = 2; }
                else { child.IsRegistration = (allCount == rgCount) ? 1 : (rgCount == 0) ? 0 : 2; }

            }
        }

        private void GetCheckCountDIPJournalAccess(List<TreeItem> tree, out int AllCount, out int ViewCount, out int RegCount, out bool ViewGr, out bool RegGr)
        {
            int allCount = 0;
            int viewCount = 0;
            int regCount = 0;
            bool viewGr = false;
            bool regGr = false;

            foreach (var item in tree)
            {
                var child = (FrontDIPJournalAccessBase)item;

                allCount++;
                if (child.IsView > 0) viewCount++;
                if (child.IsRegistration > 0) regCount++;

                if (child.IsView == 2) viewGr = true;
                if (child.IsRegistration == 2) regGr = true;

            }

            AllCount = allCount;
            ViewCount = viewCount;
            RegCount = regCount;
            ViewGr = viewGr;
            RegGr = regGr;
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
                companies = _dictDb.GetAgentClientCompaniesForDIPRJournalPositions(context, positionId, new FilterDictionaryAgentOrg()
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
                if (item.IsLeaf ?? true) continue;

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

        #region [+] Permissions ...
        public IEnumerable<FrontPermission> GetUserPermissions(IContext context, FilterPermissionsAccess filter = null)
        {

            return _adminDb.GetUserPermissionsAccess(context, filter ?? GetFilterPermissionsAccessByContext(context, false));
        }

        public IEnumerable<FrontModule> GetRolePermissions(IContext context, FilterAdminRolePermissionsDIP filter)
        {
            return _adminDb.GetRolePermissions(context, filter);
        }

        public FilterPermissionsAccess GetFilterPermissionsAccessByContext(IContext context, bool isPositionFromContext, List<int> permissionIDs = null, int? actionId = null, int? moduleId = null)
        {
            var res = new FilterPermissionsAccess { UserId = context.CurrentAgentId };

            res.PositionsIdList = isPositionFromContext
                ? new List<int> { context.CurrentPositionId }.Intersect(context.CurrentPositionsIdList).ToList()
                : context.CurrentPositionsIdList;
            res.ActionId = actionId;
            res.PermissionIDs = permissionIDs;
            res.ModuleId = moduleId;
            return res;
        }

        public bool ExistsPermissionsAccess(IContext context, FilterPermissionsAccess filter)
        {
            return _adminDb.ExistsPermissionsAccess(context, filter);
        }

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

            var data = _adminDb.GetAdminAccesses(context);
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
                if (!data.Actions.Any(x => x.PermissionId.HasValue && x.Id == actionId.Value))
                    return true; //если экшина в таблице нет или на него не назначен пермишен, то разрешаем

                while (!res && actionId.HasValue && data.Actions.Any(x => x.PermissionId.HasValue && x.Id == actionId.Value))
                {
                    var filter = GetFilterPermissionsAccessByContext(context, model.PositionId.HasValue, null, actionId.Value);
                    res = _adminDb.ExistsPermissionsAccess(context, filter);
                    actionId = null;
                }
            }
            else
            {
                var qry = data.UserRoles.Join(data.PositionRoles, ur => ur.RoleId, r => r.RoleId, (u, r) => new { URole = u, PR = r });

                res = !model.PositionsIdList.Except(qry.Where(x => x.URole.AgentId == model.UserId).Select(x => x.PR.PositionId)).Any();
            }
            if (res || !isThrowExeception) return res;
            {
                if (model.DocumentActionId == null)
                {
                    throw new AccessIsDenied();
                }

                var actionName = string.Empty;
                var a = data.Actions.FirstOrDefault(x => x.Id == model.DocumentActionId);

                if (a != null)
                {
                    actionName = a.Description;
                }

                throw new ActionIsDenied(actionName); //TODO Сергей!!!Как красиво передать string obj, string act, int? id = null в сообщение?
            }
        }

        public bool VerifyAccess(IContext context, EnumActions action, bool isPositionFromContext = true, bool isThrowExeception = true)
        {
            return VerifyAccess(context, new VerifyAccess { DocumentActionId = (int)action, IsPositionFromContext = isPositionFromContext }, isThrowExeception);
        }

        public bool VerifySubordination(IContext context, VerifySubordination model, bool isThrowException = false)
        {
            var res = _adminDb.VerifySubordination(context, model);
            if (!res && isThrowException)
                throw new SubordinationHasBeenViolated();
            return res;
        }

        #endregion`


        #endregion
    }
}
