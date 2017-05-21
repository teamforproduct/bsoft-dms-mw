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
using BL.Model.FullTextSearch;
using LinqKit;
using System.IO;
using BL.Model.SystemCore;
using System;
using BL.CrossCutting.Helpers;
using BL.Database.Helper;
using BL.Model.Common;

namespace BL.Database.Documents
{
    public class TemplateDocumentsDbProcess : CoreDb.CoreDb, ITemplateDocumentsDbProcess
    {
        #region TemplateDocuments

        private IQueryable<TemplateDocuments> GetTemplateDocumentQuery(IContext context, FilterTemplateDocument filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id);
            if (filter != null)
            {
                if (filter.IDs?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocuments>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.DocumentDirectionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocuments>(false);
                    filterContains = filter.DocumentDirectionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentDirectionId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.DocumentTypeId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocuments>(false);
                    filterContains = filter.DocumentTypeId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentTypeId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!String.IsNullOrEmpty(filter.DocumentSubject))
                {
                    qry = qry.Where(x => x.Description.Contains(filter.DocumentSubject));
                }
                if (filter.RegistrationJournalId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocuments>(false);
                    filterContains = filter.RegistrationJournalId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RegistrationJournalId == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!String.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<TemplateDocuments>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name)
                                .Aggregate(filterContains, (current, value) => current.Or(e => e.Name.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }
                if (!String.IsNullOrEmpty(filter.NameExectly))
                {
                    qry = qry.Where(x => string.Equals(x.Name, filter.NameExectly));
                }
                if (!String.IsNullOrEmpty(filter.Description))
                {
                    var filterContains = PredicateBuilder.New<TemplateDocuments>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Description)
                                .Aggregate(filterContains, (current, value) => current.Or(e => e.Description.Contains(value)).Expand());
                    qry = qry.Where(filterContains);
                }
            }
            return qry;
        }

        public bool ExistsTemplateDocuments(IContext context, FilterTemplateDocument filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = GetTemplateDocumentQuery(context, filter).Any();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontTemplateDocument> GetTemplateDocument(IContext context, FilterTemplateDocument filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentQuery(context, filter);

                qry = qry.OrderByDescending(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontTemplateDocument>();

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

        public IEnumerable<FrontMainTemplateDocument> GetMainTemplateDocument(IContext context, IBaseFilter filter, UIPaging paging, UISorting sotring)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentQuery(context, filter as FilterTemplateDocument);

                qry = qry.OrderByDescending(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainTemplateDocument>();

                var res = qry.Select(x => new FrontMainTemplateDocument
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

        public List<int> GetTemplateDocumentIDs(IContext context, IBaseFilter filter, UISorting sotring)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentQuery(context, filter as FilterTemplateDocument);

                qry = qry.OrderByDescending(x => x.Name);

                //if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainTemplateDocument>();

                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocument GetTemplateDocumentByDocumentId(IContext context, int documentId)
        {
            int templateDocumentId = 0;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                templateDocumentId = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsInWork = true })
                    .Where(x => x.Id == documentId).Select(x => x.TemplateDocumentId) .FirstOrDefault();
                var res = GetTemplateDocument(context, templateDocumentId);
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocument GetTemplateDocument(IContext context, int templateDocumentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var templateDocument =
                    dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == templateDocumentId)
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
                            DocumentSubject = x.DocumentSubject,
                            RegistrationJournalId = x.RegistrationJournalId,
                            RegistrationJournalName = x.RegistrationJournal.Name,
                            SenderAgentId = x.SenderAgentId,
                            SenderAgentName = x.SenderAgent.Name,
                            SenderAgentPersonId = x.SenderAgentPersonId,
                            SenderAgentPersonName = x.SenderAgentPerson.Agent.Name,
                            Addressee = x.Addressee,
                            IsUsedInDocument = x.Documents.Any(),
                        }).FirstOrDefault();

                if (templateDocument != null)
                    templateDocument.Properties = CommonQueries.GetPropertyValues(context, new FilterPropertyValue { RecordId = new List<int> { templateDocumentId }, Object = new List<EnumObjects> { EnumObjects.TemplateDocument } });
                transaction.Complete();
                return templateDocument;
            }
        }

        public int AddOrUpdateTemplate(IContext context, InternalTemplateDocument template)
        {
            // we should not implement it now
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocuments()
                {
                    ClientId = context.Client.Id,
                    Name = template.Name,
                    IsHard = template.IsHard,
                    IsForProject = template.IsForProject,
                    IsForDocument = template.IsForDocument,
                    DocumentDirectionId = (int)template.DocumentDirection,
                    DocumentTypeId = template.DocumentTypeId,
                    DocumentSubject = template.DocumentSubject,
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
                    dbContext.SafeAttach(newTemplate);
                    var entity = dbContext.Entry(newTemplate);
                    entity.State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    dbContext.TemplateDocumentsSet.Add(newTemplate);
                }

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, newTemplate.Id, EnumObjects.TemplateDocument,
                    template.Id > 0 ? EnumOperationType.UpdateFull : EnumOperationType.AddNew);


                if (template.Properties != null && template.Properties.Any())
                {
                    CommonQueries.ModifyPropertyValues(context, new InternalPropertyValues { Object = EnumObjects.TemplateDocument, RecordId = newTemplate.Id, PropertyValues = template.Properties });
                }
                dbContext.SaveChanges();
                transaction.Complete();

                return newTemplate.Id;

            }
        }
        public InternalTemplateDocument CopyTemplatePrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.Id == id)
                    .Select(x => new InternalTemplateDocument
                    {
                        Name = x.Name,
                        IsHard = x.IsHard,
                        IsForProject = x.IsForProject,
                        IsForDocument = x.IsForDocument,
                        DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                        DocumentTypeId = x.DocumentTypeId,
                        DocumentSubject = x.DocumentSubject,
                        Description = x.Description,
                        RegistrationJournalId = x.RegistrationJournalId,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        Addressee = x.Addressee,
                        IsActive = x.IsActive,
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.Tasks = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.Client.Id).Where(y => y.DocumentId == id)
                        .Select(y => new InternalTemplateDocumentTask()
                        {
                            Task = y.Task,
                            Description = y.Description,
                        }).ToList();
                doc.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(y => y.DocumentId == id)
                    .Select(y => new InternalTemplateDocumentRestrictedSendList()
                    {
                        PositionId = y.PositionId,
                        AccessLevel = (EnumAccessLevels)y.AccessLevelId
                    }).ToList();
                doc.SendLists = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(y => y.DocumentId == id)
                    .Select(y => new InternalTemplateDocumentSendList()
                    {
                        SendType = (EnumSendTypes)y.SendTypeId,
                        StageType = (EnumStageTypes?)y.StageTypeId,
                        TargetPositionId = y.TargetPositionId,
                        TargetAgentId = y.TargetAgentId,
                        TaskName = y.Task.Task,
                        IsWorkGroup = y.IsWorkGroup,
                        IsAddControl = y.IsAddControl,
                        SelfDescription = y.SelfDescription,
                        SelfDueDay = y.SelfDueDay,
                        SelfAttentionDay = y.SelfAttentionDay,
                        Description = y.Description,
                        Stage = y.Stage,
                        DueDay = y.DueDay,
                        AccessLevel = (EnumAccessLevels)y.AccessLevelId,
                    }).ToList();
                doc.Files = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.DocumentId == id)
                    .Select(x => new InternalTemplateDocumentFile
                    {
                        ClientId = x.Document.ClientId,
                        EntityTypeId = x.Document.EntityTypeId,
                        DocumentId = x.DocumentId,
                        OrderInDocument = x.OrderNumber,
                        Type = (EnumFileTypes)x.TypeId,
                        Hash = x.Hash,
                        Description = x.Description,
                        PdfCreated = x.IsPdfCreated ?? false,
                        LastPdfAccess = x.LastPdfAccessDate,
                        File = new BaseFile
                        {
                            Extension = x.Extention,
                            Name = x.Name,
                            FileType = x.FileType,
                            FileSize = x.FileSize,
                        }
                    }).ToList();
                doc.Papers = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.DocumentId == id)
                    .Select(x => new InternalTemplateDocumentPaper
                    {
                        IsCopy = x.IsCopy,
                        IsMain = x.IsMain,
                        IsOriginal = x.IsOriginal,
                        OrderNumber = x.OrderNumber,
                        PageQuantity = x.PageQuantity,
                        Name = x.Name,
                        Description = x.Description,
                    }).ToList();
                doc.Properties = CommonQueries.GetInternalPropertyValues(context, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.TemplateDocument }, RecordId = new List<int> { id } }).ToList();

                transaction.Complete();
                return doc;
            }
        }
        public InternalTemplateDocument DeleteTemplatePrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.Id == id)
                    .Select(x => new InternalTemplateDocument
                    {
                        Id = x.Id,
                        FileCount = x.DocumentFiles.Count,
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }
        public void DeleteTemplate(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;

                dbContext.TemplateDocumentSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentRestrictedSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentTasksSet.RemoveRange(
                    dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentPapersSet.RemoveRange(
                    dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.DocumentId == id));
                dbContext.TemplateDocumentFilesSet.RemoveRange(
                    dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.DocumentId == id));

                CommonQueries.DeletePropertyValues(context, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.TemplateDocument }, RecordId = new List<int> { id } });

                dbContext.TemplateDocumentsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, ddt.Id, EnumObjects.TemplateDocument, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public bool CanModifyTemplate(IContext context, ModifyTemplateDocument template)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                //TODO: Уточнить безнес-логику, в каких случаях можно менять/удалять шаблон документа
                var count = dbContext.DocumentsSet //Without security restrictions
                    .Where(x => x.ClientId == context.Client.Id).Count(x => x.TemplateDocumentId == template.Id);
                transaction.Complete();
                return count == 0;
            }
        }

        public bool CanAddTemplate(IContext context, AddTemplateDocument template)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var count = dbContext.TemplateDocumentsSet.Count(x => x.ClientId == context.Client.Id && x.Name == template.Name);
                transaction.Complete();
                return count == 0;
            }
        }
        public bool CanModifyTemplate(IContext context, int templateId)
        {
            return CanModifyTemplate(context, new ModifyTemplateDocument() { Id = templateId });
        }

        #endregion TemplateDocuments

        #region TemplateDocumentSendLists

        public IEnumerable<FrontTemplateDocumentSendList> GetTemplateDocumentSendLists(IContext context, FilterTemplateDocumentSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).AsQueryable();
                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<TemplateDocumentSendLists>(false);
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
                    if (filter.StageType.HasValue)
                    {
                        qry = qry.Where(x => x.StageTypeId == (int)filter.StageType);
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
                qry = qry.OrderBy(x => x.StageTypeId).ThenBy(x => x.Stage).ThenBy(x => x.SendTypeId).ThenBy(x => x.Id);
                var res = qry.Select(x => new FrontTemplateDocumentSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    SendType = x.SendTypeId,
                    StageType = x.StageTypeId,

                    Description = x.Description,
                    Stage = x.Stage,
                    IsWorkGroup = x.IsWorkGroup,
                    IsAddControl = x.IsAddControl,
                    TaskId = x.TaskId,
                    TaskName = x.Task.Task,
                    DueDay = x.DueDay,
                    SelfDueDay = x.SelfDueDay,
                    SelfAttentionDay = x.SelfAttentionDay,
                    SelfDescription = x.SelfDescription,
                    AccessLevel = x.AccessLevelId,
                    TargetPositionId = x.TargetPositionId,
                    TargetPositionName = x.TargetPosition.Name,
                    TargetPositionDepartmentId = x.TargetPosition.Department.Id,
                    TargetPositionDepartmentName = x.TargetPosition.Department.Name,
                    TargetPositionExecutorAgentName = (x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : (string)null)),
                    TargetAgentId = x.TargetAgentId,
                    TargetAgentName = x.TargetAgent.Name,

                    SendTypeName = x.SendType.Name,
                    StageTypeName = x.StageType.Name,
                    AccessLevelName = x.AccessLevel.Name,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocumentSendList GetTemplateDocumentSendList(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateDocumentSendList
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            SendType = x.SendTypeId,
                            StageType = x.StageTypeId,
                            Description = x.Description,
                            Stage = x.Stage,
                            TaskId = x.TaskId,
                            TaskName = x.Task.Task,
                            DueDay = x.DueDay,
                            AccessLevel = x.AccessLevelId,
                            TargetPositionId = x.TargetPositionId,
                            TargetPositionName = x.TargetPosition.Name,
                            TargetPositionDepartmentId = x.TargetPosition.Department.Id,
                            TargetPositionDepartmentName = x.TargetPosition.Department.Name,
                            TargetPositionExecutorAgentName = (x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : (string)null)),
                            TargetAgentId = x.TargetAgentId,
                            TargetAgentName = x.TargetAgent.Name,
                            SendTypeName = x.SendType.Name,
                            StageTypeName = x.StageType.Name,
                            AccessLevelName = x.AccessLevel.Name,
                            IsWorkGroup = x.IsWorkGroup,
                            IsAddControl = x.IsAddControl,
                            SelfDueDay = x.SelfDueDay,
                            SelfAttentionDay = x.SelfAttentionDay,
                            SelfDescription = x.SelfDescription,
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int AddOrUpdateTemplateSendList(IContext context, InternalTemplateDocumentSendList template)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocumentSendLists()
                {
                    DocumentId = template.DocumentId,
                    Description = template.Description,
                    IsWorkGroup = template.IsWorkGroup,
                    IsAddControl = template.IsAddControl,
                    SelfDescription = template.SelfDescription,
                    SelfDueDay = template.SelfDueDay,
                    SelfAttentionDay = template.SelfAttentionDay,
                    TargetPositionId = template.TargetPositionId,
                    AccessLevelId = (int)template.AccessLevel,
                    DueDay = template.DueDay,
                    SendTypeId = (int)template.SendType,
                    StageTypeId = (int?)template.StageType,
                    Stage = template.Stage,
                    TargetAgentId = template.TargetAgentId,
                    TaskId = template.TaskId,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };
                var entityState = System.Data.Entity.EntityState.Added;
                if (template.Id > 0)
                {
                    newTemplate.Id = template.Id;
                    entityState = System.Data.Entity.EntityState.Modified;
                }

                dbContext.SafeAttach(newTemplate);
                var entity = dbContext.Entry(newTemplate);
                entity.State = entityState;

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, newTemplate.Id, EnumObjects.TemplateDocumentSendList, template.Id > 0 ? EnumOperationType.UpdateFull : EnumOperationType.AddNew);
                transaction.Complete();
                return newTemplate.Id;
            }
        }

        public void DeleteTemplateSendList(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentSendListsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, ddt.Id, EnumObjects.TemplateDocumentSendList, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion TemplateDocumentSendLists

        #region TemplateDocumentRestrictedSendList
        private IQueryable<TemplateDocumentRestrictedSendLists> GetTemplateDocumentRestrictedSendListsQuery(IContext context, FilterTemplateDocumentRestrictedSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).AsQueryable();
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocumentRestrictedSendLists>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocumentRestrictedSendLists>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

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
            return qry;
        }

        public IEnumerable<FrontTemplateDocumentRestrictedSendList> GetTemplateDocumentRestrictedSendLists(IContext context, FilterTemplateDocumentRestrictedSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentRestrictedSendListsQuery(context, filter);
                var res = qry.Select(x => new FrontTemplateDocumentRestrictedSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.Position.Id,
                    AccessLevel = x.AccessLevelId,
                    PositionName = x.Position.Name,
                    PositionExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : null),
                    AccessLevelName = x.AccessLevel.Name,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public bool ExistsTemplateDocumentRestrictedSendLists(IContext context, FilterTemplateDocumentRestrictedSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentRestrictedSendListsQuery(context, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocumentRestrictedSendList GetTemplateDocumentRestrictedSendList(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateDocumentRestrictedSendList
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            PositionId = x.Position.Id,
                            PositionName = x.Position.Name,
                            PositionExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : null),
                            AccessLevelName = x.AccessLevel.Name,
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int AddOrUpdateTemplateRestrictedSendList(IContext context, InternalTemplateDocumentRestrictedSendList template)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocumentRestrictedSendLists()
                {

                    DocumentId = template.DocumentId,
                    PositionId = template.PositionId,
                    AccessLevelId = (int)template.AccessLevel,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };
                var entityState = System.Data.Entity.EntityState.Added;
                if (template.Id > 0)
                {
                    newTemplate.Id = template.Id;
                    entityState = System.Data.Entity.EntityState.Modified;
                }

                dbContext.SafeAttach(newTemplate);
                var entity = dbContext.Entry(newTemplate);
                entity.State = entityState;

                dbContext.SaveChanges();

                transaction.Complete();
                return newTemplate.Id;
            }
        }

        public void DeleteTemplateRestrictedSendList(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentRestrictedSendListsSet.Remove(ddt);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public bool CanAddTemplateRestrictedSendList(IContext context, ModifyTemplateDocumentRestrictedSendList list)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var count =
                    dbContext.TemplateDocumentRestrictedSendListsSet.Count(
                        x => x.Document.ClientId == context.Client.Id && x.DocumentId == list.DocumentId && x.PositionId == list.PositionId);
                transaction.Complete();
                return count == 0;
            }
        }

        #endregion TemplateDocumentRestrictedSendList

        #region TemplateDocumentAccess
        private IQueryable<TemplateDocumentAccesses> GetTemplateDocumentAccessesQuery(IContext context, FilterTemplateDocumentAccess filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.TemplateDocumentAccessesSet.Where(x => x.Document.ClientId == context.Client.Id).AsQueryable();
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocumentAccesses>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocumentAccesses>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

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

            }
            return qry;
        }

        public IEnumerable<FrontTemplateDocumentAccess> GetTemplateDocumentAccesses(IContext context, FilterTemplateDocumentAccess filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentAccessesQuery(context, filter);
                var res = qry.Select(x => new FrontTemplateDocumentAccess
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    PositionId = x.Position.Id,
                    PositionName = x.Position.Name,
                    PositionExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : null),
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public bool ExistsTemplateDocumentAccesses(IContext context, FilterTemplateDocumentAccess filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentAccessesQuery(context, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateDocumentAccess GetTemplateDocumentAccess(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentAccessesSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateDocumentAccess
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            PositionId = x.Position.Id,
                            PositionName = x.Position.Name,
                            PositionExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : null),
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public int AddOrUpdateTemplateAccess(IContext context, InternalTemplateDocumentAccess template)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocumentAccesses()
                {

                    DocumentId = template.DocumentId,
                    PositionId = template.PositionId,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };
                var entityState = System.Data.Entity.EntityState.Added;
                if (template.Id > 0)
                {
                    newTemplate.Id = template.Id;
                    entityState = System.Data.Entity.EntityState.Modified;
                }

                dbContext.SafeAttach(newTemplate);
                var entity = dbContext.Entry(newTemplate);
                entity.State = entityState;

                dbContext.SaveChanges();

                transaction.Complete();
                return newTemplate.Id;
            }
        }

        public void DeleteTemplateAccess(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentRestrictedSendListsSet.Remove(ddt);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public bool CanAddTemplateAccess(IContext context, ModifyTemplateDocumentAccess list)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var count =
                    dbContext.TemplateDocumentAccessesSet.Count(
                        x => x.Document.ClientId == context.Client.Id && x.DocumentId == list.DocumentId && x.PositionId == list.PositionId);
                transaction.Complete();
                return count == 0;
            }
        }

        #endregion TemplateDocumentAccess

        #region TemplateDocumentTasks

        public IEnumerable<FrontTemplateDocumentTask> GetTemplateDocumentTasks(IContext context, FilterTemplateDocumentTask filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.Client.Id).AsQueryable();

                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<TemplateDocumentTasks>(false);
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

        public FrontTemplateDocumentTask GetTemplateDocumentTask(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
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

        public int AddOrUpdateTemplateTask(IContext context, InternalTemplateDocumentTask template)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateDocumentTasks()
                {

                    DocumentId = template.DocumentId,
                    Task = template.Task,
                    Description = template.Description,
                    LastChangeDate = template.LastChangeDate,
                    LastChangeUserId = template.LastChangeUserId
                };
                var entityState = System.Data.Entity.EntityState.Added;
                if (template.Id != 0)
                {
                    newTemplate.Id = (int)template.Id;
                    entityState = System.Data.Entity.EntityState.Modified;
                }

                dbContext.SafeAttach(newTemplate);
                var entity = dbContext.Entry(newTemplate);
                entity.State = entityState;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, newTemplate.Id, EnumObjects.TemplateDocumentTask,
                    template.Id > 0 ? EnumOperationType.UpdateFull : EnumOperationType.AddNew);
                transaction.Complete();
                return newTemplate.Id;
            }
        }

        public bool CanAddTemplateTask(IContext context, AddTemplateDocumentTask task)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var count = dbContext.TemplateDocumentTasksSet.Count(x =>
                    (x.Document.ClientId == context.Client.Id && x.DocumentId == task.DocumentId && x.Task == task.Task)
                    );
                transaction.Complete();
                return count == 0;
            }
        }

        public void DeleteTemplateTask(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.Client.Id).FirstOrDefault(x => x.Id == id);
                if (ddt == null) return;
                dbContext.TemplateDocumentTasksSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, ddt.Id, EnumObjects.TemplateDocumentTask, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion TemplateDocumentTasks

        #region TemplateDocumentPapers

        private IQueryable<TemplateDocumentPapers> GetTemplateDocumentPapersQuery(IContext context, FilterTemplateDocumentPaper filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id).AsQueryable();
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocumentPapers>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocumentPapers>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());
                    qry = qry.Where(filterContains);
                }
                if (filter.TemplateId.HasValue)
                {
                    qry = qry.Where(x => x.DocumentId == filter.TemplateId.Value);
                }
            }
            return qry;
        }
        public bool ExistsTemplateDocumentPapers(IContext context, FilterTemplateDocumentPaper filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentPapersQuery(context, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<FrontTemplateDocumentPaper> GetTemplateDocumentPapers(IContext context, FilterTemplateDocumentPaper filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateDocumentPapersQuery(context, filter);
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

        public FrontTemplateDocumentPaper GetTemplateDocumentPaper(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
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
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (papers != null && papers.Any())
                {
                    foreach (var paper in papers)
                    {
                        var paperDb = ModelConverter.GetDbTemplateDocumentPaper(paper);
                        dbContext.TemplateDocumentPapersSet.Add(paperDb);
                        dbContext.SaveChanges();
                        paper.Id = paperDb.Id;
                        res.Add(paperDb.Id);
                    }
                }
                CommonQueries.AddFullTextCacheInfo(context, res, EnumObjects.TemplateDocumentPaper, EnumOperationType.AddNew);
                transaction.Complete();
            }
            return res;
        }

        public void ModifyTemplatePaper(IContext context, InternalTemplateDocumentPaper item)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var itemDb = ModelConverter.GetDbTemplateDocumentPaper(item);
                dbContext.SafeAttach(itemDb);
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
                CommonQueries.AddFullTextCacheInfo(context, item.Id, EnumObjects.TemplateDocumentPaper, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public InternalTemplateDocument ModifyTemplatePaperPrepare(IContext context, int? id, AddTemplateDocumentPaper model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalTemplateDocument
                    {
                        Id = x.Id,
                    }).FirstOrDefault();
                if (doc == null) return null;
                if (id.HasValue)
                {
                    doc.Papers = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => (x.Id == id))//|| x.Name == model.Name) && x.DocumentId == model.DocumentId)
                        .Select(x => new InternalTemplateDocumentPaper
                        {
                            Id = x.Id,
                        }).ToList();
                }
                else // if (model.Id == 0)
                {
                    doc.MaxPaperOrderNumber = dbContext.TemplateDocumentPapersSet
                        .Where(
                            x =>
                                x.DocumentId == model.DocumentId && x.Name == model.Name && x.IsMain == model.IsMain &&
                                x.IsCopy == model.IsCopy && x.IsOriginal == model.IsOriginal)
                        .OrderByDescending(x => x.OrderNumber).Select(x => x.OrderNumber).FirstOrDefault();
                }
                transaction.Complete();
                return doc;
            }
        }

        public void DeleteTemplatePaper(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                dbContext.TemplateDocumentPapersSet.RemoveRange(dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id));
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, id, EnumObjects.TemplateDocumentPaper, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion TemplateDocumentPapers

        #region TemplateDocumentAttachedFiles

        public IQueryable<TemplateDocumentFiles> GetTemplateAttachedFilesQuery(IContext context, FilterTemplateAttachedFile filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id).AsQueryable();
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateDocumentFiles>(false);
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
                if (!string.IsNullOrEmpty(filter.Extention))
                {
                    qry = qry.Where(x => x.Extention.Contains(filter.Extention));
                }

                if (!string.IsNullOrEmpty(filter.NameExactly))
                {
                    qry = qry.Where(x => x.Name == filter.NameExactly);
                }
                if (!string.IsNullOrEmpty(filter.ExtentionExactly))
                {
                    qry = qry.Where(x => x.Extention == filter.ExtentionExactly);
                }
            }
            return qry;
        }

        public bool ExistsTemplateAttachedFiles(IContext context, FilterTemplateAttachedFile filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateAttachedFilesQuery(context, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontTemplateDocumentFile> GetTemplateAttachedFiles(IContext context, FilterTemplateAttachedFile filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateAttachedFilesQuery(context, filter);
                var res = qry.Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                        (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new FrontTemplateDocumentFile
                        {
                            Id = x.fl.Id,
                            DocumentId = x.fl.DocumentId,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            TypeName = x.fl.Type.Code,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,

                            OrderInDocument = x.fl.OrderNumber,
                            Description = x.fl.Description,
                            File = new BaseFile
                            {
                                FileType = x.fl.FileType,
                                FileSize = x.fl.FileSize,
                                Name = x.fl.Name,
                                Extension = x.fl.Extention,
                            }
                        }).ToList();
                transaction.Complete();
                return res;
            }
        }


        public FrontTemplateDocumentFile GetTemplateAttachedFile(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentFilesSet
                        .Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.Id == id)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id, (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new FrontTemplateDocumentFile
                        {
                            Id = x.fl.Id,
                            DocumentId = x.fl.DocumentId,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            TypeName = x.fl.Type.Code,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            OrderInDocument = x.fl.OrderNumber,
                            Description = x.fl.Description,
                            PdfCreated = x.fl.IsPdfCreated ?? false,
                            LastPdfAccess = x.fl.LastPdfAccessDate,
                            File = new BaseFile
                            {
                                FileType = x.fl.FileType,
                                FileSize = x.fl.FileSize,
                                Name = x.fl.Name,
                                Extension = x.fl.Extention,
                            }
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }


        public int GetNextFileOrderNumber(IContext context, int templateId)
        {
            var res = 1;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
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

        public int AddNewFile(IContext context, InternalTemplateDocumentFile docFile)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.TemplateDocumentFilesSet.Add(fl);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, fl.Id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.AddNew);
                docFile.Id = fl.Id;
                transaction.Complete();
                return fl.Id;
            }
        }
        public InternalTemplateDocumentFile UpdateFilePrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var file = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.Id == id)
                        .Select(x => new InternalTemplateDocumentFile
                        {
                            Id = x.Id,
                            ClientId = x.Document.ClientId,
                            EntityTypeId = x.Document.EntityTypeId,
                            DocumentId = x.DocumentId,
                            OrderInDocument = x.OrderNumber,
                            Type = (EnumFileTypes)x.TypeId,
                            Description = x.Description,
                            PdfCreated = x.IsPdfCreated ?? false,
                            LastPdfAccess = x.LastPdfAccessDate//??DateTime.MinValue,
                            //Name = x.Name,
                            //Extension = x.Extention,
                            //FileType = x.FileType,
                            //FileSize = x.FileSize,
                            //Hash = x.Hash,
                        }).FirstOrDefault();
                transaction.Complete();
                return file;
            }
        }

        public void UpdateFile(IContext context, InternalTemplateDocumentFile docFile)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.SafeAttach(fl);

                var entry = dbContext.Entry(fl);
                entry.Property(x => x.TypeId).IsModified = true;
                entry.Property(x => x.Description).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;

                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, docFile.Id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void UpdateFilePdfView(IContext context, InternalTemplateDocumentFile docFile)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.SafeAttach(fl);
                var entry = dbContext.Entry(fl);
                entry.Property(x => x.IsPdfCreated).IsModified = true;
                entry.Property(x => x.LastPdfAccessDate).IsModified = true;

                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, docFile.Id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public InternalTemplateDocumentFile DeleteTemplateAttachedFilePrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var file = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.Id == id)
                        .Select(x => new InternalTemplateDocumentFile
                        {
                            Id = x.Id,
                            ClientId = x.Document.ClientId,
                            EntityTypeId = x.Document.EntityTypeId,
                            DocumentId = x.DocumentId,
                            OrderInDocument = x.OrderNumber
                        }).FirstOrDefault();
                transaction.Complete();
                return file;
            }
        }
        public void DeleteTemplateAttachedFile(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                dbContext.TemplateDocumentFilesSet.RemoveRange(
                    dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id).Where(
                        x => x.Id == id));
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, id, EnumObjects.TemplateDocumentAttachedFiles, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public bool CanAddTemplateAttachedFile(IContext context, AddTemplateAttachedFile model, BaseFile file)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var fileName = file.Name;
                var fileExtention = file.Extension;
                var res = dbContext.TemplateDocumentFilesSet.Any(x => x.Document.ClientId == context.Client.Id && x.DocumentId == model.DocumentId && x.Extention == fileExtention && x.Name == fileName);
                transaction.Complete();
                return !res;
            }
        }

        #endregion TemplateDocumentAttachedFiles
    }

}