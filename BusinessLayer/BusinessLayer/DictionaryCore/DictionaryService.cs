
using BL.Logic.DictionaryCore.Interfaces;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;
using BL.CrossCutting.DependencyInjection;
using BL.Database.Dictionaries.Interfaces;

namespace BL.Logic.DictionaryCore
{
    public class DictionaryService : IDictionaryService
    {
        #region DictionaryAgents
        public BaseDictionaryAgent GetDictionaryAgent(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryAgent(context, id);
        }

        public IEnumerable<BaseDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryAgents(context, filter);
        }
        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        public BaseDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryAgentPerson(context, id);
        }

        public IEnumerable<BaseDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryAgentPersons(context, filter);
        }
        #endregion DictionaryAgentPersons

        #region DictionaryCompanies
        public BaseDictionaryCompany GetDictionaryCompany(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryCompany(context, id);
        }

        public IEnumerable<BaseDictionaryCompany> GetDictionaryCompanies(IContext context, FilterDictionaryCompany filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryCompanies(context, filter);
        }
        #endregion DictionaryCompanies

        #region DictionaryDepartments
        public BaseDictionaryDepartment GetDictionaryDepartment(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryDepartment(context, id);
        }

        public IEnumerable<BaseDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryDepartments(context, filter);
        }
        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        public BaseDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryDocumentDirection(context, id);
        }

        public IEnumerable<BaseDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryDocumentDirections(context, filter);
        }
        #endregion DictionaryDepartments

        #region DictionaryDocumentSubjects
        public BaseDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryDocumentSubject(context, id);
        }

        public IEnumerable<BaseDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryDocumentSubjects(context, filter);
        }
        #endregion DictionaryDocumentSubjects

        #region DictionaryDocumentTypes
        public BaseDictionaryDocumentType GetDictionaryDocumentType(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryDocumentType(context, id);
        }

        public IEnumerable<BaseDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryDocumentTypes(context, filter);
        }
        #endregion DictionaryDocumentSubjects

        #region DictionaryEventTypes
        public BaseDictionaryEventType GetDictionaryEventType(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryEventType(context, id);
        }

        public IEnumerable<BaseDictionaryEventType> GetDictionaryEventTypes(IContext context, FilterDictionaryEventType filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryEventTypes(context, filter);
        }
        #endregion DictionaryEventTypes

        #region DictionaryImportanceEventTypes
        public BaseDictionaryImportanceEventType GetDictionaryImportanceEventType(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryImportanceEventType(context, id);
        }

        public IEnumerable<BaseDictionaryImportanceEventType> GetDictionaryImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryImportanceEventTypes(context, filter);
        }
        #endregion DictionaryImportanceEventTypes

        #region DictionaryLinkTypes
        public BaseDictionaryLinkType GetDictionaryLinkType(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryLinkType(context, id);
        }

        public IEnumerable<BaseDictionaryLinkType> GetDictionaryLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryLinkTypes(context, filter);
        }
        #endregion DictionaryLinkTypes

        #region DictionaryPositions
        public BaseDictionaryPosition GetDictionaryPosition(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryPosition(context, id);
        }

        public IEnumerable<BaseDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryPositions(context, filter);
        }
        #endregion DictionaryPositions

        #region DictionaryRegistrationJournals
        public BaseDictionaryRegistrationJournal GetDictionaryRegistrationJournal(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryRegistrationJournal(context, id);
        }

        public IEnumerable<BaseDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryRegistrationJournals(context, filter);
        }
        #endregion DictionaryRegistrationJournals

        #region DictionaryResultTypes
        public BaseDictionaryResultType GetDictionaryResultType(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryResultType(context, id);
        }

        public IEnumerable<BaseDictionaryResultType> GetDictionaryResultTypes(IContext context, FilterDictionaryResultType filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryResultTypes(context, filter);
        }
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        public BaseDictionarySendType GetDictionarySendType(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionarySendType(context, id);
        }

        public IEnumerable<BaseDictionarySendType> GetDictionarySendTypes(IContext context, FilterDictionarySendType filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionarySendTypes(context, filter);
        }
        #endregion DictionarySendTypes

        #region DictionaryStandartSendListContents
        public BaseDictionaryStandartSendListContent GetDictionaryStandartSendListContent(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryStandartSendListContent(context, id);
        }

        public IEnumerable<BaseDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryStandartSendListContents(context, filter);
        }
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        public BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryStandartSendList(context, id);
        }

        public IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryStandartSendLists(context, filter);
        }
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        public BaseDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionarySubordinationType(context, id);
        }

        public IEnumerable<BaseDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionarySubordinationTypes(context, filter);
        }
        #endregion DictionarySubordinationTypes

    }
}
