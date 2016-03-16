using BL.Logic.DictionaryCore.Interfaces;
using System.Collections.Generic;
using BL.Model.DictionaryCore;
using BL.Database.Dictionaries.Interfaces;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;

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
        public BaseDictionaryAgent GetDictionaryAgent(IContext context, int id)
        {

            return _dictDb.GetDictionaryAgent(context, id);
        }

        public IEnumerable<BaseDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter)
        {

            return _dictDb.GetDictionaryAgents(context, filter);
        }
        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        public FrontDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id)
        {

            return _dictDb.GetDictionaryAgentPerson(context, id);
        }

        public IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {

            return _dictDb.GetDictionaryAgentPersons(context, filter);
        }
        #endregion DictionaryAgentPersons

        #region DictionaryAgentAdress
        public FrontDictionaryAgentAddress GetDictionaryAgentAddress(IContext context, int id)
        {
            return _dictDb.GetDictionaryAgentAddress(context, id);
        }

        public IEnumerable<FrontDictionaryAgentAddress> GetDictionaryAgentAddresses(IContext context, FilterDictionaryAgentAddress filter)
        {
            return _dictDb.GetDictionaryAgentAddresses(context, filter);
        }
        #endregion

        #region DictionaryAddressTypes

        public FrontDictionaryAddressType GetDictionaryAddressType(IContext context, int id)
        {
            return _dictDb.GetDictionaryAddressTypes(context, new FilterDictionaryAddressType { AddressTypeId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            return _dictDb.GetDictionaryAddressTypes(context, filter);
        }


        #endregion


        #region DictionaryCompanies
        public BaseDictionaryCompany GetDictionaryCompany(IContext context, int id)
        {

            return _dictDb.GetDictionaryCompany(context, id);
        }

        public IEnumerable<BaseDictionaryCompany> GetDictionaryCompanies(IContext context, FilterDictionaryCompany filter)
        {

            return _dictDb.GetDictionaryCompanies(context, filter);
        }
        #endregion DictionaryCompanies

        #region DictionaryContacts
        public FrontDictionaryContact GetDictionaryContact(IContext context, int id)
        {
            return _dictDb.GetDictionaryContacts(context, new FilterDictionaryContact { Id = id }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryContact> GetDictionaryContacts(IContext context, FilterDictionaryContact filter)
        {
            return _dictDb.GetDictionaryContacts(context, filter);  
        }
        #endregion

        #region DictionaryContactTypes
        public FrontDictionaryContactType GetDictionaryContactType(IContext context, int id)
        {
            return _dictDb.GetDictionaryContactTypes(context, new FilterDictionaryContactType { ContactTypeId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryContactType> GetDictionaryContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            return _dictDb.GetDictionaryContactTypes(context, filter);
        }
        #endregion

      
        #region DictionaryDepartments
        public BaseDictionaryDepartment GetDictionaryDepartment(IContext context, int id)
        {

            return _dictDb.GetDictionaryDepartment(context, id);
        }

        public IEnumerable<BaseDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter)
        {

            return _dictDb.GetDictionaryDepartments(context, filter);
        }
        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        public BaseDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id)
        {

            return _dictDb.GetDictionaryDocumentDirection(context, id);
        }

        public IEnumerable<BaseDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {

            return _dictDb.GetDictionaryDocumentDirections(context, filter);
        }
        #endregion DictionaryDepartments

        #region DictionaryDocumentSubjects
        public FrontDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id)
        {

            return _dictDb.GetDictionaryDocumentSubjects(context, new FilterDictionaryDocumentSubject {DocumentSubjectId = new List<int> {id}}).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {

            return _dictDb.GetDictionaryDocumentSubjects(context, filter);
        }
        #endregion DictionaryDocumentSubjects

        #region DictionaryDocumentTypes
        // следить за списком полей необхдимых в каждом конкретном случае
        public FrontDictionaryDocumentType GetDictionaryDocumentType(IContext context, int id)
        {
            return _dictDb.GetDictionaryDocumentTypes(context, new FilterDictionaryDocumentType { DocumentTypeId = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            return _dictDb.GetDictionaryDocumentTypes(context, filter);
        }

        #endregion DictionaryDocumentTypes

        #region DictionaryEventTypes
        public BaseDictionaryEventType GetDictionaryEventType(IContext context, int id)
        {

            return _dictDb.GetDictionaryEventType(context, id);
        }

        public IEnumerable<BaseDictionaryEventType> GetDictionaryEventTypes(IContext context, FilterDictionaryEventType filter)
        {

            return _dictDb.GetDictionaryEventTypes(context, filter);
        }
        #endregion DictionaryEventTypes

        #region DictionaryImportanceEventTypes
        public BaseDictionaryImportanceEventType GetDictionaryImportanceEventType(IContext context, int id)
        {

            return _dictDb.GetDictionaryImportanceEventType(context, id);
        }

        public IEnumerable<BaseDictionaryImportanceEventType> GetDictionaryImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter)
        {

            return _dictDb.GetDictionaryImportanceEventTypes(context, filter);
        }
        #endregion DictionaryImportanceEventTypes

        #region DictionaryLinkTypes
        public BaseDictionaryLinkType GetDictionaryLinkType(IContext context, int id)
        {

            return _dictDb.GetDictionaryLinkType(context, id);
        }

        public IEnumerable<BaseDictionaryLinkType> GetDictionaryLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {

            return _dictDb.GetDictionaryLinkTypes(context, filter);
        }
        #endregion DictionaryLinkTypes

        #region DictionaryPositions
        public BaseDictionaryPosition GetDictionaryPosition(IContext context, int id)
        {

            return _dictDb.GetDictionaryPosition(context, id);
        }

        public IEnumerable<BaseDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter)
        {

            return _dictDb.GetDictionaryPositions(context, filter);
        }
        #endregion DictionaryPositions

        #region DictionaryRegistrationJournals
        public BaseDictionaryRegistrationJournal GetDictionaryRegistrationJournal(IContext context, int id)
        {

            return _dictDb.GetDictionaryRegistrationJournal(context, id);
        }

        public IEnumerable<BaseDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {

            return _dictDb.GetDictionaryRegistrationJournals(context, filter);
        }
        #endregion DictionaryRegistrationJournals

        #region DictionaryResultTypes
        public BaseDictionaryResultType GetDictionaryResultType(IContext context, int id)
        {

            return _dictDb.GetDictionaryResultType(context, id);
        }

        public IEnumerable<BaseDictionaryResultType> GetDictionaryResultTypes(IContext context, FilterDictionaryResultType filter)
        {

            return _dictDb.GetDictionaryResultTypes(context, filter);
        }
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        public BaseDictionarySendType GetDictionarySendType(IContext context, int id)
        {

            return _dictDb.GetDictionarySendType(context, id);
        }

        public IEnumerable<BaseDictionarySendType> GetDictionarySendTypes(IContext context, FilterDictionarySendType filter)
        {

            return _dictDb.GetDictionarySendTypes(context, filter);
        }
        #endregion DictionarySendTypes

        #region DictionaryStandartSendListContents
        public BaseDictionaryStandartSendListContent GetDictionaryStandartSendListContent(IContext context, int id)
        {

            return _dictDb.GetDictionaryStandartSendListContent(context, id);
        }

        public IEnumerable<BaseDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {

            return _dictDb.GetDictionaryStandartSendListContents(context, filter);
        }
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        public BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id)
        {

            return _dictDb.GetDictionaryStandartSendList(context, id);
        }

        public IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {

            return _dictDb.GetDictionaryStandartSendLists(context, filter);
        }
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        public BaseDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id)
        {

            return _dictDb.GetDictionarySubordinationType(context, id);
        }

        public IEnumerable<BaseDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {

            return _dictDb.GetDictionarySubordinationTypes(context, filter);
        }
        #endregion DictionarySubordinationTypes

        #region DictionaryTags
        public IEnumerable<FrontDictionaryTag> GetDictionaryTags(IContext context, FilterDictionaryTag filter)
        {
            return _dictDb.GetDictionaryTags(context, filter);
        }

        public FrontDictionaryTag GetDictionaryTag(IContext context, int id)
        {
            return _dictDb.GetDictionaryTags(context, new FilterDictionaryTag { TagId = new List<int> { id } }).FirstOrDefault();
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
            return _dictDb.GetCustomDictionaryType(context, id);
        }
        #endregion CustomDictionaryTypes

        #region CustomDictionaries
        public IEnumerable<FrontCustomDictionary> GetCustomDictionaries(IContext context, FilterCustomDictionary filter)
        {
            return _dictDb.GetCustomDictionaries(context, filter);
        }

        public FrontCustomDictionary GetCustomDictionary(IContext context, int id)
        {
            return _dictDb.GetCustomDictionary(context, id);
        }
        #endregion CustomDictionaries
    }
}
