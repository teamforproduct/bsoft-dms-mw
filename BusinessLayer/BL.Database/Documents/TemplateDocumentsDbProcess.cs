using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Database.DBModel.Template;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;

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
                templateDocumentId =
                    dbContext.DocumentsSet.Where(x => x.Id == documentId)
                        .Select(x => x.TemplateDocumentId)
                        .FirstOrDefault();
            }

            return GetTemplateDocument(ctx, templateDocumentId);
        }

        public FrontTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var templateDocument =
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
                                    AccessLevelId = (int) y.AccessLevelId
                                }).ToList(),
                            SendLists = x.SendLists.Select(y => new FrontTemplateDocumentSendLists()
                            {
                                SendType = (EnumSendTypes) y.SendTypeId,
                                TargetPositionId = y.TargetPositionId,
                                Description = y.Description,
                                Stage = y.Stage,
                                Task = y.Task.Task,
                                DueDay = y.DueDay,
                                AccessLevelId = (int) y.AccessLevelId
                            }).ToList()
                        }).FirstOrDefault();

                templateDocument.Properties = CommonQueries.GetPropertyValues(dbContext, new FilterPropertyValue { RecordId = new List<int> { templateDocumentId }, Object = new List<EnumObjects> { EnumObjects.TemplateDocuments } });

                return templateDocument;
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
                    newTemplate.Id = (int) template.Id;
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

                dbContext.TemplateDocumentSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentRestrictedSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentTasksSet.RemoveRange(
                    dbContext.TemplateDocumentTasksSet.Where(x => x.DocumentId == id));

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
                    if (filter.Id?.Count > 0)
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
                    SendType = (EnumSendTypes) x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    Stage = x.Stage,
                    Task = x.Task.Task,
                    DueDay = x.DueDay,
                    //AccessLevel = (EnumDocumentAccesses) x.AccessLevelId,
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
                return
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.Id == id)
                        .Select(x => new FrontTemplateDocumentSendLists()
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            SendType = (EnumSendTypes) x.SendTypeId,
                            TargetPositionId = x.TargetPositionId,
                            Description = x.Description,
                            Stage = x.Stage,
                            Task = x.Task.Task,
                            DueDay = x.DueDay,
                            //AccessLevel = (EnumDocumentAccesses) x.AccessLevelId,
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
                var newTemplate = new TemplateDocumentSendLists()
                {
                    DocumentId = template.DocumentId,
                    Description = template.Description,
                    IsAddControl = template.IsAddControl,
                    IsAvailableWithinTask = template.IsAvailableWithinTask,
                    TargetPositionId = template.TargetPositionId,
                    AccessLevelId = (int) template.AccessLevel,
                    DueDay = template.DueDay,
                    SendTypeId = (int) template.SendType,
                    SourcePositionId = template.SourcePositionId,
                    Stage = template.Stage,
                    TargetAgentId = template.TargetAgentId,
                    TaskId = template.TaskId,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };

                if (template.Id.HasValue)
                {
                    newTemplate.Id = (int) template.Id;
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

        #region TemplateDocumentRestrictedSendList

        public IEnumerable<FrontTemplateDocumentRestrictedSendLists> GetTemplateDocumentRestrictedSendLists(
            IContext ctx,
            int templateId, FilterTemplateDocumentRestrictedSendList filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.TemplateDocumentRestrictedSendListsSet.AsQueryable();
                qry = qry.Where(x => x.DocumentId == (int) filter.DocumentId);

                if (filter.Id?.Count > 0)
                {
                    qry = qry.Where(x => filter.Id.Contains(x.Id));
                }

                if (filter.PositionId.HasValue)
                {
                    qry = qry.Where(x => x.PositionId == filter.PositionId);
                }

                if (filter.AccessLevel.HasValue)
                {
                    qry = qry.Where(x => x.AccessLevelId == (int) filter.AccessLevel);
                }

                return qry.Select(x => new FrontTemplateDocumentRestrictedSendLists()
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.Position.Id,
                    //AccessLevel = (EnumDocumentAccesses) x.AccessLevelId,
                    PositionName = x.Position.Name,
                    AccessLevelName = x.AccessLevel.Name,
                }).ToList();
            }
        }

        public FrontTemplateDocumentRestrictedSendLists GetTemplateDocumentRestrictedSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return
                    dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Id == id)
                        .Select(x => new FrontTemplateDocumentRestrictedSendLists()
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            PositionId = x.Position.Id,
                            //AccessLevel = (EnumDocumentAccesses) x.AccessLevelId,
                            PositionName = x.Position.Name,
                            AccessLevelName = x.AccessLevel.Name,
                        }).FirstOrDefault();
            }
        }

        public int AddOrUpdateTemplateRestrictedSendList(IContext ctx,
            ModifyTemplateDocumentRestrictedSendLists template)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var newTemplate = new TemplateDocumentRestrictedSendLists()
                {

                    DocumentId = template.DocumentId,
                    PositionId = template.PositionId,
                    AccessLevelId = (int) template.AccessLevel,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };

                if (template.Id.HasValue)
                {
                    newTemplate.Id = (int) template.Id;
                }

                dbContext.TemplateDocumentRestrictedSendListsSet.Attach(newTemplate);

                var entity = dbContext.Entry(newTemplate);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();

                return newTemplate.Id;
            }
        }

        public void DeleteTemplateRestrictedSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var ddt = dbContext.TemplateDocumentRestrictedSendListsSet.FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentRestrictedSendListsSet.Remove(ddt);
                dbContext.SaveChanges();
            }
        }

        #endregion TemplateDocumentRestrictedSendList

        #region TemplateDocumentTasks

        public IEnumerable<FrontTemplateDocumentTasks> GetTemplateDocumentTasks(IContext ctx, int templateId,
            FilterTemplateDocumentTask filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.TemplateDocumentTasksSet.AsQueryable();
                qry = qry.Where(x => x.DocumentId == templateId);

                if (filter.Id?.Count>0)
                {
                    qry = qry.Where(x => filter.Id.Contains(x.Id));
                }
                if (filter.PositionId.HasValue)
                {
                    qry = qry.Where(x => x.PositionId == filter.PositionId);
                }
                if (!string.IsNullOrEmpty(filter.Task))
                {
                    qry = qry.Where(x => x.Task.Contains(filter.Task));
                }

                return qry.Select(x => new FrontTemplateDocumentTasks
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.PositionId,
                    PositionName = x.Position.Name,
                    Task = x.Task,
                    Description = x.Description
                }).ToList();

            }
        }

        public FrontTemplateDocumentTasks GetTemplateDocumentTask(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return
                    dbContext.TemplateDocumentTasksSet.Where(x => x.Id == id).Select(x => new FrontTemplateDocumentTasks
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        PositionId = x.PositionId,
                        Task = x.Task,
                        Description = x.Description,
                        PositionName = x.Position.Name
                    }).FirstOrDefault();
            }
        }

        public int AddOrUpdateTemplateTask(IContext ctx, InternalTemplateDocumentTask template)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var newTemplate = new TemplateDocumentTasks()
                {

                    DocumentId = template.DocumentId,
                    PositionId = template.PositionId,
                    Task = template.Task,
                    Description = template.Description,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };

                if (template.Id.HasValue)
                {
                    newTemplate.Id = (int) template.Id;
                }

                dbContext.TemplateDocumentTasksSet.Attach(newTemplate);

                var entity = dbContext.Entry(newTemplate);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();

                return newTemplate.Id;
            }
        }

        public void DeleteTemplateTask(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var ddt = dbContext.TemplateDocumentTasksSet.FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentTasksSet.Remove(ddt);
                dbContext.SaveChanges();
            }
        }

        #endregion TemplateDocumentTasks

        #region TemplateDocumentAttachedFiles

        public IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx,
            FilterTemplateAttachedFile filter,
            int templateId)
        {
            using (var dbContext = new DmsContext(ctx))
            {

                var qry = dbContext.TemplateDocumentFilesSet.AsQueryable();
                qry = qry.Where(x => x.DocumentId == templateId);

                if (filter.Id.HasValue)
                {
                    qry = qry.Where(x => x.Id == filter.Id);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
                }

                return
                    qry.Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                        (d, a) => new {fl = d, agName = a.Name})
                        .Select(x => new FrontTemplateAttachedFile
                        {
                            Id = x.fl.Id,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extention,
                            //FileContent = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            FileType = x.fl.FileType,
                            FileSize = x.fl.FileSize,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber

                        }).ToList();
            }
        }


        public FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return
                    dbContext.TemplateDocumentFilesSet.Where(
                        x => x.Id == id)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new {fl = d, agName = a.Name})
                        .Select(x => new FrontTemplateAttachedFile
                        {
                            Id = x.fl.Id,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extention,
                            //FileContent = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            FileType = x.fl.FileType,
                            FileSize = x.fl.FileSize,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber

                        }).FirstOrDefault();
            }
        }


        public int GetNextFileOrderNumber(IContext ctx, int templateId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                if (dbContext.TemplateDocumentFilesSet.Any(x => x.DocumentId == templateId))
                {
                    return
                        dbContext.TemplateDocumentFilesSet.Where(x => x.DocumentId == templateId)
                            .Max(x => x.OrderNumber) + 1;
                }
            }
            return 1;
        }

        public int AddNewFile(IContext ctx, InternalTemplateAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.TemplateDocumentFilesSet.Add(fl);
                dbContext.SaveChanges();
                docFile.Id = fl.Id;
                return fl.Id;
            }
        }

        public void UpdateFile(IContext ctx, InternalTemplateAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.TemplateDocumentFilesSet.Attach(fl);
                var entry = dbContext.Entry(fl);
                entry.Property(x => x.Name).IsModified = true;
                entry.Property(x => x.Extention).IsModified = true;
                entry.Property(x => x.FileType).IsModified = true;
                entry.Property(x => x.FileSize).IsModified = true;
                entry.Property(x => x.IsAdditional).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.Hash).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public void DeleteTemplateAttachedFile(IContext ctx, InternalTemplateAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                dbContext.TemplateDocumentFilesSet.RemoveRange(
                    dbContext.TemplateDocumentFilesSet.Where(
                        x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument));
                dbContext.SaveChanges();

            }
        }

        #endregion TemplateDocumentAttachedFiles
    }

}