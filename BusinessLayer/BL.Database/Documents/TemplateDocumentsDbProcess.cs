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
using BL.Model.SystemCore.InternalModel;
using System.Transactions;
using BL.Model.FullTextSearch;
using LinqKit;
using System.IO;
using BL.Model.SystemCore;
using System;
using System.Data.Entity;
using BL.CrossCutting.Helpers;

namespace BL.Database.Documents
{
    public class TemplateDocumentsDbProcess : CoreDb.CoreDb, ITemplateDocumentsDbProcess
    {
        #region TemplateDocuments

        public IEnumerable<FrontTemplateDocument> GetTemplateDocument(IContext ctx, FilterTemplateDocument filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId);
                if (filter != null)
                {
                    if (filter.IDs?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocuments>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());
                        qry = qry.Where(filterContains);
                    }
                    if (filter.DocumentDirectionId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocuments>();
                        filterContains = filter.DocumentDirectionId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.DocumentDirectionId == value).Expand());
                        qry = qry.Where(filterContains);
                    }
                    if (filter.DocumentTypeId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocuments>();
                        filterContains = filter.DocumentTypeId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.DocumentTypeId == value).Expand());
                        qry = qry.Where(filterContains);
                    }
                    if (filter.DocumentSubjectId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocuments>();
                        filterContains = filter.DocumentSubjectId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.DocumentSubjectId == value).Expand());
                        qry = qry.Where(filterContains);
                    }
                    if (filter.RegistrationJournalId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocuments>();
                        filterContains = filter.RegistrationJournalId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.RegistrationJournalId == value).Expand());
                        qry = qry.Where(filterContains);
                    }
                    if (!String.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocuments>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name)
                                    .Aggregate(filterContains, (current, value) => current.Or(e => e.Name.Contains(value)).Expand());
                        qry = qry.Where(filterContains);
                    }
                    if (!String.IsNullOrEmpty(filter.Description))
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocuments>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Description)
                                    .Aggregate(filterContains, (current, value) => current.Or(e => e.Description.Contains(value)).Expand());
                        qry = qry.Where(filterContains);
                    }
                }
                qry = qry.OrderByDescending(x => x.Name);

                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontTemplateDocument>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }

                var res = qry.Select(x => new FrontTemplateDocument
                {
                    Id = x.Id,
                    DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                    DocumentDirectionName = x.DocumentDirection.Name,
                    IsHard = x.IsHard,
                    IsForProject = x.IsForProject,
                    IsForDocument = x.IsForDocument,
                    IsActive = x.IsActive,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentTypeName = x.DocumentType.Name,
                    Name = x.Name,
                    Description = x.Description,
                    //DocumentSubjectId = x.DocumentSubjectId,
                    //LastChangeDate = x.LastChangeDate,
                    //RegistrationJournalId = x.RegistrationJournalId,
                    //DocumentSubjectName = x.DocumentSubject.Name,
                    //LastChangeUserId = x.LastChangeUserId,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId)
        {
            int templateDocumentId = 0;
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                templateDocumentId =
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Id == documentId)
                        .Select(x => x.TemplateDocumentId)
                        .FirstOrDefault();


                var res = GetTemplateDocument(ctx, templateDocumentId);
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var templateDocument =
                    dbContext.TemplateDocumentsSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(x => x.Id == templateDocumentId)
                        .Select(x => new FrontTemplateDocument
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsHard = x.IsHard,
                            IsForProject = x.IsForProject,
                            IsForDocument = x.IsForDocument,
                            IsActive = x.IsActive,
                            DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                            DocumentDirectionName = x.DocumentDirection.Name,
                            DocumentTypeId = x.DocumentTypeId,
                            DocumentTypeName = x.DocumentType.Name,
                            Description = x.Description,
                            DocumentSubjectId = x.DocumentSubjectId,
                            DocumentSubjectName = x.DocumentSubject.Name,
                            RegistrationJournalId = x.RegistrationJournalId,
                            RegistrationJournalName = x.RegistrationJournal.Name,
                            SenderAgentId = x.SenderAgentId,
                            SenderAgentName = x.SenderAgent.Name,
                            SenderAgentPersonId = x.SenderAgentPersonId,
                            SenderAgentPersonName = x.SenderAgentPerson.Agent.Name,
                            Addressee = x.Addressee,
                        }).FirstOrDefault();

                if (templateDocument != null)
                    templateDocument.Properties = CommonQueries.GetPropertyValues(dbContext, ctx, new FilterPropertyValue { RecordId = new List<int> { templateDocumentId }, Object = new List<EnumObjects> { EnumObjects.TemplateDocument } });
                transaction.Complete();
                return templateDocument;
            }
        }

        public int AddOrUpdateTemplate(IContext ctx, InternalTemplateDocument template, IEnumerable<InternalPropertyValue> properties)
        {
            // we should not implement it now
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocuments()
                {
                    ClientId = ctx.CurrentClientId,
                    Name = template.Name,
                    IsHard = template.IsHard,
                    IsForProject = template.IsForProject,
                    IsForDocument = template.IsForDocument,
                    DocumentDirectionId = (int)template.DocumentDirection,
                    DocumentTypeId = template.DocumentTypeId,
                    DocumentSubjectId = template.DocumentSubjectId,
                    Description = template.Description,
                    RegistrationJournalId = template.RegistrationJournalId,
                    SenderAgentId = template.SenderAgentId,
                    SenderAgentPersonId = template.SenderAgentPersonId,
                    Addressee = template.Addressee,
                    IsActive = template.IsActive,
                    LastChangeUserId = template.LastChangeUserId,
                    LastChangeDate = template.LastChangeDate
                };

                if (template.Id > 0)
                {
                    newTemplate.Id = template.Id;
                }

                if (template.Id > 0)
                {
                    dbContext.TemplateDocumentsSet.Attach(newTemplate);

                    var entity = dbContext.Entry(newTemplate);
                    entity.State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    dbContext.TemplateDocumentsSet.Add(newTemplate);
                }

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, newTemplate.Id, EnumObjects.DictionaryDocumentType,
                    template.Id > 0 ? EnumOperationType.Update : EnumOperationType.AddNew);


                if (properties != null && properties.Any())
                {
                    CommonQueries.ModifyPropertyValues(dbContext, ctx, new InternalPropertyValues { Object = EnumObjects.TemplateDocument, RecordId = newTemplate.Id, PropertyValues = properties });
                }

                transaction.Complete();

                return newTemplate.Id;

            }
        }

        public void DeleteTemplate(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;

                dbContext.TemplateDocumentSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentRestrictedSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentTasksSet.RemoveRange(
                    dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentPapersSet.RemoveRange(
                    dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == id));

                CommonQueries.DeletePropertyValues(dbContext, context, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.TemplateDocument }, RecordId = new List<int> { id } });

                dbContext.TemplateDocumentsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.TemplateDocument, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public bool CanModifyTemplate(IContext ctx, ModifyTemplateDocument template)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                //TODO: Уточнить безнес-логику, в каких случаях можно менять/удалять шаблон документа
                var count = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).Count(x => x.TemplateDocumentId == template.Id);
                transaction.Complete();
                return count == 0;
            }
        }

        public bool CanAddTemplate(IContext ctx, ModifyTemplateDocument template)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {

                var count = dbContext.TemplateDocumentsSet.Count(x => x.ClientId == ctx.CurrentClientId && x.Name == template.Name);
                transaction.Complete();
                return count == 0;
            }
        }
        public bool CanModifyTemplate(IContext ctx, int templateId)
        {
            return CanModifyTemplate(ctx, new ModifyTemplateDocument() { Id = templateId });
        }

        #endregion TemplateDocuments

        #region TemplateDocumentSendLists

        public IEnumerable<FrontTemplateDocumentSendList> GetTemplateDocumentSendLists(IContext ctx, FilterTemplateDocumentSendList filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();
                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocumentSendLists>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                    if (filter.TemplateId.HasValue)
                    {
                        qry = qry.Where(x => x.DocumentId == filter.TemplateId.Value);
                    }
                    if (filter.SendType.HasValue)
                    {
                        qry = qry.Where(x => x.SendTypeId == (int)filter.SendType);
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
                }
                var res = qry.Select(x => new FrontTemplateDocumentSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    SendType = x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    Stage = x.Stage,

                    Task = x.Task.Task,
                    DueDay = x.DueDay,
                    AccessLevelId = x.AccessLevelId,
                    TargetPositionName = x.TargetPosition.Name,
                    SendTypeName = x.SendType.Name,
                    AccessLevelName = x.AccessLevel.Name,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocumentSendList GetTemplateDocumentSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateDocumentSendList
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            SendType = x.SendTypeId,
                            TargetPositionId = x.TargetPositionId,
                            Description = x.Description,
                            Stage = x.Stage,
                            Task = x.Task.Task,
                            DueDay = x.DueDay,
                            AccessLevelId = x.AccessLevelId,
                            TargetPositionName = x.TargetPosition.Name,
                            SendTypeName = x.SendType.Name,
                            AccessLevelName = x.AccessLevel.Name,
                            IsWorkGroup = x.IsWorkGroup,
                            IsAddControl = x.IsAddControl,
                            SelfDueDate = x.SelfDueDate,
                            SelfDueDay = x.SelfDueDay,
                            SelfAttentionDate = x.SelfAttentionDate,
                            IsAvailableWithinTask = x.IsAvailableWithinTask
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int AddOrUpdateTemplateSendList(IContext ctx, InternalTemplateDocumentSendList template)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocumentSendLists()
                {
                    DocumentId = template.DocumentId,
                    Description = template.Description,
                    IsWorkGroup = template.IsWorkGroup,
                    IsAddControl = template.IsAddControl,
                    SelfDueDate = template.SelfDueDate,
                    SelfDueDay = template.SelfDueDay,
                    SelfAttentionDate = template.SelfAttentionDate,
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
                var entityState = System.Data.Entity.EntityState.Modified;
                if (template.Id > 0)
                {
                    newTemplate.Id = (int)template.Id;
                    entityState = System.Data.Entity.EntityState.Added;
                }

                dbContext.TemplateDocumentSendListsSet.Attach(newTemplate);
                var entity = dbContext.Entry(newTemplate);
                entity.State = entityState;

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, newTemplate.Id, EnumObjects.TemplateDocumentSendList, template.Id > 0 ? EnumOperationType.Update : EnumOperationType.AddNew);
                transaction.Complete();
                return newTemplate.Id;
            }
        }

        public void DeleteTemplateSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentSendListsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.TemplateDocumentSendList, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion TemplateDocumentSendLists

        #region TemplateDocumentRestrictedSendList

        public IEnumerable<FrontTemplateDocumentRestrictedSendList> GetTemplateDocumentRestrictedSendLists(IContext ctx, FilterTemplateDocumentRestrictedSendList filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();

                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocumentRestrictedSendLists>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                    if (filter.TemplateId.HasValue)
                    {
                        qry = qry.Where(x => x.DocumentId == filter.TemplateId.Value);
                    }

                    if (filter.PositionId.HasValue)
                    {
                        qry = qry.Where(x => x.PositionId == filter.PositionId);
                    }

                    if (filter.AccessLevel.HasValue)
                    {
                        qry = qry.Where(x => x.AccessLevelId == (int)filter.AccessLevel);
                    }
                }
                var res = qry.Select(x => new FrontTemplateDocumentRestrictedSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.Position.Id,
                    AccessLevelId = x.AccessLevelId,
                    PositionName = x.Position.Name,
                    AccessLevelName = x.AccessLevel.Name,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocumentRestrictedSendList GetTemplateDocumentRestrictedSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateDocumentRestrictedSendList
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            PositionId = x.Position.Id,
                            //AccessLevel = (EnumDocumentAccesses) x.AccessLevelId,
                            PositionName = x.Position.Name,
                            AccessLevelName = x.AccessLevel.Name,
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int AddOrUpdateTemplateRestrictedSendList(IContext ctx, InternalTemplateDocumentRestrictedSendList template)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocumentRestrictedSendLists()
                {

                    DocumentId = template.DocumentId,
                    PositionId = template.PositionId,
                    AccessLevelId = (int)template.AccessLevel,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };
                var entityState = System.Data.Entity.EntityState.Modified;
                if (template.Id > 0)
                {
                    newTemplate.Id = template.Id;
                    entityState = System.Data.Entity.EntityState.Added;
                }

                dbContext.TemplateDocumentRestrictedSendListsSet.Attach(newTemplate);
                var entity = dbContext.Entry(newTemplate);
                entity.State = entityState;

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, newTemplate.Id, EnumObjects.TemplateDocumentRestrictedSendList,
                    template.Id > 0 ? EnumOperationType.Update : EnumOperationType.AddNew);
                transaction.Complete();
                return newTemplate.Id;
            }
        }

        public void DeleteTemplateRestrictedSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentRestrictedSendListsSet.Remove(ddt);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.TemplateDocumentRestrictedSendList, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public bool CanAddTemplateRestrictedSendList(IContext ctx, ModifyTemplateDocumentRestrictedSendLists list)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var count =
                    dbContext.TemplateDocumentRestrictedSendListsSet.Count(
                        x => x.Document.ClientId == ctx.CurrentClientId && x.DocumentId == list.DocumentId && x.PositionId == list.PositionId);
                transaction.Complete();
                return count == 0;
            }
        }



        #endregion TemplateDocumentRestrictedSendList

        #region TemplateDocumentTasks

        public IEnumerable<FrontTemplateDocumentTask> GetTemplateDocumentTasks(IContext ctx, FilterTemplateDocumentTask filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();

                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocumentTasks>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                    if (filter.TemplateId.HasValue)
                    {
                        qry = qry.Where(x => x.DocumentId == filter.TemplateId.Value);
                    }
                    if (!string.IsNullOrEmpty(filter.Task))
                    {
                        qry = qry.Where(x => x.Task.Contains(filter.Task));
                    }
                }
                var res = qry.Select(x => new FrontTemplateDocumentTask
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Task = x.Task,
                    Description = x.Description
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocumentTask GetTemplateDocumentTask(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(x => x.Id == id)
                    .Select(x => new FrontTemplateDocumentTask
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Task = x.Task,
                        Description = x.Description,
                    }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int AddOrUpdateTemplateTask(IContext ctx, InternalTemplateDocumentTask template)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocumentTasks()
                {

                    DocumentId = template.DocumentId,
                    Task = template.Task,
                    Description = template.Description,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };
                var entityState = System.Data.Entity.EntityState.Modified;
                if (template.Id.HasValue)
                {
                    newTemplate.Id = (int)template.Id;
                    entityState = System.Data.Entity.EntityState.Added;
                }

                dbContext.TemplateDocumentTasksSet.Attach(newTemplate);
                var entity = dbContext.Entry(newTemplate);
                entity.State = entityState;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, newTemplate.Id, EnumObjects.TemplateDocumentTask,
                    template.Id > 0 ? EnumOperationType.Update : EnumOperationType.AddNew);
                transaction.Complete();
                return newTemplate.Id;
            }
        }

        public bool CanAddTemplateTask(IContext ctx, ModifyTemplateDocumentTask task)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var count = dbContext.TemplateDocumentTasksSet.Count(x =>
                    (x.Document.ClientId == ctx.CurrentClientId && x.DocumentId == task.DocumentId && x.Task == task.Task)
                    );
                transaction.Complete();
                return count == 0;
            }
        }

        public void DeleteTemplateTask(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentTasksSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.TemplateDocumentTask, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion TemplateDocumentTasks

        #region TemplateDocumentPapers

        public IEnumerable<FrontTemplateDocumentPaper> GetTemplateDocumentPapers(IContext ctx, FilterTemplateDocumentPaper filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();

                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocumentPapers>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                    if (filter.TemplateId.HasValue)
                    {
                        qry = qry.Where(x => x.DocumentId == filter.TemplateId.Value);
                    }
                }
                var res = qry.Select(x => new FrontTemplateDocumentPaper
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Name = x.Name,
                    Description = x.Description,
                    IsMain = x.IsMain,
                    IsOriginal = x.IsOriginal,
                    IsCopy = x.IsCopy,
                    PageQuantity = x.PageQuantity,
                    OrderNumber = x.OrderNumber,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocumentPaper GetTemplateDocumentPaper(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(x => x.Id == id)
                    .Select(x => new FrontTemplateDocumentPaper
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Name = x.Name,
                        Description = x.Description,
                        IsMain = x.IsMain,
                        IsOriginal = x.IsOriginal,
                        IsCopy = x.IsCopy,
                        PageQuantity = x.PageQuantity,
                        OrderNumber = x.OrderNumber,
                    }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<int> AddTemplateDocumentPapers(IContext context, IEnumerable<InternalTemplateDocumentPaper> papers)
        {
            List<int> res = new List<int>();
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                if (papers != null && papers.Any())
                {
                    foreach (var paper in papers)
                    {
                        var paperDb = ModelConverter.GetDbTemplateDocumentPaper(paper);
                        dbContext.TemplateDocumentPapersSet.Add(paperDb);
                        dbContext.SaveChanges();
                        res.Add(paperDb.Id);
                    }
                }
                transaction.Complete();
            }
            return res;
        }

        public void ModifyTemplatePaper(IContext context, InternalTemplateDocumentPaper item)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var itemDb = ModelConverter.GetDbTemplateDocumentPaper(item);
                dbContext.TemplateDocumentPapersSet.Attach(itemDb);
                var entry = dbContext.Entry(itemDb);
                entry.Property(e => e.Name).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.IsMain).IsModified = true;
                entry.Property(e => e.IsOriginal).IsModified = true;
                entry.Property(e => e.IsCopy).IsModified = true;
                entry.Property(e => e.PageQuantity).IsModified = true;
                //entry.Property(e => e.OrderNumber).IsModified = true;
                entry.Property(e => e.LastChangeUserId).IsModified = true;
                entry.Property(e => e.LastChangeDate).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalTemplateDocument ModifyTemplatePaperPrepare(IContext context, ModifyTemplateDocumentPaper model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalTemplateDocument
                    {
                        Id = x.Id,
                    }).FirstOrDefault();
                if (doc == null) return null;
                if (model.Id == 0)
                {
                    doc.MaxPaperOrderNumber = dbContext.DocumentPapersSet
                        .Where(
                            x =>
                                x.DocumentId == model.DocumentId && x.Name == model.Name && x.IsMain == model.IsMain &&
                                x.IsCopy == model.IsCopy && x.IsOriginal == model.IsOriginal)
                        .OrderByDescending(x => x.OrderNumber).Select(x => x.OrderNumber).FirstOrDefault();
                }
                else
                {
                    doc.Papers = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => (x.Id == model.Id))//|| x.Name == model.Name) && x.DocumentId == model.DocumentId)
                        .Select(x => new InternalTemplateDocumentPaper
                        {
                            Id = x.Id,
                        }).ToList();
                }
                transaction.Complete();
                return doc;
            }
        }

        public void DeleteTemplatePaper(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.TemplateDocumentPapersSet.RemoveRange(dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(x => x.Id == id));
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        #endregion TemplateDocumentPapers

        #region TemplateDocumentAttachedFiles

        public IEnumerable<FrontTemplateAttachedFile> GetTemplateAttachedFiles(IContext ctx, FilterTemplateAttachedFile filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();
                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<TemplateDocumentFiles>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                    if (filter.TemplateId.HasValue)
                    {
                        qry = qry.Where(x => x.DocumentId == filter.TemplateId.Value);
                    }
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(filter.Name));
                    }
                }
                var res = qry.Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                        (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new FrontTemplateAttachedFile
                        {
                            Id = x.fl.Id,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extention,
                            //FileContent = x.fl.Content,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            TypeName = x.fl.Type.Name,
                            Hash = x.fl.Hash,
                            FileType = x.fl.FileType,
                            FileSize = x.fl.FileSize,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber,
                            Description = x.fl.Description,

                        }).ToList();
                transaction.Complete();
                return res;
            }
        }


        public FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentFilesSet
                        .Where(x => x.Document.ClientId == ctx.CurrentClientId)
                        .Where(x => x.Id == id)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id, (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new FrontTemplateAttachedFile
                        {
                            Id = x.fl.Id,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extention,
                            //FileContent = x.fl.Content,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            TypeName = x.fl.Type.Name,
                            Hash = x.fl.Hash,
                            FileType = x.fl.FileType,
                            FileSize = x.fl.FileSize,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber,
                            Description = x.fl.Description

                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }


        public int GetNextFileOrderNumber(IContext ctx, int templateId)
        {
            var res = 1;
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                if (dbContext.TemplateDocumentFilesSet.Any(x => x.DocumentId == templateId))
                {
                    res =
                        dbContext.TemplateDocumentFilesSet.Where(x => x.DocumentId == templateId)
                            .Max(x => x.OrderNumber) + 1;
                }
                transaction.Complete();
            }
            return res;
        }

        public int AddNewFile(IContext ctx, InternalTemplateAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.TemplateDocumentFilesSet.Add(fl);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, fl.Id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.AddNew);
                docFile.Id = fl.Id;
                transaction.Complete();
                return fl.Id;
            }
        }

        public void UpdateFile(IContext ctx, InternalTemplateAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.TemplateDocumentFilesSet.Attach(fl);

                var entry = dbContext.Entry(fl);
                entry.Property(x => x.Name).IsModified = true;
                entry.Property(x => x.Extention).IsModified = true;
                entry.Property(x => x.FileType).IsModified = true;
                entry.Property(x => x.FileSize).IsModified = true;
                entry.Property(x => x.TypeId).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.Hash).IsModified = true;
                entry.Property(x => x.Description).IsModified = true;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, docFile.Id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.Update);
                transaction.Complete();
            }
        }
        public InternalTemplateAttachedFile DeleteTemplateAttachedFilePrepare(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                var file = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.CurrentClientId)
                        .Where(x => x.Id == id)
                        .Select(x => new InternalTemplateAttachedFile
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            OrderInDocument = x.OrderNumber
                        }).FirstOrDefault();
                transaction.Complete();
                return file;
            }
        }
        public void DeleteTemplateAttachedFile(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.TemplateDocumentFilesSet.RemoveRange(
                    dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(
                        x => x.Id == id));
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCashInfo(dbContext, id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public bool CanAddTemplateAttachedFile(IContext ctx, ModifyTemplateAttachedFile file)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtention = Path.GetExtension(file.FileName).Replace(".", "");

                var res = dbContext.TemplateDocumentFilesSet.Any(x =>
                    x.Document.ClientId == ctx.CurrentClientId &&
                    ((x.DocumentId == file.DocumentId && x.OrderNumber == file.OrderInDocument) ||
                    (x.DocumentId == file.DocumentId && x.Extention == fileExtention && x.Name == fileName))
                    );
                transaction.Complete();
                return !res;
            }
        }




        #endregion TemplateDocumentAttachedFiles
    }

}