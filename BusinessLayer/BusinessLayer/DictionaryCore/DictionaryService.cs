using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins;
using BL.Database.Dictionaries;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.Common;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemCore;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.TreeBuilder;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontMainModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class DictionaryService : IDictionaryService
    {
        private readonly AdminsDbProcess _adminDb;
        private readonly DictionariesDbProcess _dictDb;
        private readonly ICommandService _commandService;

        public DictionaryService(AdminsDbProcess adminDb, DictionariesDbProcess dictDb, ICommandService commandService)
        {
            _dictDb = dictDb;
            _commandService = commandService;
        }

        public object ExecuteAction(EnumDictionaryActions act, IContext context, object param)
        {
            var cmd = DictionaryCommandFactory.GetDictionaryCommand(act, context, param);
            var res = _commandService.ExecuteCommand(cmd);
            return res;
        }

        #region DictionaryAgents
        public FrontDictionaryAgent GetAgent(IContext context, int id)
        {
            return GetAgents(context, new FilterDictionaryAgent { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<AutocompleteItem> GetShortListAgentExternal(IContext context, UIPaging paging)
        {
            return _dictDb.GetAgentExternalList(context, paging);
        }
        public IEnumerable<FrontDictionaryAgent> GetAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging)
        {
            return _dictDb.GetAgents(context, filter, paging);
        }

        /// <summary>
        /// Удаляет агента если нет выносок
        /// </summary>
        /// <param name="context"></param>
        /// <param name="agent"></param>
        public void DeleteAgentIfNoAny(IContext context, List<int> list)
        {
            foreach (int agentId in list)
            {
                if (_dictDb.ExistsAgentClientCompanies(context, new FilterDictionaryAgentOrg() { IDs = new List<int>() { agentId } })) return;


                if (_dictDb.ExistsAgentEmployees(context, new FilterDictionaryAgentEmployee() { IDs = new List<int>() { agentId } })) return;
                //if (ExistsAgentUsers(context, new FilterDictionaryAgent() { IDs = new List<int>() { agentId } })) return;
                //if (ExistsAgentPeople(context, new FilterDictionaryAgentPerson() { IDs = new List<int>() { agentId } })) return;
                if (_dictDb.ExistsAgentPersons(context, new FilterDictionaryAgentPerson() { IDs = new List<int>() { agentId } })) return;

                if (_dictDb.ExistsAgentBanks(context, new FilterDictionaryAgentBank() { IDs = new List<int>() { agentId } })) return;

                if (_dictDb.ExistsAgentCompanies(context, new FilterDictionaryAgentCompany() { IDs = new List<int>() { agentId } })) return;

                using (var transaction = Transactions.GetTransaction())
                {

                    _dictDb.DeleteAgentAddress(context, new FilterDictionaryAgentAddress { AgentIDs = new List<int> { agentId } });

                    _dictDb.DeleteAgentContacts(context, new FilterDictionaryContact { AgentIDs = new List<int> { agentId } });

                    _dictDb.DeleteAgentAccounts(context, new FilterDictionaryAgentAccount { AgentIDs = new List<int> { agentId } });

                    _dictDb.DeleteAgents(context, new FilterDictionaryAgent { IDs = new List<int> { agentId } });

                    transaction.Complete();

                }
            }
        }
        #endregion DictionaryAgents

        #region DictionaryAgentPeople
        public FrontAgentPeoplePassport GetAgentPeoplePassport(IContext context, int id)
        {
            return _dictDb.GetAgentPeoplePassport(context, id);
        }
        #endregion

        #region DictionaryAgentPersons
        public FrontAgentPerson GetAgentPerson(IContext context, int id)
        {
            return _dictDb.GetAgentPerson(context, new FilterDictionaryAgentPerson { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<AutocompleteItem> GetShortListAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryAgentPerson();

            filter.IsActive = true;

            return _dictDb.GetShortListAgentPersons(context, filter, paging);
        }

        public IEnumerable<FrontMainAgentPerson> GetMainAgentPersons(IContext context, FullTextSearch ftSearch, FilterDictionaryAgentPerson filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get(context, Modules.Person, ftSearch, filter, paging, sorting, _dictDb.GetMainAgentPersons, _dictDb.GetAgentPersonIDs);
        }

        public void DeleteAgentPerson(IContext context, int id)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                _dictDb.DeleteAgentPersons(context, new FilterDictionaryAgentPerson { IDs = new List<int> { id } });

                _dictDb.DeleteAgentPeoples(context, new FilterDictionaryAgentPeoples { IDs = new List<int> { id } });

                DeleteAgentIfNoAny(context, new List<int>() { id });

                transaction.Complete();
            }
        }

        #endregion DictionaryAgentPersons

        #region DicionaryAgentEmployees

        public FrontAgentEmployee GetDictionaryAgentEmployee(IContext context, int id)
        {
            return _dictDb.GetAgentEmployee(context, id);
        }

        public IEnumerable<ListItem> GetAgentEmployeeList(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryAgentEmployee();

            filter.IsActive = true;

            return _dictDb.GetAgentEmployeeList(context, filter, paging);
        }

        public IEnumerable<FrontMainAgentEmployee> GetMainAgentEmployees(IContext context, FullTextSearch ftSearch, FilterDictionaryAgentEmployee filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get(context, Modules.Employee, ftSearch, filter, paging, sorting, _dictDb.GetMainAgentEmployees, _dictDb.GetAgentEmployeeIDs
               , new FullTextSearchFilter { Module = Modules.Employee, IsOnlyActual = true }
               );
        }

        public FrontFile GetDictionaryAgentUserPicture(IContext context, int employeeId)
        {
            var userPicture = _dictDb.GetInternalAgentImage(context, employeeId);

            string fileContect = string.Empty;

            if (userPicture.Image != null)
                fileContect = Convert.ToBase64String(userPicture.Image);

            var uPic = new FrontFile()
            {
                Id = userPicture.Id,
                FileContent = fileContect
            };

            return uPic;
        }

        public string GetDictionaryAgentUserId(IContext context, int employeeId)
        {
            var user = _dictDb.GetInternalAgentUser(context, employeeId);

            return user.UserId;
        }

        public void SetAgentUserUserId(IContext context, InternalDictionaryAgentUser User)
        {
            _dictDb.SetAgentUserUserId(context, User);
        }
        #endregion DictionaryAgentEmployees

        #region DictionaryAgentAdress
        public FrontDictionaryAgentAddress GetAgentAddress(IContext context, int id)
        {
            return GetAgentAddresses(context, new FilterDictionaryAgentAddress { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAgentAddress> GetAgentAddresses(IContext context, FilterDictionaryAgentAddress filter)
        {
            return _dictDb.GetAgentAddresses(context, filter);
        }
        #endregion

        #region DictionaryAddressTypes

        public FrontAddressType GetDictionaryAddressType(IContext context, int id)
        {
            return _dictDb.GetAddressTypes(context, new FilterDictionaryAddressType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAddressType> GetDictionaryAddressTypes(IContext context, FullTextSearch ftSearch, FilterDictionaryAddressType filter)
        {
            if (filter == null) filter = new FilterDictionaryAddressType();

            filter.CodeName = ftSearch?.FullTextSearchString;

            var res = _dictDb.GetAddressTypes(context, filter);

            DmsResolver.Current.Get<ILogger>().AddSearchQueryLog(context, res.Any(), Modules.AddressType, ftSearch?.FullTextSearchString);

            return res;
        }

        public IEnumerable<FrontShortListAddressType> GetShortListAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            if (filter == null) filter = new FilterDictionaryAddressType();

            filter.IsActive = true;

            return _dictDb.GetShortListAddressTypes(context, filter);
        }


        #endregion

        #region DictionaryAgentAccounts
        public FrontDictionaryAgentAccount GetDictionaryAgentAccount(IContext context, int id)
        {
            return _dictDb.GetAgentAccount(context, id);
        }

        public IEnumerable<FrontDictionaryAgentAccount> GetDictionaryAgentAccounts(IContext context, FilterDictionaryAgentAccount filter)
        {
            return _dictDb.GetAgentAccounts(context, filter);
        }
        #endregion DictionaryAgentAccounts

        #region [+] ContaktPersons

        public IEnumerable<FrontContactPersons> GetAgentPersonsWithContacts(IContext context, FilterDictionaryAgentPerson filter)
        {
            return _dictDb.GetAgentPersonsWithContacts(context, filter);
        }

        #endregion


        #region DictionaryAgentCompanies
        public FrontAgentCompany GetAgentCompany(IContext context, int id)
        {
            return _dictDb.GetAgentCompanies(context, new FilterDictionaryAgentCompany { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<AutocompleteItem> GetAgentCompanyList(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryAgentCompany();

            filter.IsActive = true;

            return _dictDb.GetAgentCompanyList(context, filter, paging);
        }

        public IEnumerable<FrontMainAgentCompany> GetMainAgentCompanies(IContext context, FullTextSearch ftSearch, FilterDictionaryAgentCompany filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get(context, Modules.Company, ftSearch, filter, paging, sorting, _dictDb.GetMainAgentCompanies, _dictDb.GetAgentCompanyIDs);
        }

        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        public FrontAgentBank GetAgentBank(IContext context, int id)
        {
            return _dictDb.GetAgentBanks(context, new FilterDictionaryAgentBank { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontMainAgentBank> GetMainAgentBanks(IContext context, FullTextSearch ftSearch, FilterDictionaryAgentBank filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get<FrontMainAgentBank>(context, Modules.Bank, ftSearch, filter, paging, sorting, _dictDb.GetMainAgentBanks, _dictDb.GetAgentBankIDs);
        }

        public IEnumerable<AutocompleteItem> GetShortListAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryAgentBank();

            filter.IsActive = true;

            return _dictDb.GetShortListAgentBanks(context, filter, paging);
        }
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentUser
        public FrontDictionaryAgentUser GetDictionaryAgentUser(IContext context, int id)
        {
            return _dictDb.GetAgentUser(context, id);
        }

        public int SetAgentUserLanguage(IContext context, string languageCode)
        {
            var languageService = DmsResolver.Current.Get<ILanguages>();
            var languageId = languageService.GetLanguageIdByCode(languageCode);
            var model = new InternalDictionaryAgentUser { Id = context.CurrentAgentId, LanguageId = languageId };
            CommonDocumentUtilities.SetLastChange(context, model);
            _dictDb.SetAgentUserLanguage(context, model);
            return languageId;
        }

        public void SetDictionaryAgentUserLastPositionChose(IContext context, List<int> positionsIdList)
        {
            _dictDb.SetAgentUserLastPositionChose(context,
                new InternalDictionaryAgentUser { Id = context.CurrentAgentId, LastPositionChose = string.Join(",", positionsIdList) });
        }

        #endregion DictionaryAgentUser

        #region DictionaryContacts
        public FrontDictionaryAgentContact GetAgentContact(IContext context, int id)
        {
            return _dictDb.GetContacts(context, new FilterDictionaryContact { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAgentContact> GetAgentContacts(IContext context, FilterDictionaryContact filter)
        {
            return _dictDb.GetContacts(context, filter);
        }

        public IEnumerable<InternalDictionaryContact> GetInternalContacts(IContext context, FilterDictionaryContact filter)
        {
            return _dictDb.GetInternalContacts(context, filter);
        }
        #endregion

        #region DictionaryContactTypes
        public FrontDictionaryContactType GetDictionaryContactType(IContext context, int id)
        {
            return _dictDb.GetContactTypes(context, new FilterDictionaryContactType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryContactType> GetDictionaryContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            var res = _dictDb.GetContactTypes(context, filter);

            return res;
        }

        public IEnumerable<FrontDictionaryContactType> GetMainDictionaryContactTypes(IContext context, FullTextSearch ftSearch, FilterDictionaryContactType filter)
        {
            if (filter == null) filter = new FilterDictionaryContactType();

            filter.CodeName = ftSearch?.FullTextSearchString;

            var res = _dictDb.GetContactTypes(context, filter);

            DmsResolver.Current.Get<ILogger>().AddSearchQueryLog(context, res.Any(), Modules.ContactType, ftSearch?.FullTextSearchString);

            return res;
        }

        public IEnumerable<FrontShortListContactType> GetShortListContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            if (filter == null) filter = new FilterDictionaryContactType();

            filter.IsActive = true;

            return _dictDb.GetShortListContactTypes(context, filter);
        }
        #endregion

        #region DictionaryDepartments
        public FrontDictionaryDepartment GetDictionaryDepartment(IContext context, int id)
        {
            return _dictDb.GetDepartments(context, new FilterDictionaryDepartment { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public string GetDictionaryDepartmentPrefix(IContext context, int parentId)
        {
            return _dictDb.GetDepartmentPrefix(context, parentId);
        }

        public IEnumerable<FrontDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            return _dictDb.GetDepartments(context, filter);
        }

        public string GetDepartmentPrefix(IContext context, int parentId)
        {
            return _dictDb.GetDepartmentPrefix(context, parentId);
        }

        public IEnumerable<AutocompleteItem> GetDepartmentsShortList(IContext context, FilterDictionaryDepartment filter)
        {
            if (filter == null) filter = new FilterDictionaryDepartment();

            filter.IsActive = true;
            return _dictDb.GetShortListDepartments(context, filter);

            //IEnumerable<TreeItem> departments = null;
            //IEnumerable<TreeItem> companies = null;

            //// отделы
            //departments = _dictDb.GetDepartmentsShortList(context, new FilterDictionaryDepartment()
            //{
            //    IsActive = true,
            //    ExcludeDepartmentsWithoutJournals = true,
            //});

            //companies = _dictDb.GetAgentOrgsShortList(context, new FilterDictionaryAgentOrg()
            //{
            //    IsActive = true,
            //    DepartmentIDs = departments.Select(x => x.Id).ToList(),
            //});


            //List<TreeItem> flatList = new List<TreeItem>();

            //if (companies != null) flatList.AddRange(companies);
            //if (departments != null) flatList.AddRange(departments);

            //var fTree = new FilterTree() { Name = ftSearch?.FullTextSearchString };

            //var res = Tree.GetList(Tree.Get(flatList, fTree));

            //return res;
        }


        public void DeleteDepartments(IContext context, List<int> list, bool DeleteChildDepartments = true)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                if (DeleteChildDepartments)
                {
                    var childDepartments = _dictDb.GetDepartmentIDs(context, new FilterDictionaryDepartment() { ParentIDs = list });

                    if (childDepartments.Count > 0) DeleteDepartments(context, childDepartments, true);
                }

                var positions = _dictDb.GetPositionIDs(context, new FilterDictionaryPosition() { DepartmentIDs = list }).ToList();

                if (positions.Count() > 0) DeletePositions(context, positions);

                // AdminEmployeeDepartments
                _adminDb.DeleteDepartmentAdmins(context, new FilterAdminEmployeeDepartments { DepartmentIDs = list });

                // DictionaryRegistrationJournals
                _dictDb.DeleteRegistrationJournals(context, new FilterDictionaryRegistrationJournal { DepartmentIDs = list });

                _dictDb.DeleteDepartments(context, new FilterDictionaryDepartment { IDs = list });

                transaction.Complete();

            }
        }


        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        public FrontDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id)
        {
            return _dictDb.GetDocumentDirections(context, new FilterDictionaryDocumentDirection { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {

            return _dictDb.GetDocumentDirections(context, filter);
        }
        #endregion DictionaryDepartments


        #region DictionaryDocumentTypes
        // следить за списком полей необхдимых в каждом конкретном случае
        public FrontDictionaryDocumentType GetDictionaryDocumentType(IContext context, int id)
        {
            return _dictDb.GetMainDocumentTypes(context, new FilterDictionaryDocumentType { IDs = new List<int> { id } }, null, null).FirstOrDefault();
        }

        public IEnumerable<ListItem> GetShortListDocumentTypes(IContext context, FilterDictionaryDocumentType filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryDocumentType();

            filter.IsActive = true;

            return _dictDb.GetShortListDocumentTypes(context, filter, paging);
        }

        public IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter, UIPaging paging)
        {
            return _dictDb.GetMainDocumentTypes(context, filter, paging, null);
        }

        public IEnumerable<FrontDictionaryDocumentType> GetMainDictionaryDocumentTypes(IContext context, FullTextSearch ftSearch, FilterDictionaryDocumentType filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get(context, Modules.DocumentType, ftSearch, filter, paging, sorting, _dictDb.GetMainDocumentTypes, _dictDb.GetDocumentTypeIDs);
        }

        #endregion DictionaryDocumentTypes

        #region DictionaryEventTypes
        public FrontDictionaryEventType GetDictionaryEventType(IContext context, int id)
        {
            return _dictDb.GetEventTypes(context, new FilterDictionaryEventType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryEventType> GetDictionaryEventTypes(IContext context, FilterDictionaryEventType filter)
        {
            return _dictDb.GetEventTypes(context, filter);
        }
        #endregion DictionaryEventTypes

        #region DictionaryImportanceEventTypes
        public FrontDictionaryImportanceEventType GetDictionaryImportanceEventType(IContext context, int id)
        {
            return _dictDb.GetImportanceEventTypes(context, new FilterDictionaryImportanceEventType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryImportanceEventType> GetDictionaryImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter)
        {

            return _dictDb.GetImportanceEventTypes(context, filter);
        }
        #endregion DictionaryImportanceEventTypes

        #region DictionaryLinkTypes
        public FrontDictionaryLinkType GetDictionaryLinkType(IContext context, int id)
        {
            return _dictDb.GetLinkTypes(context, new FilterDictionaryLinkType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryLinkType> GetDictionaryLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {
            return _dictDb.GetLinkTypes(context, filter);
        }
        #endregion DictionaryLinkTypes

        #region [+] DictionaryPositions ...
        public FrontDictionaryPosition GetDictionaryPosition(IContext context, int id)
        {
            return _dictDb.GetPositions(context, new FilterDictionaryPosition { IDs = new List<int> { id } }).FirstOrDefault();
            //return _dictDb.GetPosition(context, id);
        }

        public IEnumerable<FrontDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter)
        {

            return _dictDb.GetPositions(context, filter);
        }

        public IEnumerable<ListItem> GetPositionList(IContext context, FilterDictionaryPosition filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryPosition();

            filter.IsActive = true;

            return _dictDb.GetPositionList(context, filter, paging);
        }

        public IEnumerable<AutocompleteItem> GetPositionsExecutorShortList(IContext context, FilterDictionaryPosition filter)
        {
            if (filter == null) filter = new FilterDictionaryPosition();

            filter.IsActive = true;

            return _dictDb.GetShortListPositionsExecutor(context, filter);
        }

        public IEnumerable<AutocompleteItem> GetPositionsShortList(IContext context, FilterDictionaryPosition filter)
        {
            if (filter == null) filter = new FilterDictionaryPosition();

            filter.IsActive = true;

            return _dictDb.GetShortListPositions(context, filter);
            //IEnumerable<TreeItem> positions = null;
            //IEnumerable<TreeItem> departments = null;
            //IEnumerable<TreeItem> companies = null;

            //// должности
            //var f = new FilterDictionaryPosition
            //{
            //    IsActive = true
            //};

            //positions = _dictDb.GetPositionsShortList(context, f);


            //// отделы
            //departments = _dictDb.GetDepartmentsShortList(context, new FilterDictionaryDepartment()
            //{
            //    IsActive = true,
            //    ExcludeDepartmentsWithoutPositions = true,
            //});

            //companies = _dictDb.GetAgentOrgsShortList(context, new FilterDictionaryAgentOrg()
            //{
            //    IsActive = true,
            //    DepartmentIDs = departments.Select(x => x.Id).Distinct().ToList(),
            //});


            //List<TreeItem> flatList = new List<TreeItem>();

            //if (companies != null) flatList.AddRange(companies);
            //if (positions != null) flatList.AddRange(positions);
            //if (departments != null) flatList.AddRange(departments);

            //var fTree = new FilterTree() { Name = ftSearch?.FullTextSearchString };

            //var res = Tree.GetList(Tree.Get(flatList, fTree));

            //return res;
        }

        /// <summary>
        /// Возвращает Id должнсотей, которые ниже по Order и в ниже стоящих отделах или по ИД департамента
        /// </summary>
        /// <param name="context"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public List<int> GetChildPositions(IContext context, int? positionId, int? departmentId = null, int? companyId = null)
        {
            if (companyId.HasValue)
            {
                return _dictDb.GetInternalPositions(context, new FilterDictionaryPosition { CompanyIDs = new List<int> { companyId.Value } }).Select(x => x.Id).ToList();
            }
            else
            {
                InternalDictionaryPosition position = null;
                if (positionId.HasValue)
                {
                    position = _dictDb.GetInternalPositions(context, new FilterDictionaryPosition { IDs = new List<int> { positionId.Value } }).FirstOrDefault();
                    departmentId = position.DepartmentId;
                }
                var depernment = _dictDb.GetInternalDepartments(context, new FilterDictionaryDepartment { IDs = new List<int> { departmentId.Value } }).FirstOrDefault();

                // должности в своем отделе
                var positionsInDepartment = _dictDb.GetInternalPositions(context, new FilterDictionaryPosition
                {
                    DepartmentIDs = new List<int> { depernment.Id },
                });


                IEnumerable<TreeItem> positions = null;
                IEnumerable<TreeItem> departments = null;
                List<int> res = new List<int>();

                // должности
                positions = _dictDb.GetPositionsTree(context, new FilterDictionaryPosition
                {
                    IsActive = true
                });


                // отделы
                departments = _dictDb.GetDepartmentsTree(context, new FilterDictionaryDepartment()
                {
                    IsActive = true,
                    ExcludeDepartmentsWithoutPositions = true,
                });

                List<TreeItem> flatList = new List<TreeItem>();

                if (positions != null) flatList.AddRange(positions);
                if (departments != null) flatList.AddRange(departments);

                // отстаиваю дерево начиная с моего отдела
                var fTree = new FilterTree() { StartWithTreeId = string.Concat(departmentId.Value.ToString(), "_", ((int)EnumObjects.DictionaryDepartments).ToString()) };

                var list = Tree.GetList(Tree.Get(flatList, fTree));

                res = list.Where(x => x.ObjectId == (int)EnumObjects.DictionaryPositions)
                    .Select(x => x.Id).ToList();
                if (position != null)
                    // исключаю вышестоящие должности моего отдела
                    res.RemoveAll(x => positionsInDepartment.Where(y => y.Order <= position.Order).Select(y => y.Id).ToList().Contains(x));

                return res;
            }
        }

        private void AddHigherPositions(IContext context, List<int> res, int departmentId, int positionOrder = -1)
        {
            if (departmentId < 0) return;

            var positionsInDepartment = _dictDb.GetInternalPositions(context, new FilterDictionaryPosition
            {
                DepartmentIDs = new List<int> { departmentId },
            });

            if (positionOrder > 0)
            { res.AddRange(positionsInDepartment.Where(x => x.Order < positionOrder).Select(x => x.Id).ToList()); }
            else
            { res.AddRange(positionsInDepartment.Select(x => x.Id).ToList()); }

            var depernment = _dictDb.GetInternalDepartments(context, new FilterDictionaryDepartment { IDs = new List<int> { departmentId } }).FirstOrDefault();

            if (depernment != null) AddHigherPositions(context, res, depernment.ParentId ?? -1);
        }

        /// <summary>
        /// Возвращает Id должнсотей, которые выше по Order и в выше стоящих отделах
        /// </summary>
        /// <param name="context"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        public List<int> GetParentPositions(IContext context, int positionId)
        {
            var position = _dictDb.GetInternalPositions(context, new FilterDictionaryPosition { IDs = new List<int> { positionId } }).FirstOrDefault();

            List<int> res = new List<int>();

            // Все вышестоящие в моем отделе
            AddHigherPositions(context, res, position.DepartmentId, position.Order);

            return res;
        }

        public void SetPositionOrder(IContext context, ModifyPositionOrder model)
        {
            var position = _dictDb.GetPosition(context, model.PositionId);

            if (position == null) return;

            // список должностей, которые подчиненны томуже отделу
            // сортировка Order, Name
            var positions = (List<SortPositoin>)_dictDb.GetPositionsForSort(context,
                new FilterDictionaryPosition
                {
                    DepartmentIDs = new List<int> { position.DepartmentId },
                    NotContainsIDs = new List<int> { model.PositionId }
                });

            var sp = new SortPositoin { Id = model.PositionId, NewOrder = model.Order };

            if (model.Order > positions.Count)
            {
                positions.Add(sp);
            }
            else if (model.Order <= 1)
            {
                positions.Insert(0, sp);
            }
            else
            {
                positions.Insert(model.Order - 1, sp);
            }

            int i = 0;
            foreach (var item in positions)
            {
                item.NewOrder = ++i;

                if (item.NewOrder != item.OldOrder)
                {
                    _dictDb.UpdatePositionOrder(context, item.Id, item.NewOrder);
                }
            }

        }

        public void DeletePositions(IContext context, List<int> list)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                // Удаляю настройку ролей для должности
                _adminDb.DeletePositionRoles(context, new FilterAdminPositionRole { PositionIDs = list });

                // Удаляю настройку рассылки
                _adminDb.DeleteSubordinations(context, new FilterAdminSubordination { PositionIDs = list });

                // Удаляю настройку сенд лист ????
                //#region [+] StandartSendListContents ...
                //var filterStandartSendListContents = PredicateBuilder.New<DictionaryStandartSendListContents>(false);
                //filterStandartSendListContents = list.Aggregate(filterStandartSendListContents,
                //    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                //dbContext.DictionaryStandartSendListContentsSet.Where(filterStandartSendListContents).Delete();
                //#endregion

                // Удаляю настройку журналов
                _adminDb.DeleteRegistrationJournalPositions(context, new FilterAdminRegistrationJournalPosition { PositionIDs = list });

                // Удаляю руководителей подразделений
                //var filterDepartments = PredicateBuilder.New<DictionaryDepartments>(false);
                //filterDepartments = list.Aggregate(filterDepartments,
                //    (current, value) => current.Or(e => e.ChiefPositionId == value).Expand());

                //dbContext.DictionaryDepartmentsSet.Where(filterDepartments).Update(x => new DictionaryDepartments() { ChiefPositionId = null });

                // Удаляю настройку ролей для исполнителей
                _adminDb.DeleteUserRoles(context, new FilterAdminUserRole { PositionIDs = list });

                // Удаляю исполнителей
                #region [+] PositionExecutors ...
                var executors = _dictDb.GetPositionExecutorsIDs(context, new FilterDictionaryPositionExecutor { PositionIDs = list });

                foreach (var executor in executors)
                {
                    ExecuteAction(EnumDictionaryActions.DeleteExecutor, context, executor);
                }

                #endregion

                // Удаляю сами должности
                _dictDb.DeletePositions(context, new FilterDictionaryPosition { IDs = list });

                transaction.Complete();
            }
        }

        #endregion DictionaryPositions

        // Исполнители
        #region DictionaryPositinExecutors
        public FrontDictionaryPositionExecutor GetDictionaryPositionExecutor(IContext context, int id)
        {
            return _dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor() { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryPositionExecutor> GetDictionaryPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter)
        {
            return _dictDb.GetPositionExecutors(context, filter);
        }

        public IEnumerable<AutocompleteItem> GetShortListPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.IsActive = true;
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;
            //filter.PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { EnumPositionExecutionTypes.Personal, EnumPositionExecutionTypes.IO };

            return _dictDb.GetShortListPositionExecutors(context, filter, paging);
        }

        public IEnumerable<FrontDictionaryPositionExecutor> GetUserPositionExecutors(IContext context, int positionId, FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.PositionIDs = new List<int> { positionId };
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;
            filter.IsActive = true;

            // Должности к которым текущий пользователь имеет отношение на текущий момоент
            var myPositions = GetCurrentPositionExecutors(context);

            // Если запрашиваются назначения должности, к которой текущий пользователь НЕ имеет отношение
            if (!myPositions.Any(x => x.PositionId == positionId)) return new List<FrontDictionaryPositionExecutor>();

            return _dictDb.GetPositionExecutors(context, filter, EnumSortPositionExecutors.PositionExecutorType_ExecutorName);
        }

        // Возвращает актуальные назначения текущего пользователя
        public IEnumerable<FrontDictionaryPositionExecutor> GetCurrentPositionExecutors(IContext context)
        {
            return _dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
            {
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                AgentIDs = new List<int> { context.CurrentAgentId }
            });
        }

        public int GetPositionPersonalAgent(IContext context, int positionId)
        {
            var position = _dictDb.GetInternalPositions(context, new FilterDictionaryPosition
            {
                IDs = new List<int> { positionId }
            }).FirstOrDefault();

            if (position == null) return -1;

            return position.ExecutorAgentId ?? -1;
        }

        public IEnumerable<FrontDictionaryPositionExecutor> GetCurrentPositionExecutorsByAgent(IContext context, int agentId, FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            if (filter.AgentIDs == null) filter.AgentIDs = new List<int> { agentId };
            else filter.AgentIDs.Add(agentId);

            return GetCurrentPositionExecutors(context, filter);
        }

        public IEnumerable<FrontDictionaryPositionExecutor> GetCurrentPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;
            filter.IsActive = true;

            return _dictDb.GetPositionExecutors(context, filter);
        }
        #endregion DictionaryPositinExecutors

        // Типы исполнителей
        #region DictionaryPositinExecutorTypes
        public IEnumerable<FrontDictionaryPositionExecutorType> GetDictionaryPositionExecutorTypes(IContext context, FilterDictionaryPositionExecutorType filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutorType();
            filter.IsActive = true;
            return _dictDb.GetPositionExecutorTypes(context, filter);
        }
        #endregion DictionaryPositinExecutorTypes

        // Журналы регистрации
        #region DictionaryRegistrationJournals
        public FrontDictionaryRegistrationJournal GetRegistrationJournal(IContext context, int id)
        {
            return _dictDb.GetRegistrationJournals(context, new FilterDictionaryRegistrationJournal { IDs = new List<int> { id } }, null, null).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter, UIPaging paging, UISorting sorting)
        {
            return _dictDb.GetRegistrationJournals(context, filter, paging, sorting);
        }

        public IEnumerable<FrontDictionaryRegistrationJournal> GetMainRegistrationJournals(IContext context, FullTextSearch ftSearch, FilterDictionaryRegistrationJournal filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get(context, Modules.Journal, ftSearch, filter, paging, sorting, _dictDb.GetRegistrationJournals, _dictDb.GetRegistrationJournalIDs);
        }

        public IEnumerable<ITreeItem> GetRegistrationJournalsFilter(IContext context, bool searchInJournals, FullTextSearch ftSearch, FilterDictionaryJournalsTree filter)
        {
            if (filter == null) filter = new FilterDictionaryJournalsTree();
            return GetRegistrationJournalsTree(context, searchInJournals, ftSearch, filter);
        }

        private IEnumerable<ITreeItem> GetRegistrationJournalsTree(IContext context, bool searchInJournals, FullTextSearch ftSearch, FilterDictionaryJournalsTree filter, FilterDictionaryRegistrationJournal filterJoirnal = null)
        {

            int levelCount = filter?.LevelCount ?? 0;
            //IEnumerable<TreeItem> journals = null;
            IEnumerable<TreeItem> departments = null;
            IEnumerable<TreeItem> companies = null;

            //if (levelCount >= 3 || levelCount == 0)
            //{
            //    var f = filterJoirnal ?? new FilterDictionaryRegistrationJournal { IsActive = filter?.IsActive };

            //    journals = _dictDb.GetRegistrationJournalsForRegistrationJournals(context, f);
            //}

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForRegistrationJournals(context, new FilterDictionaryDepartment()
                {
                    IsActive = filter?.IsActive,
                    ExcludeDepartmentsWithoutJournals = !filter.IsShowAll,
                    //IDs = filter.DepartmentIDs
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                var f = new FilterDictionaryAgentOrg()
                {
                    IsActive = filter?.IsActive,
                };

                if (!filter.IsShowAll.HasValue)
                {
                    f.DepartmentIDs = departments.Select(x => x.Id).ToList();
                }

                companies = _dictDb.GetAgentOrgsForStaffList(context, f);
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            //if (journals != null) flatList.AddRange(journals);
            if (departments != null) flatList.AddRange(departments);

            if (filter == null) filter = new FilterDictionaryJournalsTree();

            // Полнотекстовый поиск
            if (!string.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                var service = DmsResolver.Current.Get<IFullTextSearchService>();
                var ftDict = new Dictionary<EnumObjects, List<int>>();
                var depList = new List<int>();
                var cmpList = new List<int>();
                bool IsNotAll;
                // Если включен поиск по журналам
                if (searchInJournals)
                {
                    // ищу только в журналах
                    var journals = service.SearchItemParentId(out IsNotAll, context, ftSearch.FullTextSearchString,
                    new FullTextSearchFilter { Module = Modules.Journal });

                    if (journals?.Count > 0)
                    {
                        // отделы только из найденных журналов
                        depList.AddRange(_dictDb.GetDepartmentIDs(context, new FilterDictionaryDepartment { JournalIDs = journals }));

                        // организации только из найденных отделов
                        cmpList = _dictDb.GetAgentOrgIDs(context, new FilterDictionaryAgentOrg { DepartmentIDs = depList });
                    }
                }
                else
                {
                    depList = service.SearchItemParentId(out IsNotAll, context, ftSearch.FullTextSearchString,
                    new FullTextSearchFilter { Module = Modules.Department });

                    // Получаю список ид из полнотекста
                    cmpList = service.SearchItemParentId(out IsNotAll, context, ftSearch.FullTextSearchString,
                        new FullTextSearchFilter { Module = Modules.Org });
                }

                ftDict.Add(EnumObjects.DictionaryAgentClientCompanies, cmpList);
                ftDict.Add(EnumObjects.DictionaryDepartments, depList);

                if (ftDict.Count == 0) return new List<TreeItem>();

                filter.DicIDs = ftDict;
            }


            var res = Tree.Get(flatList, filter);

            if (!string.IsNullOrEmpty(ftSearch?.FullTextSearchString) && (!ftSearch?.IsDontSaveSearchQueryLog ?? false) && res != null && res.Any())
            {
                DmsResolver.Current.Get<ILogger>().AddSearchQueryLog(context, Modules.Journal, ftSearch?.FullTextSearchString);
            }

            return res;
        }

        public IEnumerable<AutocompleteItem> GetRegistrationJournalsShortList(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            if (filter == null) filter = new FilterDictionaryRegistrationJournal();

            filter.IsActive = true;

            return _dictDb.GetShortListRegistrationJournals(context, filter);

            //IEnumerable<TreeItem> journals = null;
            //IEnumerable<TreeItem> departments = null;
            //IEnumerable<TreeItem> companies = null;

            //// журналы
            //var f = new FilterDictionaryRegistrationJournal
            //{
            //    IsActive = true
            //};

            //journals = _dictDb.GetRegistrationJournalsShortList(context, f);


            //// отделы
            //departments = _dictDb.GetDepartmentsShortList(context, new FilterDictionaryDepartment()
            //{
            //    IsActive = true,
            //    ExcludeDepartmentsWithoutJournals = true,
            //});

            //companies = _dictDb.GetAgentOrgsShortList(context, new FilterDictionaryAgentOrg()
            //{
            //    IsActive = true,
            //    DepartmentIDs = departments.Select(x => x.Id).ToList(),
            //});


            //List<TreeItem> flatList = new List<TreeItem>();

            //if (companies != null) flatList.AddRange(companies);
            //if (journals != null) flatList.AddRange(journals);
            //if (departments != null) flatList.AddRange(departments);

            //var fTree = new FilterTree() { Name = ftSearch?.FullTextSearchString };

            //var res = Tree.GetList(Tree.Get(flatList, fTree));

            //return res;
        }


        #endregion DictionaryRegistrationJournals

        // Компании
        #region DictionaryAgentClientCompanies
        public FrontDictionaryAgentClientCompany GetDictionaryAgentClientCompany(IContext context, int id)
        {

            return _dictDb.GetAgentOrgs(context, new FilterDictionaryAgentOrg { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAgentClientCompany> GetDictionaryAgentClientCompanies(IContext context, FilterDictionaryAgentOrg filter)
        {

            return _dictDb.GetAgentOrgs(context, filter);
        }
        #endregion DictionaryCompanies

        #region DictionaryResultTypes
        public FrontDictionaryResultType GetDictionaryResultType(IContext context, int id)
        {
            return _dictDb.GetResultTypes(context, new FilterDictionaryResultType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryResultType> GetDictionaryResultTypes(IContext context, FilterDictionaryResultType filter)
        {
            return _dictDb.GetResultTypes(context, filter);
        }
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        public FrontDictionarySendType GetDictionarySendType(IContext context, int id)
        {
            return _dictDb.GetSendTypes(context, new FilterDictionarySendType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionarySendType> GetDictionarySendTypes(IContext context, FilterDictionarySendType filter)
        {
            return _dictDb.GetSendTypes(context, filter);
        }
        #endregion DictionarySendTypes

        #region DictionaryStageTypes

        public IEnumerable<ListItem> GetDictionaryStageTypes(IContext context, FilterDictionaryStageType filter)
        {
            return _dictDb.GetStageTypes(context, filter);
        }
        #endregion DictionaryStageTypes

        #region DictionaryStandartSendListContents
        public FrontDictionaryStandartSendListContent GetDictionaryStandartSendListContent(IContext context, int id)
        {

            return _dictDb.GetStandartSendListContents(context, new FilterDictionaryStandartSendListContent { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {

            return _dictDb.GetStandartSendListContents(context, filter);
        }
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        public FrontDictionaryStandartSendList GetStandartSendList(IContext context, int id)
        {
            return _dictDb.GetStandartSendLists(context, new FilterDictionaryStandartSendList { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public FrontDictionaryStandartSendList GetUserStandartSendList(IContext context, int id)
        {
            return _dictDb.GetStandartSendLists(context, new FilterDictionaryStandartSendList { IDs = new List<int> { id }, AgentId = context.CurrentAgentId }, null).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {
            return _dictDb.GetStandartSendLists(context, filter, null);
        }

        public IEnumerable<AutocompleteItem> GetStandartSendListsShortList(IContext ctx, FilterDictionaryStandartSendList filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryStandartSendList();

            filter.IsActive = true;

            return _dictDb.GetStandartSendListsShortList(ctx, filter, paging);
        }

        public IEnumerable<FrontMainDictionaryStandartSendList> GetMainStandartSendLists(IContext context, FullTextSearch ftSearch, FilterDictionaryStandartSendList filter, bool SearchInPositionsOnly = false)
        {

            if (!string.IsNullOrEmpty(ftSearch?.FullTextSearchString) && SearchInPositionsOnly)
            {
                var positions = _dictDb.GetPositionIDs(context, new FilterDictionaryPosition { NameDepartmentExecutor = ftSearch?.FullTextSearchString }).ToList();

                if (!positions.Any()) return new List<FrontMainDictionaryStandartSendList>();

                if (filter == null) filter = new FilterDictionaryStandartSendList();

                filter.PositionIDs = positions;

                ftSearch.FullTextSearchString = string.Empty;
            }

            return FTS.Get(context, Modules.SendList, ftSearch, filter, null, null, _dictDb.GetMainStandartSendLists, _dictDb.GetStandartSendListIDs);
        }

        public IEnumerable<FrontMainDictionaryStandartSendList> GetMainUserStandartSendLists(IContext context, FullTextSearch ftSearch, FilterDictionaryStandartSendList filter)
        {
            if (filter == null) filter = new FilterDictionaryStandartSendList();

            filter.AgentId = context.CurrentAgentId;

            var res = GetMainStandartSendLists(context, ftSearch, filter);

            //res = res.ToList();

            // Добавляю должности, у которых еще нет типовых списков, но за которых пользователю разрешено работать
            var l = _dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
            {
                AgentIDs = new List<int> { context.CurrentAgentId },
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                ExistExecutorAgentInPositions = true,
                IsActive = true,
                NotContainsPositionIDs = res.Select(x => x.Id).ToList()
            }).ToList();

            if (l.Count() > 0)
            {
                res = res.Concat(l.Select(x => new FrontMainDictionaryStandartSendList
                {
                    Id = x.PositionId,
                    Name = x.PositionName,
                    ExecutorName = x.AgentName,
                    ExecutorTypeSuffix = x.PositionExecutorTypeSuffix,
                    DepartmentIndex = x.DepartmentIndex,
                    DepartmentName = x.DepartmentName,
                }));

                res = res.OrderBy(x => x.Name).ToList();
            }

            return res;
        }

        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        public IEnumerable<ListItem> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {

            return _dictDb.GetSubordinationTypes(context, filter);
        }
        #endregion DictionarySubordinationTypes

        #region DictionaryTags
        public IEnumerable<FrontMainTag> GetMainTags(IContext context, FullTextSearch ftSearch, FilterDictionaryTag filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get(context, Modules.Tags, ftSearch, filter, paging, sorting, _dictDb.GetMainTags, _dictDb.GetTagIDs);
        }

        public IEnumerable<ListItem> GetTagList(IContext context, FilterDictionaryTag filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryTag();

            filter.IsActive = true;

            return _dictDb.GetTagList(context, filter, paging);
        }

        public FrontTag GetTag(IContext context, int id)
        {
            return _dictDb.GetTag(context, new FilterDictionaryTag { IDs = new List<int> { id } }).FirstOrDefault();
        }
        #endregion DictionaryTags

        #region AdminAccessLevels
        public FrontAdminAccessLevel GetAdminAccessLevel(IContext context, int id)
        {
            return _dictDb.GetAdminAccessLevels(context, new FilterAdminAccessLevel { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter)
        {
            return _dictDb.GetAdminAccessLevels(context, filter);
        }

        #endregion AdminAccessLevels

        #region CustomDictionaryTypes
        public IEnumerable<FrontCustomDictionaryType> GetMainCustomDictionaryTypes(IContext context, FullTextSearch ftSearch, FilterCustomDictionaryType filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get(context, Modules.CustomDictionaries, ftSearch, filter, paging, sorting, _dictDb.GetMainCustomDictionaryTypes, _dictDb.GetCustomDictionaryTypeIDs);
        }

        public IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter)
        {
            return _dictDb.GetCustomDictionaryTypes(context, filter);
        }

        public FrontCustomDictionaryType GetCustomDictionaryType(IContext context, int id)
        {
            return GetCustomDictionaryTypes(context, new FilterCustomDictionaryType { IDs = new List<int> { id } }).FirstOrDefault();
        }
        #endregion CustomDictionaryTypes

        #region CustomDictionaries
        public IEnumerable<FrontCustomDictionary> GetMainCustomDictionaries(IContext context, FullTextSearch ftSearch, FilterCustomDictionary filter, UIPaging paging, UISorting sorting)
        {
            return FTS.Get(context, Modules.CustomDictionaries, ftSearch, filter, paging, sorting,
                _dictDb.GetMainCustomDictionaries, _dictDb.GetCustomDictionarieIDs,
                new FullTextSearchFilter { Module = Modules.CustomDictionaries, ParentObjectId = filter.TypeId },
                IsUseParentId: false);
        }

        public IEnumerable<FrontCustomDictionary> GetCustomDictionaries(IContext context, FilterCustomDictionary filter, UIPaging paging, UISorting sorting)
        {
            return _dictDb.GetMainCustomDictionaries(context, filter, paging, sorting);
        }

        public FrontCustomDictionary GetCustomDictionary(IContext context, int id)
        {
            return _dictDb.GetMainCustomDictionaries(context, new FilterCustomDictionary { IDs = new List<int> { id } }, null, null).FirstOrDefault();
        }
        #endregion CustomDictionaries

        //public IEnumerable<ITreeItem> GetStaffList(IContext context, FilterTree filter)
        //{
        //    return _dictDb.GetStaffList(context, filter);
        //}

        //public void AddStaffList(IContext context)
        //{
        //    _dictDb.AddStaffList(context);
        //}

        #region [+] StaffList ...


        public IEnumerable<ITreeItem> GetStaffList(IContext context, FullTextSearch ftSearch, FilterDictionaryStaffList filter)
        {

            // Тонкий момент, проверяю не является ли сотрудник локальным администратором.
            // Если не локальный значит, надеюсь, что глобальный и отображаю все
            var adminService = DmsResolver.Current.Get<IAdminService>();
            var employeeDepartments = adminService.GetInternalEmployeeDepartments(context, context.Employee.Id);

            if (employeeDepartments != null)
            {
                List<int> safeList = new List<int>();

                var deps = _dictDb.GetInternalDepartments(context, new FilterDictionaryDepartment { IDs = employeeDepartments });

                safeList.AddRange(employeeDepartments);

                // собираю список Id включая родительские отделы
                foreach (var dep in deps)
                {
                    if (dep.Path != null) safeList.AddRange(dep.Path?.Split('/').Select(x => int.Parse(x)));
                }

                safeList = safeList.Distinct().ToList();

                // Если передан фильтр, то проверяю чтобы переданный id были из safeList
                if (filter?.DepartmentIDs?.Count() > 0)
                {
                    filter.DepartmentIDs = filter.DepartmentIDs.Where(x => safeList.Contains(x)).ToList();
                }
                else
                {
                    if (filter == null) filter = new FilterDictionaryStaffList();
                    filter.DepartmentIDs = safeList;
                }

            }

            int levelCount = filter?.LevelCount ?? 0;
            IEnumerable<TreeItem> executors = null;
            IEnumerable<TreeItem> positions = null;
            IEnumerable<TreeItem> departments = null;
            IEnumerable<TreeItem> companies = null;

            if (levelCount >= 4 || levelCount == 0)
            {
                executors = _dictDb.GetPositionExecutorsForTree(context, new FilterDictionaryPositionExecutor()
                {
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow,
                    IsActive = filter?.IsActive,
                });
            }

            if (levelCount >= 3 || levelCount == 0)
            {
                positions = _dictDb.GetPositionsForStaffList(context, new FilterDictionaryPosition()
                {
                    IsActive = filter?.IsActive,
                    DepartmentIDs = employeeDepartments,
                });
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForStaffList(context, new FilterDictionaryDepartment()
                {
                    IsActive = filter?.IsActive,
                    IDs = filter?.DepartmentIDs
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                companies = _dictDb.GetAgentOrgsForStaffList(context, new FilterDictionaryAgentOrg()
                {
                    IsActive = filter?.IsActive,
                    DepartmentIDs = employeeDepartments,
                });
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (positions != null) flatList.AddRange(positions);
            if (departments != null) flatList.AddRange(departments);
            if (executors != null) flatList.AddRange(executors);

            // Полнотекстовый поиск
            if (!string.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                var service = DmsResolver.Current.Get<IFullTextSearchService>();
                var ftDict = new Dictionary<EnumObjects, List<int>>();
                var list = new List<int>();
                bool IsNotAll;
                // Получаю список ид из полнотекста
                list = service.SearchItemParentId(out IsNotAll, context, ftSearch.FullTextSearchString, new FullTextSearchFilter { Module = Modules.Org });
                if (list.Count > 0) ftDict.Add(EnumObjects.DictionaryAgentClientCompanies, list);

                list = service.SearchItemParentId(out IsNotAll, context, ftSearch.FullTextSearchString, new FullTextSearchFilter { Module = Modules.Department });
                if (list.Count > 0) ftDict.Add(EnumObjects.DictionaryDepartments, list);

                list = service.SearchItemParentId(out IsNotAll, context, ftSearch.FullTextSearchString, new FullTextSearchFilter { Module = Modules.Position });
                if (list.Count > 0) ftDict.Add(EnumObjects.DictionaryPositions, list);

                list = service.SearchItemParentId(out IsNotAll, context, ftSearch.FullTextSearchString, new FullTextSearchFilter { Module = Modules.Employee });

                if (list.Count > 0)
                {
                    list = _dictDb.GetPositionExecutorsIDs(context, new FilterDictionaryPositionExecutor { AgentIDs = list });
                    if (list.Count > 0) ftDict.Add(EnumObjects.DictionaryPositionExecutors, list);
                }

                if (ftDict.Count == 0) return new List<TreeItem>();

                filter.DicIDs = ftDict;
            }

            var res = Tree.Get(flatList, filter);

            if (!string.IsNullOrEmpty(ftSearch?.FullTextSearchString) && (!ftSearch?.IsDontSaveSearchQueryLog ?? false) && res != null && res.Any())
            {
                DmsResolver.Current.Get<ILogger>().AddSearchQueryLog(context, Modules.Org, ftSearch?.FullTextSearchString);
            }

            return res;
        }

        #endregion


        public IEnumerable<int> GetFavouriteList(IContext context, IEnumerable<ListItem> list, string module, string feature)
        {
            // список избранных, отсортированный по актуальности
            var fList = _dictDb.GetFavouriteList(context, module, feature);

            // только те которые содержатся в списке
            fList = fList.Where(x => list.Any(y => y.Id == x));

            // беру 10 актуальных
            fList = fList.Take(10);

            return fList;
        }

        public FrontUserFavorites GetUserFavourites(IContext context)
        {
            var res = new FrontUserFavorites();

            res.ListEmployees = _dictDb.GetFavouriteList(context, Modules.List, Features.Employees).ToList();
            res.ListCompanies = _dictDb.GetFavouriteList(context, Modules.List, Features.Companies).ToList();
            res.ListDepartments = _dictDb.GetFavouriteList(context, Modules.List, Features.Departments).ToList();
            res.ListJournals = _dictDb.GetFavouriteList(context, Modules.List, Features.Journals).ToList();
            res.ListPersons = _dictDb.GetFavouriteList(context, Modules.List, Features.Persons).ToList();
            res.ListPositions = _dictDb.GetFavouriteList(context, Modules.List, Features.Positions).ToList();
            res.ListTags = _dictDb.GetFavouriteList(context, Modules.List, Features.Tags).ToList();

            return res;
        }

        private List<InternalAgentFavourite> PrepareAgentFavouriteItems(IContext context, List<int> list, string module, string feature)
        {
            var items = list.Select(x => new InternalAgentFavourite
            {
                ObjectId = x,
                AgentId = context.CurrentAgentId,
                Date = DateTime.UtcNow,
                Module = module,
                Feature = feature,

            }).ToList();

            CommonDocumentUtilities.SetLastChange(context, items);

            return items;

        }

        public void SetUserFavorite(IContext context, AddAgentFavourite model)
        {
            var uf = _dictDb.GetInternalAgentFavourite(context, new FilterAgentFavourite
            {
                AgentIDs = new List<int> { context.CurrentAgentId },
                ObjectIDs = new List<int> { model.ObjectId },
                FeatureExact = model.Feauture,
                ModuleExact = model.Module,
            }).FirstOrDefault();

            if (uf == null)
            {
                _dictDb.AddAgentFavourite(context, uf);
            }
            else
            {
                uf.Date = DateTime.UtcNow;
                _dictDb.UpdateAgentFavourite(context, uf);
            }
        }

        public void SetUserFavoritesBulk(IContext context, FrontUserFavorites model)
        {
            if (model == null) return;

            using (var transaction = Transactions.GetTransaction())
            {
                _dictDb.DeleteAgentFavourite(context, new FilterAgentFavourite { AgentIDs = new List<int> { context.CurrentAgentId } });

                if (model.ListEmployees?.Count > 0)
                {
                    var items = PrepareAgentFavouriteItems(context, model.ListEmployees, Modules.List, Features.Employees);
                    _dictDb.AddAgentFavourites(context, items);
                }

                if (model.ListCompanies?.Count > 0)
                {
                    var items = PrepareAgentFavouriteItems(context, model.ListCompanies, Modules.List, Features.Companies);
                    _dictDb.AddAgentFavourites(context, items);
                }

                if (model.ListDepartments?.Count > 0)
                {
                    var items = PrepareAgentFavouriteItems(context, model.ListDepartments, Modules.List, Features.Departments);
                    _dictDb.AddAgentFavourites(context, items);
                }

                if (model.ListJournals?.Count > 0)
                {
                    var items = PrepareAgentFavouriteItems(context, model.ListJournals, Modules.List, Features.Journals);
                    _dictDb.AddAgentFavourites(context, items);
                }

                if (model.ListPersons?.Count > 0)
                {
                    var items = PrepareAgentFavouriteItems(context, model.ListPersons, Modules.List, Features.Persons);
                    _dictDb.AddAgentFavourites(context, items);
                }

                if (model.ListPositions?.Count > 0)
                {
                    var items = PrepareAgentFavouriteItems(context, model.ListPositions, Modules.List, Features.Positions);
                    _dictDb.AddAgentFavourites(context, items);
                }

                if (model.ListTags?.Count > 0)
                {
                    var items = PrepareAgentFavouriteItems(context, model.ListTags, Modules.List, Features.Tags);
                    _dictDb.AddAgentFavourites(context, items);
                }

                transaction.Complete();

            }

        }


    }
}
