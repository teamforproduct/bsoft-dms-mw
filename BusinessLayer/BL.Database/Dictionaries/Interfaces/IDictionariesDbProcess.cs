using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Database.Dictionaries.Interfaces
{
    public interface IDictionariesDbProcess
    {
        #region DictionaryAgents
        BaseDictionaryAgent GetDictionaryAgent(IContext context, int id);

        IEnumerable<BaseDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter);
        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        InternalDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id);

        IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter);
        #endregion DictionaryAgentPersons

        #region DictionaryAgentAddress

        FrontDictionaryAgentAddress GetDictionaryAgentAddress(IContext context, int id);
        void UpdateDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr);
        void DeleteDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr);
        int AddDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr);
        IEnumerable<FrontDictionaryAgentAddress> GetDictionaryAgentAddresses(IContext context, FilterDictionaryAgentAddress filter);
        
        #endregion

        #region DicionaryAddressTypes

        InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext context,
            FilterDictionaryAddressType filter);
        void UpdateDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType);
        void DeleteDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType);
        int AddDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType);
        IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter);

        #endregion

        #region DictionaryCompanies
        BaseDictionaryCompany GetDictionaryCompany(IContext context, int id);

        IEnumerable<BaseDictionaryCompany> GetDictionaryCompanies(IContext context, FilterDictionaryCompany filter);
        #endregion DictionaryCompanies

        #region DictionaryContacts
        InternalDictionaryContact GetInternalDictionaryContact(IContext context,
           FilterDictionaryContact filter);
        void UpdateDictionaryContact(IContext context, InternalDictionaryContact contact);
        void DeleteDictionaryContact(IContext context, InternalDictionaryContact contact);
        int AddDictionaryContact(IContext context, InternalDictionaryContact contact);
        IEnumerable<FrontDictionaryContact> GetDictionaryContacts(IContext context, FilterDictionaryContact filter);
        #endregion

        #region DictionaryContactTypes
        InternalDictionaryContactType GetInternalDictionaryContactType(IContext context,FilterDictionaryContactType filter);
        void UpdateDictionaryContactType(IContext context, InternalDictionaryContactType contactType);
        void DeleteDictionaryContactType(IContext context, InternalDictionaryContactType contactType);
        int AddDictionaryContactType(IContext context, InternalDictionaryContactType contactType);
        IEnumerable<FrontDictionaryContactType> GetDictionaryContactTypes(IContext context, FilterDictionaryContactType filter);
        #endregion




        #region DictionaryDepartments
        BaseDictionaryDepartment GetDictionaryDepartment(IContext context, int id);

        IEnumerable<BaseDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter);
        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        BaseDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id);

        IEnumerable<BaseDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter);
        #endregion DictionaryDepartments

        #region DictionaryDocumentSubjects
        BaseDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id);

        IEnumerable<BaseDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter);
        #endregion DictionaryDocumentSubjects

        #region DictionaryDocumentTypes

        InternalDictionaryDocumentType GetInternalDictionaryDocumentType(IContext context,
            FilterDictionaryDocumentType filter);
        void UpdateDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType);
        void DeleteDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType);
        int AddDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType);
        IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter);
        #endregion
        

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

        #region DictionaryPositions
        BaseDictionaryPosition GetDictionaryPosition(IContext context, int id);

        IEnumerable<BaseDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter);
        IEnumerable<InternalDictionaryPositionWithActions> GetDictionaryPositionsWithActions(IContext context, FilterDictionaryPosition filter);
        #endregion DictionaryPositions

        #region DictionaryRegistrationJournals
        BaseDictionaryRegistrationJournal GetDictionaryRegistrationJournal(IContext context, int id);

        IEnumerable<BaseDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter);
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

        InternalDictionaryTag GetInternalDictionaryTags(IContext context, FilterDictionaryTag filter);
        IEnumerable<FrontDictionaryTag> GetDictionaryTags(IContext context, FilterDictionaryTag filter);
        int AddDictionaryTag(IContext context, InternalDictionaryTag model);
        void UpdateDictionaryTag(IContext context, InternalDictionaryTag model);
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