using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
using BL.Model.SystemCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Common;
using BL.Model.Tree;
using static BL.Database.Dictionaries.DictionariesDbProcess;
using BL.Model.Enums;
using BL.Model.AdminCore.FrontModel;
using BL.Model.DictionaryCore.FrontMainModel;

namespace BL.Database.Dictionaries.Interfaces
{
    public interface IDictionariesDbProcess
    {
        #region DictionaryAgents
        int AddAgent(IContext context, InternalDictionaryAgent addr);
        void UpdateAgent(IContext context, InternalDictionaryAgent addr);
        void DeleteAgent(IContext context, int agentId);
        FrontDictionaryAgentUser GetAgentUser(IContext context, int id);
        IEnumerable<FrontDictionaryAgent> GetAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging);
        IEnumerable<AutocompleteItem> GetAgentExternalList(IContext context, UIPaging paging);
        bool ExistsAgents(IContext context, FilterDictionaryAgent filter);
        #endregion DictionaryAgents

        #region DictionaryAgentPeople
        FrontAgentPeoplePassport GetAgentPeoplePassport(IContext context, int id);
        void UpdateAgentPeoplePassport(IContext context, InternalDictionaryAgentPeople people);
        #endregion

        #region DictionaryAgentPerson

        int AddAgentPerson(IContext context, InternalDictionaryAgentPerson person);
        void UpdateAgentPerson(IContext context, InternalDictionaryAgentPerson person);
        void DeleteAgentPerson(IContext context, int id);
        bool ExistsAgentPersons(IContext context, FilterDictionaryAgentPerson filter);
        IEnumerable<FrontContactPersons> GetAgentPersonsWithContacts(IContext context, FilterDictionaryAgentPerson filter);
        IEnumerable<FrontMainAgentPerson> GetMainAgentPersons(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetAgentPersonIDs(IContext context, IBaseFilter filter, UISorting sorting);
        IEnumerable<AutocompleteItem> GetShortListAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging);
        IEnumerable<InternalDictionaryAgentPerson> GetInternalAgentPersons(IContext context, FilterDictionaryAgentPerson filter);
        IEnumerable<FrontAgentPerson> GetAgentPerson(IContext context, FilterDictionaryAgentPerson filter);
        #endregion DictionaryAgentPerson

        #region DictionaryAgentEmployee

        void UpdateAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee);
        void DeleteAgentEmployee(IContext context, int id);
        int AddAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee);
        bool ExistsAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter);
        FrontAgentEmployee GetAgentEmployee(IContext context, int id);
        IEnumerable<FrontMainAgentEmployee> GetMainAgentEmployees(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetAgentEmployeeIDs(IContext context, IBaseFilter filter, UISorting sorting);
        int GetAgentEmployeePersonnelNumber(IContext context);
        IEnumerable<ListItem> GetAgentEmployeeList(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging);
        #endregion DictionaryAgentEmployee

        #region DictionaryAgentUser

        InternalDictionaryAgentUser GetInternalAgentUser(IContext context, int id);

        void SetAgentImage(IContext context, InternalDictionaryAgentImage User);
        InternalDictionaryAgentImage GetInternalAgentImage(IContext context, int id);
        void SetAgentUserLanguage(IContext context, InternalDictionaryAgentUser User);
        void SetAgentUserLastPositionChose(IContext context, InternalDictionaryAgentUser User);
        void SetAgentUserUserId(IContext context, InternalDictionaryAgentUser User);
        #endregion

        #region DictionaryAgentAddress

        FrontDictionaryAgentAddress GetAgentAddress(IContext context, int id);
        void UpdateAgentAddress(IContext context, InternalDictionaryAgentAddress addr);
        void DeleteAgentAddress(IContext context, int id);
        int AddAgentAddress(IContext context, InternalDictionaryAgentAddress addr);
        IEnumerable<FrontDictionaryAgentAddress> GetAgentAddresses(IContext context, FilterDictionaryAgentAddress filter);

        IEnumerable<int> GetAgentsIDByAddress(IContext context, IEnumerable<int> addresses);

        #endregion

        #region DicionaryAddressTypes

        InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext context,
            FilterDictionaryAddressType filter);
        void UpdateAddressType(IContext context, InternalDictionaryAddressType addrType);
        void DeleteAddressType(IContext context, int id);
        int AddAddressType(IContext context, InternalDictionaryAddressType addrType);
        bool ExistsAddressTypeSpecCode(IContext context, int addressTypeId);
        IEnumerable<FrontAddressType> GetAddressTypes(IContext context, FilterDictionaryAddressType filter);
        IEnumerable<FrontShortListAddressType> GetShortListAddressTypes(IContext context, FilterDictionaryAddressType filter);
        string GetAddressTypeSpecCode(IContext context, int id);

        #endregion

        #region DictionaryAgentCompanies
        int AddAgentCompany(IContext context, InternalDictionaryAgentCompany company);
        void UpdateAgentCompany(IContext context, InternalDictionaryAgentCompany company);
        void DeleteAgentCompanies(IContext context, List<int> list);
        IEnumerable<AutocompleteItem> GetAgentCompanyList(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging);
        IEnumerable<FrontAgentCompany> GetAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging);
        IEnumerable<FrontMainAgentCompany> GetMainAgentCompanies(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetAgentCompanyIDs(IContext context, IBaseFilter filter, UISorting sorting);
        bool ExistsAgentCompanies(IContext context, FilterDictionaryAgentCompany filter);
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        int AddAgentBank(IContext context, InternalDictionaryAgentBank bank);
        void UpdateAgentBank(IContext context, InternalDictionaryAgentBank bank);
        void DeleteAgentBank(IContext context, int id);
        bool ExistsAgentBanks(IContext context, FilterDictionaryAgentBank filter);
        FrontAgentBank GetAgentBank(IContext context, int id);
        IEnumerable<FrontMainAgentBank> GetMainAgentBanks(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetAgentBankIDs(IContext context, IBaseFilter filter, UISorting sorting);
        IEnumerable<AutocompleteItem> GetShortListAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging);
        #endregion DictionaryAgentBanks

        #region DictionaryAgentAccounts
        FrontDictionaryAgentAccount GetAgentAccount(IContext context, int id);
        void UpdateAgentAccount(IContext context, InternalDictionaryAgentAccount account);
        void DeleteAgentAccount(IContext context, int id);
        int AddAgentAccount(IContext context, InternalDictionaryAgentAccount account);
        IEnumerable<FrontDictionaryAgentAccount> GetAgentAccounts(IContext context, FilterDictionaryAgentAccount filter);
        IEnumerable<InternalDictionaryAgentAccount> GetInternalAgentAccounts(IContext context, FilterDictionaryAgentAccount filter);
        #endregion DictionaryAgentAccounts

        #region DictionaryContacts


        int AddContact(IContext context, InternalDictionaryContact contact);
        void UpdateContact(IContext context, InternalDictionaryContact contact);
        void DeleteContacts(IContext context, FilterDictionaryContact filter);
        IEnumerable<FrontDictionaryAgentContact> GetContacts(IContext context, FilterDictionaryContact filter);
        IEnumerable<InternalDictionaryContact> GetInternalContacts(IContext context, FilterDictionaryContact filter);
        IEnumerable<int> GetAgentsIDByContacts(IContext context, IEnumerable<int> contacts);
        #endregion

        #region DictionaryContactTypes

        string GetContactTypeSpecCode(IContext context, int id);

        int GetContactsTypeId(IContext context, EnumContactTypes type);
        FrontDictionaryContactType GetInternalDictionaryContactType(IContext context, FilterDictionaryContactType filter);
        void UpdateContactType(IContext context, InternalDictionaryContactType contactType);
        void DeleteContactType(IContext context, int id);
        int AddContactType(IContext context, InternalDictionaryContactType contactType);
        bool ExistsContactTypeSpecCode(IContext context, int contactTypeId);
        IEnumerable<FrontShortListContactType> GetShortListContactTypes(IContext context, FilterDictionaryContactType filter);
        IEnumerable<FrontDictionaryContactType> GetContactTypes(IContext context, FilterDictionaryContactType filter);
        #endregion

        #region [+] DictionaryDepartments ...
        int AddDepartment(IContext context, InternalDictionaryDepartment model);
        void UpdateDepartment(IContext context, InternalDictionaryDepartment model);
        void UpdateDepartmentCode(IContext context, string code, FilterDictionaryDepartment filter);
        void DeleteDepartments(IContext context, List<int> list);
        bool ExistsDictionaryDepartment(IContext context, FilterDictionaryDepartment filter);
        InternalDictionaryDepartment GetDepartment(IContext context, FilterDictionaryDepartment filter);
        IEnumerable<InternalDictionaryDepartment> GetInternalDepartments(IContext context, FilterDictionaryDepartment filter);

        IEnumerable<FrontDictionaryDepartment> GetDepartments(IContext context, FilterDictionaryDepartment filter);
        IEnumerable<FrontDictionaryDepartmentTreeItem> GetDepartmentsForStaffList(IContext context, FilterDictionaryDepartment filter);
        IEnumerable<FrontDictionaryDepartmentTreeItem> GetDepartmentsForRegistrationJournals(IContext context, FilterDictionaryDepartment filter);
        IEnumerable<AutocompleteItem> GetShortListDepartments(IContext context, FilterDictionaryDepartment filter);
        IEnumerable<TreeItem> GetDepartmentsTree(IContext context, FilterDictionaryDepartment filter);
        IEnumerable<TreeItem> GetDepartmentsForDIPSubordinations(IContext context, int sourcePositionId, FilterDictionaryDepartment filter);
        IEnumerable<TreeItem> GetDepartmentsForDIPJournalAccess(IContext context, int journalId, FilterDictionaryDepartment filter);
        IEnumerable<TreeItem> GetDepartmentsForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryDepartment filter);
        string GetDepartmentPrefix(IContext context, int parentId);

        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        IEnumerable<FrontDictionaryDocumentDirection> GetDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter);
        #endregion DictionaryDepartments

        // Тематики документов
        #region DictionaryDocumentSubjects
        int AddDocumentSubject(IContext context, InternalDictionaryDocumentSubject docType);
        void UpdateDocumentSubject(IContext context, InternalDictionaryDocumentSubject docType);
        void DeleteDocumentSubject(IContext context, int id);
        bool ExistsDictionaryDocumentSubject(IContext context, FilterDictionaryDocumentSubject filter);
        IEnumerable<InternalDictionaryDocumentSubject> GetInternalDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter);
        IEnumerable<FrontDictionaryDocumentSubject> GetDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter);
        #endregion DictionaryDocumentSubjects

        // Типы документов
        #region DictionaryDocumentTypes
        void UpdateDocumentType(IContext context, InternalDictionaryDocumentType docType);
        void DeleteDocumentType(IContext context, int id);
        int AddDocumentType(IContext context, InternalDictionaryDocumentType docType);
        IEnumerable<ListItem> GetShortListDocumentTypes(IContext context, FilterDictionaryDocumentType filter, UIPaging paging);
        IEnumerable<InternalDictionaryDocumentType> GetInternalDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter);
        IEnumerable<FrontDictionaryDocumentType> GetMainDocumentTypes(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetDocumentTypeIDs(IContext context, IBaseFilter filter, UISorting sorting);
        #endregion DictionaryDocumentTypes

        #region DictionaryEventTypes
        IEnumerable<FrontDictionaryEventType> GetEventTypes(IContext context, FilterDictionaryEventType filter);
        #endregion DictionaryEventTypes

        #region DictionaryImportanceEventTypes
        IEnumerable<FrontDictionaryImportanceEventType> GetImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter);
        #endregion DictionaryImportanceEventTypes

        #region DictionaryLinkTypes
        IEnumerable<FrontDictionaryLinkType> GetLinkTypes(IContext context, FilterDictionaryLinkType filter);
        #endregion DictionaryLinkTypes

        #region [+] DictionaryPositions ...

        int AddPosition(IContext context, InternalDictionaryPosition position);
        void UpdatePosition(IContext context, InternalDictionaryPosition position);
        void DeletePositions(IContext context, List<int> list);

        void UpdatePositionOrder(IContext context, int positionId, int order);
        void UpdatePositionExecutor(IContext context, List<int> positionId = null);
        InternalDictionaryPositionExecutorForDocument GetExecutorAgentIdByPositionId(IContext context, int id);
        FrontDictionaryPosition GetPosition(IContext context, int id);

        IEnumerable<FrontDictionaryPosition> GetPositions(IContext context, FilterDictionaryPosition filter);
        IEnumerable<int> GetPositionIDs(IContext context, FilterDictionaryPosition filter);
        IEnumerable<InternalDictionaryPosition> GetInternalPositions(IContext context, FilterDictionaryPosition filter);
        IEnumerable<AutocompleteItem> GetShortListPositionsExecutor(IContext context, FilterDictionaryPosition filter);

        IEnumerable<AutocompleteItem> GetShortListPositions(IContext context, FilterDictionaryPosition filter);
        IEnumerable<TreeItem> GetPositionsTree(IContext context, FilterDictionaryPosition filter);
        IEnumerable<FrontDIPSubordinationsPosition> GetPositionsForStaffList(IContext context, FilterDictionaryPosition filter);

        IEnumerable<FrontDIPSubordinationsPosition> GetPositionsForDIPSubordinations(IContext context, int sourcePositionId, FilterDictionaryPosition filter);
        IEnumerable<FrontDIPJournalAccessPosition> GetPositionsForDIPJournalAccess(IContext context, int journalId, FilterDictionaryPosition filter);
        IEnumerable<FrontDIPRegistrationJournalPositions> GetPositionsForDIPRegistrationJournals(IContext context, int registrationJournalId, FilterDictionaryPosition filter);
        IEnumerable<SortPositoin> GetPositionsForSort(IContext context, FilterDictionaryPosition filter);

        IEnumerable<ListItem> GetPositionList(IContext context, FilterDictionaryPosition filter, UIPaging paging);
        #endregion DictionaryPositions

        // Исполнители
        #region [+] DictionaryPositionExecutors ...
        int AddExecutor(IContext context, InternalDictionaryPositionExecutor model);
        void UpdateExecutor(IContext context, InternalDictionaryPositionExecutor model);
        void DeleteExecutors(IContext context, List<int> list);
        bool ExistsPositionExecutor(IContext context, FilterDictionaryPositionExecutor filter);
        IEnumerable<InternalDictionaryPositionExecutor> GetInternalPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter);
        IEnumerable<FrontDictionaryPositionExecutor> GetPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter);
        List<int> GetPositionExecutorsIDs(IContext context, FilterDictionaryPositionExecutor filter);
        IEnumerable<AutocompleteItem> GetShortListPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter, UIPaging paging);
        IEnumerable<TreeItem> GetPositionExecutorsForTree(IContext context, FilterDictionaryPositionExecutor filter);
        IEnumerable<FrontDictionaryPositionExecutorExtended> GetPositionExecutorsDIPUserRoles(IContext context, FilterDictionaryPositionExecutor filter);
        #endregion DictionaryPositionExecutors

        // Типы исполнителей
        #region DictionaryPositionExecutorTypes
        IEnumerable<InternalDictionaryPositionExecutorType> GetInternalDictionaryPositionExecutorType(IContext context, FilterDictionaryPositionExecutorType filter);
        IEnumerable<FrontDictionaryPositionExecutorType> GetPositionExecutorTypes(IContext context, FilterDictionaryPositionExecutorType filter);
        #endregion DictionaryPositionExecutorTypes

        // Журналы регистрации
        #region DictionaryRegistrationJournals
        int AddRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal docType);
        void UpdateRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal docType);
        void DeleteRegistrationJournal(IContext context, int id);
        bool ExistsDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter);
        IEnumerable<InternalDictionaryRegistrationJournal> GetInternalRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter);
        IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetRegistrationJournalIDs(IContext context, IBaseFilter filter, UISorting sorting);
        IEnumerable<TreeItem> GetRegistrationJournalsForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryRegistrationJournal filter);
        IEnumerable<TreeItem> GetRegistrationJournalsForRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter);
        IEnumerable<AutocompleteItem> GetShortListRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter);
        #endregion DictionaryRegistrationJournals

        #region [+] DictionaryAgentClientCompanies ...
        int AddAgentOrg(IContext context, InternalDictionaryAgentOrg docType);
        void UpdateAgentOrg(IContext context, InternalDictionaryAgentOrg docType);
        void DeleteAgentOrg(IContext context, List<int> list);
        bool ExistsAgentClientCompanies(IContext context, FilterDictionaryAgentOrg filter);
        InternalDictionaryAgentOrg GetInternalAgentOrg(IContext context, FilterDictionaryAgentOrg filter);
        IEnumerable<FrontDictionaryAgentClientCompany> GetAgentOrgs(IContext context, FilterDictionaryAgentOrg filter);
        IEnumerable<TreeItem> GetAgentOrgsForStaffList(IContext context, FilterDictionaryAgentOrg filter);
        IEnumerable<TreeItem> GetShortListAgentOrgs(IContext context, FilterDictionaryAgentOrg filter);
        IEnumerable<TreeItem> GetAgentOrgsForDIPSubordinations(IContext context, int sourcePositionId, FilterDictionaryAgentOrg filter);
        IEnumerable<TreeItem> GetAgentOrgsForDIPJournalAccess(IContext context, int journalId, FilterDictionaryAgentOrg filter);
        IEnumerable<TreeItem> GetAgentClientCompaniesForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryAgentOrg filter);
        #endregion

        #region DictionaryResultTypes
        IEnumerable<FrontDictionaryResultType> GetResultTypes(IContext context, FilterDictionaryResultType filter);
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        IEnumerable<FrontDictionarySendType> GetSendTypes(IContext context, FilterDictionarySendType filter);
        #endregion DictionarySendTypes

        #region DictionaryStageTypes
        IEnumerable<ListItem> GetStageTypes(IContext context, FilterDictionaryStageType filter);
        #endregion DictionaryStageTypes

        #region DictionaryStandartSendListContents
        int AddStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content);
        void UpdateStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content);
        void DeleteStandartSendListContent(IContext context, int id);
        IEnumerable<FrontDictionaryStandartSendListContent> GetStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter);
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        int AddStandartSendList(IContext context, InternalDictionaryStandartSendList list);
        void UpdateStandartSendList(IContext context, InternalDictionaryStandartSendList list);
        void DeleteStandartSendList(IContext context, int id);
        IEnumerable<FrontDictionaryStandartSendList> GetStandartSendLists(IContext context, FilterDictionaryStandartSendList filter, UIPaging paging);
        IEnumerable<FrontMainDictionaryStandartSendList> GetMainStandartSendLists(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetStandartSendListIDs(IContext context, IBaseFilter filter, UISorting sorting);
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        IEnumerable<ListItem> GetSubordinationTypes(IContext context, FilterDictionarySubordinationType filter);

        #endregion DictionarySubordinationTypes

        #region DictionaryTags
        int AddTag(IContext context, InternalDictionaryTag model);
        void UpdateTag(IContext context, InternalDictionaryTag model);
        void DeleteTag(IContext context, int id);
        IEnumerable<FrontTag> GetTag(IContext ctx, FilterDictionaryTag filter);
        IEnumerable<ListItem> GetTagList(IContext ctx, FilterDictionaryTag filter, UIPaging paging);
        IEnumerable<InternalDictionaryTag> GetInternalTags(IContext context, FilterDictionaryTag filter);
        IEnumerable<FrontMainTag> GetMainTags(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetTagIDs(IContext context, IBaseFilter filter, UISorting sorting);
        int DocsWithTagCount(IContext context, int tagId);
        #endregion DictionaryTags

        #region Admins
        #region AdminAccessLevels
        IEnumerable<FrontAdminAccessLevel> GetAdminAccessLevels(IContext ctx, FilterAdminAccessLevel filter);
        #endregion AdminAccessLevels
        #endregion

        #region CustomDictionaryTypes
        int AddCustomDictionaryType(IContext context, InternalCustomDictionaryType model);
        void UpdateCustomDictionaryType(IContext context, InternalCustomDictionaryType model);
        void DeleteCustomDictionaryType(IContext context, int id);

        IEnumerable<InternalCustomDictionaryType> GetInternalCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter);

        //FrontCustomDictionaryType GetCustomDictionaryTypeWithCustomDictionaries(IContext context, int id);

        IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter);

        IEnumerable<FrontCustomDictionaryType> GetMainCustomDictionaryTypes(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetCustomDictionaryTypeIDs(IContext context, IBaseFilter filter, UISorting sorting);
        #endregion CustomDictionaryTypes

        #region CustomDictionaries
        int AddCustomDictionary(IContext context, InternalCustomDictionary model);
        void UpdateCustomDictionary(IContext context, InternalCustomDictionary model);
        void DeleteCustomDictionary(IContext context, int id);

        IEnumerable<InternalCustomDictionary> GetInternalCustomDictionarys(IContext context, FilterCustomDictionary filter);

        IEnumerable<FrontCustomDictionary> GetMainCustomDictionaries(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting);
        List<int> GetCustomDictionarieIDs(IContext context, IBaseFilter filter, UISorting sorting);
        #endregion CustomDictionaries


        #region FavouriteList
        IEnumerable<int> GetFavouriteList(IContext context, string module, string feature);
        int AddAgentFavourite(IContext context, InternalAgentFavourite model);
        void AddAgentFavourites(IContext context, IEnumerable<InternalAgentFavourite> list);
        void UpdateAgentFavourite(IContext context, InternalAgentFavourite model);
        void DeleteAgentFavourite(IContext context, FilterAgentFavourite filter);
        IEnumerable<InternalAgentFavourite> GetInternalAgentFavourite(IContext context, FilterAgentFavourite filter);
        #endregion
    }
}