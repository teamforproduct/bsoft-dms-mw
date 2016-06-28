using BL.Model.DictionaryCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.SystemCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore.Interfaces
{
    public interface IDictionaryService
    {
        object ExecuteAction(EnumDictionaryActions act, IContext context, object param);

        #region DictionaryAgents
        FrontDictionaryAgent GetDictionaryAgent(IContext context, int id);
        IEnumerable<FrontDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter,UIPaging paging);
       
        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        FrontDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id);

        IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging);

        #endregion DictionaryAgentPersons

        #region DictionaryAgentCompanies
        FrontDictionaryAgentCompany GetDictionaryAgentCompany(IContext context, int id);

        IEnumerable<FrontDictionaryAgentCompany> GetDictionaryAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging);
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentAccounts
        FrontDictionaryAgentAccount GetDictionaryAgentAccount(IContext context, int id);

        IEnumerable<FrontDictionaryAgentAccount> GetDictionaryAgentAccounts(IContext context, int AgentId,FilterDictionaryAgentAccount filter);
        #endregion DictionaryAgentAccounts

        #region DictionaryAgentBanks
        FrontDictionaryAgentBank GetDictionaryAgentBank(IContext context, int id);

        IEnumerable<FrontDictionaryAgentBank> GetDictionaryAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging);
        #endregion DictionaryAgentBanks

        #region DictionaryAgentEmployees
        FrontDictionaryAgentEmployee GetDictionaryAgentEmployee(IContext context, int id);

        IEnumerable<FrontDictionaryAgentEmployee> GetDictionaryAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging);

        FrontDictionaryAgentEmployee GetDictionaryAgentEmployeePersonnelNumber(IContext context);

        #endregion DictionaryAgentEmployees

        #region DictionaryAgentAdress
        FrontDictionaryAgentAddress GetDictionaryAgentAddress(IContext context, int id);

        IEnumerable<FrontDictionaryAgentAddress> GetDictionaryAgentAddresses(IContext context,int agentId, FilterDictionaryAgentAddress filter);
        #endregion

        #region DictionaryAddressTypes
        FrontDictionaryAddressType GetDictionaryAddressType(IContext context, int id);

        IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter);
        #endregion

        #region DictionaryContacts
        FrontDictionaryContact GetDictionaryContact(IContext context, int id);

        IEnumerable<FrontDictionaryContact> GetDictionaryContacts(IContext context, int agentId,FilterDictionaryContact filter);
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
        #endregion DictionaryPositions

        // Исполнители
        #region DictionaryPositionExecutors
        FrontDictionaryPositionExecutor GetDictionaryPositionExecutor(IContext context, int id);

        IEnumerable<FrontDictionaryPositionExecutor> GetDictionaryPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter);
        #endregion DictionaryPositionExecutors

        // Типы исполнителей
        #region DictionaryPositionExecutorTypes
        FrontDictionaryPositionExecutorType GetDictionaryPositionExecutorType(IContext context, int id);
        IEnumerable<FrontDictionaryPositionExecutorType> GetDictionaryPositionExecutorTypes(IContext context, FilterDictionaryPositionExecutorType filter);
        #endregion DictionaryPositionExecutorTypes


        // Журналы регистрации
        #region DictionaryRegistrationJournals
        FrontDictionaryRegistrationJournal GetDictionaryRegistrationJournal(IContext context, int id);

        IEnumerable<FrontDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter);
        #endregion DictionaryRegistrationJournals

        // Компании
        #region DictionaryCompanies
        FrontDictionaryCompany GetDictionaryCompany(IContext context, int id);

        IEnumerable<FrontDictionaryCompany> GetDictionaryCompanies(IContext context, FilterDictionaryCompany filter);
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
    }
}
