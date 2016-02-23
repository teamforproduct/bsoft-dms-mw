using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;
using System.Collections.Generic;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore.Interfaces
{
    public interface IDictionaryService
    {
        object ExecuteAction(EnumDictionaryAction act, IContext context, object param);

        #region DictionaryAgents
        BaseDictionaryAgent GetDictionaryAgent(IContext context, int id);

        IEnumerable<BaseDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter);
        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        BaseDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id);

        IEnumerable<BaseDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter);
        #endregion DictionaryAgentPersons

        #region DictionaryCompanies
        BaseDictionaryCompany GetDictionaryCompany(IContext context, int id);

        IEnumerable<BaseDictionaryCompany> GetDictionaryCompanies(IContext context, FilterDictionaryCompany filter);
        #endregion DictionaryCompanies

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

        #region DictionaryPositions
        BaseDictionaryPosition GetDictionaryPosition(IContext context, int id);

        IEnumerable<BaseDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter);
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
        IEnumerable<FrontDictionaryTag> GetDictionaryTags(IContext context, FilterDictionaryTag filter);
        FrontDictionaryTag GetDictionaryTag(IContext context, int id);
        #endregion DictionaryTags
    }
}
