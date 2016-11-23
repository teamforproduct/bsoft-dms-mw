﻿using System.Collections.Generic;
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

namespace BL.Database.Documents
{
    public class TemplateDocumentsDbProcess : CoreDb.CoreDb, ITemplateDocumentsDbProcess
    {
        #region TemplateDocuments

        public IEnumerable<FrontTemplateDocument> GetTemplateDocument(IContext ctx, FilterTemplateDocument filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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
                return res;
            }
        }

        public FrontTemplateDocument GetTemplateDocumentByDocumentId(IContext ctx, int documentId)
        {
            int templateDocumentId = 0;
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                templateDocumentId =
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Id == documentId)
                        .Select(x => x.TemplateDocumentId)
                        .FirstOrDefault();
            }

            return GetTemplateDocument(ctx, templateDocumentId);
        }

        public FrontTemplateDocument GetTemplateDocument(IContext ctx, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate,
                            //RestrictedSendLists =
                            //    x.RestrictedSendLists.Select(y => new FrontTemplateDocumentRestrictedSendLists
                            //    {
                            //        PositionId = y.PositionId,
                            //        PositionName = y.Position.Name,
                            //        AccessLevelId = (int)y.AccessLevelId
                            //    }).ToList(),
                            //SendLists = x.SendLists.Select(y => new FrontTemplateDocumentSendLists
                            //{
                            //    SendType = y.SendTypeId,
                            //    SendTypeName = y.SendType.Name,
                            //    TargetPositionId = y.TargetPositionId,
                            //    TargetPositionName = y.TargetPosition.Name,
                            //    Description = y.Description,
                            //    Stage = y.Stage,
                            //    Task = y.Task.Task,
                            //    DueDay = y.DueDay,
                            //    AccessLevelId = (int)y.AccessLevelId,
                            //    AccessLevelName = y.AccessLevel.Name,
                            //}).ToList(),                         
                        }).FirstOrDefault();

                if (templateDocument != null)
                    templateDocument.Properties = CommonQueries.GetPropertyValues(dbContext, ctx, new FilterPropertyValue { RecordId = new List<int> { templateDocumentId }, Object = new List<EnumObjects> { EnumObjects.TemplateDocument } });

                return templateDocument;
            }
        }

        public int AddOrUpdateTemplate(IContext ctx, InternalTemplateDocument template, IEnumerable<InternalPropertyValue> properties)
        {
            // we should not implement it now
            //var dbContext = GetUserDmsContext(context);
            using (var dbContext = new DmsContext(ctx))
            {
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
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

                    CommonQueries.AddFullTextCashInfo(dbContext, newTemplate.Id, EnumObjects.DictionaryDocumentType, 
                        template.Id > 0 ? EnumOperationType.Update : EnumOperationType.AddNew);

                    dbContext.SaveChanges();

                    if (properties != null && properties.Any())
                    {
                        CommonQueries.ModifyPropertyValues(dbContext, ctx, new InternalPropertyValues { Object = EnumObjects.TemplateDocument, RecordId = newTemplate.Id, PropertyValues = properties });
                    }

                    transaction.Complete();

                    return newTemplate.Id;
                }
            }
        }

        public void DeleteTemplate(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;

                dbContext.TemplateDocumentSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentRestrictedSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentTasksSet.RemoveRange(
                    dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == id));

                CommonQueries.DeletePropertyValues(dbContext, context, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.TemplateDocument }, RecordId = new List<int> { id } });

                dbContext.TemplateDocumentsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.TemplateDocument, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public bool CanModifyTemplate(IContext ctx, ModifyTemplateDocument template)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //TODO: Уточнить безнес-логику, в каких случаях можно менять/удалять шаблон документа
                var count = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).Count(x => x.TemplateDocumentId == template.Id);

                return count == 0;
            }
        }

        public bool CanAddTemplate(IContext ctx, ModifyTemplateDocument template)
        {
            using (var dbContext = new DmsContext(ctx))
            {

                var count = dbContext.TemplateDocumentsSet.Count(x => x.ClientId == ctx.CurrentClientId && x.Name == template.Name);

                return count == 0;
            }
        }
        public bool CanModifyTemplate(IContext ctx, int templateId)
        {
            return CanModifyTemplate(ctx, new ModifyTemplateDocument() { Id = templateId });
        }

        #endregion TemplateDocuments

        #region TemplateDocumentSendLists

        public IEnumerable<FrontTemplateDocumentSendLists> GetTemplateDocumentSendLists(IContext ctx, int templateId,
            FilterTemplateDocumentSendList filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();
                if (filter.Id?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<TemplateDocumentSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
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

                return qry.Select(x => new FrontTemplateDocumentSendLists()
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

            }
        }

        public FrontTemplateDocumentSendLists GetTemplateDocumentSendList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateDocumentSendLists()
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            SendType = x.SendTypeId,
                            TargetPositionId = x.TargetPositionId,
                            Description = x.Description,
                            Stage = x.Stage,
                            Task = x.Task.Task,
                            DueDay = x.DueDay,
                            AccessLevelId =  x.AccessLevelId,
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
            }
        }

        public int AddOrUpdateTemplateSendList(IContext ctx, InternalTemplateDocumentSendList template)
        {
            using (var dbContext = new DmsContext(ctx))
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

                if (template.Id > 0)
                {
                    newTemplate.Id = (int)template.Id;
                }

                dbContext.TemplateDocumentSendListsSet.Attach(newTemplate);

                CommonQueries.AddFullTextCashInfo(dbContext, newTemplate.Id, EnumObjects.TemplateDocumentSendList, 
                    template.Id>0 ? EnumOperationType.Update : EnumOperationType.AddNew);

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
                var ddt = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentSendListsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.TemplateDocumentSendList, EnumOperationType.Delete);
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
                var qry = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();
                qry = qry.Where(x => x.DocumentId == (int)filter.DocumentId);

                if (filter.Id?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<TemplateDocumentRestrictedSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionId.HasValue)
                {
                    qry = qry.Where(x => x.PositionId == filter.PositionId);
                }

                if (filter.AccessLevel.HasValue)
                {
                    qry = qry.Where(x => x.AccessLevelId == (int)filter.AccessLevel);
                }

                return qry.Select(x => new FrontTemplateDocumentRestrictedSendLists()
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.Position.Id,
                    AccessLevelId = x.AccessLevelId,
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
                    dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(x => x.Id == id)
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
            InternalTemplateDocumentRestrictedSendList template)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var newTemplate = new TemplateDocumentRestrictedSendLists()
                {

                    DocumentId = template.DocumentId,
                    PositionId = template.PositionId,
                    AccessLevelId = (int)template.AccessLevel,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };

                if (template.Id > 0)
                {
                    newTemplate.Id = template.Id;
                }

                dbContext.TemplateDocumentRestrictedSendListsSet.Attach(newTemplate);
                CommonQueries.AddFullTextCashInfo(dbContext, newTemplate.Id, EnumObjects.TemplateDocumentRestrictedSendList,
                    template.Id>0 ? EnumOperationType.Update : EnumOperationType.AddNew);
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
                var ddt = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentRestrictedSendListsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.TemplateDocumentRestrictedSendList, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public bool CanAddTemplateRestrictedSendList(IContext ctx, ModifyTemplateDocumentRestrictedSendLists list)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var count =
                    dbContext.TemplateDocumentRestrictedSendListsSet.Count(
                        x => x.Document.ClientId == ctx.CurrentClientId && x.DocumentId == list.DocumentId && x.PositionId == list.PositionId);
                return count == 0;
            }
        }



        #endregion TemplateDocumentRestrictedSendList

        #region TemplateDocumentTasks

        public IEnumerable<FrontTemplateDocumentTasks> GetTemplateDocumentTasks(IContext ctx, int templateId,
            FilterTemplateDocumentTask filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();
                qry = qry.Where(x => x.DocumentId == templateId);

                if (filter.Id?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<TemplateDocumentTasks>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
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
                    dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontTemplateDocumentTasks
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
                    newTemplate.Id = (int)template.Id;
                }

                dbContext.TemplateDocumentTasksSet.Attach(newTemplate);
                CommonQueries.AddFullTextCashInfo(dbContext, newTemplate.Id, EnumObjects.TemplateDocumentTask,
                    template.Id>0 ? EnumOperationType.Update : EnumOperationType.AddNew);
                var entity = dbContext.Entry(newTemplate);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();

                return newTemplate.Id;
            }
        }

        public bool CanAddTemplateTask(IContext ctx, ModifyTemplateDocumentTasks task)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var count = dbContext.TemplateDocumentTasksSet.Count(x =>
                    (x.Document.ClientId == ctx.CurrentClientId && x.DocumentId == task.DocumentId && x.Task == task.Task)
                    );

                return count == 0;
            }
        }

        public void DeleteTemplateTask(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var ddt = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentTasksSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.TemplateDocumentTask, EnumOperationType.Delete);
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

                var qry = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == ctx.CurrentClientId).AsQueryable();
                qry = qry.Where(x => x.DocumentId == templateId);

                if (filter.FileId.HasValue)
                {
                    qry = qry.Where(x => x.Id == filter.FileId);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
                }

                return
                    qry.Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
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
            }
        }


        public FrontTemplateAttachedFile GetTemplateAttachedFile(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return
                    dbContext.TemplateDocumentFilesSet
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
                CommonQueries.AddFullTextCashInfo(dbContext, fl.Id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.AddNew);
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

                CommonQueries.AddFullTextCashInfo(dbContext, docFile.Id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.Update);
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
            }
        }

        public void DeleteTemplateAttachedFile(IContext ctx, InternalTemplateAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                dbContext.TemplateDocumentFilesSet.RemoveRange(
                    dbContext.TemplateDocumentFilesSet.Where(x=>x.Document.ClientId == ctx.CurrentClientId).Where(
                        x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument));
                CommonQueries.AddFullTextCashInfo(dbContext, docFile.Id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.Delete);
                dbContext.SaveChanges();

            }
        }

        public bool CanAddTemplateAttachedFile(IContext ctx, ModifyTemplateAttachedFile file)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtention = Path.GetExtension(file.FileName).Replace(".", "");

                var res = dbContext.TemplateDocumentFilesSet.Any(x =>
                    x.Document.ClientId == ctx.CurrentClientId &&
                    ((x.DocumentId == file.DocumentId && x.OrderNumber == file.OrderInDocument) ||
                    (x.DocumentId == file.DocumentId && x.Extention == fileExtention && x.Name == fileName))
                    );

                return !res;
            }
        }




        #endregion TemplateDocumentAttachedFiles
    }

}