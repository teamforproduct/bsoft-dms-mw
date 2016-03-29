﻿using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
using BL.Model.SystemCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Database.Dictionaries.Interfaces
{
    public interface IDictionariesDbProcess
    {
        #region DictionaryAgents
        FrontDictionaryAgent GetDictionaryAgent(IContext context, int id);
        void UpdateDictionaryAgent(IContext context, InternalDictionaryAgent addr);
        void DeleteDictionaryAgent(IContext context, InternalDictionaryAgent addr);
        int AddDictionaryAgent(IContext context, InternalDictionaryAgent addr);
        IEnumerable<FrontDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter,UIPaging paging);
        #endregion DictionaryAgents

        #region DictionaryAgentPerson

        FrontDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id);
        void UpdateDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson addr);
        void DeleteDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson addr);
        int AddDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson addr);
        IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging);
        #endregion DictionaryAgentPerson

        #region DictionaryAgentEmployee

        FrontDictionaryAgentEmployee GetDictionaryAgentEmployee(IContext context, int id);
        void UpdateDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee);
        void DeleteDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee);
        int AddDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee);
        IEnumerable<FrontDictionaryAgentEmployee> GetDictionaryAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging);
        #endregion DictionaryAgentEmployee

        #region DictionaryAgentAddress

        FrontDictionaryAgentAddress GetDictionaryAgentAddress(IContext context, int id);
        void UpdateDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr);
        void DeleteDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr);
        int AddDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr);
        IEnumerable<FrontDictionaryAgentAddress> GetDictionaryAgentAddresses(IContext context, int agentId, FilterDictionaryAgentAddress filter);

        #endregion

        #region DicionaryAddressTypes

        InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext context,
            FilterDictionaryAddressType filter);
        void UpdateDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType);
        void DeleteDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType);
        int AddDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType);
        IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter);

        #endregion

        #region DictionaryAgentCompanies
        FrontDictionaryAgentCompany GetDictionaryAgentCompany(IContext context, int id);
        void UpdateDictionaryAgentCompany(IContext context, InternalDictionaryAgentCompany company);
        void DeleteDictionaryAgentCompany(IContext context, InternalDictionaryAgentCompany company);
        int AddDictionaryAgentCompany(IContext context, InternalDictionaryAgentCompany company);
        IEnumerable<FrontDictionaryAgentCompany> GetDictionaryAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging);
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        FrontDictionaryAgentBank GetDictionaryAgentBank(IContext context, int id);
        void UpdateDictionaryAgentBank(IContext context, InternalDictionaryAgentBank bank);
        void DeleteDictionaryAgentBank(IContext context, InternalDictionaryAgentBank bank);
        int AddDictionaryAgentBank(IContext context, InternalDictionaryAgentBank bank);
        IEnumerable<FrontDictionaryAgentBank> GetDictionaryAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging);
        #endregion DictionaryAgentBanks

        #region DictionaryAgentAccounts
        FrontDictionaryAgentAccount GetDictionaryAgentAccount(IContext context, int id);
        void UpdateDictionaryAgentAccount(IContext context, InternalDictionaryAgentAccount account);
        void DeleteDictionaryAgentAccount(IContext context, InternalDictionaryAgentAccount account);
        int AddDictionaryAgentAccount(IContext context, InternalDictionaryAgentAccount account);
        IEnumerable<FrontDictionaryAgentAccount> GetDictionaryAgentAccounts(IContext context, int AgentId, FilterDictionaryAgentAccount filter);
        #endregion DictionaryAgentAccounts

        #region DictionaryContacts
        FrontDictionaryContact GetDictionaryContact(IContext context,
           FilterDictionaryContact filter);
        void UpdateDictionaryContact(IContext context, InternalDictionaryContact contact);
        void DeleteDictionaryContact(IContext context, InternalDictionaryContact contact);
        int AddDictionaryContact(IContext context, InternalDictionaryContact contact);
        IEnumerable<FrontDictionaryContact> GetDictionaryContacts(IContext context, int agentId, FilterDictionaryContact filter);
        #endregion

        #region DictionaryContactTypes
        FrontDictionaryContactType GetInternalDictionaryContactType(IContext context, FilterDictionaryContactType filter);
        void UpdateDictionaryContactType(IContext context, InternalDictionaryContactType contactType);
        void DeleteDictionaryContactType(IContext context, InternalDictionaryContactType contactType);
        int AddDictionaryContactType(IContext context, InternalDictionaryContactType contactType);
        IEnumerable<FrontDictionaryContactType> GetDictionaryContactTypes(IContext context, FilterDictionaryContactType filter);
        #endregion

        // Структура предприятия
        #region DictionaryDepartments
        int AddDictionaryDepartment(IContext context, InternalDictionaryDepartment docType);
        void UpdateDictionaryDepartment(IContext context, InternalDictionaryDepartment docType);
        void DeleteDictionaryDepartment(IContext context, InternalDictionaryDepartment docType);
        bool ExistsDictionaryDepartment(IContext context, FilterDictionaryDepartment filter);
        InternalDictionaryDepartment GetDictionaryDepartment(IContext context, FilterDictionaryDepartment filter);

        IEnumerable<FrontDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter);
        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        FrontDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id);

        IEnumerable<FrontDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter);
        #endregion DictionaryDepartments

        // Тематики документов
        #region DictionaryDocumentSubjects
        int AddDictionaryDocumentSubject(IContext context, InternalDictionaryDocumentSubject docType);
        void UpdateDictionaryDocumentSubject(IContext context, InternalDictionaryDocumentSubject docType);
        void DeleteDictionaryDocumentSubject(IContext context, InternalDictionaryDocumentSubject docType);
        bool ExistsDictionaryDocumentSubject(IContext context, FilterDictionaryDocumentSubject filter);
        InternalDictionaryDocumentSubject GetInternalDictionaryDocumentSubject(IContext context, FilterDictionaryDocumentSubject filter);
        IEnumerable<FrontDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter);
        #endregion DictionaryDocumentSubjects

        // Типы документов
        #region DictionaryDocumentTypes
        void UpdateDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType);
        void DeleteDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType);
        int AddDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType);
        InternalDictionaryDocumentType GetInternalDictionaryDocumentType(IContext context, FilterDictionaryDocumentType filter);
        IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter);
        #endregion DictionaryDocumentTypes

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

        int AddPosition(IContext context, InternalDictionaryPosition position);
        void UpdatePosition(IContext context, InternalDictionaryPosition position);
        void DeletePosition(IContext context, InternalDictionaryPosition position);
        bool ExistsPosition(IContext context, FilterDictionaryPosition filter);

        int? GetExecutorAgentIdByPositionId(IContext context, int id);
        FrontDictionaryPosition GetDictionaryPosition(IContext context, int id);

        IEnumerable<FrontDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter);
        IEnumerable<InternalDictionaryPositionWithActions> GetDictionaryPositionsWithActions(IContext context, FilterDictionaryPosition filter);
        #endregion DictionaryPositions

        // Журналы регистрации
        #region DictionaryRegistrationJournals
        int AddDictionaryRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal docType);
        void UpdateDictionaryRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal docType);
        void DeleteDictionaryRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal docType);
        bool ExistsDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter);
        InternalDictionaryRegistrationJournal GetInternalDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter);
        IEnumerable<FrontDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter);
        #endregion DictionaryRegistrationJournals

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
        void UpdateDictionaryStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content);
        void DeleteDictionaryStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content);
        int AddDictionaryStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content);
        IEnumerable<FrontDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter);
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id);

        IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter);
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        FrontDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id);

        IEnumerable<FrontDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter);

        #endregion DictionarySubordinationTypes

        #region DictionaryTags

        InternalDictionaryTag GetInternalDictionaryTags(IContext context, FilterDictionaryTag filter);
        IEnumerable<FrontDictionaryTag> GetDictionaryTags(IContext context, FilterDictionaryTag filter);
        int AddDictionaryTag(IContext context, InternalDictionaryTag model);
        void UpdateDictionaryTag(IContext context, InternalDictionaryTag model);
        void DeleteDictionaryTag(IContext context, InternalDictionaryTag model);
        #endregion DictionaryTags

        #region Admins
        #region AdminAccessLevels
        FrontAdminAccessLevel GetAdminAccessLevel(IContext ctx, int id);

        IEnumerable<FrontAdminAccessLevel> GetAdminAccessLevels(IContext ctx, FilterAdminAccessLevel filter);
        #endregion AdminAccessLevels
        #endregion

        #region CustomDictionaryTypes
        void UpdateCustomDictionaryType(IContext context, InternalCustomDictionaryType model);

        int AddCustomDictionaryType(IContext context, InternalCustomDictionaryType model);

        void DeleteCustomDictionaryType(IContext context, int id);

        InternalCustomDictionaryType GetInternalCustomDictionaryType(IContext context, FilterCustomDictionaryType filter);

        FrontCustomDictionaryType GetCustomDictionaryType(IContext context, int id);

        IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter);
        #endregion CustomDictionaryTypes

        #region CustomDictionaries
        void UpdateCustomDictionary(IContext context, InternalCustomDictionary model);

        int AddCustomDictionary(IContext context, InternalCustomDictionary model);

        void DeleteCustomDictionary(IContext context, int id);

        InternalCustomDictionary GetInternalCustomDictionary(IContext context, FilterCustomDictionary filter);

        FrontCustomDictionary GetCustomDictionary(IContext context, int id);

        IEnumerable<FrontCustomDictionary> GetCustomDictionaries(IContext context, FilterCustomDictionary filter);
        #endregion CustomDictionaries
    }
}