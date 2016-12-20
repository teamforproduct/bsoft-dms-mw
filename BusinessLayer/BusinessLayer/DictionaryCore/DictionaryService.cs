using System;
using BL.Logic.DictionaryCore.Interfaces;
using System.Collections.Generic;
using BL.Model.SystemCore;
using BL.Database.Dictionaries.Interfaces;
using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.Common;
using BL.Model.Tree;
using BL.Logic.TreeBuilder;
using static BL.Database.Dictionaries.DictionariesDbProcess;
using BL.Model.DictionaryCore.FrontMainModel;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Logic.DictionaryCore
{
    public class DictionaryService : IDictionaryService
    {
        private readonly IDictionariesDbProcess _dictDb;
        private readonly ICommandService _commandService;

        public DictionaryService(IDictionariesDbProcess dictDb, ICommandService commandService)
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


        public IEnumerable<FrontDictionaryAgent> GetAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging)
        {
            var newFilter = new FilterDictionaryAgent();
            if (!string.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgents, filter.FullTextSearchString, ResolveSearchResultAgents);

            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetAgents(context, newFilter, paging);
        }

        private List<int> GetIDsForDictionaryFullTextSearch(IContext context, EnumObjects dictionaryType, string filter, Func<IContext, IEnumerable<FullTextSearchResult>, IEnumerable<FullTextSearchResult>> filterFunct = null)
        {
            var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
            var ftRes = ftService.SearchDictionary(context, filter);
            if (filterFunct == null)
            {
                ftRes = ftRes.Where(x => x.ObjectType == dictionaryType);
            }
            else
            {
                ftRes = filterFunct(context, ftRes);
            }

            var resWithRanges =
                ftRes.GroupBy(x => x.ObjectId)
                    .Select(x => new { DocId = x.Key, Rate = x.Max(s => s.Score) })
                    .OrderByDescending(x => x.Rate);
            if (resWithRanges.Any())
            {

                return resWithRanges.Select(x => x.DocId).ToList();
            }
            return new List<int> { -1 };
        }

        private IEnumerable<FullTextSearchResult> ResolveSearchResultAgents(IContext ctx, IEnumerable<FullTextSearchResult> ftRes)
        {

            var agentTypes = new List<EnumObjects>
            {
                EnumObjects.DictionaryAgents,
                EnumObjects.DictionaryAgentBanks,
                EnumObjects.DictionaryAgentCompanies,
                EnumObjects.DictionaryAgentEmployees,
                EnumObjects.DictionaryAgentPersons
            };

            var res = new List<FullTextSearchResult>();
            res.AddRange(ftRes
                .Where(x => agentTypes.Contains(x.ObjectType)));

            // Внимание GetAgentsIDByAddress клиентозависимый!!!
            var tmp = _dictDb.GetAgentsIDByAddress(ctx,
                ftRes.Where(x => x.ObjectType == EnumObjects.DictionaryAgentAddresses).Select(y => y.ObjectId).ToList());

            res.AddRange(tmp.Select(x => new FullTextSearchResult
            {
                DocumentId = 0,
                ObjectId = x,
                ObjectType = EnumObjects.DictionaryAgents,
                Score = 0
            }));

            tmp = _dictDb.GetAgentsIDByContacts(ctx,
                ftRes.Where(x => x.ObjectType == EnumObjects.DictionaryContacts).Select(y => y.ObjectId).ToList());

            res.AddRange(tmp.Select(x => new FullTextSearchResult
            {
                DocumentId = 0,
                ObjectId = x,
                ObjectType = EnumObjects.DictionaryAgents,
                Score = 0
            }));

            return res;
        }


        //public bool IsAgentOneRole(IContext context, int id, EnumDictionaryAgentTypes source)
        //{

        //    var agent = GetDictionaryAgent(context, id);

        //    switch (source)
        //    {
        //        case EnumDictionaryAgentTypes.isEmployee:
        //            if (!agent.IsCompany && !agent.IsBank) { return true; }
        //            break;
        //        case EnumDictionaryAgentTypes.isCompany:
        //            if (!agent.IsIndividual && !agent.IsEmployee && !agent.IsBank) { return true; }
        //            break;
        //        case EnumDictionaryAgentTypes.isIndividual:
        //            if (!agent.IsCompany && !agent.IsBank) { return true; }
        //            break;
        //        case EnumDictionaryAgentTypes.isBank:
        //            if (!agent.IsEmployee && !agent.IsCompany && !agent.IsIndividual) { return true; }
        //            break;
        //    }
        //    return false;
        //}

        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        public FrontDictionaryAgentPerson GetAgentPerson(IContext context, int id)
        {
            return _dictDb.GetAgentPersons(context, new FilterDictionaryAgentPerson { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            var newFilter = new FilterDictionaryAgentPerson();

            if (!string.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentPersons, filter.FullTextSearchString);
            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetAgentPersons(context, newFilter, paging);
        }
        #endregion DictionaryAgentPersons

        #region DicionaryAgentEmployees

        public FrontDictionaryAgentEmployee GetDictionaryAgentEmployee(IContext context, int id)
        {
            return _dictDb.GetAgentEmployee(context, id);
        }

        public IEnumerable<ListItem> GetAgentEmployeeList(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryAgentEmployee();

            filter.IsActive = true;

            return _dictDb.GetAgentEmployeeList(context, filter, paging);
        }

        public IEnumerable<FrontMainDictionaryAgentEmployee> GetDictionaryAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {

            var newFilter = new FilterDictionaryAgentEmployee();

            if (!String.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentEmployees, filter.FullTextSearchString);

            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetAgentEmployeesMain(context, newFilter, paging);

        }

        public FrontDictionaryAgentUserPicture GetDictionaryAgentUserPicture(IContext context, int employeeId)
        {
            var userPicture = _dictDb.GetInternalAgentImage(context, employeeId);

            string fileContect = string.Empty;

            if (userPicture.Image != null)
                fileContect = Convert.ToBase64String(userPicture.Image);

            var uPic = new FrontDictionaryAgentUserPicture()
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

        public FrontDictionaryAddressType GetDictionaryAddressType(IContext context, int id)
        {
            return _dictDb.GetAddressTypes(context, new FilterDictionaryAddressType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            var newFilter = new FilterDictionaryAddressType();

            if (!String.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAddressType, filter.FullTextSearchString);

            }
            else
            {
                newFilter = filter;
            }


            return _dictDb.GetAddressTypes(context, newFilter);
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

        public IEnumerable<FrontContactPersons> GetContactPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            return _dictDb.GetContactPersons(context, filter);
        }

        public IEnumerable<FrontContactPersons> GetContactPerson(IContext context, int id)
        {
            return _dictDb.GetContactPersons(context, new FilterDictionaryAgentPerson { IDs = new List<int> { id } });
        }

        #endregion


        #region DictionaryAgentCompanies
        public FrontDictionaryAgentCompany GetAgentCompany(IContext context, int id)
        {
            return _dictDb.GetAgentCompanies(context, new FilterDictionaryAgentCompany { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<ListItem> GetAgentCompanyList(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryAgentCompany();

            filter.IsActive = true;

            return _dictDb.GetAgentCompanyList(context, filter, paging);
        }

        public IEnumerable<FrontDictionaryAgentCompany> GetAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {

            var newFilter = new FilterDictionaryAgentCompany();
            if (!String.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentCompanies, filter.FullTextSearchString, ResolveSearchResultAgentCompanies);

            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetAgentCompanies(context, newFilter, paging);

        }


        private IEnumerable<FullTextSearchResult> ResolveSearchResultAgentCompanies(IContext ctx, IEnumerable<FullTextSearchResult> ftRes)
        {

            var agentTypes = new List<EnumObjects>
            {

                EnumObjects.DictionaryAgentCompanies,

            };

            var res = new List<FullTextSearchResult>();
            res.AddRange(ftRes
                .Where(x => agentTypes.Contains(x.ObjectType)));

            var tmp = _dictDb.GetAgentsIDByAddress(ctx,
                ftRes.Where(x => x.ObjectType == EnumObjects.DictionaryAgentAddresses).Select(y => y.ObjectId).ToList());

            res.AddRange(tmp.Select(x => new FullTextSearchResult
            {
                DocumentId = 0,
                ObjectId = x,
                ObjectType = EnumObjects.DictionaryAgents,
                Score = 0
            }));

            tmp = _dictDb.GetAgentsIDByContacts(ctx,
                ftRes.Where(x => x.ObjectType == EnumObjects.DictionaryContacts).Select(y => y.ObjectId).ToList());

            res.AddRange(tmp.Select(x => new FullTextSearchResult
            {
                DocumentId = 0,
                ObjectId = x,
                ObjectType = EnumObjects.DictionaryAgents,
                Score = 0
            }));

            return res;
        }

        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        public FrontDictionaryAgentBank GetAgentBank(IContext context, int id)
        {
            return _dictDb.GetAgentBanks(context, new FilterDictionaryAgentBank { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAgentBank> GetDictionaryAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            var newFilter = new FilterDictionaryAgentBank();
            if (!String.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentBanks, filter.FullTextSearchString, ResolveSearchResultAgentBanks);

            }
            else
            {

                newFilter = filter;
            }

            return _dictDb.GetAgentBanks(context, newFilter, paging);

        }

        private IEnumerable<FullTextSearchResult> ResolveSearchResultAgentBanks(IContext ctx, IEnumerable<FullTextSearchResult> ftRes)
        {

            var agentTypes = new List<EnumObjects>
            {

                EnumObjects.DictionaryAgentBanks,

            };

            var res = new List<FullTextSearchResult>();
            res.AddRange(ftRes
                .Where(x => agentTypes.Contains(x.ObjectType)));

            var tmp = _dictDb.GetAgentsIDByAddress(ctx,
                ftRes.Where(x => x.ObjectType == EnumObjects.DictionaryAgentAddresses).Select(y => y.ObjectId).ToList());

            res.AddRange(tmp.Select(x => new FullTextSearchResult
            {
                DocumentId = 0,
                ObjectId = x,
                ObjectType = EnumObjects.DictionaryAgents,
                Score = 0
            }));

            tmp = _dictDb.GetAgentsIDByContacts(ctx,
                ftRes.Where(x => x.ObjectType == EnumObjects.DictionaryContacts).Select(y => y.ObjectId).ToList());

            res.AddRange(tmp.Select(x => new FullTextSearchResult
            {
                DocumentId = 0,
                ObjectId = x,
                ObjectType = EnumObjects.DictionaryAgents,
                Score = 0
            }));

            return res;
        }

        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        public FrontDictionaryAgentUser GetDictionaryAgentUser(IContext context, int id)
        {
            return _dictDb.GetAgentUser(context, id);
        }
        #endregion DictionaryAgentBanks

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
            var newFilter = new FilterDictionaryContactType();

            if (!String.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryContactType, filter.FullTextSearchString);

            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetContactTypes(context, newFilter);
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

        // Тематики документов
        #region DictionaryDocumentSubjects
        public FrontDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id)
        {

            return _dictDb.GetDocumentSubjects(context, new FilterDictionaryDocumentSubject { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {

            return _dictDb.GetDocumentSubjects(context, filter);
        }
        #endregion DictionaryDocumentSubjects

        #region DictionaryDocumentTypes
        // следить за списком полей необхдимых в каждом конкретном случае
        public FrontDictionaryDocumentType GetDictionaryDocumentType(IContext context, int id)
        {
            return _dictDb.GetDocumentTypes(context, new FilterDictionaryDocumentType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {

            var newFilter = new FilterDictionaryDocumentType();

            if (!String.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAddressType, filter.FullTextSearchString);

            }
            else
            {
                newFilter = filter;
            }


            return _dictDb.GetDocumentTypes(context, newFilter);

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

            return _dictDb.GetPosition(context, id);
        }

        public IEnumerable<FrontDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter)
        {

            return _dictDb.GetPositions(context, filter);
        }

        public IEnumerable<ListItem> GetPositionList(IContext context, FilterDictionaryPosition filter)
        {
            return _dictDb.GetPositionList(context, filter);
        }

        public void SetPositionOrder(IContext context, ModifyPositionOrder model)
        {
            var position = _dictDb.GetPosition(context, model.PositionId);

            if (position == null) return;

            // список должностей, которые подчиненны томуже отделу
            // сортировка Order, Name
            List<SortPositoin> positions = (List<SortPositoin>)_dictDb.GetPositionsForSort(context,
                new FilterDictionaryPosition
                {
                    DepartmentIDs = new List<int> { position.DepartmentId },
                    NotContainsIDs = new List<int> { model.PositionId }
                });

            SortPositoin sp = new SortPositoin() { Id = model.PositionId, NewOrder = model.Order };

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
                { _dictDb.UpdatePositionOrder(context, item.Id, item.NewOrder); }
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
        public FrontDictionaryPositionExecutorType GetDictionaryPositionExecutorType(IContext context, int id)
        {

            return _dictDb.GetPositionExecutorTypes(context, new FilterDictionaryPositionExecutorType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryPositionExecutorType> GetDictionaryPositionExecutorTypes(IContext context, FilterDictionaryPositionExecutorType filter)
        {

            return _dictDb.GetPositionExecutorTypes(context, filter);
        }
        #endregion DictionaryPositinExecutorTypes

        // Журналы регистрации
        #region DictionaryRegistrationJournals
        public FrontDictionaryRegistrationJournal GetRegistrationJournal(IContext context, int id)
        {

            return _dictDb.GetRegistrationJournals(context, new FilterDictionaryRegistrationJournal { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {

            var newFilter = new FilterDictionaryRegistrationJournal();

            if (!String.IsNullOrEmpty(filter.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryRegistrationJournals, filter.FullTextSearchString);

            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetRegistrationJournals(context, newFilter);
        }

        public IEnumerable<ITreeItem> GetRegistrationJournalsTree(IContext context, FilterTree filter)
        {

            int levelCount = filter?.LevelCount ?? 0;
            IEnumerable<TreeItem> journals = null;
            IEnumerable<TreeItem> departments = null;
            IEnumerable<TreeItem> companies = null;

            if (levelCount >= 3 || levelCount == 0)
            {
                journals = _dictDb.GetRegistrationJournalsForRegistrationJournals(context, new FilterDictionaryRegistrationJournal()
                {
                    IsActive = filter?.IsActive
                });
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForRegistrationJournals(context, new FilterDictionaryDepartment()
                {
                    IsActive = filter?.IsActive,
                    //IDs = filter.DepartmentIDs
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                companies = _dictDb.GetAgentClientCompaniesForStaffList(context, new FilterDictionaryAgentClientCompany()
                {
                    IsActive = filter?.IsActive
                });
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (journals != null) flatList.AddRange(journals);
            if (departments != null) flatList.AddRange(departments);

            var res = Tree.Get(flatList, filter);

            //AddCodePathDepartment(res);

            return res;
        }

        #endregion DictionaryRegistrationJournals

        // Компании
        #region DictionaryAgentClientCompanies
        public FrontDictionaryAgentClientCompany GetDictionaryAgentClientCompany(IContext context, int id)
        {

            return _dictDb.GetAgentClientCompanies(context, new FilterDictionaryAgentClientCompany { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAgentClientCompany> GetDictionaryAgentClientCompanies(IContext context, FilterDictionaryAgentClientCompany filter)
        {

            return _dictDb.GetAgentClientCompanies(context, filter);
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
        public FrontDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id)
        {
            return _dictDb.GetStandartSendLists(context, new FilterDictionaryStandartSendList { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {
            return _dictDb.GetStandartSendLists(context, filter);
        }
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        public FrontDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id)
        {
            return _dictDb.GetSubordinationTypes(context, new FilterDictionarySubordinationType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {

            return _dictDb.GetSubordinationTypes(context, filter);
        }
        #endregion DictionarySubordinationTypes

        #region DictionaryTags
        public IEnumerable<FrontDictionaryTag> GetDictionaryTags(IContext context, FilterDictionaryTag filter)
        {
            return _dictDb.GetTags(context, filter);
        }

        public FrontDictionaryTag GetDictionaryTag(IContext context, int id)
        {
            return _dictDb.GetTags(context, new FilterDictionaryTag { IDs = new List<int> { id } }).FirstOrDefault();
        }
        #endregion DictionaryTags

        #region AdminAccessLevels
        public FrontAdminAccessLevel GetAdminAccessLevel(IContext context, int id)
        {
            return _dictDb.GetAdminAccessLevel(context, id);
        }

        public IEnumerable<FrontAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter)
        {
            return _dictDb.GetAdminAccessLevels(context, filter);
        }

        #endregion AdminAccessLevels

        #region CustomDictionaryTypes
        public IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter)
        {
            return _dictDb.GetCustomDictionaryTypes(context, filter);
        }

        public FrontCustomDictionaryType GetCustomDictionaryType(IContext context, int id)
        {
            return _dictDb.GetCustomDictionaryTypeWithCustomDictionaries(context, id);
        }
        #endregion CustomDictionaryTypes

        #region CustomDictionaries
        public IEnumerable<FrontCustomDictionary> GetCustomDictionaries(IContext context, FilterCustomDictionary filter)
        {
            return _dictDb.GetCustomDictionaries(context, filter);
        }

        public FrontCustomDictionary GetCustomDictionary(IContext context, int id)
        {
            return _dictDb.GetCustomDictionaries(context, new FilterCustomDictionary { IDs = new List<int> { id } }).FirstOrDefault();
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

        public void AddStaffList(IContext context)
        {
            for (int c = 1; c <= 10; c++)
            {
                int compID = _dictDb.AddAgentClientCompany(context, new InternalDictionaryAgentClientCompany()
                {
                    Name = string.Concat("Компания №", string.Format("{0:00}", c)),
                    FullName = string.Concat("Компания номер ", string.Format("{0:00}", c)),
                    IsActive = true,
                    LastChangeDate = DateTime.UtcNow,
                    LastChangeUserId = context.CurrentAgentId
                });

                int? depParId = null;

                for (int d = 1; d <= 100; d++)
                {

                    int depId = _dictDb.AddDepartment(context, new InternalDictionaryDepartment()
                    {
                        Code = string.Format("{0:000}/{0:000}", d, d),
                        Index = string.Format("{0:000}", d),
                        Name = string.Concat("Отдел №", string.Format("{0:000}", d)),
                        FullName = string.Concat("Отдел номер ", string.Format("{0:000}", d)),
                        IsActive = true,
                        CompanyId = compID,
                        ParentId = depParId,
                        LastChangeDate = DateTime.UtcNow,
                        LastChangeUserId = context.CurrentAgentId
                    });

                    if (d == 20 || d == 50 || d == 65 || d == 93) depParId = depId;

                    int posId = _dictDb.AddPosition(context, new InternalDictionaryPosition()
                    {
                        Name = "Руководитель отдела",
                        FullName = "Руководитель отдела",
                        IsActive = true,
                        DepartmentId = depId,
                        Order = 1,
                        LastChangeDate = DateTime.UtcNow,
                        LastChangeUserId = context.CurrentAgentId
                    });

                    posId = _dictDb.AddPosition(context, new InternalDictionaryPosition()
                    {
                        Name = "Менеджер по работе с клиентами",
                        FullName = "Менеджер по работе с клиентами",
                        IsActive = true,
                        DepartmentId = depId,
                        Order = 2,
                        LastChangeDate = DateTime.UtcNow,
                        LastChangeUserId = context.CurrentAgentId
                    });

                    posId = _dictDb.AddPosition(context, new InternalDictionaryPosition()
                    {
                        Name = "Менеджер по IT и дизайну",
                        FullName = "Менеджер по IT и дизайну",
                        IsActive = true,
                        DepartmentId = depId,
                        Order = 3,
                        LastChangeDate = DateTime.UtcNow,
                        LastChangeUserId = context.CurrentAgentId
                    });

                    posId = _dictDb.AddPosition(context, new InternalDictionaryPosition()
                    {
                        Name = "Менеджер по продажам",
                        FullName = "Менеджер по продажам",
                        IsActive = true,
                        DepartmentId = depId,
                        Order = 4,
                        LastChangeDate = DateTime.UtcNow,
                        LastChangeUserId = context.CurrentAgentId
                    });

                    posId = _dictDb.AddPosition(context, new InternalDictionaryPosition()
                    {
                        Name = "Рабочий",
                        FullName = "Рабочий",
                        IsActive = true,
                        DepartmentId = depId,
                        Order = 5,
                        LastChangeDate = DateTime.UtcNow,
                        LastChangeUserId = context.CurrentAgentId
                    });

                }
            }
        }

        public IEnumerable<ITreeItem> GetStaffList(IContext context, FilterDictionaryStaffList filter)
        {

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
                    IsActive = filter.IsActive
                });
            }

            if (levelCount >= 3 || levelCount == 0)
            {
                positions = _dictDb.GetPositionsForStaffList(context, new FilterDictionaryPosition()
                {
                    IsActive = filter.IsActive
                });
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForStaffList(context, new FilterDictionaryDepartment()
                {
                    IsActive = filter.IsActive,
                    IDs = filter.DepartmentIDs
                });
            }

            if (levelCount >= 1 || levelCount == 0)
            {
                companies = _dictDb.GetAgentClientCompaniesForStaffList(context, new FilterDictionaryAgentClientCompany()
                {
                    IsActive = filter.IsActive
                });
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (positions != null) flatList.AddRange(positions);
            if (departments != null) flatList.AddRange(departments);
            if (executors != null) flatList.AddRange(executors);

            var res = Tree.Get(flatList, filter);

            //AddCodePathDepartment(res);

            return res;
        }


        #endregion

    }
}
