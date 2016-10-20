using BL.CrossCutting.Context;
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

namespace BL.Logic.AdminCore
{
    public class AdminService : IAdminService
    {
        private readonly IAdminsDbProcess _adminDb;
        private readonly IDictionariesDbProcess _dictDb;
        private readonly ICommandService _commandService;

        private const int _MINUTES_TO_UPDATE_INFO = 5;

        private Dictionary<string, StoreInfo> accList;

        public AdminService(IAdminsDbProcess adminDb, IDictionariesDbProcess dictDb, ICommandService commandService)
        {
            _adminDb = adminDb;
            _dictDb = dictDb;
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
                if ((DateTime.Now - so.LastUsage).TotalMinutes > _MINUTES_TO_UPDATE_INFO)
                {
                    var lst = _adminDb.GetAdminAccesses(context);
                    so.StoreObject = lst;
                    so.LastUsage = DateTime.Now;
                    return lst;
                }
                return so.StoreObject as AdminAccessInfo;
            }
            var nlst = _adminDb.GetAdminAccesses(context);
            var nso = new StoreInfo
            {
                LastUsage = DateTime.Now,
                StoreObject = nlst
            };
            accList.Add(key, nso);
            return nlst;
        }
        public Employee GetEmployee(IContext context, string userId)
        {
            return _adminDb.GetEmployee(context, userId);
        }

        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return _adminDb.GetPositionsByUser(employee);
        }

        public Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context)
        {
            return _adminDb.GetCurrentPositionsAccessLevel(context);
        }

        public IEnumerable<FrontAdminUserRole> GetPositionsByCurrentUser(IContext context)
        {
            return _adminDb.GetPositionsByUser(context, new FilterAdminUserRole() { UserIDs = new List<int>() { context.CurrentAgentId } });
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

                var qry = data.ActionAccess
                    .Join(data.Actions, aa => aa.ActionId, ac => ac.Id, (aa, ac) => new { ActAccess = aa, Act = ac })
                    .Join(data.PositionRoles, aa => aa.ActAccess.RoleId, r => r.Id, (aa, r) => new { aa.ActAccess, aa.Act, Role = r });
                // test it really good!
                res = qry.Any(x => x.Act.Id == model.DocumentActionId
                && data.UserRoles.Where(s => s.RoleId == x.Role.Id).Any(y => y.UserId == model.UserId)
                && (((model.PositionId == null) && (model.PositionsIdList.Contains(x.Role.PositionId))) || (x.Role.PositionId == model.PositionId))
                && (!x.Act.IsGrantable || (x.Act.IsGrantable && (!x.Act.IsGrantableByRecordId || x.ActAccess.RecordId == 0 || x.ActAccess.RecordId == model.RecordId))));
            }
            else
            {
                var qry = data.UserRoles.Join(data.PositionRoles, ur => ur.RoleId, r => r.RoleId, (u, r) => new { URole = u, PR = r });

                res = !model.PositionsIdList.Except(qry.Where(x => x.URole.UserId == model.UserId).Select(x => x.PR.PositionId)).Any();
            }
            if (!res && isThrowExeception)
            {
                throw new AccessIsDenied(); //TODO Сергей!!!Как красиво передать string obj, string act, int? id = null в сообщение?
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
        #endregion

        #region [+] RoleAction ...
        public IEnumerable<FrontAdminRoleAction> GetRoleActions(IContext context, FilterAdminRoleAction filter)
        {
            return _adminDb.GetRoleActions(context, filter);
        }

        public IEnumerable<FrontAdminRoleAction> GetRoleActionsDIP(IContext context, int roleId, FilterAdminRoleActionDIP filter)
        {

            if (filter.IsChecked == true)
            {
                List<int> actions = _adminDb.GetActionsByRoles(context, new FilterAdminRoleAction()
                {
                    RoleIDs = new List<int> { roleId }
                });

                if (filter.IDs == null) filter.IDs = new List<int>();

                filter.IDs.AddRange(actions);
            }

            return _adminDb.GetRoleActionsDIP(context, roleId, filter);
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
        //public IEnumerable<FrontAdminUserRole> GetUserRolesDIP(IContext context, FilterAdminRole filter)
        //{
        //    if (filter.UserIDs?.Count > 0)
        //    {
        //        if (filter.IsChecked == true)
        //        {
        //            List<int> roles = _adminDb.GetRolesByUsers(context, new FilterAdminUserRole()
        //            {
        //                UserIDs = filter.UserIDs,
        //                StartDate = filter.StartDate,
        //                EndDate = filter.EndDate
        //            });

        //            if (filter.IDs == null) filter.IDs = new List<int>();

        //            filter.IDs.AddRange(roles);
        //        }
        //        else if ((filter.PositionIDs?.Count ?? 0) == 0)
        //        {
        //            // определяю должности, которые исполняет сотрудник
        //            var executors = _dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
        //            { AgentIDs = filter.UserIDs, StartDate = filter.StartDate, EndDate = filter.EndDate });

        //            if (filter.PositionIDs == null) filter.PositionIDs = new List<int>();

        //            filter.PositionIDs.AddRange(executors.Select(x => x.PositionId));

        //        }
        //    }

        //    if (filter.PositionIDs?.Count > 0)
        //    {
        //        // сужение до ролей, котрые принадлежат указанным должностям
        //        var positionRoles = _adminDb.GetInternalPositionRoles(context, new FilterAdminPositionRole { PositionIDs = filter.PositionIDs });

        //        if (filter.IDs == null) filter.IDs = new List<int>();

        //        filter.IDs.AddRange(positionRoles.Where(x => filter.IDs.Contains(x.Id)).Select(x => x.RoleId).ToList());
        //    }

        //    return _adminDb.GetUserRolesDIP(context, filter);
        //}

        public IEnumerable<ITreeItem> GetUserRolesDIP(IContext context, int userId, FilterDIPAdminUserRole filter)
        {
            var positionIDs = new List<int>();

            if (filter.IsChecked == true)
            {
                //List<int> roles = _adminDb.GetRolesByUsers(context, new FilterAdminUserRole()
                //{
                //    UserIDs = filter.UserIDs,
                //    StartDate = filter.StartDate,
                //    EndDate = filter.EndDate
                //});

                //if (filter.IDs == null) filter.IDs = new List<int>();

                //filter.IDs.AddRange(roles);
            }
            else

            if (filter.PositionId == null)
            {
                // определяю должности, которые исполняет сотрудник
                var executors = _dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
                { AgentIDs = new List<int> { userId }, StartDate = filter.StartDate, EndDate = filter.EndDate });

                positionIDs.AddRange(executors.Select(x => x.PositionId));
            }
            else
            {
                positionIDs.Add(filter.PositionId ?? -1);
            }



            var positionRoles = _adminDb.GetPositionRolesDIPUserRoles(context, new FilterAdminPositionRole()
            {
                PositionIDs = positionIDs,
            });

            var positionExecutors = _dictDb.GetPositionExecutorsDIPUserRoles(context, new FilterDictionaryPositionExecutor()
            {
                PositionIDs = (List<int>)positionRoles.Select(x => x.PositionId),
            });

            var userExecutors = _adminDb.GetUserRoles(context, new FilterAdminUserRole()
            {
                PositionIDs = (List<int>)positionRoles.Select(x => x.PositionId),
                UserIDs = new List<int> { userId }
            });

            List<TreeItem> flatList = new List<TreeItem>();

            flatList.AddRange(positionExecutors);
            flatList.AddRange(positionRoles);

            var res = Tree.GetList(Tree.Get(flatList, filter));

            return res;
        }

        public void AddAllPositionRoleForUser(IContext context, ModifyDictionaryPositionExecutor positionExecutor)
        {
            var positionRoles = _adminDb.GetInternalPositionRoles(context, new FilterAdminPositionRole() { PositionIDs = new List<int> { positionExecutor.PositionId } });

            if (positionRoles.Count() == 0) return;

            var userRoles = new List<InternalAdminUserRole>();

            userRoles = positionRoles.Select(x => new InternalAdminUserRole()
            {
                UserId = positionExecutor.AgentId,
                RoleId = x.RoleId,
                PositionId = positionExecutor.PositionId,
                StartDate = positionExecutor.StartDate,
                EndDate = positionExecutor.EndDate ?? DateTime.MaxValue,
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
            var res = Tree.Get(flatList, new FilterTree() { });

            // растановка галочек для групп(компании, отделы)
            SetCheckForDepartmentsAndCompanies(res);

            if (filter.IsChecked)
            {
                var safeList = new List<string>();

                FormSafeList(res, safeList, filter);

                if (safeList.Count > 0)
                {
                    flatList.RemoveAll(r => !safeList.Contains(r.TreeId));
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

        private void FormSafeList(List<TreeItem> tree, List<string> safeList, FilterTree filter)
        {
            if (tree != null)
            {
                foreach (var item in tree)
                {
                    //if (item.ObjectId == (int)EnumObjects.DictionaryPositions)
                    //{
                    var pos = (FrontDIPSubordinationsBase)item;

                    if (pos.IsExecution > 0 || pos.IsInforming > 0)
                    {
                        safeList.AddRange(item.Path.Split('/'));
                    }
                    //}

                    FormSafeList((List<TreeItem>)item.Childs, safeList, filter);
                }
            }
        }

        private void SetCheckForDepartmentsAndCompanies(List<TreeItem> tree)
        {
            if (tree == null) return;

            foreach (var item in tree)
            {
                if (item.IsList) continue;

                SetCheckForDepartmentsAndCompanies((List<TreeItem>)item.Childs);

                var child = (FrontDIPSubordinationsBase)item;

                int allCount = 0;
                int infCount = 0;
                int excCount = 0;
                bool infGr = false;
                bool excGr = false;

                GetCheckCount((List<TreeItem>)child.Childs, out allCount, out infCount, out excCount, out infGr, out excGr);

                if (infGr) { child.IsExecution = 2; }
                else { child.IsExecution = (allCount == excCount) ? 1 : (excCount == 0) ? 0 : 2; }

                if (infGr) { child.IsInforming = 2; }
                else { child.IsInforming = (allCount == infCount) ? 1 : (infCount == 0) ? 0 : 2; }

            }
        }

        private void GetCheckCount(List<TreeItem> tree, out int AllCount, out int InfCount, out int ExcCount, out bool InfGr, out bool ExcGr)
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

        #region [+] MainMenu ...
        public IEnumerable<TreeItem> GetAdminMainMenu(IContext context)
        {
            return _adminDb.GetMainMenu(context);
        }
        #endregion


    }
}
