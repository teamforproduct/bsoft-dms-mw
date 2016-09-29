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
        public IEnumerable<FrontAdminRoleAction> GetAdminRoleActions(IContext context, FilterAdminRoleAction filter)
        {
            return _adminDb.GetRoleActions(context, filter);
        }
        #endregion

        #region [+] PositionRoles ...
        public IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminRole filter)
        {
            return _adminDb.GetPositionRoles(context, filter);
        }

        public FrontAdminPositionRole GetPositionRole(IContext context, int id)
        {
            return _adminDb.GetPositionRole(context, id);
        }
        #endregion

        #region [+] UserRoles ...
        public IEnumerable<FrontAdminUserRole> GetAdminUserRoles(IContext context, FilterAdminUserRole filter)
        {
            return _adminDb.GetUserRoles(context, filter);
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

                positions = _dictDb.GetPositionsForTreeSend(context, positionId, new FilterDictionaryPosition()
                {
                    IsActive = filter.IsActive,
                    //IDs = targetPositions
                });
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForTree(context, new FilterDictionaryDepartment()
                {
                    IsActive = filter.IsActive,
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                companies = _dictDb.GetAgentClientCompaniesForTree(context, new FilterDictionaryAgentClientCompany()
                {
                    IsActive = filter.IsActive
                });
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (positions != null) flatList.AddRange(positions);
            if (departments != null) flatList.AddRange(departments);

            var res = Tree.Get(flatList, filter);

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

            return Tree.GetList(res);
        }

        private void FormSafeList(List<TreeItem> tree, List<string> safeList, FilterTree filter)
        {
            if (tree != null)
            {
                foreach (var item in tree)
                {
                    if (item.ObjectId == (int)EnumObjects.DictionaryPositions)
                    {
                        var pos = (FrontDictionaryPositionTreeItem)item;

                        if (pos.IsExecution == 1 || pos.IsInforming == 1)
                        {
                            safeList.AddRange(item.Path.Split('/'));
                        }
                    }

                    FormSafeList((List<TreeItem>)item.Childs, safeList, filter);
                }
            }
        }


        #endregion

        #region [+] MainMenu ...
        public IEnumerable<TreeItem> GetAdminMainMenu(IContext context)
        {
            return _adminDb.GetMainMenu(context);
        }
        #endregion


    }
}
