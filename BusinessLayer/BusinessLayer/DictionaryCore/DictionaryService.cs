using BL.Logic.DictionaryCore.Interfaces;
using System.Collections.Generic;
using BL.Model.DictionaryCore;
using BL.Model.SystemCore;
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
        public FrontDictionaryAgent GetDictionaryAgent(IContext context, int id)
        {

            return _dictDb.GetDictionaryAgent(context, id);
        }

        public IEnumerable<FrontDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter,UIPaging paging)
        {

            return _dictDb.GetDictionaryAgents(context, filter,paging);
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
        public FrontDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id)
        {

            return _dictDb.GetDictionaryAgentPerson(context, id);
        }

        public IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging)
        {

            return _dictDb.GetDictionaryAgentPersons(context, filter,paging);
        }
        #endregion DictionaryAgentPersons

        #region DicionaryAgentEmployees

        public FrontDictionaryAgentEmployee GetDictionaryAgentEmployee(IContext context, int id)
        {
            return _dictDb.GetDictionaryAgentEmployee(context, id);
        }

        public IEnumerable<FrontDictionaryAgentEmployee> GetDictionaryAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            return _dictDb.GetDictionaryAgentEmployees(context, filter,paging);
        }
       
        #endregion DictionaryAgentEmployees
       
        #region DictionaryAgentAdress
        public FrontDictionaryAgentAddress GetDictionaryAgentAddress(IContext context, int id)
        {
            return _dictDb.GetDictionaryAgentAddress(context, id);
        }

        public IEnumerable<FrontDictionaryAgentAddress> GetDictionaryAgentAddresses(IContext context,int agentId, FilterDictionaryAgentAddress filter)
        {
            return _dictDb.GetDictionaryAgentAddresses(context, agentId,filter);
        }
        #endregion

        #region DictionaryAddressTypes

        public FrontDictionaryAddressType GetDictionaryAddressType(IContext context, int id)
        {
            return _dictDb.GetDictionaryAddressTypes(context, new FilterDictionaryAddressType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            return _dictDb.GetDictionaryAddressTypes(context, filter);
        }


        #endregion

        #region DictionaryAgentAccounts
        public FrontDictionaryAgentAccount GetDictionaryAgentAccount(IContext context, int id)
        {
            return _dictDb.GetDictionaryAgentAccount(context, id);
        }

        public IEnumerable<FrontDictionaryAgentAccount> GetDictionaryAgentAccounts(IContext context, int AgentId,FilterDictionaryAgentAccount filter)
        {
            return _dictDb.GetDictionaryAgentAccounts(context, AgentId,filter);
        }
        #endregion DictionaryAgentAccounts

        #region DictionaryAgentCompanies
        public FrontDictionaryAgentCompany GetDictionaryAgentCompany(IContext context, int id)
        {

            return _dictDb.GetDictionaryAgentCompany(context, id);
        }

        public IEnumerable<FrontDictionaryAgentCompany> GetDictionaryAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {

            return _dictDb.GetDictionaryAgentCompanies(context, filter,paging);
        }
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        public FrontDictionaryAgentBank GetDictionaryAgentBank(IContext context, int id)
        {

            return _dictDb.GetDictionaryAgentBank(context, id);
        }

        public IEnumerable<FrontDictionaryAgentBank> GetDictionaryAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging)
        {

            return _dictDb.GetDictionaryAgentBanks(context, filter,paging);
        }
        #endregion DictionaryAgentCompanies

        #region DictionaryContacts
        public FrontDictionaryContact GetDictionaryContact(IContext context, int id)
        {
            return _dictDb.GetDictionaryContact(context, new FilterDictionaryContact { Id = id });
        }

        public IEnumerable<FrontDictionaryContact> GetDictionaryContacts(IContext context,int agentId, FilterDictionaryContact filter)
        {
            return _dictDb.GetDictionaryContacts(context,agentId, filter);  
        }
        #endregion

        #region DictionaryContactTypes
        public FrontDictionaryContactType GetDictionaryContactType(IContext context, int id)
        {
            return _dictDb.GetDictionaryContactTypes(context, new FilterDictionaryContactType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryContactType> GetDictionaryContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            return _dictDb.GetDictionaryContactTypes(context, filter);
        }
        #endregion
              
        #region DictionaryDepartments
        public FrontDictionaryDepartment GetDictionaryDepartment(IContext context, int id)
        {

            return _dictDb.GetDictionaryDepartments(context, new FilterDictionaryDepartment { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter)
        {

            return _dictDb.GetDictionaryDepartments(context, filter);
        }
        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        public FrontDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id)
        {

            return _dictDb.GetDictionaryDocumentDirection(context, id);
        }

        public IEnumerable<FrontDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {

            return _dictDb.GetDictionaryDocumentDirections(context, filter);
        }
        #endregion DictionaryDepartments

        // Тематики документов
        #region DictionaryDocumentSubjects
        public FrontDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id)
        {

            return _dictDb.GetDictionaryDocumentSubjects(context, new FilterDictionaryDocumentSubject {IDs = new List<int> {id}}).FirstOrDefault();
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
            return _dictDb.GetDictionaryDocumentTypes(context, new FilterDictionaryDocumentType { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            return _dictDb.GetDictionaryDocumentTypes(context, filter);
        }

        #endregion DictionaryDocumentTypes

        #region DictionaryEventTypes
        public FrontDictionaryEventType GetDictionaryEventType(IContext context, int id)
        {

            return _dictDb.GetDictionaryEventType(context, id);
        }

        public IEnumerable<FrontDictionaryEventType> GetDictionaryEventTypes(IContext context, FilterDictionaryEventType filter)
        {

            return _dictDb.GetDictionaryEventTypes(context, filter);
        }
        #endregion DictionaryEventTypes

        #region DictionaryImportanceEventTypes
        public FrontDictionaryImportanceEventType GetDictionaryImportanceEventType(IContext context, int id)
        {

            return _dictDb.GetDictionaryImportanceEventType(context, id);
        }

        public IEnumerable<FrontDictionaryImportanceEventType> GetDictionaryImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter)
        {

            return _dictDb.GetDictionaryImportanceEventTypes(context, filter);
        }
        #endregion DictionaryImportanceEventTypes

        #region DictionaryLinkTypes
        public FrontDictionaryLinkType GetDictionaryLinkType(IContext context, int id)
        {

            return _dictDb.GetDictionaryLinkType(context, id);
        }

        public IEnumerable<FrontDictionaryLinkType> GetDictionaryLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {

            return _dictDb.GetDictionaryLinkTypes(context, filter);
        }
        #endregion DictionaryLinkTypes

        // Штатное расписание
        #region DictionaryPositions
        public FrontDictionaryPosition GetDictionaryPosition(IContext context, int id)
        {

            return _dictDb.GetDictionaryPosition(context, id);
        }

        public IEnumerable<FrontDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter)
        {

            return _dictDb.GetDictionaryPositions(context, filter);
        }
        #endregion DictionaryPositions

        // Журналы регистрации
        #region DictionaryRegistrationJournals
        public FrontDictionaryRegistrationJournal GetDictionaryRegistrationJournal(IContext context, int id)
        {

            return _dictDb.GetDictionaryRegistrationJournals(context, new FilterDictionaryRegistrationJournal { IDs = new List<int> { id } }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {

            return _dictDb.GetDictionaryRegistrationJournals(context, filter);
        }
        #endregion DictionaryRegistrationJournals

        #region DictionaryResultTypes
        public FrontDictionaryResultType GetDictionaryResultType(IContext context, int id)
        {

            return _dictDb.GetDictionaryResultType(context, id);
        }

        public IEnumerable<FrontDictionaryResultType> GetDictionaryResultTypes(IContext context, FilterDictionaryResultType filter)
        {

            return _dictDb.GetDictionaryResultTypes(context, filter);
        }
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        public FrontDictionarySendType GetDictionarySendType(IContext context, int id)
        {

            return _dictDb.GetDictionarySendType(context, id);
        }

        public IEnumerable<FrontDictionarySendType> GetDictionarySendTypes(IContext context, FilterDictionarySendType filter)
        {

            return _dictDb.GetDictionarySendTypes(context, filter);
        }
        #endregion DictionarySendTypes

        #region DictionaryStandartSendListContents
        public FrontDictionaryStandartSendListContent GetDictionaryStandartSendListContent(IContext context, int id)
        {

            return _dictDb.GetDictionaryStandartSendListContent(context, id);
        }

        public IEnumerable<FrontDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {

            return _dictDb.GetDictionaryStandartSendListContents(context, filter);
        }
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        public FrontDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id)
        {

            return _dictDb.GetDictionaryStandartSendList(context, id);
        }

        public IEnumerable<FrontDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {

            return _dictDb.GetDictionaryStandartSendLists(context, filter);
        }
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        public FrontDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id)
        {

            return _dictDb.GetDictionarySubordinationType(context, id);
        }

        public IEnumerable<FrontDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
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
            return _dictDb.GetDictionaryTags(context, new FilterDictionaryTag { IDs = new List<int> { id } }).FirstOrDefault();
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
