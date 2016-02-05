using System.Collections.Generic;
using BL.CrossCutting.Common;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore;
using System;
using BL.Logic.DocumentCore.Interfaces;
using System.Linq;
using BL.Model.SystemCore;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.Enums;
using System.Web.Script.Serialization;
using BL.Model.DocumentCore.Actions;
using BL.Model.Database;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore
{
    internal class DocumentService : IDocumentService
    {
        #region Documents
        public int SaveDocument(IContext context, FullDocument document)
        {
            Command cmd;
            if (document.Id == 0) // new document
            {
                cmd = new AddDocument(context, document);
            }
            else
            {
                cmd = new UpdateDocument(context, document);
            }

            if (cmd.CanExecute())
            {
                cmd.Execute();
            }
            return document.Id;
        }

        public IEnumerable<FullDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            return documentDb.GetDocuments(ctx, filters, paging);
        }

        public FullDocument GetDocument(IContext ctx, int documentId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            return documentDb.GetDocument(ctx, documentId);
        }

        public int AddDocumentByTemplateDocument(IContext context, int templateDocumentId)
        {
            var db = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var baseTemplateDocument = db.GetTemplateDocument(context, templateDocumentId);
            var baseDocument = new FullDocument
            {
                TemplateDocumentId = baseTemplateDocument.Id,
                CreateDate = DateTime.Now,
                DocumentSubjectId = baseTemplateDocument.DocumentSubjectId,
                Description = baseTemplateDocument.Description,
                ExecutorPositionId = context.CurrentPositionId, ////
                SenderAgentId = baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External?baseTemplateDocument.SenderAgentId:null,
                SenderAgentPersonId = baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External ? baseTemplateDocument.SenderAgentPersonId : null,
                Addressee = baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External ? baseTemplateDocument.Addressee : null
            };

            if (baseTemplateDocument.RestrictedSendLists != null && baseTemplateDocument.RestrictedSendLists.Any())
            {
                baseDocument.RestrictedSendLists = baseTemplateDocument.RestrictedSendLists.Select(x => new BaseDocumentRestrictedSendList()
                {
                    PositionId = x.PositionId,
                    AccessLevelId = (int)x.AccessLevel
                }).ToList();
            }

            if (baseTemplateDocument.SendLists != null && baseTemplateDocument.SendLists.Any())
            {
                baseDocument.SendLists = baseTemplateDocument.SendLists.Select(x => new BaseDocumentSendList
                {
                    OrderNumber = x.OrderNumber,
                    SendType =x.SendType,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevel = x.AccessLevel,
                    IsInitial = true,
                    EventId = null
                }).ToList();
            }

            var evt = new BaseDocumentEvent
            {
                EventType = EnumEventTypes.AddNewDocument,
                Description = "Create",
                LastChangeUserId = context.CurrentAgentId,
                SourceAgentId = context.CurrentAgentId,
                TargetAgentId = context.CurrentAgentId,
                TargetPositionId = context.CurrentPositionId,
                SourcePositionId = context.CurrentPositionId
            };

            baseDocument.Events = new List<BaseDocumentEvent> { evt };

            var acc = new BaseDocumentAccess
            {
                AccessLevel = EnumDocumentAccess.PersonalRefIO,
                IsInWork = true,
                IsFavourite = false,
                LastChangeDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId,
                PositionId = context.CurrentPositionId,
            };

            baseDocument.Accesses = new List<BaseDocumentAccess>() { acc };

            return SaveDocument(context, baseDocument);
        }

        public int ModifyDocument(IContext context, ModifyDocument document)
        {
            var baseDocument = new FullDocument(document);
            var db = DmsResolver.Current.Get<ITemplateDocumentsDbProcess>();
            var baseTemplateDocument = db.GetTemplateDocument(context, baseDocument.TemplateDocumentId);
            if (baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External)
            {
                throw new UserPositionIsNotDefined();
            }
            if ( 
                (
                        (baseTemplateDocument.DocumentDirection == EnumDocumentDirections.Inner)
                    &&
                    (
                        baseDocument.SenderAgentId != null || 
                        baseDocument.SenderAgentPersonId != null || 
                        !string.IsNullOrEmpty(baseDocument.SenderNumber) || 
                        baseDocument.SenderDate != null || 
                        !string.IsNullOrEmpty(baseDocument.Addressee)
                    )
                )
                ||
                (
                        (baseTemplateDocument.DocumentDirection == EnumDocumentDirections.External)
                    &&
                    (
                        baseDocument.SenderAgentId == null ||
                        baseDocument.SenderAgentPersonId == null ||
                        string.IsNullOrEmpty(baseDocument.SenderNumber) ||
                        baseDocument.SenderDate == null ||
                        string.IsNullOrEmpty(baseDocument.Addressee)
                    )
                )
                )
            {
                throw new WrongInformationAboutCorrespondent();
            }
            return SaveDocument(context, baseDocument);
        }
        #endregion Documents

        #region DocumentRestrictedSendLists
        public int AddRestrictedSendList(IContext context, ModifyDocumentRestrictedSendList restrictedSendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var id = documentDb.AddRestrictedSendList(context, restrictedSendList);
            return id;
        }

        public void AddRestrictedSendListByStandartSendLists(IContext context, ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dicStandSendList = dictDb.GetDictionaryStandartSendList(context, model.StandartSendListId);

            var restrictedSendLists = dicStandSendList.StandartSendListContents.Select(x => new ModifyDocumentRestrictedSendList
            {
                DocumentId = model.DocumentId,
                PositionId = x.TargetPositionId,
                AccessLevelId = x.AccessLevelId.GetValueOrDefault()
            });
            var docDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            docDb.AddRestrictedSendList(context, restrictedSendLists);
        }

        public void DeleteRestrictedSendList(IContext context, int restrictedSendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteRestrictedSendList(context, restrictedSendListId);
        }
        #endregion DocumentRestrictedSendLists

        #region DocumentSendLists
        public int AddSendList(IContext context, ModifyDocumentSendList sendList)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var id = documentDb.AddSendList(context, sendList);
            return id;
        }

        public void AddSendListByStandartSendLists(IContext context, ModifyDocumentSendListByStandartSendList model)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            var dicStandSendList = dictDb.GetDictionaryStandartSendList(context, model.StandartSendListId);

            var sendLists = dicStandSendList.StandartSendListContents.Select(x => new ModifyDocumentSendList
            {
                DocumentId = model.DocumentId,
                OrderNumber = x.OrderNumber,
                SendType = (EnumSendType)x.SendTypeId,
                TargetPositionId = x.TargetPositionId,
                Description = x.Description,
                DueDate = x.DueDate,
                DueDay = x.DueDay.GetValueOrDefault(),
                AccessLevel = (EnumDocumentAccess)x.AccessLevelId.GetValueOrDefault(),
            });
            var docDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            docDb.AddSendList(context, sendLists);
        }

        public void DeleteSendList(IContext context, int sendListId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteSendList(context, sendListId);
        }

        #endregion DocumentSendLists

        #region DocumentSavedFilters
        public int SaveSavedFilter(IContext context, ModifyDocumentSavedFilter savedFilter)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            if (savedFilter.Id == 0) // new savedFilter
            {
                documentDb.AddSavedFilters(context, savedFilter);
            }
            else
            {
                documentDb.UpdateSavedFilters(context, savedFilter);
            }

            return savedFilter.Id;
        }

        public IEnumerable<BaseDocumentSavedFilter> GetSavedFilters(IContext ctx)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var savedFilters = documentDb.GetSavedFilters(ctx).ToList();
            var js = new JavaScriptSerializer();
            for (int i = 0, l = savedFilters.Count; i < l; i++)
            {
                try
                {
                    savedFilters[i].Filter = js.DeserializeObject(savedFilters[i].Filter.ToString());
                }
                catch
                {
                }
            }
            return savedFilters;
        }

        public BaseDocumentSavedFilter GetSavedFilter(IContext ctx, int savedFilterId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var savedFilter = documentDb.GetSavedFilter(ctx, savedFilterId);
            var js = new JavaScriptSerializer();
            try
            {
                savedFilter.Filter = js.DeserializeObject(savedFilter.Filter.ToString());
            }
            catch
            {
            }
            return savedFilter;
        }
        public void DeleteSavedFilter(IContext context, int savedFilterId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.DeleteSavedFilter(context, savedFilterId);
        }
        #endregion DocumentSavedFilters

        #region DocumentEvents

        public void AddDocumentComment(IContext context, AddNote note)
        {
            var evt = new BaseDocumentEvent
            {
                DocumentId = note.Id,
                Description = note.Description,
                EventType = EnumEventTypes.AddNote,
                SourceAgentId = context.CurrentAgentId,
                TargetAgentId = context.CurrentAgentId,
                SourcePositionId = context.CurrentPositionId,
                TargetPositionId = context.CurrentPositionId,
                LastChangeUserId = context.CurrentAgentId,

            };

            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();
            db.AddDocumentEvent(context, evt);
        }

        #endregion

        #region Operation with document
        public int AddDocumentAccess(IContext ctx, BaseDocumentAccess access)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            return documentDb.AddDocumentAccess(ctx, access);
        }

        public void RemoveDocumentAccess(IContext ctx, int accessId)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();
            documentDb.RemoveDocumentAccess(ctx, accessId);
        }

        public void ChangeDocumentWorkStatus(IContext context, ChangeWorkStatus newStatus)
        {
            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var acc = db.GetDocumentAccess(context, newStatus.Id);
            if (acc == null)
            {
                throw new UserHasNoAccessToDocument();
            }
            acc.IsInWork = newStatus.IsInWork;
            var ea = new EventAccessModel
            {
                DocumentAccess = acc,
                DocumentEvent = new BaseDocumentEvent
                {
                    DocumentId = newStatus.Id,
                    SourceAgentId = context.CurrentAgentId,
                    TargetAgentId = context.CurrentAgentId,
                    SourcePositionId = context.CurrentPositionId,
                    TargetPositionId = context.CurrentPositionId,
                    Description = newStatus.Description,
                    EventType = newStatus.IsInWork ? EnumEventTypes.SetInWork : EnumEventTypes.SetOutWork,
                    LastChangeUserId = context.CurrentAgentId
                }
            };

            db.SetDocumentInformation(context, ea);
        }

        public void ChangeFavouritesForDocument(IContext context, ChangeFavourites model)
        {
            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();
            var acc = db.GetDocumentAccess(context, model.DocumentId);
            acc.IsFavourite = model.IsFavourite;
            db.UpdateDocumentAccess(context, acc);
        }

        public void ControlOn(IContext context, ControlOn model)
        {
            var db = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var docWait = new BaseDocumentWaits
            {
                DocumentId = model.DocumentId,
                Description = model.Description,
                DueDate = model.DueDate,
                AttentionDate = model.AttentionDate,
                OnEvent = new BaseDocumentEvent
                {
                    DocumentId = model.DocumentId,
                    EventType = EnumEventTypes.ControlOn,
                    CreateDate = DateTime.Now,
                    Date = DateTime.Now,
                    Description = model.Description,
                    SourcePositionId = context.CurrentPositionId,
                    SourceAgentId = context.CurrentAgentId,
                    TargetPositionId = context.CurrentPositionId,
                    TargetAgentId = context.CurrentAgentId
                }
            };
        }

        public int CopyDocument(IContext context, CopyDocument model)
        {
            var documentDb = DmsResolver.Current.Get<IDocumentsDbProcess>();

            var document = documentDb.GetDocument(context, model.DocumentId);

            document.Id = 0;
            document.CreateDate = DateTime.Now;
            document.IsFavourite = false;
            document.RegistrationNumber = null;
            document.RegistrationNumberPrefix = null;
            document.RegistrationNumberSuffix = null;
            document.RegistrationJournalId = null;

            //if (document.RestrictedSendLists != null && document.RestrictedSendLists.Any())
            //{
            //    var restrictedSendLists = document.RestrictedSendLists.ToList();
            //    for(int i=0,l= restrictedSendLists.Count;i< l;i++)
            //    {
            //        restrictedSendLists[i].Id = 0;
            //    }
            //    document.RestrictedSendLists = restrictedSendLists;
            //}

            //if (document.SendLists != null && document.SendLists.Any())
            //{
            //    var sendLists = document.SendLists.ToList();
            //    for (int i = 0, l = sendLists.Count; i < l; i++)
            //    {
            //        sendLists[i].Id = 0;
            //    }
            //    document.SendLists = sendLists;
            //}

            var evt = new BaseDocumentEvent
            {
                EventType = EnumEventTypes.AddNewDocument,
                Description = "Create",
                LastChangeUserId = context.CurrentAgentId,
                SourceAgentId = context.CurrentAgentId,
                TargetAgentId = context.CurrentAgentId,
                TargetPositionId = context.CurrentPositionId,
                SourcePositionId = context.CurrentPositionId
            };

            document.Events = new List<BaseDocumentEvent> { evt };

            var acc = new BaseDocumentAccess
            {
                AccessLevel = EnumDocumentAccess.PersonalRefIO,
                IsInWork = true,
                IsFavourite = false,
                LastChangeDate = DateTime.Now,
                LastChangeUserId = context.CurrentAgentId,
                PositionId = context.CurrentPositionId,
            };

            document.Accesses = new List<BaseDocumentAccess>() { acc };

            return SaveDocument(context, document);
        }

        #endregion
    }
        public void RegisterDocument(IContext context, RegisterDocument model)
        {
            var docDB = DmsResolver.Current.Get<IDocumentsDbProcess>();


            if (model.RegistrationNumber == null|| string.IsNullOrEmpty(model.RegistrationNumberPrefix)|| model.IsOnlyGetNextNumber)
            {   //get next number
                var dictDB = DmsResolver.Current.Get<IDictionariesDbProcess>();
                var dictRegJournal = dictDB.GetDictionaryRegistrationJournal(context, model.RegistrationJournalId);
                model.RegistrationNumberPrefix = dictRegJournal.PrefixFormula;
                model.RegistrationNumberSuffix = dictRegJournal.SuffixFormula;
                model.RegistrationNumber = null;
                docDB.AddTemporaryRegistration(context, model);
            }
            else
            {
                var registerDocument = docDB.GetTemporaryRegistration(context, model.Id);
                if (registerDocument.RegistrationJournalId != model.RegistrationJournalId
                        || registerDocument.RegistrationNumberPrefix != model.RegistrationNumberPrefix
                        || registerDocument.RegistrationNumberSuffix != model.RegistrationNumberSuffix
                        || registerDocument.RegistrationNumber != model.RegistrationNumber
                        || registerDocument.RegistrationDate != model.RegistrationDate
                    )
                {
                    docDB.AddTemporaryRegistration(context, model);
                }
            }



            if (!model.IsOnlyGetNextNumber)
            {   //проставляем 
                docDB.SetDocumentRegistration(context, model.Id);
            }
        }

        #endregion
    }
}