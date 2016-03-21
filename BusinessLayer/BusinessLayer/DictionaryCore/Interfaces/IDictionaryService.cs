using BL.Model.DictionaryCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
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
        IEnumerable<FrontDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter);
        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        FrontDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id);

        IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter);

        #endregion DictionaryAgentPersons

        #region DictionaryAgentCompanies
        FrontDictionaryAgentCompany GetDictionaryAgentCompany(IContext context, int id);

        IEnumerable<FrontDictionaryAgentCompany> GetDictionaryAgentCompanies(IContext context, FilterDictionaryAgentCompany filter);
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        FrontDictionaryAgentBank GetDictionaryAgentBank(IContext context, int id);

        IEnumerable<FrontDictionaryAgentBank> GetDictionaryAgentBanks(IContext context, FilterDictionaryAgentBank filter);
        #endregion DictionaryAgentBanks

        #region DictionaryAgentEmployees
        FrontDictionaryAgentEmployee GetDictionaryAgentEmployee(IContext context, int id);

        IEnumerable<FrontDictionaryAgentEmployee> GetDictionaryAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter);

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
        BaseDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id);

        IEnumerable<BaseDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter);
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
        BaseDictionaryEventType GetDictionaryEventType(IContext context, int id);

        IEnumerable<BaseDictionaryEventType> GetDictionaryEventTypes(IContext context, FilterDictionaryEventType filter);
        #endregion DictionaryEventTypes

        #region DictionaryImportanceEventTypes
        BaseDictionaryImportanceEventType GetDictionaryImportanceEventType(IContext context, int id);

        IEnumerable<BaseDictionaryImportanceEventType> GetDictionaryImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter);
        #endregion DictionaryImportanceEventTypes

        #region DictionaryLinkTypes
        BaseDictionaryLinkType GetDictionaryLinkType(IContext context, int id);

        IEnumerable<BaseDictionaryLinkType> GetDictionaryLinkTypes(IContext context, FilterDictionaryLinkType filter);
        #endregion DictionaryLinkTypes

        // Штатное расписание
        #region DictionaryPositions
        FrontDictionaryPosition GetDictionaryPosition(IContext context, int id);

        IEnumerable<FrontDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter);
        #endregion DictionaryPositions

        // Журналы регистрации
        #region DictionaryRegistrationJournals
        FrontDictionaryRegistrationJournal GetDictionaryRegistrationJournal(IContext context, int id);

        IEnumerable<FrontDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter);
        #endregion DictionaryRegistrationJournals

        #region DictionaryResultTypes
        BaseDictionaryResultType GetDictionaryResultType(IContext context, int id);

        IEnumerable<BaseDictionaryResultType> GetDictionaryResultTypes(IContext context, FilterDictionaryResultType filter);
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        BaseDictionarySendType GetDictionarySendType(IContext context, int id);

        IEnumerable<BaseDictionarySendType> GetDictionarySendTypes(IContext context, FilterDictionarySendType filter);
        #endregion DictionarySendTypes

        #region DictionaryStandartSendListContents
        BaseDictionaryStandartSendListContent GetDictionaryStandartSendListContent(IContext context, int id);

        IEnumerable<BaseDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter);
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id);

        IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter);
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        BaseDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id);

        IEnumerable<BaseDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter);
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
