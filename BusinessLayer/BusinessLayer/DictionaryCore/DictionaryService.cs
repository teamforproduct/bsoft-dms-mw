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
using LinqKit;

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

        private List<FullTextResultList> GetIDsForDictionaryFullTextSearchNew(IContext context, List<EnumObjects> dictionaryTypes, string filter, Func<IContext, IEnumerable<FullTextSearchResult>, IEnumerable<FullTextSearchResult>> filterFunct = null)
        {
            var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
            var ftRes = ftService.SearchDictionary(context, filter);
            if (filterFunct == null)
            {
                //if (dictionaryTypes.Count > 0)
                //{
                //   var filterContains = PredicateBuilder.False<FullTextSearchResult>();
                //    filterContains = dictionaryTypes.Aggregate(filterContains,
                //        (current, value) => current.Or(e => e.ObjectType == value).Expand());

                //    ftRes = ftRes.Where(filterContains);
                //}

                ftRes = ftRes.Where(x => dictionaryTypes.Contains(x.ObjectType));
            }
            else
            {
                ftRes = filterFunct(context, ftRes);
            }
            var resWithRanges =
                ftRes.GroupBy(x => new { x.ObjectId, x.ObjectType })
                    .Select(x => new { ObjectId = x.Key.ObjectId, Rate = x.Max(s => s.Score), ObjectType = x.Key.ObjectType })
                    .OrderByDescending(x => x.Rate);
            if (resWithRanges.Any())
            {
                return resWithRanges.Select(x => new FullTextResultList { ObjectId = x.ObjectId, ObjectType = x.ObjectType }).ToList();
            }
            return null;
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

        #region DictionaryAgentPeople
        public FrontAgentPeoplePassport GetAgentPeoplePassport(IContext context, int id)
        {
            return _dictDb.GetAgentPeoplePassport(context, id);
        }
        #endregion

        #region DictionaryAgentPersons
        public FrontAgentPerson GetAgentPerson(IContext context, int id)
        {
            return _dictDb.GetAgentPerson(context, id);
        }

        public IEnumerable<ListItem> GetShortListAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryAgentPerson();

            filter.IsActive = true;

            return _dictDb.GetShortListAgentPersons(context, filter, paging);
        }

        public IEnumerable<FrontMainAgentPerson> GetMainAgentPersons(IContext context, FullTextSearch ftSearch, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            var newFilter = new FilterDictionaryAgentPerson();

            if (!string.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentPersons, ftSearch.FullTextSearchString);
            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetMainAgentPersons(context, newFilter, paging);
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

        public IEnumerable<FrontMainAgentEmployee> GetMainAgentEmployees(IContext context, FullTextSearch ftSearch, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {

            var newFilter = new FilterDictionaryAgentEmployee();

            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                //newFilter.IDs =  GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentEmployees, ftSearch.FullTextSearchString);
                var list = GetIDsForDictionaryFullTextSearchNew(context,
                    new List<EnumObjects> { EnumObjects.DictionaryAgentEmployees, EnumObjects.DictionaryContacts, EnumObjects.DictionaryAgentAddresses, EnumObjects.DictionaryPositions },
                    ftSearch.FullTextSearchString);

                newFilter.IDs = list.Where(x => x.ObjectType == EnumObjects.DictionaryAgentEmployees).Select(x => x.ObjectId).ToList();
                newFilter.AddressIDs = list.Where(x => x.ObjectType == EnumObjects.DictionaryAgentAddresses).Select(x => x.ObjectId).ToList();
                newFilter.ContactIDs = list.Where(x => x.ObjectType == EnumObjects.DictionaryContacts).Select(x => x.ObjectId).ToList();
                newFilter.PositionIDs = list.Where(x => x.ObjectType == EnumObjects.DictionaryPositions).Select(x => x.ObjectId).ToList();
            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetAgentEmployeesMain(context, newFilter, paging);

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

        public IEnumerable<FrontAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter)
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
            return _dictDb.GetAgentCompany(context, id);
        }

        public IEnumerable<ListItem> GetAgentCompanyList(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryAgentCompany();

            filter.IsActive = true;

            return _dictDb.GetAgentCompanyList(context, filter, paging);
        }

        public IEnumerable<FrontMainAgentCompany> GetMainAgentCompanies(IContext context, FullTextSearch ftSearch, FilterDictionaryAgentCompany filter, UIPaging paging)
        {

            var newFilter = new FilterDictionaryAgentCompany();
            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentCompanies, ftSearch.FullTextSearchString, ResolveSearchResultAgentCompanies);

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
        public FrontAgentBank GetAgentBank(IContext context, int id)
        {
            return _dictDb.GetAgentBank(context, id);
        }

        public IEnumerable<FrontMainAgentBank> GetMainAgentBanks(IContext context, FullTextSearch ftSearch, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            var newFilter = new FilterDictionaryAgentBank();
            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAgentBanks, ftSearch.FullTextSearchString, ResolveSearchResultAgentBanks);

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

        #region DictionaryAgentUser
        public FrontDictionaryAgentUser GetDictionaryAgentUser(IContext context, int id)
        {
            return _dictDb.GetAgentUser(context, id);
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

        public IEnumerable<ListItemWithPath> GetDepartmentShortList(IContext context, FilterTree filter, UIPaging paging)
        {
            var tree = GetRegistrationJournalsTree(context, new FilterDictionaryJournalsTree { IsShowAll = true, IsActive = true });

            var list = new List<ListItemWithPath>();

            GetDepartmentShortList(tree, list, string.Empty);

            return list;

        }

        private void GetDepartmentShortList(IEnumerable<ITreeItem> tree, List<ListItemWithPath> list, string path)
        {

            foreach (var treeItem in tree)
            {
                var name = treeItem.Name.Trim();

                if (treeItem.ObjectId == (int)EnumObjects.DictionaryDepartments)
                {
                    var tmp = new ListItemWithPath
                    {
                        Id = treeItem.Id,
                        Name = name,
                        Path = path
                    };

                    list.Add(tmp);

                    // чтобы лишний раз не пытаться выполнить GetDepartmentShortList для листа
                    continue;
                }

                if (treeItem is FrontDictionaryDepartmentTreeItem)
                { name = (treeItem as FrontDictionaryDepartmentTreeItem).Code.Trim() + " " + name; }

                GetDepartmentShortList(treeItem.Childs, list, path + (path == string.Empty ? string.Empty : " -> ") + name);

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
            return _dictDb.GetDocumentTypes(context, new FilterDictionaryDocumentType { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<ListItem> GetShortListDocumentTypes(IContext context, FilterDictionaryDocumentType filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryDocumentType();

            filter.IsActive = true;

            return _dictDb.GetShortListDocumentTypes(context, filter, paging);
        }

        public IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter, UIPaging paging)
        {
            return _dictDb.GetDocumentTypes(context, filter, paging);
        }

        public IEnumerable<FrontDictionaryDocumentType> GetMainDictionaryDocumentTypes(IContext context, FullTextSearch ftSearch, FilterDictionaryDocumentType filter, UIPaging paging)
        {

            var newFilter = new FilterDictionaryDocumentType();

            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryAddressType, ftSearch.FullTextSearchString);
            }
            else
            {
                newFilter = filter;
            }


            return _dictDb.GetDocumentTypes(context, newFilter, paging);

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

        public IEnumerable<ListItem> GetPositionList(IContext context, FilterDictionaryPosition filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryPosition();

            filter.IsActive = true;

            return _dictDb.GetPositionList(context, filter, paging);
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

            return _dictDb.GetRegistrationJournals(context, new FilterDictionaryRegistrationJournal { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter, UIPaging paging)
        {
            return _dictDb.GetRegistrationJournals(context, filter, paging);
        }

        public IEnumerable<FrontDictionaryRegistrationJournal> GetMainRegistrationJournals(IContext context, FullTextSearch ftSearch, FilterDictionaryRegistrationJournal filter, UIPaging paging)
        {

            var newFilter = new FilterDictionaryRegistrationJournal();

            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryRegistrationJournals, ftSearch.FullTextSearchString);
            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetRegistrationJournals(context, newFilter, paging);
        }

        public IEnumerable<ITreeItem> GetRegistrationJournalsFilter(IContext context, FilterDictionaryJournalsTree filter)
        {
            if (filter == null) filter = new FilterDictionaryJournalsTree();
            filter.LevelCount = 2;
            return GetRegistrationJournalsTree(context, filter);
        }

        private IEnumerable<ITreeItem> GetRegistrationJournalsTree(IContext context, FilterDictionaryJournalsTree filter, FilterDictionaryRegistrationJournal filterJoirnal = null)
        {

            int levelCount = filter?.LevelCount ?? 0;
            IEnumerable<TreeItem> journals = null;
            IEnumerable<TreeItem> departments = null;
            IEnumerable<TreeItem> companies = null;

            if (levelCount >= 3 || levelCount == 0)
            {
                var f = filterJoirnal ?? new FilterDictionaryRegistrationJournal { IsActive = filter?.IsActive };

                journals = _dictDb.GetRegistrationJournalsForRegistrationJournals(context, f);
            }

            if (levelCount >= 2 || levelCount == 0)
            {
                departments = _dictDb.GetDepartmentsForRegistrationJournals(context, filter.IsShowAll,  new FilterDictionaryDepartment()
                {
                    IsActive = filter?.IsActive,
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

                companies = _dictDb.GetAgentClientCompaniesForStaffList(context, f);
            }

            List<TreeItem> flatList = new List<TreeItem>();

            if (companies != null) flatList.AddRange(companies);
            if (journals != null) flatList.AddRange(journals);
            if (departments != null) flatList.AddRange(departments);

            if (filter == null) filter = new FilterDictionaryJournalsTree();

            // Удаляю ветви которые не заканчиваются журналами 
            //filter.RemoveEmptyBranchesByObject = new List<EnumObjects> { EnumObjects.DictionaryRegistrationJournals };

            var res = Tree.Get(flatList, filter);

            return res;
        }

        public IEnumerable<ListItemWithPath> GetRegistrationJournalShortList(IContext context, FilterDictionaryJournalsTree filter, FilterDictionaryRegistrationJournal filterJoirnal, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryJournalsTree();
            filter.IsShowAll = true;
            filter.IsActive = true;

            var tree = GetRegistrationJournalsTree(context, filter, filterJoirnal);

            var list = new List<ListItemWithPath>();

            GetRegistrationJournalShortList(tree, list, string.Empty);

            return list;

        }

        private void GetRegistrationJournalShortList(IEnumerable<ITreeItem> tree, List<ListItemWithPath> list, string path)
        {

            foreach (var treeItem in tree)
            {

                if (treeItem.ObjectId == (int)EnumObjects.DictionaryRegistrationJournals)
                {
                    var tmp = new ListItemWithPath
                    {
                        Id = treeItem.Id,
                        Name = treeItem.Name,
                        Path = path
                    };

                    list.Add(tmp);

                    // чтобы лишний раз не пытаться выполнить GetRegistrationJournalShortList для листа
                    continue;
                }

                var name = treeItem.Name;

                if (treeItem is FrontDictionaryDepartmentTreeItem)
                { name = (treeItem as FrontDictionaryDepartmentTreeItem).Code.Trim() + " " + name; }

                GetRegistrationJournalShortList(treeItem.Childs, list, path + (path == string.Empty ? string.Empty : " -> ") + name);
            }
        }

        #endregion DictionaryRegistrationJournals

        // Компании
        #region DictionaryAgentClientCompanies
        public FrontDictionaryAgentClientCompany GetDictionaryAgentClientCompany(IContext context, int id)
        {

            return _dictDb.GetAgentClientCompanies(context, new FilterDictionaryAgentOrg { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAgentClientCompany> GetDictionaryAgentClientCompanies(IContext context, FilterDictionaryAgentOrg filter)
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
        public FrontDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id)
        {
            return _dictDb.GetStandartSendLists(context, new FilterDictionaryStandartSendList { IDs = new List<int> { id } }, null).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {
            return _dictDb.GetStandartSendLists(context, filter, null);
        }

        public IEnumerable<FrontMainDictionaryStandartSendList> GetMainStandartSendLists(IContext context, FullTextSearch ftSearch, FilterDictionaryStandartSendList filter, UIPaging paging)
        {
            var newFilter = new FilterDictionaryStandartSendList();
            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryStandartSendLists, ftSearch.FullTextSearchString);
            }
            else
            {
                newFilter = filter;
            }

            var sendLists = _dictDb.GetStandartSendLists(context, filter, paging);

            var res = sendLists.GroupBy(x => new { x.PositionId, x.PositionName, x.PositionExecutorName, x.PositionExecutorTypeSuffix })
                 .OrderBy(x => x.Key.PositionName)
                 .Select(x => new FrontMainDictionaryStandartSendList()
                 {
                     Id = x.Key.PositionId ?? -1,
                     Name = x.Key.PositionName,
                     ExecutorName = x.Key.PositionExecutorName,
                     ExecutorTypeSuffix = x.Key.PositionExecutorTypeSuffix,
                     SendLists = x.OrderBy(y => y.Name).ToList()
                 });


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
        public IEnumerable<FrontMainTag> GetMainTags(IContext context, FullTextSearch ftSearch, FilterDictionaryTag filter, UIPaging paging)
        {
            var newFilter = new FilterDictionaryTag();
            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                newFilter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.DictionaryTag, ftSearch.FullTextSearchString);
            }
            else
            {
                newFilter = filter;
            }

            return _dictDb.GetMainTags(context, filter, paging);
        }

        public IEnumerable<ListItem> GetTagList(IContext context, FilterDictionaryTag filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryTag();

            filter.IsActive = true;

            return _dictDb.GetTagList(context, filter, paging);
        }

        public FrontTag GetTag(IContext context, int id)
        {
            return _dictDb.GetTag(context, id);
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
        public IEnumerable<FrontCustomDictionary> GetMainCustomDictionaries(IContext context, FullTextSearch ftSearch, FilterCustomDictionary filter, UIPaging paging)
        {
            if (!String.IsNullOrEmpty(ftSearch?.FullTextSearchString))
            {
                if (filter == null) filter = new FilterCustomDictionary();
                filter.IDs = GetIDsForDictionaryFullTextSearch(context, EnumObjects.CustomDictionaries, ftSearch.FullTextSearchString);
            }

            return _dictDb.GetCustomDictionaries(context, filter, paging);
        }

        public IEnumerable<FrontCustomDictionary> GetCustomDictionaries(IContext context, FilterCustomDictionary filter, UIPaging paging)
        {
            return _dictDb.GetCustomDictionaries(context, filter, paging);
        }

        public FrontCustomDictionary GetCustomDictionary(IContext context, int id)
        {
            return _dictDb.GetCustomDictionaries(context, new FilterCustomDictionary { IDs = new List<int> { id } }, null).FirstOrDefault();
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
                    IsActive = filter?.IsActive
                });
            }

            if (levelCount >= 3 || levelCount == 0)
            {
                positions = _dictDb.GetPositionsForStaffList(context, new FilterDictionaryPosition()
                {
                    IsActive = filter?.IsActive
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
                companies = _dictDb.GetAgentClientCompaniesForStaffList(context, new FilterDictionaryAgentOrg()
                {
                    IsActive = filter?.IsActive
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
