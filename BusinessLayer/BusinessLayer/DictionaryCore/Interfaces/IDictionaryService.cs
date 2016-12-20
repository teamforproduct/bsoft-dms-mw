using BL.Model.DictionaryCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.SystemCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Common;
using BL.Model.Tree;
using BL.Model.DictionaryCore.FrontMainModel;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Logic.DictionaryCore.Interfaces
{
    public interface IDictionaryService
    {
        object ExecuteAction(EnumDictionaryActions act, IContext context, object param);

        #region DictionaryAgents
        FrontDictionaryAgent GetAgent(IContext context, int id);
        FrontDictionaryAgentUser GetDictionaryAgentUser(IContext context, int id);
        IEnumerable<FrontDictionaryAgent> GetAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging);

        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        FrontDictionaryAgentPerson GetAgentPerson(IContext context, int id);

        IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging);

        #endregion DictionaryAgentPersons

        #region DictionaryContactPersons
        IEnumerable<FrontContactPersons> GetContactPersons(IContext context, FilterDictionaryAgentPerson filter);
        IEnumerable<FrontContactPersons> GetContactPerson(IContext context, int id);
        #endregion

        #region DictionaryAgentCompanies
        FrontDictionaryAgentCompany GetAgentCompany(IContext context, int id);
        IEnumerable<ListItem> GetAgentCompanyList(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging);
        IEnumerable<FrontDictionaryAgentCompany> GetAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging);
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentAccounts
        FrontDictionaryAgentAccount GetDictionaryAgentAccount(IContext context, int id);

        IEnumerable<FrontDictionaryAgentAccount> GetDictionaryAgentAccounts(IContext context, FilterDictionaryAgentAccount filter);
        #endregion DictionaryAgentAccounts

        #region DictionaryAgentBanks
        FrontDictionaryAgentBank GetAgentBank(IContext context, int id);

        IEnumerable<FrontDictionaryAgentBank> GetDictionaryAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging);
        #endregion DictionaryAgentBanks

        #region DictionaryAgentEmployees
        FrontDictionaryAgentEmployee GetDictionaryAgentEmployee(IContext context, int id);

        IEnumerable<FrontMainDictionaryAgentEmployee> GetDictionaryAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging);

        FrontDictionaryAgentUserPicture GetDictionaryAgentUserPicture(IContext context, int employeeId);
        string GetDictionaryAgentUserId(IContext context, int employeeId);

        IEnumerable<ListItem> GetAgentEmployeeList(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging);

        void SetAgentUserUserId(IContext context, InternalDictionaryAgentUser User);
        #endregion DictionaryAgentEmployees

        #region DictionaryAgentAdress
        FrontDictionaryAgentAddress GetAgentAddress(IContext context, int id);

        IEnumerable<FrontDictionaryAgentAddress> GetAgentAddresses(IContext context, FilterDictionaryAgentAddress filter);
        #endregion

        #region DictionaryAddressTypes
        FrontDictionaryAddressType GetDictionaryAddressType(IContext context, int id);

        IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter);
        #endregion

        #region DictionaryContacts
        FrontDictionaryAgentContact GetAgentContact(IContext context, int id);

        IEnumerable<FrontDictionaryAgentContact> GetAgentContacts(IContext context, FilterDictionaryContact filter);
        IEnumerable<InternalDictionaryContact> GetInternalContacts(IContext context, FilterDictionaryContact filter);
        #endregion

        #region DictionaryContactTypes
        FrontDictionaryContactType GetDictionaryContactType(IContext context, int id);

        IEnumerable<FrontDictionaryContactType> GetDictionaryContactTypes(IContext context, FilterDictionaryContactType filter);
        #endregion

        //  #region DictionaryCompanies
        //  BaseDictionaryCompany GetDictionaryCompany(IContext context, int id);
        //  IEnumerable<BaseDictionaryCompany> GetDictionaryCompanies(IContext context, FilterDictionaryAgentCompany filter);
        //  #endregion DictionaryCompanies

        // Структура предприятия
        #region DictionaryDepartments
        FrontDictionaryDepartment GetDictionaryDepartment(IContext context, int id);

        IEnumerable<FrontDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter);

        string GetDepartmentPrefix(IContext context, int parentId);
        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        FrontDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id);

        IEnumerable<FrontDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter);
        #endregion DictionaryDepartments

        // Тематики документов
        #region DictionaryDocumentSubjects
        FrontDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id);

        IEnumerable<FrontDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter);
        #endregion DictionaryDocumentSubjects

        #region DictionaryDocumentTypes
        FrontDictionaryDocumentType GetDictionaryDocumentType(IContext context, int id);

        IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter);
        #endregion DictionaryDocumentSubjects

        #region DictionaryEventTypes
        FrontDictionaryEventType GetDictionaryEventType(IContext context, int id);

        IEnumerable<FrontDictionaryEventType> GetDictionaryEventTypes(IContext context, FilterDictionaryEventType filter);
        #endregion DictionaryEventTypes

        #region DictionaryImportanceEventTypes
        FrontDictionaryImportanceEventType GetDictionaryImportanceEventType(IContext context, int id);

        IEnumerable<FrontDictionaryImportanceEventType> GetDictionaryImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter);
        #endregion DictionaryImportanceEventTypes

        #region DictionaryLinkTypes
        FrontDictionaryLinkType GetDictionaryLinkType(IContext context, int id);

        IEnumerable<FrontDictionaryLinkType> GetDictionaryLinkTypes(IContext context, FilterDictionaryLinkType filter);
        #endregion DictionaryLinkTypes

        // Штатное расписание
        #region DictionaryPositions
        FrontDictionaryPosition GetDictionaryPosition(IContext context, int id);

        IEnumerable<FrontDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter);

        IEnumerable<ListItem> GetPositionList(IContext context, FilterDictionaryPosition filter);

        void SetPositionOrder(IContext context, ModifyPositionOrder model);

        #endregion DictionaryPositions

        // Исполнители
        #region DictionaryPositionExecutors
        FrontDictionaryPositionExecutor GetDictionaryPositionExecutor(IContext context, int id);

        IEnumerable<FrontDictionaryPositionExecutor> GetDictionaryPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter);
        IEnumerable<FrontDictionaryPositionExecutor> GetCurrentPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter);
        IEnumerable<FrontDictionaryPositionExecutor> GetCurrentPositionExecutorsByAgent(IContext context, int agentId, FilterDictionaryPositionExecutor filter);
        #endregion DictionaryPositionExecutors

        // Типы исполнителей
        #region DictionaryPositionExecutorTypes
        FrontDictionaryPositionExecutorType GetDictionaryPositionExecutorType(IContext context, int id);
        IEnumerable<FrontDictionaryPositionExecutorType> GetDictionaryPositionExecutorTypes(IContext context, FilterDictionaryPositionExecutorType filter);
        #endregion DictionaryPositionExecutorTypes


        // Журналы регистрации
        #region DictionaryRegistrationJournals
        FrontDictionaryRegistrationJournal GetRegistrationJournal(IContext context, int id);

        IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter);
        IEnumerable<ITreeItem> GetRegistrationJournalsTree(IContext context, FilterTree filter);
        #endregion DictionaryRegistrationJournals

        // Компании
        #region DictionaryAgentClientCompanies
        FrontDictionaryAgentClientCompany GetDictionaryAgentClientCompany(IContext context, int id);

        IEnumerable<FrontDictionaryAgentClientCompany> GetDictionaryAgentClientCompanies(IContext context, FilterDictionaryAgentClientCompany filter);
        #endregion DictionaryCompanies


        #region DictionaryResultTypes
        FrontDictionaryResultType GetDictionaryResultType(IContext context, int id);

        IEnumerable<FrontDictionaryResultType> GetDictionaryResultTypes(IContext context, FilterDictionaryResultType filter);
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        FrontDictionarySendType GetDictionarySendType(IContext context, int id);

        IEnumerable<FrontDictionarySendType> GetDictionarySendTypes(IContext context, FilterDictionarySendType filter);
        #endregion DictionarySendTypes

        #region DictionaryStandartSendListContents
        FrontDictionaryStandartSendListContent GetDictionaryStandartSendListContent(IContext context, int id);

        IEnumerable<FrontDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter);
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        FrontDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id);

        IEnumerable<FrontDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter);
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        FrontDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id);

        IEnumerable<FrontDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter);
        #endregion DictionarySubordinationTypes

        #region DictionaryTags
        IEnumerable<FrontDictionaryTag> GetDictionaryTags(IContext context, FilterDictionaryTag filter);
        FrontDictionaryTag GetDictionaryTag(IContext context, int id);
        #endregion DictionaryTags

        #region Dictionary Admin
        #region AdminAccessLevels
        FrontAdminAccessLevel GetAdminAccessLevel(IContext context, int id);

        IEnumerable<FrontAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter);
        #endregion AdminAccessLevels

        #endregion

        #region CustomDictionaryTypes
        IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter);

        FrontCustomDictionaryType GetCustomDictionaryType(IContext context, int id);
        #endregion CustomDictionaryTypes

        #region CustomDictionaries
        IEnumerable<FrontCustomDictionary> GetCustomDictionaries(IContext context, FilterCustomDictionary filter);

        FrontCustomDictionary GetCustomDictionary(IContext context, int id);
        #endregion CustomDictionaries

        #region [+] StaffList ...
        IEnumerable<ITreeItem> GetStaffList(IContext context, FilterDictionaryStaffList filter);
        
        void AddStaffList(IContext context);
        #endregion
    }
}
