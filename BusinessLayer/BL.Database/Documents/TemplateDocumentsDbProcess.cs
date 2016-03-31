using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Database.DBModel.Template;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Database.Documents
{
    public class TemplateDocumentsDbProcess : CoreDb.CoreDb, ITemplateDocumentsDbProcess
    {

        #region TemplateDocuments
        public IEnumerable<FrontTemplateDocument> GetTemplateDocument(IContext ctx)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return dbContext.TemplateDocumentsSet.Select(x => new FrontTemplateDocument
                {
                    Id = x.Id,
                    DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                    DocumentTypeId = x.DocumentTypeId,
                    Name = x.Name,
                    Description = x.Description,
                    DocumentSubjectId = x.DocumentSubjectId,
                    LastChangeDate = x.LastChangeDate,
                    RegistrationJournalId = x.RegistrationJournalId,
                    DocumentSubjectName = x.DocumentSubject.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    DocumentDirectionName = x.DocumentDirection.Name
                }).ToList();
            }
        }

        public FrontTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId)
        {
            int templateDocumentId = 0;
            using (var dbContext = new DmsContext(ctx))
            {
                templateDocumentId = dbContext.DocumentsSet.Where(x => x.Id == documentId).Select(x => x.TemplateDocumentId).FirstOrDefault();
            }

            return GetTemplateDocument(ctx, templateDocumentId);
        }

        public FrontTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return
                    dbContext.TemplateDocumentsSet.Where(x => x.Id == templateDocumentId)
                        .Select(x => new FrontTemplateDocument
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsHard = x.IsHard,
                            DocumentDirection = (EnumDocumentDirections) x.DocumentDirectionId,
                            DocumentTypeId = x.DocumentTypeId,
                            Description = x.Description,
                            DocumentSubjectId = x.DocumentSubjectId,
                            RegistrationJournalId = x.RegistrationJournalId,
                            SenderAgentId = x.SenderAgentId,
                            SenderAgentPersonId = x.SenderAgentPersonId,
                            Addressee = x.Addressee,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate,
                            RestrictedSendLists =
                                x.RestrictedSendLists.Select(y => new FrontTemplateDocumentRestrictedSendLists()
                                {
                                    PositionId = y.PositionId,
                                    AccessLevel = (EnumDocumentAccesses) y.AccessLevelId
                                }).ToList(),
                            SendLists = x.SendLists.Select(y => new FrontTemplateDocumentSendLists()
                            {
                                SendType = (EnumSendTypes) y.SendTypeId,
                                TargetPositionId = y.TargetPositionId,
                                Description = y.Description,
                                Stage = y.Stage,
                                Task = y.Task.Task,
                                DueDay = y.DueDay,
                                AccessLevel = (EnumDocumentAccesses) y.AccessLevelId
                            }).ToList()
                        }).FirstOrDefault();
            }
        }

        public int AddOrUpdateTemplate(IContext ctx, ModifyTemplateDocument template)
        {
            // we should not implement it now
            //var dbContext = GetUserDmsContext(context);
            using (var dbContext = new DmsContext(ctx))
            {
                var newTemplate = new TemplateDocuments()
                    {
                        Name = template.Name,
                        IsHard = template.IsHard,
                        DocumentDirectionId = (int) template.DocumentDirection,
                        DocumentTypeId = template.DocumentTypeId,
                        DocumentSubjectId = template.DocumentSubjectId,
                        Description = template.Description,
                        RegistrationJournalId = template.RegistrationJournalId,
                        SenderAgentId = template.SenderAgentId,
                        SenderAgentPersonId = template.SenderAgentPersonId,
                        Addressee = template.Addressee,
                        IsActive = template.IsActive
                    };

                if (template.Id.HasValue)
                {
                    newTemplate.Id = (int)template.Id;
                }

                dbContext.TemplateDocumentsSet.Attach(newTemplate);

                var entity = dbContext.Entry(newTemplate);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();

                return newTemplate.Id;
            }
        }

        public void DeleteTemplate(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = dbContext.TemplateDocumentsSet.FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentsSet.Remove(ddt);
                dbContext.SaveChanges();
            }
        }

        public bool CanModifyTemplate(IContext ctx, ModifyTemplateDocument template)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //TODO: Уточнить безнес-логику, в каких случаях можно менять/удалять шаблон документа
                var count = dbContext.DocumentsSet.Count(x => x.TemplateDocumentId == template.Id);

                return count == 0;
            }
        }

        public bool CanModifyTemplate(IContext ctx, int templateId)
        {
               return CanModifyTemplate(ctx, new ModifyTemplateDocument() {Id = templateId});           
        }
        #endregion TemplateDocuments

        #region TemplateDocumentSendLists

        public IEnumerable<FrontTemplateDocumentSendLists> GetTemplateDocumentSendLists(IContext ctx, int templateId,
            FilterTemplateDocumentSendList filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.TemplateDocumentSendListsSet.AsQueryable();
                if (filter.Id.Count > 0)
                {
                    qry = qry.Where(x => filter.Id.Contains(x.Id));
                }
                if (filter.SendType.HasValue)
                {
                    qry = qry.Where(x => x.SendTypeId == (int) filter.SendType);
                }
                if (filter.TargetPositionId.HasValue)
                {
                    qry = qry.Where(x => x.TargetPositionId == filter.TargetPositionId);
                }
                if (filter.Stage.HasValue)
                {
                    qry = qry.Where(x => x.Stage == filter.Stage);
                }
                if (!string.IsNullOrEmpty(filter.Task))
                {
                    qry = qry.Where(x => x.Task.Task.Contains(filter.Task));
                }

                return qry.Select(x => new FrontTemplateDocumentSendLists()
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    SendType = (EnumSendTypes)x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    Stage = x.Stage,
                    Task = x.Task.Task,
                    DueDay = x.DueDay,
                    AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                    PositionName = x.TargetPosition.Name,
                    SendTypeName = x.SendType.Name,
                    AccessLevelName = x.AccessLevel.Name,
                }).ToList();

            }
        }

        public FrontTemplateDocumentSendLists GetTemplateDocumentSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return dbContext.TemplateDocumentSendListsSet.Where(x => x.Id == id).Select(x => new FrontTemplateDocumentSendLists()
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    SendType = (EnumSendTypes) x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    Stage = x.Stage,
                    Task = x.Task.Task,
                    DueDay = x.DueDay,
                    AccessLevel = (EnumDocumentAccesses) x.AccessLevelId,
                    PositionName = x.TargetPosition.Name,
                    SendTypeName = x.SendType.Name,
                    AccessLevelName = x.AccessLevel.Name,
                    IsAddControl = x.IsAddControl,
                    IsAvailableWithinTask = x.IsAvailableWithinTask
                }).FirstOrDefault();
            }
        }

        public int AddOrUpdateTemplateSendList(IContext ctx, ModifyTemplateDocumentSendLists template)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var newTemplate=new TemplateDocumentSendLists()
                {
                   DocumentId=template.DocumentId,
                   Description = template.Description,
                   IsAddControl = template.IsAddControl,
                   IsAvailableWithinTask = template.IsAvailableWithinTask,
                   TargetPositionId = template.TargetPositionId,
                   AccessLevelId = (int)template.AccessLevel,
                   DueDay = template.DueDay,
                   SendTypeId = (int)template.SendType,
                   SourcePositionId = template.SourcePositionId,
                   Stage = template.Stage,
                   TargetAgentId = template.TargetAgentId,
                   TaskId = template.TaskId,
                   LastChangeDate = template.LastChangeDate,
                   LastChangeUserId = template.LastChangeUserId
                };

                if (template.Id.HasValue)
                {
                    newTemplate.Id = (int)template.Id;
                }

                dbContext.TemplateDocumentSendListsSet.Attach(newTemplate);

                var entity = dbContext.Entry(newTemplate);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();

                return newTemplate.Id;
            }
        }

        public void DeleteTemplateSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var ddt = dbContext.TemplateDocumentSendListsSet.FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentSendListsSet.Remove(ddt);
                dbContext.SaveChanges();
            }
        }

        #endregion TemplateDocumentSendLists
    }
}