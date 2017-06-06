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
using System.Data.Entity;

namespace BL.Database.Documents
{
    public class TemplateDbProcess : CoreDb.CoreDb, ITemplateDbProcess
    {
        #region Template

        private IQueryable<TemplateDocuments> GetTemplateQuery(IContext context, FilterTemplate filter)
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

        public bool ExistsTemplates(IContext context, FilterTemplate filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = GetTemplateQuery(context, filter).Any();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontTemplate> GetTemplate(IContext context, FilterTemplate filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateQuery(context, filter);

                qry = qry.OrderByDescending(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontTemplate>();

                var res = qry.Select(x => new FrontTemplate
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

        public IEnumerable<FrontMainTemplate> GetMainTemplate(IContext context, IBaseFilter filter, UIPaging paging, UISorting sotring)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateQuery(context, filter as FilterTemplate);

                qry = qry.OrderByDescending(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainTemplate>();

                var res = qry.Select(x => new FrontMainTemplate
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

        public List<int> GetTemplateIDs(IContext context, IBaseFilter filter, UISorting sotring)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateQuery(context, filter as FilterTemplate);

                qry = qry.OrderByDescending(x => x.Name);

                //if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainTemplateDocument>();

                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplate GetTemplateByDocumentId(IContext context, int documentId)
        {
            int templateDocumentId = 0;
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                templateDocumentId = CommonQueries.GetDocumentQuery(context, new FilterDocument { DocumentId = new List<int> { documentId }, IsInWork = true })
                    .Where(x => x.Id == documentId).Select(x => x.TemplateDocumentId) .FirstOrDefault();
                var res = GetTemplate(context, templateDocumentId);
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplate GetTemplate(IContext context, int templateDocumentId)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var templateDocument =
                    dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id).Where(x => x.Id == templateDocumentId)
                        .Select(x => new FrontTemplate
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
                    templateDocument.Properties = CommonQueries.GetPropertyValues(context, new FilterPropertyValue { RecordId = new List<int> { templateDocumentId }, Object = new List<EnumObjects> { EnumObjects.Template } });
                transaction.Complete();
                return templateDocument;
            }
        }

        public int AddOrUpdateTemplate(IContext context, InternalTemplate template)
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

                CommonQueries.AddFullTextCacheInfo(context, newTemplate.Id, EnumObjects.Template,
                    template.Id > 0 ? EnumOperationType.UpdateFull : EnumOperationType.AddNew);


                if (template.Properties != null && template.Properties.Any())
                {
                    CommonQueries.ModifyPropertyValues(context, new InternalPropertyValues { Object = EnumObjects.Template, RecordId = newTemplate.Id, PropertyValues = template.Properties });
                }
                dbContext.SaveChanges();
                transaction.Complete();

                return newTemplate.Id;

            }
        }
        public InternalTemplate CopyTemplatePrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.Id == id)
                    .Select(x => new InternalTemplate
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
                        .Select(y => new InternalTemplateTask()
                        {
                            Task = y.Task,
                            Description = y.Description,
                        }).ToList();
                doc.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(y => y.DocumentId == id)
                    .Select(y => new InternalTemplateRestrictedSendList()
                    {
                        PositionId = y.PositionId,
                        AccessLevel = (EnumAccessLevels)y.AccessLevelId
                    }).ToList();
                var sendLists = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(y => y.DocumentId == id)
                    .Select(y => new InternalTemplateSendList()
                    {
                        Id = y.Id,
                        SendType = (EnumSendTypes)y.SendTypeId,
                        StageType = (EnumStageTypes?)y.StageTypeId,
                        TaskName = y.Task.Task,
                        IsAddControl = y.IsAddControl,
                        SelfDescription = y.SelfDescription,
                        SelfDueDay = y.SelfDueDay,
                        SelfAttentionDay = y.SelfAttentionDay,
                        Description = y.Description,
                        Stage = y.Stage,
                        DueDay = y.DueDay,
                        AccessLevel = (EnumAccessLevels)y.AccessLevelId,
                    }).ToList();
                CommonQueries.SetAccessGroups(context, sendLists);
                doc.SendLists = sendLists;
                doc.Files = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.DocumentId == id)
                    .Select(x => new InternalTemplateFile
                    {
                        DocumentId = x.DocumentId,
                        OrderInDocument = x.OrderNumber,
                        Type = (EnumFileTypes)x.TypeId,
                        Hash = x.Hash,
                        Description = x.Description,
                        PdfCreated = x.IsPdfCreated ?? false,
                        PdfAcceptable = x.PdfAcceptable ?? false,
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
                    .Select(x => new InternalTemplatePaper
                    {
                        IsCopy = x.IsCopy,
                        IsMain = x.IsMain,
                        IsOriginal = x.IsOriginal,
                        OrderNumber = x.OrderNumber,
                        PageQuantity = x.PageQuantity,
                        Name = x.Name,
                        Description = x.Description,
                    }).ToList();
                doc.Properties = CommonQueries.GetInternalPropertyValues(context, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.Template }, RecordId = new List<int> { id } }).ToList();

                transaction.Complete();
                return doc;
            }
        }
        public InternalTemplate DeleteTemplatePrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.Id == id)
                    .Select(x => new InternalTemplate
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

                CommonQueries.DeletePropertyValues(context, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.Template }, RecordId = new List<int> { id } });

                dbContext.TemplateDocumentsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, ddt.Id, EnumObjects.Template, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public bool CanModifyTemplate(IContext context, ModifyTemplate template)
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

        public bool CanAddTemplate(IContext context, AddTemplate template)
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
            return CanModifyTemplate(context, new ModifyTemplate() { Id = templateId });
        }

        #endregion Template

        #region TemplateSendList

        public IEnumerable<FrontTemplateSendList> GetTemplateSendLists(IContext context, FilterTemplateSendList filter)
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
                var res = qry.Select(x => new FrontTemplateSendList
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    SendType = x.SendTypeId,
                    StageType = x.StageTypeId,

                    Description = x.Description,
                    Stage = x.Stage,
                    IsAddControl = x.IsAddControl,
                    TaskId = x.TaskId,
                    TaskName = x.Task.Task,
                    DueDay = x.DueDay,
                    SelfDueDay = x.SelfDueDay,
                    SelfAttentionDay = x.SelfAttentionDay,
                    SelfDescription = x.SelfDescription,
                    AccessLevel = x.AccessLevelId,
                    SendTypeName = x.SendType.Name,
                    StageTypeName = x.StageType.Name,
                    AccessLevelName = x.AccessLevel.Name,
                }).ToList();
                CommonQueries.SetAccessGroups(context, res);
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateSendList GetTemplateSendList(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateSendList
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
                            SendTypeName = x.SendType.Name,
                            StageTypeName = x.StageType.Name,
                            AccessLevelName = x.AccessLevel.Name,
                            IsAddControl = x.IsAddControl,
                            SelfDueDay = x.SelfDueDay,
                            SelfAttentionDay = x.SelfAttentionDay,
                            SelfDescription = x.SelfDescription,
                        }).FirstOrDefault();
                CommonQueries.SetAccessGroups(context, new List<FrontTemplateSendList> { res });
                transaction.Complete();
                return res;
            }
        }

        public int AddTemplateSendList(IContext context, InternalTemplateSendList sendList)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var sendListsDb = ModelConverter.GetDbTemplateSendList(sendList, true);
                dbContext.TemplateDocumentSendListsSet.Add(sendListsDb);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, sendListsDb.Id, EnumObjects.DocumentSendLists, EnumOperationType.AddNew);
                transaction.Complete();
                return sendListsDb.Id;
            }
        }

        public int ModifyTemplateSendList(IContext context, InternalTemplateSendList sendList)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                //TODO MayBe divide delete and add?
                dbContext.TemplateDocumentSendListAccessGroupsSet.RemoveRange(
                    dbContext.TemplateDocumentSendListAccessGroupsSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.SendListId == sendList.Id));
                dbContext.SaveChanges();
                var sendListsDb = ModelConverter.GetDbTemplateSendList(sendList, false);
                dbContext.SafeAttach(sendListsDb);
                var entity = dbContext.Entry(sendListsDb);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();
                if (sendList.AccessGroups?.Any() ?? false)
                {
                    var accessesDb = ModelConverter.GetDbTemplateSendListAccessGroups(sendList.AccessGroups);
                    dbContext.TemplateDocumentSendListAccessGroupsSet.AddRange(accessesDb);
                    dbContext.SaveChanges();
                }
                CommonQueries.AddFullTextCacheInfo(context, sendListsDb.Id, EnumObjects.TemplateSendList, EnumOperationType.UpdateFull);
                transaction.Complete();
                return sendListsDb.Id;
            }
        }

        public void DeleteTemplateSendList(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                dbContext.TemplateDocumentSendListAccessGroupsSet.RemoveRange(
                    dbContext.TemplateDocumentSendListAccessGroupsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.SendListId == id));
                dbContext.TemplateDocumentSendListsSet.RemoveRange(
                    dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id));
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, id, EnumObjects.TemplateSendList, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion TemplateSendList

        #region TemplateRestrictedSendList
        private IQueryable<TemplateDocumentRestrictedSendLists> GetTemplateRestrictedSendListsQuery(IContext context, FilterTemplateRestrictedSendList filter)
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

        public IEnumerable<FrontTemplateRestrictedSendList> GetTemplateRestrictedSendLists(IContext context, FilterTemplateRestrictedSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateRestrictedSendListsQuery(context, filter);
                var res = qry.Select(x => new FrontTemplateRestrictedSendList
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

        public bool ExistsTemplateRestrictedSendLists(IContext context, FilterTemplateRestrictedSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateRestrictedSendListsQuery(context, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateRestrictedSendList GetTemplateRestrictedSendList(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateRestrictedSendList
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

        public int AddOrUpdateTemplateRestrictedSendList(IContext context, InternalTemplateRestrictedSendList template)
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

        public bool CanAddTemplateRestrictedSendList(IContext context, ModifyTemplateRestrictedSendList list)
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

        #endregion TemplateRestrictedSendList

        #region TemplateAccess
        private IQueryable<TemplateAccesses> GetTemplateAccessesQuery(IContext context, FilterTemplateAccess filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.TemplateDocumentAccessesSet.Where(x => x.Document.ClientId == context.Client.Id).AsQueryable();
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateAccesses>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<TemplateAccesses>(true);
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

        public IEnumerable<FrontTemplateAccess> GetTemplateAccesses(IContext context, FilterTemplateAccess filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateAccessesQuery(context, filter);
                var res = qry.Select(x => new FrontTemplateAccess
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

        public bool ExistsTemplateAccesses(IContext context, FilterTemplateAccess filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateAccessesQuery(context, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        public FrontTemplateAccess GetTemplateAccess(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentAccessesSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
                        .Select(x => new FrontTemplateAccess
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

        public int AddOrUpdateTemplateAccess(IContext context, InternalTemplateAccess template)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var newTemplate = new TemplateAccesses()
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

        public bool CanAddTemplateAccess(IContext context, ModifyTemplateAccess list)
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

        #endregion TemplateAccess

        #region TemplateTask

        public IEnumerable<FrontTemplateTask> GetTemplateTasks(IContext context, FilterTemplateTask filter)
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
                var res = qry.Select(x => new FrontTemplateTask
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

        public FrontTemplateTask GetTemplateTask(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
                    .Select(x => new FrontTemplateTask
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

        public int AddOrUpdateTemplateTask(IContext context, InternalTemplateTask template)
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

                CommonQueries.AddFullTextCacheInfo(context, newTemplate.Id, EnumObjects.TemplateTask,
                    template.Id > 0 ? EnumOperationType.UpdateFull : EnumOperationType.AddNew);
                transaction.Complete();
                return newTemplate.Id;
            }
        }

        public bool CanAddTemplateTask(IContext context, AddTemplateTask task)
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

                CommonQueries.AddFullTextCacheInfo(context, ddt.Id, EnumObjects.TemplateTask, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion TemplateTask

        #region TemplatePaper

        private IQueryable<TemplateDocumentPapers> GetTemplatePapersQuery(IContext context, FilterTemplatePaper filter)
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
        public bool ExistsTemplatePapers(IContext context, FilterTemplatePaper filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplatePapersQuery(context, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<FrontTemplatePaper> GetTemplatePapers(IContext context, FilterTemplatePaper filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplatePapersQuery(context, filter);
                var res = qry.Select(x => new FrontTemplatePaper
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

        public FrontTemplatePaper GetTemplatePaper(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id).Where(x => x.Id == id)
                    .Select(x => new FrontTemplatePaper
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

        public IEnumerable<int> AddTemplatePapers(IContext context, IEnumerable<InternalTemplatePaper> papers)
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
                CommonQueries.AddFullTextCacheInfo(context, res, EnumObjects.TemplatePaper, EnumOperationType.AddNew);
                transaction.Complete();
            }
            return res;
        }

        public void ModifyTemplatePaper(IContext context, InternalTemplatePaper item)
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
                CommonQueries.AddFullTextCacheInfo(context, item.Id, EnumObjects.TemplatePaper, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public InternalTemplate ModifyTemplatePaperPrepare(IContext context, int? id, AddTemplatePaper model)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.Client.Id)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalTemplate
                    {
                        Id = x.Id,
                    }).FirstOrDefault();
                if (doc == null) return null;
                if (id.HasValue)
                {
                    doc.Papers = dbContext.TemplateDocumentPapersSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => (x.Id == id))//|| x.Name == model.Name) && x.DocumentId == model.DocumentId)
                        .Select(x => new InternalTemplatePaper
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
                CommonQueries.AddFullTextCacheInfo(context, id, EnumObjects.TemplatePaper, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion TemplatePaper

        #region TemplateFile

        public IQueryable<TemplateDocumentFiles> GetTemplateFilesQuery(IContext context, FilterTemplateFile filter)
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

        public bool ExistsTemplateFiles(IContext context, FilterTemplateFile filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateFilesQuery(context, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontTemplateFile> GetTemplateFiles(IContext context, FilterTemplateFile filter)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTemplateFilesQuery(context, filter);
                var res = qry.Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                        (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new FrontTemplateFile
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


        public FrontTemplateFile GetTemplateFile(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.TemplateDocumentFilesSet
                        .Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.Id == id)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id, (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new FrontTemplateFile
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
                            PdfAcceptable = x.fl.PdfAcceptable ?? false,
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

        public int AddTemplateFile(IContext context, InternalTemplateFile docFile)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.TemplateDocumentFilesSet.Add(fl);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(context, fl.Id, EnumObjects.TemplateFiles, EnumOperationType.AddNew);
                docFile.Id = fl.Id;
                transaction.Complete();
                return fl.Id;
            }
        }
        public InternalTemplateFile ModifyTemplateFilePrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var file = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.Id == id)
                        .Select(x => new InternalTemplateFile
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            OrderInDocument = x.OrderNumber,
                            Type = (EnumFileTypes)x.TypeId,
                            Description = x.Description,
                            PdfCreated = x.IsPdfCreated ?? false,
                            PdfAcceptable = x.PdfAcceptable ?? false,
                            LastPdfAccess = x.LastPdfAccessDate
                        }).FirstOrDefault();
                transaction.Complete();
                return file;
            }
        }

        public void ModifyTemplateFile(IContext context, InternalTemplateFile docFile)
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
                CommonQueries.AddFullTextCacheInfo(context, docFile.Id, EnumObjects.TemplateFiles, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void ModifyTemplateFilePdfView(IContext context, InternalTemplateFile docFile)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbTemplateFile(docFile);
                dbContext.SafeAttach(fl);
                var entry = dbContext.Entry(fl);
                entry.Property(x => x.IsPdfCreated).IsModified = true;
                entry.Property(x => x.LastPdfAccessDate).IsModified = true;
                entry.Property(x => x.PdfAcceptable).IsModified = true;

                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, docFile.Id, EnumObjects.TemplateFiles, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public InternalTemplateFile DeleteTemplateFilePrepare(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var file = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id)
                        .Where(x => x.Id == id)
                        .Select(x => new InternalTemplateFile
                        {
                            Id = x.Id,
                            DocumentId = x.DocumentId,
                            OrderInDocument = x.OrderNumber
                        }).FirstOrDefault();
                transaction.Complete();
                return file;
            }
        }
        public void DeleteTemplateFile(IContext context, int id)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                dbContext.TemplateDocumentFilesSet.RemoveRange(
                    dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.Client.Id).Where(
                        x => x.Id == id));
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(context, id, EnumObjects.TemplateFiles, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public bool CanAddTemplateFile(IContext context, AddTemplateFile model, BaseFile file)
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

        #endregion TemplateFile
    }

}