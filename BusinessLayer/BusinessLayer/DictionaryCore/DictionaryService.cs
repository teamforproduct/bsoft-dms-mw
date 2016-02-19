using System;
using BL.Logic.DictionaryCore.Interfaces;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;
using BL.Database.Dictionaries.Interfaces;
using System.Linq;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public class DictionaryService : IDictionaryService
    {
        private readonly IDictionariesDbProcess _dictDb;

        public DictionaryService(IDictionariesDbProcess dictDb)
        {
            _dictDb = dictDb;
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
        public BaseDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id)
        {
            
            return _dictDb.GetDictionaryAgentPerson(context, id);
        }

        public IEnumerable<BaseDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            
            return _dictDb.GetDictionaryAgentPersons(context, filter);
        }
        #endregion DictionaryAgentPersons

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
        public BaseDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id)
        {
            
            return _dictDb.GetDictionaryDocumentSubject(context, id);
        }

        public IEnumerable<BaseDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {
            
            return _dictDb.GetDictionaryDocumentSubjects(context, filter);
        }
        #endregion DictionaryDocumentSubjects

        #region DictionaryDocumentTypes
        // следить за списком полей необхдимых в каждом конкретном случае
        public FrontDictionaryDocumentType GetDictionaryDocumentType(IContext context, int id)
        {
            return _dictDb.GetDictionaryDocumentTypes(context, new FilterDictionaryDocumentType {Id = new List<int> {id} }).FirstOrDefault();
        }

        public IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            return _dictDb.GetDictionaryDocumentTypes(context, filter);
        }

        public void ModifyDictionaryDocumentType(IContext context, ModifyDictionaryDocumentType docType)
        {
            var spr = _dictDb.GetInternalDictionaryDocumentType(context, new FilterDictionaryDocumentType { Name = docType.Name });
            if (spr != null)
            {
                throw new DictionaryRecordNotUnique();
            }
            try
            {
                var newDocType = new InternalDictionaryDocumentType
                {
                    Id = docType.Id,
                    Name = docType.Name,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                };
                _dictDb.UpdateDictionaryDocumentType(context, newDocType);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
        }

        public int AddDictionaryDocumentType(IContext context, ModifyDictionaryDocumentType docType)
        {
            var spr = _dictDb.GetInternalDictionaryDocumentType(context, new FilterDictionaryDocumentType { Name = docType.Name });
            if (spr != null)
            {
                throw new DictionaryRecordNotUnique();
            }
            try
            {
                var newDocType = new InternalDictionaryDocumentType
                {
                    Name = docType.Name,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                };
                return _dictDb.AddDictionaryDocumentType(context, newDocType);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        #endregion DictionaryDocumentSubjects

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
            return _dictDb.GetDictionaryTags(context, new FilterDictionaryTag { Id = new List<int> { id } }).FirstOrDefault();
        }

        public int AddDictionaryTag(IContext context, ModifyDictionaryTag model)
        {
            try
            {
                var item = new InternalDictionaryTag
                {
                    PositionId = context.CurrentPositionId,
                    Color = model.Color,
                    Name = model.Name,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                };
                return _dictDb.AddDictionaryTag(context, item);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        public void ModifyDictionaryTag(IContext context, ModifyDictionaryTag model)
        {
            try
            {
                var item = new InternalDictionaryTag
                {
                    Id = model.Id,
                    Name = model.Name,
                    Color = model.Color,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                };
                _dictDb.UpdateDictionaryTag(context, item);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch(DictionaryTagNotFoundOrUserHasNoAccess)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
        }

        #endregion DictionaryTags

    }
}
