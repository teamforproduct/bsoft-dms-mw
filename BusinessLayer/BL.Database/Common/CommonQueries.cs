using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Document;
using BL.Database.DBModel.System;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.InternalModel;
using System.Text;
using System;
using BL.CrossCutting.DependencyInjection;
using BL.Database.FileWorker;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.CrossCutting.CryptographicWorker;
using LinqKit;
using BL.Database.DBModel.Dictionary;
using System.Data.Entity;
using BL.Database.DBModel.Encryption;
using BL.Model.EncryptionCore.Filters;
using BL.Database.Encryption.Interfaces;
using BL.Database.Reports;
using iTextSharp.text.pdf;
using System.IO;
using BL.Database.Documents.Interfaces;
using BL.Model.Reports.FrontModel;
using System.Data.Entity.Core.Objects;
using System.Threading.Tasks;
using BL.Database.DBModel.Admin;

namespace BL.Database.Common
{
    internal static class CommonQueries
    {
        #region Documents
        public static IQueryable<DBModel.Document.Documents> GetDocumentQuery(DmsContext dbContext, IContext ctx, IQueryable<FrontDocumentAccess> userAccesses = null, bool? isVerifyExecutorPosition = null, bool isVerifyAccessLevel = true, bool isVerifyIsInWork = false)
        {
            var qry = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                if (userAccesses == null)
                {
                    var filterContains = PredicateBuilder.False<DocumentAccesses>();
                    filterContains = isVerifyAccessLevel
                        ? ctx.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                        : ctx.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                    if (isVerifyIsInWork)
                        filterContains = filterContains.And(x => x.IsInWork == true).Expand();
                    if (isVerifyExecutorPosition.HasValue && isVerifyExecutorPosition.Value)
                        filterContains = filterContains.And(x => x.PositionId == x.Document.ExecutorPositionId).Expand();
                    if (isVerifyExecutorPosition.HasValue && !isVerifyExecutorPosition.Value)
                        filterContains = filterContains.And(x => x.PositionId == ctx.CurrentPositionId).Expand();
                    qry = qry.Where(x => x.Accesses.AsQueryable().Where(filterContains).Any());
                }
                else
                    qry = qry.Where(x => userAccesses.Select(a => a.DocumentId).Contains(x.Id));

                //if (isVerifyExecutorPosition)
                //{
                //    var filterExecutorPositionContains = PredicateBuilder.False<DBModel.Document.Documents>();
                //    filterExecutorPositionContains = ctx.CurrentPositionsIdList.Aggregate(filterExecutorPositionContains, (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());
                //    qry = qry.Where(filterExecutorPositionContains);
                //}
                //else 
                if (!isVerifyExecutorPosition.HasValue && userAccesses == null)  //доступ к журналам проверяем, если нет ограничений на Accesses
                {
                    var filterPositionsIdList = PredicateBuilder.False<AdminRegistrationJournalPositions>();
                    filterPositionsIdList = ctx.CurrentPositionsIdList.Aggregate(filterPositionsIdList, (current, value) => current.Or(e => e.PositionId == value).Expand());
                    if (dbContext.AdminRegistrationJournalPositionsSet
                        .Where(filterPositionsIdList).Where(x => x.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.View)
                        .Select(x => x.RegJournalId).Any())
                    {
                        var qryRJA = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable()
                            .Where(x => x.RegistrationJournalId.HasValue
                                        && dbContext.AdminRegistrationJournalPositionsSet
                                            .Where(filterPositionsIdList).Where(y => y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.View)
                                            .Select(y => y.RegJournalId).Contains(x.RegistrationJournalId.Value));
                        var qryCont = qry.Concat(qryRJA);
                        var qry1 = dbContext.DocumentsSet.AsQueryable();
                        qry = qry1.Where(x => qryCont.Select(y => y.Id).Contains(x.Id));
                    }
                }
            }
            return qry;

        }

        public static IQueryable<DBModel.Document.Documents> GetDocumentQuery(IContext ctx, DmsContext dbContext, FilterDocument filter, bool isVerifyAccessLevel = true)
        {
            IQueryable<FrontDocumentAccess> acc = null;

            #region Filter access
            if (filter != null && (filter.IsInWork.HasValue || filter.IsFavourite.HasValue || filter.AccessLevelId?.Count() > 0))
            {
                acc = GetDocumentAccesses(ctx, dbContext, false, true, isVerifyAccessLevel);
                if (filter.IsInWork.HasValue)
                {
                    acc = acc.Where(x => x.IsInWork == filter.IsInWork);
                }

                if (filter.IsFavourite.HasValue)
                {
                    acc = acc.Where(x => x.IsFavourite == filter.IsFavourite);
                }

                if (filter.AccessLevelId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<FrontDocumentAccess>();
                    filterContains = filter.AccessLevelId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AccessLevelId == value).Expand());

                    acc = acc.Where(filterContains);
                }
            }
            #endregion Filter access

            var qry = CommonQueries.GetDocumentQuery(dbContext, ctx, acc);

            if (filter == null || !filter.IsIgnoreRegistered)
            {
                qry = qry.Where(x => x.IsRegistered.HasValue);
            }

            if (filter != null)
            {
                #region Base

                if (filter.AllLinkedDocuments && filter.DocumentId?.Count() == 1)
                {
                    filter.DocumentId = GetLinkedDocumentIds(ctx, dbContext, filter.DocumentId.First()).ToList();
                }

                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentFromDate.HasValue)
                {
                    qry = qry.Where(x => (x.RegistrationDate ?? x.CreateDate) >= filter.DocumentFromDate.Value);
                }

                if (filter.DocumentToDate.HasValue)
                {
                    qry = qry.Where(x => (x.RegistrationDate ?? x.CreateDate) <= filter.DocumentToDate.Value);
                }

                if (filter.CreateFromDate.HasValue)
                {
                    qry = qry.Where(x => x.CreateDate >= filter.CreateFromDate.Value);
                }

                if (filter.CreateToDate.HasValue)
                {
                    qry = qry.Where(x => x.CreateDate <= filter.CreateToDate.Value);
                }

                if (filter.RegistrationFromDate.HasValue)
                {
                    qry = qry.Where(x => x.RegistrationDate >= filter.RegistrationFromDate.Value);
                }

                if (filter.RegistrationToDate.HasValue)
                {
                    qry = qry.Where(x => x.RegistrationDate <= filter.RegistrationToDate.Value);
                }

                if (filter.SenderFromDate.HasValue)
                {
                    qry = qry.Where(x => x.SenderDate >= filter.SenderFromDate.Value);
                }

                if (filter.SenderToDate.HasValue)
                {
                    qry = qry.Where(x => x.SenderDate <= filter.SenderToDate.Value);
                }

                if (!String.IsNullOrEmpty(filter.Description))
                {
                    qry = qry.Where(x => x.Description.Contains(filter.Description));
                }

                if (!String.IsNullOrEmpty(filter.RegistrationNumber))
                {
                    qry =
                        qry.Where(
                            x =>
                                (
                                x.RegistrationNumber.HasValue
                                ? x.RegistrationNumberPrefix + x.RegistrationNumber.ToString() + x.RegistrationNumberSuffix
                                : "#" + x.Id.ToString()
                                )
                                    .Contains(filter.RegistrationNumber));
                }

                if (!String.IsNullOrEmpty(filter.Addressee))
                {
                    qry = qry.Where(x => x.Addressee.Contains(filter.Addressee));
                }

                if (filter.SenderAgentPersonId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.SenderAgentPersonId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SenderAgentPersonId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filter.SenderNumber))
                {
                    qry = qry.Where(x => x.SenderNumber.Contains(filter.SenderNumber));
                }

                if (filter.DocumentTypeId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.DocumentTypeId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TemplateDocument.DocumentTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TemplateDocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.TemplateDocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TemplateDocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentDirectionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.DocumentDirectionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TemplateDocument.DocumentDirectionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentSubjectId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.DocumentSubjectId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentSubjectId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.RegistrationJournalId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.RegistrationJournalId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RegistrationJournalId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ExecutorPositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.ExecutorPositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ExecutorPositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.ExecutorPositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ExecutorDepartmentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.ExecutorDepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.SenderAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filter.SenderAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SenderAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsRegistered.HasValue)
                {
                    qry = qry.Where(x => x.IsRegistered == filter.IsRegistered.Value);
                }

                if (filter.IsInMyControl ?? false)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                #endregion Base

                #region Subscription
                if ((filter.SubscriptionPositionId?.Count() > 0) ||
                    (filter.SubscriptionPositionExecutorAgentId?.Count() > 0) ||
                    (filter.SubscriptionDepartmentId?.Count() > 0))
                {
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = new List<int> { (int)EnumSubscriptionStates.Sign, (int)EnumSubscriptionStates.Visa, (int)EnumSubscriptionStates.Аgreement, (int)EnumSubscriptionStates.Аpproval }
                        .Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SubscriptionStateId == value).Expand());

                    if (filter.SubscriptionPositionId?.Count() > 0)
                    {
                        var filterContainsSubscriptionPositionId = PredicateBuilder.False<DocumentSubscriptions>();
                        filterContainsSubscriptionPositionId = filter.SubscriptionPositionId.Aggregate(filterContainsSubscriptionPositionId,
                            (current, value) => current.Or(e => e.DoneEvent.SourcePositionId == value).Expand());

                        qry = qry.Where(x =>
                                    x.Subscriptions.AsQueryable().Where(filterContains)
                                        .Any(filterContainsSubscriptionPositionId));
                    }

                    if (filter.SubscriptionPositionExecutorAgentId?.Count() > 0)
                    {
                        var filterContainsSubscriptionPositionExecutorAgentId = PredicateBuilder.False<DocumentSubscriptions>();
                        filterContainsSubscriptionPositionExecutorAgentId = filter.SubscriptionPositionExecutorAgentId.Aggregate(filterContainsSubscriptionPositionExecutorAgentId,
                            (current, value) => current.Or(e => e.DoneEvent.SourcePositionExecutorAgentId == value).Expand());

                        qry = qry.Where(x =>
                                    x.Subscriptions.AsQueryable().Where(filterContains)
                                        .Any(filterContainsSubscriptionPositionExecutorAgentId));
                    }

                    if (filter.SubscriptionDepartmentId?.Count() > 0)
                    {
                        var filterContainsSubscriptionDepartmentId = PredicateBuilder.False<DocumentSubscriptions>();
                        filterContainsSubscriptionDepartmentId = filter.SubscriptionDepartmentId.Aggregate(filterContainsSubscriptionDepartmentId,
                            (current, value) => current.Or(e => e.DoneEvent.SourcePosition.DepartmentId == value).Expand());

                        qry = qry.Where(x =>
                                    x.Subscriptions.AsQueryable().Where(filterContains)
                                        .Any(filterContainsSubscriptionDepartmentId));
                    }
                }
                #endregion Subscription

                #region Task
                //TODO Перепроверить доступы
                //TODO Должно соответсвовать всем или хотябы одному
                if (filter.TaskId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTasks>();
                    filterContains = filter.TaskId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Tasks.AsQueryable().Any(filterContains));
                }

                if (!string.IsNullOrEmpty(filter.TaskDescription))
                {
                    qry = qry.Where(x => x.Tasks.Any(y => y.Task.Contains(filter.TaskDescription)));
                }
                #endregion Task

                #region Tag
                if ((filter.TagId?.Count() > 0) ||
                    !string.IsNullOrEmpty(filter.TagDescription))
                {
                    //var filterContainsPosition = PredicateBuilder.False<DocumentTags>();
                    //filterContainsPosition = ctx.CurrentPositionsIdList.Aggregate(filterContainsPosition,
                    //    (current, value) => current.Or(e => !e.Tag.PositionId.HasValue || e.Tag.PositionId == value).Expand());

                    if (filter.TagId?.Count() > 0)
                    {
                        //var filterContains = PredicateBuilder.True<DocumentTags>();
                        //filterContains = filter.TagId.Aggregate(filterContains,
                        //    (current, value) => current.And(e => e.Tag.Id == value).Expand());
                        //qry = qry.Where(x => x.Tags.AsQueryable()
                        ////.Where(filterContainsPosition)
                        //.Any(filterContains));
                        foreach (var id in filter.TagId)
                        {
                            qry = qry.Where(x => x.Tags.Any(y => y.TagId == id));
                        }
                    }

                    if (!string.IsNullOrEmpty(filter.TagDescription))
                    {
                        qry = qry.Where(x => x.Tags.AsQueryable()
                        //.Where(filterContainsPosition)
                        .Any(y => y.Tag.Name.Contains(filter.TagDescription)));
                    }
                }
                #endregion Tag

                #region Property
                //TODO Вынести в отдельный фильтр
                if (filter.FilterProperties?.Count() > 0)
                {
                    foreach (var filterProperty in filter.FilterProperties)
                    {
                        var qryTmp = dbContext.PropertyValuesSet.Where(x => x.PropertyLink.Property.ClientId == ctx.CurrentClientId)
                                        .Where(x => x.PropertyLinkId == filterProperty.PropertyLinkId && qry.Select(y => y.Id).Contains(x.RecordId));

                        switch (filterProperty.ValueType)
                        {
                            case EnumValueTypes.Text:
                                qryTmp = qryTmp.Where(x => x.ValueString.Contains(filterProperty.Text));
                                break;
                            case EnumValueTypes.Number:
                                if (filterProperty.NumberFrom.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.NumberFrom <= x.ValueNumeric);
                                }
                                if (filterProperty.NumberTo.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.NumberTo >= x.ValueNumeric);
                                }
                                break;
                            case EnumValueTypes.Date:
                                if (filterProperty.DateFrom.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.DateFrom <= x.ValueDate);
                                }
                                if (filterProperty.DateTo.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.DateTo >= x.ValueDate);
                                }
                                break;
                            case EnumValueTypes.Api:
                                if (!(filterProperty.Ids?.Count() > 0))
                                {
                                    filterProperty.Ids = new List<int>();
                                }
                                //TODO Contains
                                var ids = filterProperty.Ids.Select(y => (double?)y).ToList();
                                qryTmp = qryTmp.Where(x => ids.Contains(x.ValueNumeric));
                                break;
                        }
                        qry = qry.Where(x => qryTmp.Select(y => y.RecordId).Contains(x.Id));
                    }
                }

                #endregion Property
            }
            return qry;
        }
        #endregion

        #region Events
        public static IQueryable<DocumentEvents> GetDocumentEventQuery(IContext ctx, DmsContext dbContext, FilterDocumentEvent filter, bool isVerifyAccessLevel = true)
        {
            var qrys = GetDocumentEventQueryWithoutUnion(ctx, dbContext, filter, isVerifyAccessLevel);
            var res = qrys.First();
            foreach (var qry in qrys.Skip(1).ToList())
            {
                res = res.Concat(qry);
            }
            return res;
        }


        public static IQueryable<DocumentEvents> GetEventsNativelyQuery(IContext ctx, DmsContext dbContext, FilterDocumentEventNatively filter)
        {
            var qry = dbContext.DocumentEventsSet.AsQueryable();

            if (filter.Date?.HasValue == true)
            {
                qry = qry.Where(x => x.Date >= filter.Date.DateBeg & x.Date <= filter.Date.DateEnd);
            }

            if (filter.ReadDate?.HasValue == true)
            {
                qry = qry.Where(x => x.ReadDate.HasValue && x.ReadDate >= filter.ReadDate.DateBeg && x.ReadDate <= filter.ReadDate.DateEnd);
            }

            if (filter.SourcePositionIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.False<DocumentEvents>();
                filterContains = filter.SourcePositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourcePositionId == value).Expand());
                qry = qry.Where(filterContains);
            }

            if (filter.SourcePositionExecutorAgentIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.False<DocumentEvents>();
                filterContains = filter.SourcePositionExecutorAgentIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourcePositionExecutorAgentId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.SourceAgentIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.False<DocumentEvents>();
                filterContains = filter.SourceAgentIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourceAgentId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.TargetPositionIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.False<DocumentEvents>();
                filterContains = filter.TargetPositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());
                qry = qry.Where(filterContains);
            }

            if (filter.TargetPositionExecutorAgentIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.False<DocumentEvents>();
                filterContains = filter.TargetPositionExecutorAgentIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.TargetPositionExecutorAgentId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.ReadAgentIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.False<DocumentEvents>();
                filterContains = filter.ReadAgentIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.ReadAgentId == value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;

        }


        public static List<IQueryable<DocumentEvents>> GetDocumentEventQueryWithoutUnion(IContext ctx, DmsContext dbContext, FilterDocumentEvent filter, bool isVerifyAccessLevel = true)
        {
            var qry = dbContext.DocumentEventsSet.AsQueryable();

            qry = qry.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);

            if (filter != null)
            {
                if (filter.EventId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.EventId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AllLinkedDocuments && filter.DocumentId?.Count() == 1)
                {
                    filter.DocumentId = GetLinkedDocumentIds(ctx, dbContext, filter.DocumentId.First()).ToList();
                }

                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsNew.HasValue)
                {
                    if (filter.IsNew.Value)
                    {
                        qry = qry.Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId);
                    }
                    else
                    {
                        qry = qry.Where(x => x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId);
                    }
                }
                if (filter.IsSingleSubject.HasValue)
                {
                    if (filter.IsSingleSubject.Value)
                    {
                        qry = qry.Where(x => x.SourcePositionId == x.TargetPositionId);
                    }
                    else
                    {
                        qry = qry.Where(x => x.TargetPositionId != x.SourcePositionId);
                    }
                }

                if (filter.FromDate.HasValue)
                {
                    qry = qry.Where(x => x.CreateDate >= filter.FromDate.Value);
                }

                if (filter.ToDate.HasValue)
                {
                    qry = qry.Where(x => x.CreateDate <= filter.ToDate.Value);
                }

                if (filter.TypeId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.TypeId.Select(x => (int)x).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.EventTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ImportanceEventTypeId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.ImportanceEventTypeId.Select(x => (int)x).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.EventType.ImportanceEventTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Description))
                {
                    qry = qry.Where(x => x.Description.Contains(filter.Description));
                }

                if (filter.PositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.PositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value || e.TargetPositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.SourcePositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.SourcePositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.TargetPositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.TargetPositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.PositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.PositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionExecutorAgentId == value || e.TargetPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.SourcePositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.SourcePositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetPositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.TargetPositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.TargetAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetAgentId == value || e.TargetPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.SourceAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.SourceAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourceAgentId == value || e.SourcePositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.AgentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.AgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e =>
                            e.TargetAgentId == value
                            || e.SourceAgentId == value
                            || e.ReadAgentId == value
                            || e.SourcePositionExecutorAgentId == value
                            || e.TargetPositionExecutorAgentId == value
                            ).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.DepartmentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.DepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePosition.DepartmentId == value || e.TargetPosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.SourceDepartmentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.SourceDepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetDepartmentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.DepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            //TODO Что то придумать с union
            var res = new List<IQueryable<DocumentEvents>>();

            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = isVerifyAccessLevel
                    ? ctx.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                    : ctx.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));

                var filterPositionContains = PredicateBuilder.False<DocumentEvents>();
                filterPositionContains = ctx.CurrentPositionsIdList.Aggregate(filterPositionContains,
                    (current, value) => current.Or(e =>
                        e.TargetPositionId == value
                        || e.SourcePositionId == value).Expand());

                var filterTaskAccessesContains = PredicateBuilder.False<DocumentTaskAccesses>();
                filterTaskAccessesContains = ctx.CurrentPositionsIdList.Aggregate(filterTaskAccessesContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                res.Add(qry.Where(x => !x.IsAvailableWithinTask).Where(filterPositionContains));
                res.Add(qry.Where(x => x.IsAvailableWithinTask && x.Task.TaskAccesses.AsQueryable().Any(filterTaskAccessesContains)));

                //qry = qry.Where(x => !x.IsAvailableWithinTask).Where(filterPositionContains)
                //         .Concat(qry.Where(x => x.IsAvailableWithinTask && x.Task.TaskAccesses.AsQueryable().Any(filterTaskAccessesContains)));
            }
            else
            {
                res.Add(qry);
            }

            return res;
        }

        public static IEnumerable<FrontDocumentEvent> GetDocumentEvents(IContext ctx, DmsContext dbContext, FilterBase filter, UIPaging paging)
        {
            var qrys = GetDocumentEventQueryWithoutUnion(ctx, dbContext, filter?.Event);

            if (filter?.Document != null)
            {
                var documentIds = CommonQueries.GetDocumentQuery(ctx, dbContext, filter?.Document)
                                    .Select(x => x.Id);

                qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
            }

            if (filter?.File != null)
            {
                var documentIds = CommonQueries.GetDocumentFileQuery(ctx, dbContext, filter?.File).Select(x => x.DocumentId);

                qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
            }

            if (filter?.Wait != null)
            {
                var waits = CommonQueries.GetDocumentWaitQuery(ctx, dbContext, filter?.Wait);

                var waitOnEventIds = waits.Select(x => x.OnEventId);
                var waitOffEventIds = waits.Select(x => x.OffEventId);

                qrys = qrys.Select(qry => { return qry.Where(x => waitOnEventIds.Contains(x.Id) || waitOffEventIds.Contains(x.Id)); }).ToList();
            }

            //TODO Sort
            qrys = qrys.Select(qry => { return qry.OrderByDescending(x => x.Date).AsQueryable(); }).ToList();

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                    paging.Counters = new UICounters
                    {
                        Counter1 = qrys.Sum(qry => qry.Where(filterContains).Count(x => !x.ReadDate.HasValue && !(x.TargetPositionId == x.SourcePositionId))),
                        Counter3 = qrys.Sum(qry => qry.Count()),
                    };

                    paging.TotalItemsCount = paging.Counters.Counter3.GetValueOrDefault();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentEvent>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    if (qrys.Count > 1)
                    {
                        var take1 = paging.PageSize * (paging.CurrentPage - 1) + paging.PageSize;

                        qrys = qrys.Select(qry => qry.Take(() => take1)).ToList();

                        var qryConcat = qrys.First();

                        foreach (var qry in qrys.Skip(1).ToList())
                        {
                            qryConcat = qryConcat.Concat(qry);
                        }

                        qrys.Clear();
                        qrys.Add(qryConcat);
                    }

                    //TODO Sort
                    qrys = qrys.Select(qry => { return qry.OrderByDescending(x => x.Date).AsQueryable(); }).ToList();

                    qrys = qrys.Select(qry => qry.Skip(() => skip).Take(() => take)).ToList();
                }
            }

            if ((paging?.IsAll ?? true) && (filter == null || filter.Event == null || ((filter.Event.DocumentId?.Count ?? 0) == 0 && (filter.Event.EventId?.Count ?? 0) == 0)))
            {
                throw new WrongAPIParameters();
            }

            IQueryable<DocumentEvents> qryRes = qrys.First(); ;

            if (qrys.Count > 1)
            {
                foreach (var qry in qrys.Skip(1).ToList())
                {
                    qryRes = qryRes.Concat(qry);
                }
            }


            var maxDateTime = DateTime.UtcNow.AddYears(50);
            var isNeedRegistrationFullNumber = !(filter?.Event?.DocumentId?.Any() ?? false);

            var qryView = dbContext.DocumentEventsSet.Where(x => qryRes.Select(y => y.Id).Contains(x.Id))

                //TODO Sort
                .OrderByDescending(x => x.Date)
                .Select(x => new
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    EventType = x.EventTypeId,
                    EventTypeName = x.EventType.Name,
                    Date = x.Date,
                    CreateDate = x.Date != x.CreateDate ? (DateTime?)x.CreateDate : null,
                    Task = x.Task.Task,
                    Description = x.Description,
                    AddDescription = x.AddDescription,
                    SourcePositionExecutorAgentName = x.SourcePositionExecutorAgent.Name + (x.SourcePositionExecutorType.Suffix != null ? " (" + x.SourcePositionExecutorType.Suffix + ")" : null),
                    TargetPositionExecutorAgentName = (x.TargetPositionExecutorAgent.Name + (x.TargetPositionExecutorType.Suffix != null ? " (" + x.TargetPositionExecutorType.Suffix + ")" : null))
                                                      ?? x.TargetAgent.Name,
                    DocumentDate = (x.Document.LinkId.HasValue || isNeedRegistrationFullNumber) ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                    RegistrationNumber = x.Document.RegistrationNumber,
                    RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                    RegistrationFullNumber = "#" + x.Document.Id,

                    OnWait = x.OnWait.Select(y => new { DueDate = y.DueDate, OffEventDate = (DateTime?)y.OffEvent.Date }).FirstOrDefault(),

                    //For IsRead
                    TargetPositionId = x.TargetPositionId,
                    SourcePositionId = x.SourcePositionId,
                    ReadDate = x.ReadDate,

                    PaperId = (int?)x.Paper.Id,
                    PaperName = x.Paper.Name,
                    PaperIsMain = (bool?)x.Paper.IsMain,
                    PaperIsOriginal = (bool?)x.Paper.IsOriginal,
                    PaperIsCopy = (bool?)x.Paper.IsCopy,
                    PaperOrderNumber = (int?)x.Paper.OrderNumber,

                    PaperPlanDate = x.PaperPlanDate,
                    PaperSendDate = x.PaperSendDate,
                    PaperRecieveDate = x.PaperRecieveDate,
                });

            var qryFE = qryView.ToList();

            var res = qryFE.Select(x => new FrontDocumentEvent
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                EventType = x.EventType,
                EventTypeName = x.EventTypeName,
                Date = x.Date,
                CreateDate = x.CreateDate,
                Task = x.Task,
                Description = x.Description,
                AddDescription = x.AddDescription,
                SourcePositionExecutorAgentName = x.SourcePositionExecutorAgentName,
                TargetPositionExecutorAgentName = x.TargetPositionExecutorAgentName,
                DocumentDate = x.DocumentDate,
                RegistrationNumber = x.RegistrationNumber,
                RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                RegistrationFullNumber = x.RegistrationFullNumber,

                DueDate = x.OnWait != null ? x.OnWait.DueDate > maxDateTime ? null : x.OnWait.DueDate : null,
                CloseDate = x.OnWait != null ? x.OnWait.OffEventDate : null,
                IsOnEvent = x.OnWait != null,

                IsRead = !x.TargetPositionId.HasValue || x.TargetPositionId == x.SourcePositionId || !ctx.CurrentPositionsIdList.Contains(x.TargetPositionId ?? 0) ? null : (bool?)x.ReadDate.HasValue,

                PaperId = x.PaperId,
                PaperName = x.PaperName,
                PaperIsMain = x.PaperIsMain,
                PaperIsOriginal = x.PaperIsOriginal,
                PaperIsCopy = x.PaperIsCopy,
                PaperOrderNumber = x.PaperOrderNumber,

                PaperPlanDate = x.PaperPlanDate,
                PaperSendDate = x.PaperSendDate,
                PaperRecieveDate = x.PaperRecieveDate,
            }).ToList();

            res.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return res;
        }

        #endregion

        #region Files
        public static IQueryable<DocumentFiles> GetDocumentFileQuery(IContext ctx, DmsContext dbContext, FilterDocumentFile filter, bool isVerifyAccessLevel = true)
        {
            var qry = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = isVerifyAccessLevel
                    ? ctx.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                    : ctx.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter.FileId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentFiles>();
                    filterContains = filter.FileId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AllLinkedDocuments && filter.DocumentId?.Count() == 1)
                {
                    filter.DocumentId = GetLinkedDocumentIds(ctx, dbContext, filter.DocumentId.First()).ToList();
                }

                if (filter.DocumentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentFiles>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.OrderInDocument?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentFiles>();
                    filterContains = filter.OrderInDocument.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OrderNumber == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.Types?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentFiles>();
                    filterContains = filter.Types.Cast<int>().Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsWorkedOut.HasValue)
                {
                    if (filter.IsWorkedOut.Value)
                    {
                        qry = qry.Where(x => x.IsWorkedOut != false);
                    }
                    else
                    {
                        qry = qry.Where(x => x.IsWorkedOut == false);
                    }
                }

                if (!filter.IsAllDeleted)
                {
                    qry = qry.Where(x => x.IsDeleted == filter.IsDeleted);
                }

                if (!filter.IsAllVersion)
                {
                    qry = qry.Where(x => x.IsMainVersion == filter.IsMainVersion);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
                }

                if (!string.IsNullOrEmpty(filter.Extension))
                {
                    qry = qry.Where(x => x.Extension.Contains(filter.Extension));
                }
                if (filter.SizeFrom.HasValue)
                {
                    qry = qry.Where(x => x.FileSize >= filter.SizeFrom);
                }
                if (filter.SizeTo.HasValue)
                {
                    qry = qry.Where(x => x.FileSize <= filter.SizeTo);
                }
                if (filter.CreateFromDate.HasValue)
                {
                    qry = qry.Where(x => x.Date >= filter.CreateFromDate.Value);
                }
                if (filter.CreateToDate.HasValue)
                {
                    qry = qry.Where(x => x.Date <= filter.CreateToDate);
                }
                if (filter.AgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentFiles>();
                    filterContains = filter.AgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.LastChangeUserId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.IsMyFiles ?? false)
                {
                    var filterContains = PredicateBuilder.False<DocumentFiles>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public static IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, DmsContext dbContext, FilterBase filter, UIPaging paging = null)
        {
            var qry = CommonQueries.GetDocumentFileQuery(ctx, dbContext, filter?.File);

            if (filter?.Document != null)
            {
                var documentIds = CommonQueries.GetDocumentQuery(ctx, dbContext, filter?.Document, true)
                                    .Select(x => x.Id);

                qry = qry.Where(x => documentIds.Contains(x.DocumentId));
            }

            if (filter?.Event != null)
            {
                var eventsDocumentIds = CommonQueries.GetDocumentEventQuery(ctx, dbContext, filter?.Event)
                                            .Select(x => x.DocumentId);

                qry = qry.Where(x => eventsDocumentIds.Contains(x.DocumentId));
            }

            if (filter?.Wait != null)
            {
                var waitsDocumentIds = CommonQueries.GetDocumentWaitQuery(ctx, dbContext, filter?.Wait)
                                            .Select(x => x.DocumentId);

                qry = qry.Where(x => waitsDocumentIds.Contains(x.DocumentId));
            }

            //TODO Sort
            qry = qry.OrderByDescending(x => x.LastChangeDate);

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = qry.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentAttachedFile>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    qry = qry.Skip(() => skip).Take(() => take);
                }
            }

            if ((paging?.IsAll ?? true) && (filter == null || filter.File == null || ((filter.File.DocumentId?.Count ?? 0) == 0 && (filter.File.FileId?.Count ?? 0) == 0)))
            {
                throw new WrongAPIParameters();
            }

            var isNeedRegistrationFullNumber = !(filter?.File?.DocumentId?.Any() ?? false);

            var qryFE = dbContext.DocumentFilesSet.Where(x => qry.Select(y => y.Id).Contains(x.Id))
                            .OrderByDescending(x => x.LastChangeDate)
                            .Join(dbContext.DictionaryAgentsSet, o => o.LastChangeUserId, i => i.Id, (file, agent) => new FrontDocumentAttachedFile
                            {
                                Id = file.Id,
                                Date = file.Date,
                                DocumentId = file.DocumentId,
                                Extension = file.Extension,
                                FileContent = file.Content,
                                FileType = file.FileType,
                                FileSize = file.FileSize,
                                Type = (EnumFileTypes)file.TypeId,
                                TypeName = file.Type.Name,
                                IsMainVersion = file.IsMainVersion,
                                IsDeleted = file.IsDeleted,
                                IsWorkedOut = file.IsWorkedOut ?? true,
                                Description = file.Description,
                                Hash = file.Hash,
                                LastChangeDate = file.LastChangeDate,
                                LastChangeUserId = file.LastChangeUserId,
                                LastChangeUserName = agent.Name,
                                Name = file.Name,
                                OrderInDocument = file.OrderNumber,
                                Version = file.Version,
                                WasChangedExternal = false,
                                ExecutorPositionName = file.ExecutorPosition.Name,
                                ExecutorPositionExecutorAgentName = file.ExecutorPositionExecutorAgent.Name + (file.ExecutorPositionExecutorType.Suffix != null ? " (" + file.ExecutorPositionExecutorType.Suffix + ")" : null),

                                DocumentDate = (file.Document.LinkId.HasValue || isNeedRegistrationFullNumber) ? file.Document.RegistrationDate ?? file.Document.CreateDate : (DateTime?)null,
                                RegistrationNumber = file.Document.RegistrationNumber,
                                RegistrationNumberPrefix = file.Document.RegistrationNumberPrefix,
                                RegistrationNumberSuffix = file.Document.RegistrationNumberSuffix,
                                RegistrationFullNumber = "#" + file.Document.Id,
                            });

            var res = qryFE.ToList();

            if (res.Any(x => x.IsMainVersion))
            {
                var filterContains = PredicateBuilder.False<DocumentFiles>();
                filterContains = res.Where(x => x.IsMainVersion).Select(x => new { x.DocumentId, x.OrderInDocument }).ToList()
                                    .Aggregate(filterContains,
                    (current, value) => current.Or(e => e.DocumentId == value.DocumentId && e.OrderNumber == value.OrderInDocument).Expand());

                var isNotAllWorkedOut = dbContext.DocumentFilesSet.Where(filterContains)
                            .Where(x => !x.IsDeleted)
                            .GroupBy(x => new { x.DocumentId, x.OrderNumber })
                            .Select(x => new
                            {
                                DocumentId = x.Key.DocumentId,
                                OrderNumber = x.Key.OrderNumber,
                                IsNotAllWorkedOut = x.Any(y => y.IsWorkedOut == false)
                            }).ToList();

                res.ForEach(x => x.IsNotAllWorkedOut = isNotAllWorkedOut.FirstOrDefault(y => y.DocumentId == x.DocumentId && y.OrderNumber == x.OrderInDocument)?.IsNotAllWorkedOut ?? false);
            }

            res.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return res;
        }

        public static IEnumerable<InternalDocumentAttachedFile> GetInternalDocumentFiles(IContext ctx, DmsContext dbContext, int documentId)
        {
            var sq = GetDocumentFileQuery(ctx, dbContext, new FilterDocumentFile { DocumentId = new List<int> { documentId } });

            return
                sq.Select(x => new InternalDocumentAttachedFile
                {
                    Id = x.Id,
                    Date = x.Date,
                    DocumentId = x.DocumentId,
                    Extension = x.Extension,
                    FileContent = x.Content,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                    Type = (EnumFileTypes)x.TypeId,

                    IsMainVersion = x.IsMainVersion,
                    Description = x.Description,
                    IsDeleted = x.IsDeleted,
                    IsWorkedOut = x.IsWorkedOut,

                    Hash = x.Hash,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                    Name = x.Name,
                    OrderInDocument = x.OrderNumber,
                    Version = x.Version,
                    WasChangedExternal = false
                }).ToList();
        }
        #endregion

        #region Waits
        public static IQueryable<DocumentWaits> GetDocumentWaitQuery(IContext ctx, DmsContext dbContext, FilterDocumentWait filter, bool isVerifyAccessLevel = true)
        {
            var qrys = GetDocumentWaitQueryWithoutUnion(ctx, dbContext, filter, isVerifyAccessLevel);
            var res = qrys.First();
            foreach (var qry in qrys.Skip(1).ToList())
            {
                res = res.Concat(qry);
            }
            return res;
        }

        public static List<IQueryable<DocumentWaits>> GetDocumentWaitQueryWithoutUnion(IContext ctx, DmsContext dbContext, FilterDocumentWait filter, bool isVerifyAccessLevel = true)
        {
            var qry = dbContext.DocumentWaitsSet.AsQueryable();

            qry = qry.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);

            //if (!(filter?.IsOpened.HasValue??false) && !(filter?.DocumentId?.Any()??false))
            //{
            //    qry = qry.Where(x => !x.OffEventId.HasValue);
            //}

            if (filter != null)
            {
                if (filter.AllLinkedDocuments && filter.DocumentId?.Count() == 1)
                {
                    filter.DocumentId = GetLinkedDocumentIds(ctx, dbContext, filter.DocumentId.First()).ToList();
                }

                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentWaits>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.OnEventId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentWaits>();
                    filterContains = filter.OnEventId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OnEventId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.OffEventId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentWaits>();
                    filterContains = filter.OffEventId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OffEventId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsOpened.HasValue)
                {
                    if (filter.IsOpened.Value)
                    {
                        qry = qry.Where(x => !x.OffEventId.HasValue);
                    }
                    else
                    {
                        qry = qry.Where(x => x.OffEventId.HasValue);
                    }
                }
                if (filter.IsOverDue.HasValue)
                {
                    if (filter.IsOverDue.Value)
                    {
                        qry = qry.Where(x => x.DueDate.HasValue && x.DueDate.Value > (x.OffEvent != null ? x.OffEvent.Date : DateTime.UtcNow));
                    }
                    else
                    {
                        qry = qry.Where(x => !(x.DueDate.HasValue && x.DueDate.Value > (x.OffEvent != null ? x.OffEvent.Date : DateTime.UtcNow)));
                    }
                }


                if (filter.DueDateFromDate.HasValue)
                {
                    qry = qry.Where(x => x.DueDate >= filter.DueDateFromDate.Value);
                }

                if (filter.DueDateToDate.HasValue)
                {
                    qry = qry.Where(x => x.DueDate <= filter.DueDateToDate.Value);
                }

                if (filter.CreateFromDate.HasValue)
                {
                    qry = qry.Where(x => x.OnEvent.Date >= filter.CreateFromDate.Value);
                }

                if (filter.CreateToDate.HasValue)
                {
                    qry = qry.Where(x => x.OnEvent.Date <= filter.CreateToDate.Value);
                }
                #region Вид контроля
                #region Чужой контроль исполнения  SendForExecution, SendForResponsibleExecution
                if ((filter.ControlToMePositionId?.Count() > 0)
                    || (filter.ControlToMePositionExecutorAgentId?.Count() > 0)
                    || (filter.ControlToMeDepartmentId?.Count() > 0))
                {
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value && e.OnEvent.SourcePositionId != value).Expand());

                        qry = qry.Where(x => !x.OffEventId.HasValue && (x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForExecution || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution))
                                .Where(filterContains);
                    }

                    if (filter.ControlToMePositionId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = filter.ControlToMePositionId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.SourcePositionId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlToMePositionExecutorAgentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = filter.ControlToMePositionExecutorAgentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.SourcePositionExecutorAgentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlToMeDepartmentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = filter.ControlToMeDepartmentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.SourcePosition.DepartmentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                }
                #endregion

                #region Собственный контроль исполнения SendForExecution, SendForResponsibleExecution

                if ((filter.ControlFromMePositionId?.Count() > 0)
                    || (filter.ControlFromMePositionExecutorAgentId?.Count() > 0)
                    || (filter.ControlFromMeDepartmentId?.Count() > 0)
                    || (filter.ControlFromMeAgentId?.Count() > 0))
                {
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPositionId != value && e.OnEvent.SourcePositionId == value).Expand());

                        qry = qry.Where(x => !x.OffEventId.HasValue && (x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForExecution || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution))
                                .Where(filterContains);
                    }

                    if (filter.ControlFromMePositionId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = filter.ControlFromMePositionId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlFromMePositionExecutorAgentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = filter.ControlFromMePositionExecutorAgentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPositionExecutorAgentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlFromMeDepartmentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = filter.ControlFromMeDepartmentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPosition.DepartmentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlFromMeAgentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.False<DocumentWaits>();
                        filterContains = filter.ControlFromMeAgentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetAgentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                }
                #endregion

                #region Самоконтроль ControlOn

                if (filter.IsSelfControl ?? false)
                {
                    var filterContains = PredicateBuilder.False<DocumentWaits>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value && e.OnEvent.SourcePositionId == value).Expand());

                    qry = qry.Where(x => !x.OffEventId.HasValue && x.OnEvent.EventTypeId == (int)EnumEventTypes.ControlOn)
                            .Where(filterContains);
                }
                #endregion

                #region Поступившие на визирование SendForVisaing, SendForАgreement, SendForАpproval, SendForSigning
                if (filter.IsVisaingToMe ?? false)
                {
                    var filterContains = PredicateBuilder.False<DocumentWaits>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value
                                            && e.OnEvent.SourcePositionId != value).Expand());

                    qry = qry.Where(x => !x.OffEventId.HasValue &&
                                        (x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForVisaing
                                        || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForАgreement
                                        || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForАpproval
                                        || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForSigning))
                            .Where(filterContains);
                }
                #endregion

                #region Отправленные на визирование SendForVisaing, SendForАgreement, SendForАpproval, SendForSigning
                if (filter.IsVisaingFromMe ?? false)
                {
                    var filterContains = PredicateBuilder.False<DocumentWaits>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OnEvent.TargetPositionId != value
                                            && e.OnEvent.SourcePositionId == value).Expand());

                    qry = qry.Where(x => !x.OffEventId.HasValue &&
                                        (x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForVisaing
                                        || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForАgreement
                                        || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForАpproval
                                        || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForSigning))
                            .Where(filterContains);
                }
                #endregion

                #region Отчеты о выполнении MarkExecution
                if (filter.IsMarkExecution ?? false)
                {
                    qry = qry.Where(x => !x.OffEventId.HasValue && x.OnEvent.EventTypeId == (int)EnumEventTypes.MarkExecution);
                }
                #endregion Отчеты о выполнении MarkExecution
                #endregion
            }

            //TODO Что то придумать с union

            var res = new List<IQueryable<DocumentWaits>>();

            if (ctx != null && !ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = isVerifyAccessLevel
                    ? ctx.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                    : ctx.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));

                var filterOnEventPositionsContains = PredicateBuilder.False<DocumentWaits>();
                filterOnEventPositionsContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventPositionsContains,
                    (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value || e.OnEvent.SourcePositionId == value).Expand());
                if (filter?.IsMyControl ?? false)
                {
                    res.Add(qry.Where(filterOnEventPositionsContains));
                }
                else
                {
                    var filterOnEventTaskAccessesContains = PredicateBuilder.False<DocumentTaskAccesses>();
                    filterOnEventTaskAccessesContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventTaskAccessesContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());
                    res.Add(qry.Where(x => !x.OnEvent.IsAvailableWithinTask).Where(filterOnEventPositionsContains));
                    res.Add(qry.Where(x => x.OnEvent.IsAvailableWithinTask && x.OnEvent.TaskId.HasValue && x.OnEvent.Task.TaskAccesses.AsQueryable().Any(filterOnEventTaskAccessesContains)));
                }
            }
            else
            {
                res.Add(qry);
            }

            return res;
        }

        public static IEnumerable<FrontDocumentWait> GetDocumentWaits(DmsContext dbContext, FilterBase filter, IContext ctx, UIPaging paging = null)
        {
            var qrys = CommonQueries.GetDocumentWaitQueryWithoutUnion(ctx, dbContext, filter?.Wait);

            if (filter?.Document != null)
            {
                var documentIds = CommonQueries.GetDocumentQuery(ctx, dbContext, filter?.Document, true).Select(x => x.Id);
                qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
            }

            if (filter?.Event != null)
            {
                var eventIds = CommonQueries.GetDocumentEventQuery(ctx, dbContext, filter?.Event).Select(x => x.Id);

                qrys = qrys.Select(qry => { return qry.Where(x => eventIds.Contains(x.OnEventId) || eventIds.Contains(x.OffEventId.Value)); }).ToList();
            }

            if (filter?.File != null)
            {
                var documentIds = CommonQueries.GetDocumentFileQuery(ctx, dbContext, filter?.File).Select(x => x.DocumentId);

                qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
            }

            //TODO Sort
            qrys = qrys.Select(qry => { return qry.OrderBy(x => x.DueDate).AsQueryable(); }).ToList();

            if (paging != null)
            {
                var isDetail = (filter?.Wait?.DocumentId == null) && (paging.IsOnlyCounter ?? false);
                List<FrontDocumentWait> groupsCounter = null;
                if (paging.IsOnlyCounter ?? true)
                {
                    var qryGroupsCounter = qrys.Select(qry => qry.GroupBy(y => new
                    {
                        IsClosed = y.OffEventId.HasValue,
                        IsOverDue = !y.OffEventId.HasValue && y.DueDate.HasValue && y.DueDate.Value <= DateTime.UtcNow,
                        DueDate = isDetail ? DbFunctions.TruncateTime(y.DueDate) : null,
                        SourcePositionExecutorAgentName = isDetail ? y.OnEvent.SourcePositionExecutorAgent.Name + (y.OnEvent.SourcePositionExecutorType.Suffix != null ? " (" + y.OnEvent.SourcePositionExecutorType.Suffix + ")" : (string)null) : null,
                        TargetPositionExecutorAgentName = isDetail ? y.OnEvent.TargetPositionExecutorAgent.Name + (y.OnEvent.TargetPositionExecutorType.Suffix != null ? " (" + y.OnEvent.TargetPositionExecutorType.Suffix + ")" : (string)null) : null,
                    })
                    .Select(y => new { Group = y.Key, RecordCount = y.Count() }).ToList()
                                        ).ToList();
                    groupsCounter = qryGroupsCounter
                                       .SelectMany(z => z)
                                       .GroupBy(z => z.Group)
                                       .Select(z => new FrontDocumentWait
                                       {
                                           IsClosed = z.Key.IsClosed,
                                           IsOverDue = z.Key.IsOverDue,
                                           DueDate = z.Key.DueDate,
                                           SourcePositionExecutorAgentName = z.Key.SourcePositionExecutorAgentName,
                                           TargetPositionExecutorAgentName = z.Key.TargetPositionExecutorAgentName,
                                           RecordCount = z.Sum(c => c.RecordCount)
                                       }).ToList();

                    paging.Counters = new UICounters
                    {
                        //Counter1 = qrys.Sum(qry => qry.Count(y => !y.OffEventId.HasValue)),
                        //Counter2 = qrys.Sum(qry => qry.Count(s => !s.OffEventId.HasValue && s.DueDate.HasValue && s.DueDate.Value < DateTime.UtcNow)),
                        //Counter3 = qrys.Sum(qry => qry.Count()),
                        Counter1 = groupsCounter.Where(y => !y.IsClosed).Sum(y => y.RecordCount),
                        Counter2 = groupsCounter.Where(y => y.IsOverDue).Sum(y => y.RecordCount),
                        Counter3 = groupsCounter.Sum(y => y.RecordCount),
                    };

                    paging.TotalItemsCount = paging.Counters.Counter3.GetValueOrDefault();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return isDetail && groupsCounter != null ? groupsCounter : new List<FrontDocumentWait>();
                }

                var skip = paging.PageSize * (paging.CurrentPage - 1);
                var take = paging.PageSize;

                if (qrys.Count > 1)
                {
                    var take1 = paging.PageSize * (paging.CurrentPage - 1) + paging.PageSize;

                    qrys = qrys.Select(qry => qry.Take(() => take1)).ToList();

                    var qryConcat = qrys.First();

                    foreach (var qry in qrys.Skip(1).ToList())
                    {
                        qryConcat = qryConcat.Concat(qry);
                    }

                    qrys.Clear();
                    qrys.Add(qryConcat);
                }

                //TODO Sort
                qrys = qrys.Select(qry => { return qry.OrderBy(x => x.DueDate).AsQueryable(); }).ToList();

                qrys = qrys.Select(qry => qry.Skip(() => skip).Take(() => take)).ToList();
            }

            IQueryable<DocumentWaits> qryRes = qrys.First();

            if (qrys.Count > 1)
            {
                foreach (var qry in qrys.Skip(1).ToList())
                {
                    qryRes = qryRes.Concat(qry);
                }
            }

            var maxDateTime = DateTime.UtcNow.AddYears(50);
            var isNeedRegistrationFullNumber = !(filter?.Wait?.DocumentId?.Any() ?? false);

            var qryFE = dbContext.DocumentWaitsSet.Where(x => qryRes.Select(y => y.Id).Contains(x.Id))

                //TODO Sort
                .OrderBy(x => x.DueDate)
                .Select(x => new FrontDocumentWait
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    ParentId = x.ParentId,
                    OnEventId = x.OnEventId,
                    OffEventId = x.OffEventId,
                    ResultTypeId = x.ResultTypeId,
                    ResultTypeName = x.ResultType.Name,
                    DueDate = x.DueDate > maxDateTime ? null : x.DueDate,
                    AttentionDate = x.AttentionDate,
                    TargetDescription = x.TargetDescription,
                    //TargetAttentionDate = x.TargetAttentionDate,
                    IsClosed = x.OffEvent != null,

                    DocumentDate = (x.Document.LinkId.HasValue || isNeedRegistrationFullNumber) ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                    RegistrationNumber = x.Document.RegistrationNumber,
                    RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                    RegistrationFullNumber = "#" + x.Document.Id,
                    //DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                    //DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                    //DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,

                    OnEvent = new FrontDocumentEvent
                    {
                        Id = x.OnEvent.Id,
                        DocumentId = x.OnEvent.DocumentId,
                        Task = x.OnEvent.Task.Task,
                        Description = x.OnEvent.Description,
                        AddDescription = x.OnEvent.AddDescription,
                        EventType = x.OnEvent.EventTypeId,
                        EventTypeName = x.OnEvent.EventType.WaitDescription/*?? x.OnEvent.EventType.Name*/,
                        Date = x.OnEvent.Date,
                        TargetPositionId = x.OnEvent.TargetPositionId,
                        SourcePositionId = x.OnEvent.SourcePositionId,
                        SourcePositionExecutorAgentId = x.OnEvent.SourcePositionExecutorAgentId,
                        TargetPositionExecutorAgentId = x.OnEvent.TargetPositionExecutorAgentId,
                        SourcePositionExecutorAgentName = x.OnEvent.SourcePositionExecutorAgent.Name + (x.OnEvent.SourcePositionExecutorType.Suffix != null ? " (" + x.OnEvent.SourcePositionExecutorType.Suffix + ")" : (string)null),
                        TargetPositionExecutorAgentName = x.OnEvent.TargetPositionExecutorAgent.Name + (x.OnEvent.TargetPositionExecutorType.Suffix != null ? " (" + x.OnEvent.TargetPositionExecutorType.Suffix + ")" : (string)null),

                        ReadAgentName = x.OnEvent.ReadAgent.Name,
                        ReadDate = x.OnEvent.ReadDate,
                        SourceAgentId = x.OnEvent.SourceAgentId,
                        SourceAgentName = x.OnEvent.SourceAgent.Name,

                        SourcePositionName = x.OnEvent.SourcePosition.Name,
                        TargetPositionName = x.OnEvent.TargetPosition.Name,
                        SourcePositionExecutorNowAgentName = x.OnEvent.SourcePosition.ExecutorAgent.Name + (x.OnEvent.SourcePosition.ExecutorType.Suffix != null ? " (" + x.OnEvent.SourcePosition.ExecutorType.Suffix + ")" : (string)null),
                        TargetPositionExecutorNowAgentName = x.OnEvent.TargetPosition.ExecutorAgent.Name + (x.OnEvent.TargetPosition.ExecutorType.Suffix != null ? " (" + x.OnEvent.TargetPosition.ExecutorType.Suffix + ")" : (string)null),
                        SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 

                    },
                    OffEvent = !x.OffEventId.HasValue
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.OffEvent.Id,
                        DocumentId = x.OffEvent.DocumentId,
                        Task = null,
                        Description = x.OffEvent.Description,
                        AddDescription = x.OffEvent.AddDescription,
                        EventType = x.OffEvent.EventTypeId,
                        EventTypeName = x.OffEvent.EventType.Name,
                        Date = x.OffEvent.Date,
                        TargetPositionId = x.OffEvent.TargetPositionId,
                        SourcePositionId = x.OffEvent.SourcePositionId,
                        SourcePositionExecutorAgentId = x.OffEvent.SourcePositionExecutorAgentId,
                        TargetPositionExecutorAgentId = x.OffEvent.TargetPositionExecutorAgentId,
                        SourcePositionExecutorAgentName = x.OffEvent.SourcePositionExecutorAgent.Name + (x.OffEvent.SourcePositionExecutorType.Suffix != null ? " (" + x.OffEvent.SourcePositionExecutorType.Suffix + ")" : (string)null),
                        TargetPositionExecutorAgentName = x.OffEvent.TargetPositionExecutorAgent.Name + (x.OffEvent.TargetPositionExecutorType.Suffix != null ? " (" + x.OffEvent.TargetPositionExecutorType.Suffix + ")" : (string)null),

                        ReadAgentName = x.OnEvent.ReadAgent.Name,
                        ReadDate = x.OnEvent.ReadDate,
                        SourceAgentId = x.OffEvent.SourceAgentId,
                        SourceAgentName = x.OffEvent.SourceAgent.Name,

                        //SourcePositionName = null,
                        //TargetPositionName = null,
                        //SourcePositionExecutorNowAgentName = null,
                        //TargetPositionExecutorNowAgentName = null,
                        //SourcePositionExecutorAgentPhoneNumber = null,
                        //TargetPositionExecutorAgentPhoneNumber = null,
                        SourcePositionName = x.OffEvent.SourcePosition.Name,
                        TargetPositionName = x.OffEvent.TargetPosition.Name,
                        SourcePositionExecutorNowAgentName = x.OffEvent.SourcePosition.ExecutorAgent.Name + (x.OffEvent.SourcePosition.ExecutorType.Suffix != null ? " (" + x.OffEvent.SourcePosition.ExecutorType.Suffix + ")" : (string)null),
                        TargetPositionExecutorNowAgentName = x.OffEvent.TargetPosition.ExecutorAgent.Name + (x.OffEvent.TargetPosition.ExecutorType.Suffix != null ? " (" + x.OffEvent.TargetPosition.ExecutorType.Suffix + ")" : (string)null),
                        SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 

                    }
                });

            var res = qryFE.ToList();

            res.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return res;

        }
        #endregion

        #region Accesses
        public static IQueryable<FrontDocumentAccess> GetDocumentAccesses(IContext ctx, DmsContext dbContext, bool isAll = false, bool isAddClientFilter = true, bool isVerifyAccessLevel = false)
        {
            var qry = dbContext.DocumentAccessesSet.AsQueryable();
            if (isAddClientFilter)
            {
                qry = qry.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId);
            }
            if (!isAll && !ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = isVerifyAccessLevel
                    ? ctx.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                    : ctx.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(filterContains);
            }
            return
                qry.Select(acc => new FrontDocumentAccess
                {
                    Id = acc.Id,
                    PositionId = acc.PositionId,
                    IsInWork = acc.IsInWork,
                    DocumentId = acc.DocumentId,
                    IsFavourite = acc.IsFavourite,
                    AccessLevelId = acc.AccessLevelId,
                    AccessLevelName = acc.AccessLevel.Name
                });
        }

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(DmsContext dbContext, IContext ctx, IEnumerable<InternalDocumentAccess> docAccesses, int documentId)
        {
            if (docAccesses == null || !docAccesses.Any()) return null;
            var accPositions = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == documentId).Select(x => x.PositionId);
            return docAccesses.Where(x => !accPositions.Contains(x.PositionId)).Select(ModelConverter.GetDbDocumentAccess);
        }

        public static IQueryable<DocumentAccesses> GetDocumentAccessesesQry(DmsContext dbContext, int documentId, IContext ctx, bool isVerifyAccessLevel = false)
        {
            var qry = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == documentId);
            if (ctx != null && !ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = isVerifyAccessLevel
                    ? ctx.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                    : ctx.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());

                //filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                //    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(filterContains);
            }
            return qry;
        }

        public static IEnumerable<InternalDocumentAccess> GetInternalDocumentAccesses(DmsContext dbContext, IContext ctx, int documentId)
        {
            return dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == documentId).Select(acc => new InternalDocumentAccess
                    {
                        LastChangeDate = acc.LastChangeDate,
                        LastChangeUserId = acc.LastChangeUserId,
                        Id = acc.Id,
                        PositionId = acc.PositionId,
                        IsInWork = acc.IsInWork,
                        DocumentId = acc.DocumentId,
                        IsFavourite = acc.IsFavourite,
                        AccessLevel = (EnumDocumentAccesses)acc.AccessLevelId
                    }).ToList();
        }
        #endregion

        #region Positions
        public static IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(DmsContext dbContext, IContext context, List<int> positionIds)
        {
            var filterContains = PredicateBuilder.False<DictionaryPositions>();
            filterContains = positionIds.Aggregate(filterContains,
                (current, value) => current.Or(e => e.Id == value).Expand());

            return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId)
                .Where(filterContains).Select(x => new InternalPositionInfo
                {
                    PositionId = x.Id,
                    PositionName = x.Name,
                    AgentId = x.ExecutorAgentId ?? 0,
                    AgentName = x.ExecutorAgentId.HasValue ? x.ExecutorAgent.Name : ""
                }).ToList();
        }
        #endregion

        #region Tasks
        public static IEnumerable<FrontDocumentTask> GetDocumentTasks(DmsContext dbContext, IContext ctx, FilterDocumentTask filter, UIPaging paging)
        {
            var tasksDb = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                tasksDb = tasksDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTasks>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    tasksDb = tasksDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTasks>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    tasksDb = tasksDb.Where(filterContains);
                }
            }

            //var tasksRes = tasksDb;

            //var tasksRes = from task in tasksDb

            //               join sl in sendListDb on task.Id equals sl.TaskId into sl
            //               from slAg in sl.DefaultIfEmpty()

            //               join ev in eventDb on task.Id equals ev.TaskId into ev
            //               from evAg in ev.DefaultIfEmpty()
            //               select new
            //               {
            //                   Task = task,
            //                   SendListDb = slAg,
            //                   Event = evAg
            //               };
            tasksDb = tasksDb.OrderByDescending(x => x.LastChangeDate);

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = tasksDb.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentTask>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    tasksDb = tasksDb.Skip(() => skip).Take(() => take);
                }
            }

            if ((paging?.IsAll ?? true) && (filter == null || ((filter.DocumentId?.Count ?? 0) == 0 && (filter.Id?.Count ?? 0) == 0)))
            {
                throw new WrongAPIParameters();
            }

            var tasks = tasksDb.Select(x => new FrontDocumentTask
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                Name = x.Task,
                Description = x.Description,

                DocumentDate = x.Document.RegistrationDate ?? x.Document.CreateDate,
                RegistrationNumber = x.Document.RegistrationNumber,
                RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                RegistrationFullNumber = "#" + x.Document.Id,

                DocumentDescription = x.Document.Description,
                DocumentTypeName = x.Document.TemplateDocument.DocumentType.Name,
                DocumentDirectionName = x.Document.TemplateDocument.DocumentDirection.Name,

                PositionId = x.PositionId,
                PositionExecutorAgentId = x.PositionExecutorAgentId,
                AgentId = x.AgentId,

                PositionExecutorAgentName = x.PositionExecutorAgent.Name + (x.PositionExecutorType.Suffix != null ? " (" + x.PositionExecutorType.Suffix + ")" : null),
                AgentName = x.Agent.Name,
                PositionName = x.Position.Name,
                PositionExecutorNowAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : (string)null),
                PositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 

                //FactResponsibleExecutorPositionName = x.SendListDb.TargetPosition.Name,
                //FactResponsibleExecutorPositionExecutorAgentName = x.SendListDb.TargetPositionExecutorAgent.Name,

                //PlanResponsibleExecutorPositionName = x.Event.TargetPosition.Name,
                //PlanResponsibleExecutorPositionExecutorAgentName = x.Event.TargetPositionExecutorAgent.Name,
            }).ToList();

            {
                var filterContains = PredicateBuilder.False<DocumentSendLists>();
                filterContains = tasks.Select(x => x.Id).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.TaskId == value).Expand());

                var sendLists = dbContext.DocumentSendListsSet.Where(filterContains)
                                    .Where(x => x.SendTypeId == (int)EnumSendTypes.SendForResponsibleExecution)
                                    .Select(x => new
                                    {
                                        TaskId = x.TaskId,
                                        ResponsibleExecutorPositionName = x.TargetPosition.Name,
                                        ResponsibleExecutorPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : null),
                                        IsFactExecutor = x.StartEventId.HasValue,
                                    }).ToList();

                tasks.ForEach(x =>
                {
                    var sendList = sendLists.FirstOrDefault(y => y.TaskId == x.Id);
                    if (sendList != null)
                    {
                        x.IsFactExecutor = sendList.IsFactExecutor;
                        x.ResponsibleExecutorPositionName = sendList.ResponsibleExecutorPositionName;
                        x.ResponsibleExecutorPositionExecutorAgentName = sendList.ResponsibleExecutorPositionExecutorAgentName;
                    }
                });

            }

            tasks.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return tasks;

        }
        #endregion

        #region Subscriptions
        public static IQueryable<DocumentSubscriptions> GetDocumentSubscriptionsQuery(DmsContext dbContext, FilterDocumentSubscription filter, IContext ctx)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                subscriptionsDb = subscriptionsDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter.DocumentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
                if (filter.SubscriptionStates?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = filter.SubscriptionStates.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumSubscriptionStates)e.SubscriptionStateId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
            }

            return subscriptionsDb;

        }
        public static IEnumerable<FrontDocumentSubscription> GetDocumentSubscriptions(DmsContext dbContext, FilterDocumentSubscription filter, IContext ctx, UIPaging paging = null)
        {

            var subscriptionsDb = GetDocumentSubscriptionsQuery(dbContext, filter, ctx);

            var subscriptionsRes = subscriptionsDb
                                    .OrderByDescending(x => x.LastChangeDate)
                                    .AsQueryable();

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = subscriptionsRes.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentSubscription>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    subscriptionsRes = subscriptionsRes
                        .Skip(() => skip).Take(() => take);
                }
            }
            if ((paging?.IsAll ?? true) && (filter == null || (filter.DocumentId?.Count ?? 0) == 0))
            {
                throw new WrongAPIParameters();
            }

            var maxDateTime = DateTime.UtcNow.AddYears(50);

            var subscriptions = subscriptionsRes.Select(x => new FrontDocumentSubscription
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                SendEventId = x.SendEventId,
                DoneEventId = x.DoneEventId,
                SubscriptionStatesId = x.SubscriptionStateId,
                SubscriptionStatesName = x.SubscriptionState.Name,
                IsSuccess = x.SubscriptionState.IsSuccess,
                Description = x.Description,

                DocumentDate = x.Document.LinkId.HasValue ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                RegistrationNumber = x.Document.RegistrationNumber,
                RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                RegistrationFullNumber = "#" + x.Document.Id,
                //DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                //DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                //DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,

                SendEvent = x.SendEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.SendEvent.Id,
                        DocumentId = x.SendEvent.DocumentId,
                        EventType = x.SendEvent.EventTypeId,
                        EventTypeName = x.SendEvent.EventType.Name,
                        DueDate = x.SendEvent.OnWait.FirstOrDefault().DueDate > maxDateTime ? null : x.SendEvent.OnWait.FirstOrDefault().DueDate,
                        Date = x.SendEvent.Date,
                        SourcePositionExecutorAgentId = x.SendEvent.SourcePositionExecutorAgentId,
                        TargetPositionExecutorAgentId = x.SendEvent.TargetPositionExecutorAgentId,
                        SourcePositionExecutorAgentName = x.SendEvent.SourcePositionExecutorAgent.Name + (x.SendEvent.SourcePositionExecutorType.Suffix != null ? " (" + x.SendEvent.SourcePositionExecutorType.Suffix + ")" : null),
                        TargetPositionExecutorAgentName = x.SendEvent.TargetPositionExecutorAgent.Name + (x.SendEvent.TargetPositionExecutorType.Suffix != null ? " (" + x.SendEvent.TargetPositionExecutorType.Suffix + ")" : null),
                        Description = x.SendEvent.Description,
                        AddDescription = x.SendEvent.AddDescription,
                        ReadAgentName = x.SendEvent.ReadAgent.Name,
                        ReadDate = x.SendEvent.ReadDate,
                        SourceAgentId = x.SendEvent.SourceAgentId,
                        SourceAgentName = x.SendEvent.SourceAgent.Name,
                        SourcePositionName = x.SendEvent.SourcePosition.Name,
                        SourcePositionId = x.SendEvent.SourcePositionId,
                        TargetPositionName = x.SendEvent.TargetPosition.Name,
                        TargetPositionId = x.SendEvent.TargetPositionId,
                        SourcePositionExecutorNowAgentName = x.SendEvent.SourcePosition.ExecutorAgent.Name + (x.SendEvent.SourcePosition.ExecutorType.Suffix != null ? " (" + x.SendEvent.SourcePosition.ExecutorType.Suffix + ")" : null),
                        TargetPositionExecutorNowAgentName = x.SendEvent.TargetPosition.ExecutorAgent.Name + (x.SendEvent.TargetPosition.ExecutorType.Suffix != null ? " (" + x.SendEvent.TargetPosition.ExecutorType.Suffix + ")" : null),
                        SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                        TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 

                    },
                DoneEvent = x.DoneEvent == null
                    ? null
                    : new FrontDocumentEvent
                    {
                        Id = x.DoneEvent.Id,
                        DocumentId = x.DoneEvent.DocumentId,
                        EventType = x.DoneEvent.EventTypeId,
                        EventTypeName = x.DoneEvent.EventType.Name,
                        DueDate = null,
                        Date = x.DoneEvent.Date,
                        SourcePositionExecutorAgentId = x.DoneEvent.SourcePositionExecutorAgentId,
                        TargetPositionExecutorAgentId = x.DoneEvent.TargetPositionExecutorAgentId,
                        SourcePositionExecutorAgentName = x.DoneEvent.SourcePositionExecutorAgent.Name + (x.DoneEvent.SourcePositionExecutorType.Suffix != null ? " (" + x.DoneEvent.SourcePositionExecutorType.Suffix + ")" : null),
                        TargetPositionExecutorAgentName = x.DoneEvent.TargetPositionExecutorAgent.Name + (x.DoneEvent.TargetPositionExecutorType.Suffix != null ? " (" + x.DoneEvent.TargetPositionExecutorType.Suffix + ")" : null),
                        Description = x.DoneEvent.Description,
                        AddDescription = x.DoneEvent.AddDescription,

                        ReadAgentName = x.SendEvent.ReadAgent.Name,
                        ReadDate = x.SendEvent.ReadDate,
                        SourceAgentId = x.DoneEvent.SourceAgentId,
                        SourceAgentName = x.DoneEvent.SourceAgent.Name,
                        //TODO Фронт очен хочет поля SourcePositionId, TargetPositionId
                        SourcePositionName = null,
                        SourcePositionId = x.DoneEvent.SourcePositionId,
                        TargetPositionName = null,
                        TargetPositionId = x.DoneEvent.TargetPositionId,
                        SourcePositionExecutorNowAgentName = x.DoneEvent.SourcePosition.ExecutorAgent.Name + (x.DoneEvent.SourcePosition.ExecutorType.Suffix != null ? " (" + x.DoneEvent.SourcePosition.ExecutorType.Suffix + ")" : null),
                        TargetPositionExecutorNowAgentName = x.DoneEvent.TargetPosition.ExecutorAgent.Name + (x.DoneEvent.TargetPosition.ExecutorType.Suffix != null ? " (" + x.DoneEvent.TargetPosition.ExecutorType.Suffix + ")" : null),
                        SourcePositionExecutorAgentPhoneNumber = null,
                        TargetPositionExecutorAgentPhoneNumber = null,
                    },

                SigningType = (EnumSigningTypes)x.SigningTypeId,
                CertificateId = x.CertificateId,
                CertificateName = x.Certificate.Name,
                CertificatePositionId = x.CertificatePositionId,
                CertificatePositionExecutorAgentId = x.CertificatePositionExecutorAgentId,
                CertificatePositionName = x.CertificatePosition.Name,
                CertificatePositionExecutorAgentName = x.CertificatePositionExecutorAgent.Name + (x.CertificatePositionExecutorType.Suffix != null ? " (" + x.CertificatePositionExecutorType.Suffix + ")" : null),
                CertificateSignCreateDate = x.CertificateSignCreateDate,


            }).ToList();

            subscriptions.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return subscriptions;

        }
        public static IEnumerable<InternalDocumentSubscription> GetInternalDocumentSubscriptions(DmsContext dbContext, IContext context, FilterDocumentSubscription filter)
        {
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId.Any())
                {
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
                if (filter.SubscriptionStates?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSubscriptions>();
                    filterContains = filter.SubscriptionStates.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumSubscriptionStates)e.SubscriptionStateId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
            }

            var subscriptions = subscriptionsDb.Select(x => new InternalDocumentSubscription
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                SendEventId = x.SendEventId,
                SubscriptionStates = (EnumSubscriptionStates)x.SubscriptionStateId,
                Hash = x.Hash,
                FullHash = x.FullHash,
                SigningType = (EnumSigningTypes)x.SigningTypeId,
                CertificateId = x.CertificateId,
                CertificateSign = x.CertificateSign,
                InternalSign = x.InternalSign,

            }).ToList();

            return subscriptions;
        }
        #endregion

        #region Tags
        public static IEnumerable<FrontDocumentTag> GetDocumentTags(DmsContext dbContext, IContext context, FilterDocumentTag filter)
        {
            var tagsDb = dbContext.DocumentTagsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTags>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    tagsDb = tagsDb.Where(filterContains);
                }

                if (filter.CurrentPositionsId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentTags>();
                    filterContains = filter.CurrentPositionsId.Aggregate(filterContains,
                        (current, value) => current.Or(e => !e.Tag.PositionId.HasValue || e.Tag.PositionId == value).Expand());

                    tagsDb = tagsDb.Where(filterContains);
                }
            }

            var tagsRes = tagsDb;

            var tags = tagsRes.Select(x => new FrontDocumentTag
            {
                TagId = x.TagId,
                DocumentId = x.DocumentId,
                PositionId = x.Tag.PositionId,
                PositionName = x.Tag.Position.Name,
                Color = x.Tag.Color,
                Name = x.Tag.Name,
                IsSystem = !x.Tag.PositionId.HasValue
            }).ToList();

            return tags;

        }
        #endregion

        #region PropertyValues
        public static IQueryable<PropertyValues> GetPropertyValuesQuery(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = dbContext.PropertyValuesSet.Where(x => x.PropertyLink.Property.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.Object?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<PropertyValues>();
                    filterContains = filter.Object.Select(x => (int)x).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PropertyLink.ObjectId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }

                if (filter.RecordId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<PropertyValues>();
                    filterContains = filter.RecordId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RecordId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
            }

            return itemsDb;

        }

        public static IEnumerable<FrontPropertyValue> GetPropertyValues(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = GetPropertyValuesQuery(dbContext, ctx, filter);

            var itemsRes = itemsDb
                .Select(x => new
                {
                    Id = x.Id,
                    PropertyLinkId = x.PropertyLinkId,
                    ValueString = x.ValueString,
                    ValueNumeric = x.ValueNumeric,
                    ValueDate = x.ValueDate,
                    PropertyCode = x.PropertyLink.Property.Code,
                    PropertyLabel = x.PropertyLink.Property.Label,
                    PropertyValueTypeCode = x.PropertyLink.Property.ValueType.Code,
                    DisplayValue = string.Empty,
                    SelectAPI = x.PropertyLink.Property.SelectAPI,
                    SelectFilter = x.PropertyLink.Property.SelectFilter,
                    SelectDescriptionFieldCode = x.PropertyLink.Property.SelectDescriptionFieldCode,
                    SelectTable = x.PropertyLink.Property.SelectTable,
                }).ToList();

            var items = new List<FrontPropertyValue>();

            foreach (var itemRes in itemsRes)
            {
                var item = new FrontPropertyValue
                {
                    PropertyLinkId = itemRes.PropertyLinkId,
                    //Value = itemRes.Value,
                    PropertyCode = itemRes.PropertyCode,
                    PropertyLabel = itemRes.PropertyLabel,
                    PropertyValueTypeCode = itemRes.PropertyValueTypeCode,
                };

                if (itemRes.ValueString != null)
                {
                    item.Value = itemRes.ValueString;
                }
                else if (itemRes.ValueNumeric.HasValue)
                {
                    item.Value = itemRes.ValueNumeric;
                }
                else if (itemRes.ValueDate.HasValue)
                {
                    item.Value = DateTime.SpecifyKind(itemRes.ValueDate.Value, DateTimeKind.Utc);
                }
                else
                {
                    item.Value = null;
                }

                if (string.IsNullOrEmpty(itemRes.SelectAPI))
                {
                    item.DisplayValue = item.Value;
                }
                else
                {
                    try
                    {
                        Type entityType = Type.GetType(itemRes.SelectTable);

                        var values = dbContext.Set(entityType);
                        int key = 0;
                        int.TryParse(item.Value.ToString(), out key);
                        var value = key > 0 ? values.Find(key) : values.Find(item.Value);

                        item.DisplayValue = (string)value.GetType().GetProperty(itemRes.SelectDescriptionFieldCode).GetValue(value, null);
                    }
                    catch
                    {
                        item.DisplayValue = item.Value;
                    }
                }
                items.Add(item);
            }

            return items;

        }

        public static IEnumerable<InternalPropertyValue> GetInternalPropertyValues(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = GetPropertyValuesQuery(dbContext, ctx, filter);

            var items = itemsDb.Select(x => new InternalPropertyValue
            {
                Id = x.Id,
                PropertyLinkId = x.PropertyLinkId,
                RecordId = x.RecordId,
                ValueString = x.ValueString,
                ValueDate = x.ValueDate,
                ValueNumeric = x.ValueNumeric,
                PropertyLink = new InternalPropertyLink
                {
                    Id = x.PropertyLink.Id,
                    PropertyId = x.PropertyLink.PropertyId,
                    Object = (EnumObjects)x.PropertyLink.ObjectId,
                    Filers = x.PropertyLink.Filers,
                    IsMandatory = x.PropertyLink.IsMandatory
                }
            }).ToList();

            return items;

        }

        public static void DeletePropertyValues(DmsContext dbContext, IContext ctx, FilterPropertyValue filter)
        {
            var itemsDb = GetPropertyValuesQuery(dbContext, ctx, filter);

            dbContext.PropertyValuesSet.RemoveRange(itemsDb);
        }
        public static void ModifyPropertyValues(DmsContext dbContext, IContext ctx, InternalPropertyValues model)
        {
            var propertyValues = dbContext.PropertyValuesSet
                .Where(x => x.PropertyLink.Property.ClientId == ctx.CurrentClientId)
                .Where(x => x.PropertyLink.ObjectId == (int)model.Object && x.RecordId == model.RecordId)
                .Select(x => new { x.Id, x.PropertyLinkId }).ToList();

            var groupJoinItems = propertyValues
               .GroupJoin(model.PropertyValues,
                   x => x.PropertyLinkId,
                   y => y.PropertyLinkId,
                   (x, y) => new { propertyValueId = x.Id, values = y })
               .ToList();

            #region modify
            var modifyItems = groupJoinItems
                .Where(x => x.values.Count() > 0)
                .Select(x => new { x.propertyValueId, value = x.values.First() })
                .Select(x => new PropertyValues
                {
                    Id = x.propertyValueId,
                    ValueString = x.value.ValueString,
                    ValueDate = x.value.ValueDate,
                    ValueNumeric = x.value.ValueNumeric,
                    LastChangeDate = x.value.LastChangeDate,
                    LastChangeUserId = x.value.LastChangeUserId,
                });

            foreach (var item in modifyItems)
            {
                dbContext.PropertyValuesSet.Attach(item);
                var entry = dbContext.Entry(item);
                entry.Property(x => x.ValueString).IsModified = true;
                entry.Property(x => x.ValueDate).IsModified = true;
                entry.Property(x => x.ValueNumeric).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
            }
            #endregion

            #region add
            var newItems = model.PropertyValues
                .Where(x => !propertyValues.Select(y => y.PropertyLinkId).Contains(x.PropertyLinkId))
                .Select(x => new PropertyValues
                {
                    PropertyLinkId = x.PropertyLinkId,
                    RecordId = model.RecordId,
                    ValueString = x.ValueString,
                    ValueDate = x.ValueDate,
                    ValueNumeric = x.ValueNumeric,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

            dbContext.PropertyValuesSet.AddRange(newItems);
            #endregion

            #region delete
            foreach (var item in groupJoinItems.Where(x => x.values.Count() == 0).Select(x => x.propertyValueId))
            {
                var itemAtt = dbContext.PropertyValuesSet.Attach(new PropertyValues { Id = item });
                dbContext.Entry(itemAtt).State = System.Data.Entity.EntityState.Deleted;
            }
            #endregion
        }
        #endregion

        #region WorkGroups
        public static IEnumerable<FrontDictionaryPosition> GetDocumentWorkGroup(DmsContext dbContext, IContext ctx, FilterDictionaryPosition filter)
        {
            var qry = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentAccesses>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentAccesses>();
                    filterContains = filter.DocumentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry.Select(x => new FrontDictionaryPosition
            {
                Id = x.PositionId,
                Name = x.Position.Name,
                DepartmentId = x.Position.DepartmentId,
                ExecutorAgentId = x.Position.ExecutorAgentId,
                DepartmentName = x.Position.Department.Name,
                ExecutorAgentName = x.Position.ExecutorAgent.Name + (x.Position.ExecutorType.Suffix != null ? " (" + x.Position.ExecutorType.Suffix + ")" : (string)null),
                PositionPhone = "Здесь нужно отображать основной номер или все контакты"// x.Position.ExecutorAgent.AgentContacts.Where(y => y.IsActive).Where(y => y.ContactType.SpecCode == EnumContactTypes.MainPhone.ToString()).ToString()
            }).Distinct().ToList();

        }
        #endregion

        #region LinkedDocuments
        public static IEnumerable<FrontDocument> GetLinkedDocuments(IContext context, DmsContext dbContext, int linkId)
        {
            //var acc = CommonQueries.GetDocumentAccesses(context, dbContext, true);

            var items = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.LinkId == linkId)
                        .OrderBy(x => x.RegistrationDate ?? x.CreateDate)
                        .Select(y => new FrontDocument
                        {
                            Id = y.Id,
                            DocumentDirectionName = y.TemplateDocument.DocumentDirection.Name,
                            DocumentTypeName = y.TemplateDocument.DocumentType.Name,

                            RegistrationNumber = y.RegistrationNumber,
                            RegistrationNumberPrefix = y.RegistrationNumberPrefix,
                            RegistrationNumberSuffix = y.RegistrationNumberSuffix,

                            DocumentDate = y.RegistrationDate ?? y.CreateDate,
                            IsRegistered = y.IsRegistered,
                            Description = y.Description,
                            ExecutorPositionId = y.ExecutorPositionId,
                            ExecutorPositionName = y.ExecutorPosition.Name,
                            ExecutorPositionExecutorAgentId = y.ExecutorPositionExecutorAgentId,
                            ExecutorPositionExecutorAgentName = y.ExecutorPositionExecutorAgent.Name + (y.ExecutorPositionExecutorType.Suffix != null ? " (" + y.ExecutorPositionExecutorType.Suffix + ")" : null),
                            Links = y.LinksDocuments.OrderBy(z => z.LastChangeDate)
                                    .Select(z => new FrontDocumentLink
                                    {
                                        Id = z.Id,
                                        DocumentId = z.DocumentId,
                                        ParentDocumentId = z.ParentDocumentId,
                                        LinkTypeName = z.LinkType.Name,
                                        IsParent = true,
                                        RegistrationNumber = z.ParentDocument.RegistrationNumber,
                                        RegistrationNumberPrefix = z.ParentDocument.RegistrationNumberPrefix,
                                        RegistrationNumberSuffix = z.ParentDocument.RegistrationNumberSuffix,
                                        RegistrationFullNumber = "#" + z.ParentDocument.Id.ToString(),
                                        DocumentDate = (z.ParentDocument.RegistrationDate ?? z.ParentDocument.CreateDate),
                                        ExecutorPositionId = z.ExecutorPositionId,
                                        ExecutorPositionName = z.ExecutorPosition.Name,
                                        ExecutorPositionExecutorAgentId = z.ExecutorPositionExecutorAgentId,
                                        ExecutorPositionExecutorAgentName = z.ExecutorPositionExecutorAgent.Name + (z.ExecutorPositionExecutorType.Suffix != null ? " (" + z.ExecutorPositionExecutorType.Suffix + ")" : null),
                                    }).Concat
                                    (y.LinksParentDocuments.OrderBy(z => z.LastChangeDate)
                                    .Select(z => new FrontDocumentLink
                                    {
                                        Id = z.Id,
                                        DocumentId = z.DocumentId,
                                        ParentDocumentId = z.ParentDocumentId,
                                        LinkTypeName = z.LinkType.Name,
                                        IsParent = false,
                                        RegistrationNumber = z.Document.RegistrationNumber,
                                        RegistrationNumberPrefix = z.Document.RegistrationNumberPrefix,
                                        RegistrationNumberSuffix = z.Document.RegistrationNumberSuffix,
                                        RegistrationFullNumber = "#" + z.Document.Id.ToString(),
                                        DocumentDate = (z.Document.RegistrationDate ?? z.Document.CreateDate),
                                        ExecutorPositionId = z.ExecutorPositionId,
                                        ExecutorPositionName = z.ExecutorPosition.Name,
                                        ExecutorPositionExecutorAgentId = z.ExecutorPositionExecutorAgentId,
                                        ExecutorPositionExecutorAgentName = z.ExecutorPositionExecutorAgent.Name + (z.ExecutorPositionExecutorType.Suffix != null ? " (" + z.ExecutorPositionExecutorType.Suffix + ")" : null),
                                    })),
                        }).ToList();
            items.ForEach(x =>
            {
                CommonQueries.ChangeRegistrationFullNumber(x);
                var links = x.Links.ToList();
                links.ForEach(y => 
                {
                    CommonQueries.ChangeRegistrationFullNumber(y);
                    y.CanDelete = context.CurrentPositionsIdList.Contains(y.ExecutorPositionId??(int)EnumSystemPositions.AdminPosition);
                });
                x.Links = links;
                x.DocumentWorkGroup = CommonQueries.GetDocumentWorkGroup(dbContext, context, new FilterDictionaryPosition { DocumentIDs = new List<int> { x.Id } });
                //TODO x.Accesses = acc.Where(y => y.DocumentId == x.Id).ToList();
            });
            return items;
        }
        public static IEnumerable<int> GetLinkedDocumentIds(IContext context, DmsContext dbContext, int documentId, bool isVerifyAccesses = true)
        {
            var qry = isVerifyAccesses ? CommonQueries.GetDocumentQuery(dbContext, context) : dbContext.DocumentsSet.AsQueryable();
            var docLinkId = qry.Where(y => y.LinkId.HasValue && y.Id == documentId).Select(y => y.LinkId).AsQueryable();
            var docIds = CommonQueries.GetDocumentQuery(dbContext, context).Where(x => x.LinkId.HasValue && docLinkId.Contains(x.LinkId)).Select(x => x.Id).ToList();
            if (!docIds.Any())
                docIds = new List<int> { documentId };
            return docIds;
        }
        #endregion

        #region SendLists
        private static IQueryable<DocumentSendLists> GetDocumentSendListQuery(DmsContext dbContext, IContext context, FilterDocumentSendList filter)
        {
            var sendListDb = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }

            }

            return sendListDb;
        }

        public static IEnumerable<FrontDocumentSendList> GetDocumentSendList(DmsContext dbContext, IContext context, FilterDocumentSendList filter)
        {
            var sendListDb = GetDocumentSendListQuery(dbContext, context, filter);

            var res = sendListDb.Select(y => new FrontDocumentSendList
            {
                Id = y.Id,
                DocumentId = y.DocumentId,
                Stage = y.Stage,
                SendType = (EnumSendTypes)y.SendTypeId,
                SendTypeName = y.SendType.Name,
                SendTypeCode = y.SendType.Code,
                StageType = (EnumStageTypes?)y.StageTypeId,
                StageTypeName = y.StageType.Name,
                StageTypeCode = y.StageType.Code,
                SendTypeIsImportant = y.SendType.IsImportant,
                SourcePositionExecutorAgentId = y.SourcePosition.ExecutorAgentId,
                TargetPositionExecutorAgentId = y.TargetPosition.ExecutorAgentId,
                SourcePositionExecutorAgentName = y.SourcePosition.ExecutorAgent.Name + (y.SourcePosition.ExecutorType.Suffix != null ? " (" + y.SourcePosition.ExecutorType.Suffix + ")" : (string)null),
                TargetPositionExecutorAgentName = (y.TargetPosition.ExecutorAgent.Name + (y.TargetPosition.ExecutorType.Suffix != null ? " (" + y.TargetPosition.ExecutorType.Suffix + ")" : (string)null))
                                                ?? y.TargetAgent.Name,
                Task = y.Task.Task,
                IsAvailableWithinTask = y.IsAvailableWithinTask,
                IsWorkGroup = y.IsWorkGroup,
                IsAddControl = y.IsAddControl,
                SelfDescription = y.SelfDescription,
                SelfDueDate = y.SelfDueDate,
                SelfDueDay = y.SelfDueDay,
                SelfAttentionDate = y.SelfAttentionDate,
                SelfAttentionDay = y.SelfAttentionDay,
                Description = y.Description,
                AddDescription = y.AddDescription,
                DueDate = y.DueDate,
                DueDay = y.DueDay,
                StartEventId = y.StartEventId,
                CloseEventId = y.CloseEventId,
                IsInitial = y.IsInitial,

                SourceAgentName = y.SourceAgent.Name,

                SourceAgentId = y.SourceAgentId,
                SourcePositionId = y.SourcePositionId,

                TargetAgentId = y.TargetAgentId,
                TargetPositionId = y.TargetPositionId,

                SourcePositionName = y.SourcePosition.Name,
                TargetPositionName = y.TargetPosition.Name,
                SourcePositionExecutorNowAgentName = y.SourcePosition.ExecutorAgent.Name + (y.SourcePosition.ExecutorType.Suffix != null ? " (" + y.SourcePosition.ExecutorType.Suffix + ")" : (string)null),
                TargetPositionExecutorNowAgentName = y.TargetPosition.ExecutorAgent.Name + (y.TargetPosition.ExecutorType.Suffix != null ? " (" + y.TargetPosition.ExecutorType.Suffix + ")" : (string)null),
                SourcePositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                TargetPositionExecutorAgentPhoneNumber = "(888)888-88-88", //TODO 
                AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                AccessLevelName = y.AccessLevel.Name,
                StartEvent = y.StartEvent == null
                                        ? null
                                        : new FrontDocumentEvent
                                        {
                                            Id = y.StartEvent.Id,
                                            EventType = y.StartEvent.EventTypeId,
                                            EventTypeName = y.StartEvent.EventType.Name,
                                            Date = y.StartEvent.Date,
                                            SourcePositionExecutorAgentId = null,
                                            TargetPositionExecutorAgentId = null,
                                            SourcePositionExecutorAgentName = y.StartEvent.SourcePositionExecutorAgent.Name + (y.StartEvent.SourcePositionExecutorType.Suffix != null ? " (" + y.StartEvent.SourcePositionExecutorType.Suffix + ")" : null),
                                            TargetPositionExecutorAgentName = (y.StartEvent.TargetPositionExecutorAgent.Name + (y.StartEvent.TargetPositionExecutorType.Suffix != null ? " (" + y.StartEvent.TargetPositionExecutorType.Suffix + ")" : null))
                                                                                ?? y.StartEvent.TargetAgent.Name,
                                            Description = y.StartEvent.Description,
                                            AddDescription = y.StartEvent.AddDescription,
                                            DueDate = y.StartEvent.OnWait.Select(z => z.DueDate).FirstOrDefault(),
                                        },
                CloseEvent = y.CloseEvent == null || y.StartEventId == y.CloseEventId
                                        ? null
                                        : new FrontDocumentEvent
                                        {
                                            Id = y.CloseEvent.Id,
                                            EventType = y.CloseEvent.EventTypeId,
                                            EventTypeName = y.CloseEvent.EventType.Name,
                                            Date = y.CloseEvent.Date,
                                            SourcePositionExecutorAgentId = y.CloseEvent.SourcePositionExecutorAgentId,
                                            TargetPositionExecutorAgentId = y.CloseEvent.TargetPositionExecutorAgentId,
                                            SourcePositionExecutorAgentName = y.CloseEvent.SourcePositionExecutorAgent.Name + (y.CloseEvent.SourcePositionExecutorType.Suffix != null ? " (" + y.CloseEvent.SourcePositionExecutorType.Suffix + ")" : null),
                                            TargetPositionExecutorAgentName = (y.CloseEvent.TargetPositionExecutorAgent.Name + (y.CloseEvent.TargetPositionExecutorType.Suffix != null ? " (" + y.CloseEvent.TargetPositionExecutorType.Suffix + ")" : null))
                                                                                ?? y.StartEvent.TargetAgent.Name,
                                            Description = y.CloseEvent.Description,
                                            AddDescription = y.CloseEvent.AddDescription,
                                            DueDate = null,
                                        },
            }).ToList();
            return res;
        }
        public static IEnumerable<InternalDocumentSendList> GetInternalDocumentSendList(DmsContext dbContext, IContext context, FilterDocumentSendList filter)
        {
            var sendListDb = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }

            }

            return sendListDb.Select(y => new InternalDocumentSendList
            {
                Id = y.Id,
                TargetAgentId = y.TargetAgentId,
                TargetPositionId = y.TargetPositionId,
                SendType = (EnumSendTypes)y.SendTypeId,
                StageType = (EnumStageTypes?)y.StageTypeId,
                Description = y.Description,
                DueDate = y.DueDate,
                DueDay = y.DueDay,
            }).ToList();
        }
        #endregion

        #region RestrictedSendLists
        public static IEnumerable<FrontDocumentRestrictedSendList> GetDocumentRestrictedSendList(DmsContext dbContext, IContext ctx, FilterDocumentRestrictedSendList filter)
        {
            var sendListDb = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentRestrictedSendLists>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentRestrictedSendLists>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }

            }
            return sendListDb.Select(y => new FrontDocumentRestrictedSendList
            {
                Id = y.Id,
                DocumentId = y.DocumentId,
                PositionId = y.PositionId,
                PositionName = y.Position.Name,
                PositionExecutorAgentName = y.Position.ExecutorAgent.Name + (y.Position.ExecutorType.Suffix != null ? " (" + y.Position.ExecutorType.Suffix + ")" : null),
                PositionExecutorAgentPhoneNumber = "(888)888-88-88",
                AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                AccessLevelName = y.AccessLevel.Name,
                DepartmentName = y.Position.Department.Name,
            }).ToList();
        }
        #endregion

        #region FullText
        public static void AddFullTextCashInfo(DmsContext dbContext, int objectId, EnumObjects objType, EnumOperationType operationType)
        {
            var cashInfo = new FullTextIndexCash
            {
                ObjectId = objectId,
                ObjectType = (int)objType,
                OperationType = (int)operationType
            };


            dbContext.FullTextIndexCashSet.Add(cashInfo);

            dbContext.SaveChanges();

        }

        public static void AddFullTextCashInfo(DmsContext dbContext, List<int> objectId, EnumObjects objType, EnumOperationType operationType)
        {
            if (objectId == null || !objectId.Any()) return;
            var cashInfos = objectId.Select(x =>
                new FullTextIndexCash
                {
                    ObjectId = x,
                    ObjectType = (int)objType,
                    OperationType = (int)operationType
                }).ToList();

            dbContext.FullTextIndexCashSet.AddRange(cashInfos);

            dbContext.SaveChanges();
        }
        #endregion

        #region Papers
        public static IEnumerable<FrontDocumentPaper> GetDocumentPapers(DmsContext dbContext, IContext ctx, FilterDocumentPaper filter, UIPaging paging)
        {
            var itemsDb = dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

            if (!ctx.IsAdmin)
            {
                var filterContains = PredicateBuilder.False<DocumentAccesses>();
                filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                itemsDb = itemsDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentPapers>();
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentPapers>();
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
            }

            itemsDb = itemsDb.OrderByDescending(x => x.LastChangeDate);

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = itemsDb.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    return new List<FrontDocumentPaper>();
                }


                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    itemsDb = itemsDb
                        .Skip(() => skip).Take(() => take);
                }
            }
            if ((paging?.IsAll ?? true) && (filter == null || ((filter.DocumentId?.Count ?? 0) == 0 && (filter.Id?.Count ?? 0) == 0)))
            {
                throw new WrongAPIParameters();
            }
            //var itemsRes = itemsDb.Select(x => x);

            var items = itemsDb.Select(x => new FrontDocumentPaper
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
                LastPaperEventId = x.LastPaperEventId,
                IsInWork = x.IsInWork,

                DocumentDate = x.Document.LinkId.HasValue ? x.Document.RegistrationDate ?? x.Document.CreateDate : (DateTime?)null,
                RegistrationNumber = x.Document.RegistrationNumber,
                RegistrationNumberPrefix = x.Document.RegistrationNumberPrefix,
                RegistrationNumberSuffix = x.Document.RegistrationNumberSuffix,
                RegistrationFullNumber = "#" + x.Document.Id,
                //DocumentDescription = x.Document.LinkId.HasValue ? x.Document.Description : null,
                //DocumentTypeName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentType.Name : null,
                //DocumentDirectionName = x.Document.LinkId.HasValue ? x.Document.TemplateDocument.DocumentDirection.Name : null,

                OwnerAgentName = x.LastPaperEvent.TargetAgent.Name,
                OwnerPositionExecutorAgentName = x.LastPaperEvent.TargetPositionExecutorAgent.Name + (x.LastPaperEvent.TargetPositionExecutorType.Suffix != null ? " (" + x.LastPaperEvent.TargetPositionExecutorType.Suffix + ")" : null),
                OwnerPositionName = x.LastPaperEvent.TargetPosition.Name,
                OwnerPositionExecutorNowAgentName = x.LastPaperEvent.TargetPosition.ExecutorAgent.Name + (x.LastPaperEvent.TargetPosition.ExecutorType.Suffix != null ? " (" + x.LastPaperEvent.TargetPosition.ExecutorType.Suffix + ")" : null),
                OwnerPositionExecutorAgentPhoneNumber = "(888)888-88-88",
                PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                PaperSendDate = x.LastPaperEvent.PaperSendDate,
                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,


            }).ToList();

            items.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

            return items;
        }
        #endregion

        #region PaperLists
        public static IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(DmsContext dbContext, IContext ctx, FilterDocumentPaperList filter)
        {
            var itemsDb = dbContext.DocumentPaperListsSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.PaperListId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentPaperLists>();
                    filterContains = filter.PaperListId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
            }

            var itemsRes = itemsDb.Select(x => x);

            var items = itemsRes.Select(x => new FrontDocumentPaperList
            {
                Id = x.Id,
                Date = x.Date,
                Description = x.Description
            }).ToList();

            return items;
        }
        #endregion

        #region RegistrationFullNumber
        public static void ChangeRegistrationFullNumber(FrontDocument item, bool isClearFields = true)
        {
            if (item.RegistrationNumber != null)
            {
                item.RegistrationFullNumber = (item.RegistrationNumberPrefix ?? "") + item.RegistrationNumber + (item.RegistrationNumberSuffix ?? "");
            }
            else
            {
                item.RegistrationFullNumber = "#" + item.Id;
            }

            if (isClearFields)
            {
                item.RegistrationNumber = null;
                item.RegistrationNumberPrefix = null;
                item.RegistrationNumberSuffix = null;
            }
        }

        public static string GetRegistrationFullNumber(InternalDocument item)
        {
            string res = null;
            try
            {
                if ((item.IsRegistered.HasValue && item.IsRegistered.Value) || item.RegistrationNumber.HasValue)
                    res = (item.RegistrationNumberPrefix ?? "") + item.RegistrationNumber + (item.RegistrationNumberSuffix ?? "");
            }
            catch { }
            return res;
        }

        public static void ChangeRegistrationFullNumber(FrontRegistrationFullNumber item, bool isClearFields = true)
        {
            if (item.DocumentDate.HasValue)
            {
                if (item.RegistrationNumber.HasValue)
                {
                    item.RegistrationFullNumber = (item.RegistrationNumberPrefix ?? "") + item.RegistrationNumber +
                                                  (item.RegistrationNumberSuffix ?? "");
                }
            }
            else
            {
                item.RegistrationFullNumber = null;
            }
            if (isClearFields)
            {
                item.RegistrationNumber = null;
                item.RegistrationNumberPrefix = null;
                item.RegistrationNumberSuffix = null;
            }
        }
        #endregion

        #region Hash
        public static InternalDocument GetDocumentHash(DmsContext dbContext, IContext ctx, int documentId, bool isUseInternalSign, bool isUseCertificateSign, InternalDocumentSubscription newSubscription, bool isAddSubscription = false, bool isFull = false, bool isContinueIfEmptySubscriptions = false)
        {
            var subscriptionStates = new List<EnumSubscriptionStates> {
                        EnumSubscriptionStates.Sign,
                        EnumSubscriptionStates.Visa,
                        EnumSubscriptionStates.Аgreement,
                        EnumSubscriptionStates.Аpproval
                        };

            List<InternalDocumentSubscription> subscriptions = CommonQueries.GetInternalDocumentSubscriptions(dbContext, ctx,
                new FilterDocumentSubscription
                {
                    DocumentId = new List<int> { documentId },
                    SubscriptionStates = subscriptionStates
                }).ToList();

            if (!isAddSubscription)
            {
                if (!subscriptions.Any() && !isContinueIfEmptySubscriptions)
                    return null;
            }

            InternalDocument document = CommonQueries.GetDocumentDigitalSignaturePrepare(dbContext, ctx, documentId, subscriptionStates);

            subscriptions = document.Subscriptions.ToList();

            var IsFilesIncorrect = false;

            if (isFull || isAddSubscription)
            {
                var fs = DmsResolver.Current.Get<IFileStore>();
                foreach (var file in document.DocumentFiles)
                {
                    if (!fs.IsFileCorrect(ctx, file))
                    {
                        //TODO
                        IsFilesIncorrect = true;
                        //throw new DocumentFileWasChangedExternally();
                    }
                }
            }

            document.Hash = CommonQueries.GetDocumentHash(document);

            if (isFull || isAddSubscription)
            {
                document.FullHash = CommonQueries.GetDocumentHash(document, true);
            }

            switch (newSubscription?.SigningType)
            {
                case EnumSigningTypes.InternalSign:
                    if (isAddSubscription)
                    {
                        if (isUseInternalSign)
                        {
                            document.InternalSign = CommonQueries.GetDocumentInternalSign(document);
                        }
                        else
                            throw new SigningTypeNotAllowed();
                    }
                    break;
                case EnumSigningTypes.CertificateSign:
                    if (isAddSubscription)
                    {
                        if (isUseCertificateSign && (newSubscription?.CertificateId).HasValue)
                        {
                            document.CertificateSign = CommonQueries.GetDocumentCertificateSign(ctx, document, newSubscription.CertificateId.Value, newSubscription.CertificatePassword);
                        }
                        else
                            throw new SigningTypeNotAllowed();
                    }
                    break;
            }

            bool IsVioalted = IsFilesIncorrect;

            if (subscriptions.Any())
            {
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                foreach (var subscription in subscriptions)
                {
                    if (IsFilesIncorrect ||
                        (!VerifyDocumentHash(subscription.Hash, document) || ((isFull || isAddSubscription) && !VerifyDocumentHash(subscription.FullHash, document, true)) ||
                        //TODO is internal sign
                        (subscription.SigningType == EnumSigningTypes.InternalSign && !VerifyDocumentInternalSign(subscription.InternalSign, document)) ||
                        //TODO is certificate sign
                        (subscription.SigningType == EnumSigningTypes.CertificateSign && !VerifyDocumentCertificateSign(ctx, subscription.CertificateSign, document))
                        ))
                    {
                        if (subscription.SigningType == EnumSigningTypes.CertificateSign)
                            IsVioalted = true;

                        subscription.SubscriptionStates = EnumSubscriptionStates.Violated;

                        var subscriptionDb = new DocumentSubscriptions
                        {
                            Id = subscription.Id,
                            SubscriptionStateId = (int)subscription.SubscriptionStates,
                            LastChangeUserId = (int)EnumSystemUsers.AdminUser,
                            LastChangeDate = DateTime.UtcNow
                        };
                        dbContext.DocumentSubscriptionsSet.Attach(subscriptionDb);
                        var entry = dbContext.Entry(subscriptionDb);
                        entry.Property(x => x.SubscriptionStateId).IsModified = true;
                        entry.Property(x => x.LastChangeUserId).IsModified = true;
                        entry.Property(x => x.LastChangeDate).IsModified = true;

                        var sendList = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Where(x => x.StartEventId == subscription.SendEventId && x.IsInitial)
                            .FirstOrDefault();

                        if (sendList != null)
                        {
                            sendList.StartEventId = null;
                            sendList.CloseEventId = null;
                            sendList.LastChangeUserId = ctx.CurrentAgentId;
                            sendList.LastChangeDate = DateTime.UtcNow;
                        }

                        dbContext.SaveChanges();

                        //TODO проверить поля
                        //var eventDb = new DocumentEvents
                        //{
                        //    DocumentId = document.Id,
                        //    EventTypeId = (int)EnumEventTypes.LaunchPlan,
                        //    SourceAgentId = ctx.CurrentAgentId,
                        //    SourcePositionId = ctx.CurrentPositionId,
                        //    //SourcePositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, sourcePositionId ?? context.CurrentPositionId),
                        //    TargetPositionId = ctx.CurrentPositionId,
                        //    //TargetPositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, targetPositionId ?? context.CurrentPositionId),
                        //    TargetAgentId = ctx.CurrentAgentId,
                        //    LastChangeUserId = ctx.CurrentAgentId,
                        //    LastChangeDate = DateTime.UtcNow,
                        //    Date = DateTime.UtcNow,
                        //    CreateDate = DateTime.UtcNow,
                        //};

                        //dbContext.DocumentEventsSet.Add(eventDb);
                    }
                }
            }
            //TODO is certificate sign
            if ((isUseCertificateSign && (newSubscription?.SigningType == EnumSigningTypes.CertificateSign && isAddSubscription)) || IsVioalted)
            {
                subscriptions = subscriptions.Where(x => subscriptionStates.Contains(x.SubscriptionStates)).ToList();
                if (isAddSubscription)
                {
                    var sub = dbContext.DictionaryPositionsSet.Where(x => x.Id == ctx.CurrentPositionId)
                        .Select(x => new InternalDocumentSubscription { DoneEventSourcePositionName = x.Name, DoneEventSourcePositionExecutorAgentName = x.ExecutorAgent.Name }).FirstOrDefault();
                    if (sub != null)
                    {
                        sub.Id = newSubscription != null ? newSubscription.Id : 0;
                        sub.DocumentId = documentId;
                        subscriptions.Add(sub);
                    }
                }
                document.Subscriptions = subscriptions;
                document.CertificateSignPdfFileIdentity = CommonQueries.GetDocumentCertificateSignPdf(dbContext, ctx, document, newSubscription?.CertificateId, newSubscription?.CertificatePassword);
            }

            if (IsFilesIncorrect)
            {
                throw new DocumentFileWasChangedExternally();
            }

            return document;
        }

        public static InternalDocument GetDocumentHashPrepare(DmsContext dbContext, IContext ctx, int documentId)
        {
            var doc = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId)
                .Select(x => new InternalDocument
                {
                    Id = x.Id,
                    DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                    Description = x.Description
                }).FirstOrDefault();

            if (doc == null)
            {
                throw new Model.Exception.DocumentNotFoundOrUserHasNoAccess();
            }

            doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, documentId).Where(x => x.Type != EnumFileTypes.SubscribePdf).ToList();

            doc.SendLists = CommonQueries.GetInternalDocumentSendList(dbContext, ctx, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

            return doc;
        }

        public static InternalDocument GetDocumentDigitalSignaturePrepare(DmsContext dbContext, IContext ctx, int documentId, List<EnumSubscriptionStates> subscriptionStates)
        {
            var doc = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId)
                .Select(x => new InternalDocument
                {
                    Id = x.Id,
                    DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                    ExecutorPositionId = x.ExecutorPositionId,
                    DocumentTypeName = x.TemplateDocument.DocumentType.Name,
                    Description = x.Description,
                    ExecutorPositionName = x.ExecutorPosition.Name,
                    ExecutorPositionExecutorAgentName = x.ExecutorPositionExecutorAgent.Name,
                    Addressee = x.Addressee,
                    SenderAgentName = x.SenderAgent.Name,
                    SenderAgentPersonName = x.SenderAgentPerson.Agent.Name,
                    RegistrationNumber = x.RegistrationNumber,
                    RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                    RegistrationFullNumber = "#" + x.Id
                }).FirstOrDefault();

            if (doc == null)
            {
                throw new Model.Exception.DocumentNotFoundOrUserHasNoAccess();
            }

            doc.RegistrationFullNumber = GetRegistrationFullNumber(doc);

            doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, documentId).Where(x => x.Type != EnumFileTypes.SubscribePdf).ToList();

            doc.SendLists = CommonQueries.GetInternalDocumentSendList(dbContext, ctx, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

            var maxDateTime = DateTime.UtcNow.AddYears(50);

            doc.Waits = CommonQueries.GetDocumentWaitQuery(ctx, dbContext, new FilterDocumentWait { DocumentId = new List<int> { doc.Id } })
                .Select(x => new InternalDocumentWait
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    CreateDate = x.OnEvent.Date,
                    TargetPositionName = x.OnEvent.TargetPosition.Name,
                    TargetPositionExecutorAgentName = x.OnEvent.TargetPositionExecutorAgent.Name,
                    SourcePositionName = x.OnEvent.SourcePosition.Name,
                    SourcePositionExecutorAgentName = x.OnEvent.SourcePositionExecutorAgent.Name,
                    DueDate = x.DueDate > maxDateTime ? null : x.DueDate,
                    IsClosed = x.OffEventId != null,
                    ResultTypeName = x.ResultType.Name,
                    AttentionDate = x.AttentionDate,
                    OnEventTypeName = x.OnEvent.EventType.Name,
                    OffEventDate = x.OffEventId.HasValue ? x.OffEvent.CreateDate : (DateTime?)null
                }).ToList();

            doc.Subscriptions = CommonQueries.GetDocumentSubscriptionsQuery(dbContext,
                new FilterDocumentSubscription
                {
                    DocumentId = new List<int> { doc.Id },
                    SubscriptionStates = subscriptionStates
                }, ctx)
                .Select(x => new InternalDocumentSubscription
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    SubscriptionStatesName = x.SubscriptionState.Name,
                    DoneEventSourcePositionName = x.DoneEventId.HasValue ? x.DoneEvent.SourcePosition.Name : string.Empty,
                    DoneEventSourcePositionExecutorAgentName = x.DoneEventId.HasValue ? x.DoneEvent.SourcePositionExecutorAgent.Name : string.Empty,

                    SendEventId = x.SendEventId,
                    SubscriptionStates = (EnumSubscriptionStates)x.SubscriptionStateId,
                    Hash = x.Hash,
                    FullHash = x.FullHash,
                    SigningType = (EnumSigningTypes)x.SigningTypeId,
                    CertificateId = x.CertificateId,
                    CertificatePositionId = x.CertificatePositionId,
                    CertificatePositionExecutorAgentId = x.CertificatePositionExecutorAgentId,
                    CertificatePositionExecutorTypeId = x.CertificatePositionExecutorTypeId,
                    CertificateSign = x.CertificateSign,
                    InternalSign = x.InternalSign,
                    Description = x.Description
                }).ToList();

            return doc;
        }

        private static string GetStringDocumentForDocumentHash(InternalDocument doc, bool isFull = false)
        {
            StringBuilder stringDocument = new StringBuilder();

            stringDocument.Append("Document");
            stringDocument.Append(doc.Id);
            stringDocument.Append(doc.DocumentTypeId);
            stringDocument.Append(doc.Description);
            stringDocument.Append("Document");

            stringDocument.Append("File");
            if (doc.DocumentFiles?.Count() > 0)
            {
                foreach (var docFile in doc.DocumentFiles.OrderBy(x => x.Id))
                {
                    stringDocument.Append(docFile.Id);
                    stringDocument.Append(docFile.FileSize);
                    stringDocument.Append(docFile.LastChangeDate);
                    stringDocument.Append(docFile.Extension);
                    stringDocument.Append(docFile.Name);
                    if (isFull)
                        stringDocument.Append(docFile.Hash);
                }
            }
            stringDocument.Append("File");

            stringDocument.Append("SendList");
            if (doc.SendLists?.Count() > 0)
            {
                foreach (var docSendList in doc.SendLists.OrderBy(x => x.Id))
                {
                    stringDocument.Append(docSendList.Id);
                    stringDocument.Append(docSendList.TargetPositionId);
                    stringDocument.Append(docSendList.TargetAgentId);
                    stringDocument.Append(docSendList.SendType);
                    stringDocument.Append(docSendList.StageType);
                    stringDocument.Append(docSendList.Description);
                    stringDocument.Append(docSendList.DueDate);
                    stringDocument.Append(docSendList.DueDay);
                }
            }
            stringDocument.Append("SendList");

            return stringDocument.ToString();
        }
        public static string GetDocumentHash(InternalDocument doc, bool isFull = false)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, isFull);

            string hash = DmsResolver.Current.Get<ICryptoService>().GetHash(stringDocument);

            return hash;
        }

        public static string GetDocumentInternalSign(InternalDocument doc)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, true);

            string sign = DmsResolver.Current.Get<IEncryptionDbProcess>().GetInternalSign(stringDocument);

            return sign;
        }

        public static string GetDocumentCertificateSign(IContext ctx, InternalDocument doc, int certificateId, string certificatePassword)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, true);

            string sign = DmsResolver.Current.Get<IEncryptionDbProcess>().GetCertificateSign(ctx, certificateId, certificatePassword, stringDocument);

            return sign;
        }

        public static FrontReport GetDocumentCertificateSignPdf(DmsContext dbContext, IContext ctx, InternalDocument doc)
        {
            var fileStore = DmsResolver.Current.Get<IFileStore>();
            var pdf = DmsResolver.Current.Get<DmsReport>().ReportExportToStream(doc, fileStore.GetFullTemplateReportFilePath(ctx, EnumReportTypes.DocumentForDigitalSignature));

            using (PdfReader reader = new PdfReader(pdf.FileContent))
            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfStamper stamper = new PdfStamper(reader, ms))
                {
                    foreach (var file in doc.DocumentFiles.Where(x => x.IsMainVersion && x.Type != EnumFileTypes.SubscribePdf))
                    {
                        var fileBytes = fileStore.GetFile(ctx, new InternalDocumentAttachedFile
                        {
                            DocumentId = file.DocumentId,
                            OrderInDocument = file.OrderInDocument,
                            Version = file.Version,
                            Name = file.Name,
                            Extension = file.Extension,
                            Hash = file.Hash
                        });

                        PdfFileSpecification pfs = PdfFileSpecification.FileEmbedded(stamper.Writer, null, $"{file.Name}.{file.Extension}", fileBytes);

                        stamper.AddFileAttachment(file.Description, pfs);
                    }
                }
                pdf.FileContent = ms.ToArray();
            }

            return pdf;
        }

        public static FilterDocumentFileIdentity GetDocumentCertificateSignPdf(DmsContext dbContext, IContext ctx, InternalDocument doc, int? certificateId, string certificatePassword)
        {
            var fileStore = DmsResolver.Current.Get<IFileStore>();
            var pdf = GetDocumentCertificateSignPdf(dbContext, ctx, doc);

            if (certificateId.HasValue)
                pdf.FileContent = DmsResolver.Current.Get<IEncryptionDbProcess>().GetCertificateSignPdf(ctx, certificateId.Value, certificatePassword, pdf.FileContent);

            var positionId = (int)EnumSystemPositions.AdminPosition;
            try
            {
                positionId = ctx.CurrentPositionId;
            }
            catch { }

            var executorPositionExecutorAgentId = dbContext.DictionaryPositionsSet.Where(x => x.Id == positionId).Select(x => x.ExecutorAgentId).FirstOrDefault().GetValueOrDefault();
            if (executorPositionExecutorAgentId == 0)
                executorPositionExecutorAgentId = ctx.CurrentAgentId;

            var att = new InternalDocumentAttachedFile
            {
                DocumentId = doc.Id,
                Date = DateTime.UtcNow,
                PostedFileData = null,
                FileData = pdf.FileContent,
                Type = EnumFileTypes.SubscribePdf,
                IsMainVersion = true,
                FileType = "",
                Name = $"{doc.Id}",
                Extension = "pdf",
                Description = string.Empty,
                IsWorkedOut = (bool?)null,

                WasChangedExternal = false,
                ExecutorPositionId = positionId,
                ExecutorPositionExecutorAgentId = executorPositionExecutorAgentId,
            };

            var operationDb = DmsResolver.Current.Get<IDocumentFileDbProcess>();

            var ordInDoc = operationDb.CheckFileForDocument(ctx, att.DocumentId, att.Name, att.Extension);
            if (ordInDoc == -1)
            {
                att.Version = 1;
                att.OrderInDocument = operationDb.GetNextFileOrderNumber(ctx, att.DocumentId);
            }
            else
            {
                att.Version = operationDb.GetFileNextVersion(ctx, att.DocumentId, ordInDoc);
                att.OrderInDocument = ordInDoc;
            }

            att.LastChangeDate = DateTime.UtcNow;
            att.LastChangeUserId = ctx.CurrentAgentId;

            fileStore.SaveFile(ctx, att);

            operationDb.AddNewFileOrVersion(ctx, att);

            return new FilterDocumentFileIdentity { DocumentId = att.DocumentId, OrderInDocument = att.OrderInDocument, Version = att.Version };
        }

        public static bool VerifyDocumentHash(string hash, InternalDocument doc, bool isFull = false)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, isFull);

            return DmsResolver.Current.Get<ICryptoService>().VerifyHash(stringDocument, hash);
        }

        public static bool VerifyDocumentInternalSign(string sign, InternalDocument doc)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, true);

            return DmsResolver.Current.Get<IEncryptionDbProcess>().VerifyInternalSign(stringDocument, sign);
        }

        public static bool VerifyDocumentCertificateSign(IContext ctx, string sign, InternalDocument doc)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, true);

            var res = DmsResolver.Current.Get<IEncryptionDbProcess>().VerifyCertificateSign(ctx, stringDocument, sign);

            return res;
        }
        #endregion

        #region Actions
        public static List<InternalDictionaryPositionWithActions> GetPositionWithActions(IContext ctx, DmsContext dbContext, List<int> positionAccesses)
        {
            var filterCurrentPositionsContains = PredicateBuilder.False<DictionaryPositions>();
            filterCurrentPositionsContains = ctx.CurrentPositionsIdList.Aggregate(filterCurrentPositionsContains,
                (current, value) => current.Or(e => e.Id == value).Expand());

            var filterPositionAccessesContains = PredicateBuilder.False<DictionaryPositions>();
            filterPositionAccessesContains = positionAccesses.Aggregate(filterPositionAccessesContains,
                (current, value) => current.Or(e => e.Id == value).Expand());

            return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.CurrentClientId)
                .Where(filterCurrentPositionsContains)
                .Where(filterPositionAccessesContains)
                .Select(x => new InternalDictionaryPositionWithActions
                {
                    Id = x.Id,
                    Name = x.Name,
                    DepartmentId = x.DepartmentId,
                    ExecutorAgentId = x.ExecutorAgentId,
                    DepartmentName = x.Department.Name,
                    ExecutorAgentName = x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null),
                }).ToList();
        }

        public static Dictionary<int, List<InternalSystemActionForDocument>> GetActionsListForCurrentPositionsList(IContext context, DmsContext dbContext, IEnumerable<EnumObjects> objects, List<int> positionAccesses)
        {
            var filterObjectsContains = PredicateBuilder.False<SystemActions>();
            filterObjectsContains = objects.Aggregate(filterObjectsContains,
                (current, value) => current.Or(e => (EnumObjects)e.ObjectId == value).Expand());

            var res = new Dictionary<int, List<InternalSystemActionForDocument>>();
            foreach (var posId in context.CurrentPositionsIdList)
            {
                if (positionAccesses.Contains(posId))
                {
                    var qry = dbContext.SystemActionsSet
                        .Where(filterObjectsContains)
                        .Where(x => x.IsVisibleInMenu &&
                                    (!x.IsGrantable ||
                                        x.RoleActions.Any(y => y.Role.PositionRoles.Any(pr => pr.PositionId == posId) &&
                                        y.Role.UserRoles.Any(z => z.PositionExecutor.AgentId == context.CurrentAgentId)))
                        );

                    var actLst = qry.Select(a => new InternalSystemActionForDocument
                    {
                        DocumentAction = (EnumDocumentActions)a.Id,
                        Object = (EnumObjects)a.ObjectId,
                        ActionCode = a.Code,
                        ObjectCode = a.Object.Code,
                        API = a.API,
                        Description = a.Description,
                        Category = a.Category
                    }).ToList();
                    res.Add(posId, actLst);
                }
            }
            return res;
        }
        #endregion

        #region TaskAccesses
        public static void ModifyDocumentTaskAccesses(DmsContext dbContext, IContext ctx, int documentId)
        {
            var qry1 = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                .Where(x => x.DocumentId == documentId /*&& x.IsAvailableWithinTask*/ && x.TaskId.HasValue)
                .GroupBy(x => new { x.TaskId, x.SourcePositionId, x.TargetPositionId })
                .Select(x => new { x.Key.TaskId, x.Key.SourcePositionId, x.Key.TargetPositionId }).ToList();
            var qry2 = qry1.GroupBy(x => new { x.TaskId, x.SourcePositionId }).Where(x => x.Key.SourcePositionId.HasValue)
                .Select(x => new { x.Key.TaskId, PositionId = x.Key.SourcePositionId }).ToList();
            var qry3 = qry1.GroupBy(x => new { x.TaskId, x.TargetPositionId }).Where(x => x.Key.TargetPositionId.HasValue)
                .Select(x => new { x.Key.TaskId, PositionId = x.Key.TargetPositionId }).ToList();
            var taNew = qry2.Union(qry3).GroupBy(x => new { x.TaskId, x.PositionId })
                .Select(x => new InternalDocumentTaskAccesses { TaskId = x.Key.TaskId.Value, PositionId = x.Key.PositionId.Value }).ToList();

            var taOld = dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Task.DocumentId == documentId)
                .Select(x => new InternalDocumentTaskAccesses { Id = x.Id, TaskId = x.TaskId, PositionId = x.PositionId }).ToList();

            var delId = taOld.GroupJoin(taNew
                , ta1 => new { ta1.TaskId, ta1.PositionId }
                , ta2 => new { ta2.TaskId, ta2.PositionId }
                , (ta1, ta2) => new { ta1.Id, ta2 }).Where(x => x.ta2.Count() == 0).Select(x => x.Id).ToList();

            var insTA = taNew.GroupJoin(taOld
                , ta1 => new { ta1.TaskId, ta1.PositionId }
                , ta2 => new { ta2.TaskId, ta2.PositionId }
                , (ta1, ta2) => new { ta1, ta2 }).Where(x => x.ta2.Count() == 0).Select(x => x.ta1).ToList();

            {
                var filterContains = PredicateBuilder.False<DocumentTaskAccesses>();
                filterContains = delId.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(filterContains));
            }

            dbContext.DocumentTaskAccessesSet.AddRange(insTA.Select(x => new DocumentTaskAccesses { TaskId = x.TaskId, PositionId = x.PositionId }));

        }
        #endregion

        #region Certificates
        public static IQueryable<EncryptionCertificates> GetCertificatesQuery(DmsContext dbContext, IContext ctx, FilterEncryptionCertificate filter)
        {
            var qry = dbContext.EncryptionCertificatesSet.Where(x => x.Agent.ClientId == ctx.CurrentClientId).AsQueryable();
            if (!ctx.IsAdmin)
            {
                qry = qry.Where(x => x.AgentId == ctx.CurrentAgentId);
            }

            if (filter != null)
            {
                if (filter.CertificateId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<EncryptionCertificates>();
                    filterContains = filter.CertificateId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AgentId?.Count() > 0 && ctx.IsAdmin)
                {
                    var filterContains = PredicateBuilder.False<EncryptionCertificates>();
                    filterContains = filter.AgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
                }

                if (filter.CreateFromDate.HasValue)
                {
                    qry = qry.Where(x => filter.CreateFromDate.Value < x.CreateDate);
                }

                if (filter.CreateToDate.HasValue)
                {
                    qry = qry.Where(x => filter.CreateToDate.Value > x.CreateDate);
                }


                if (filter.NotBefore.HasValue)
                {
                    qry = qry.Where(x => filter.NotBefore.Value < x.NotBefore);
                }

                if (filter.NotAfter.HasValue)
                {
                    qry = qry.Where(x => filter.NotAfter.Value > x.NotAfter);
                }

                if (filter.IsActive.HasValue)
                {
                    var now = DateTime.UtcNow;
                    if (filter.IsActive.Value)
                    {
                        qry = qry.Where(x => (!x.NotBefore.HasValue || x.NotBefore < now) && (!x.NotAfter.HasValue || x.NotAfter > now));
                    }
                    else
                    {
                        qry = qry.Where(x => (x.NotBefore.HasValue && x.NotBefore > now) || (x.NotAfter.HasValue || x.NotAfter < now));
                    }
                }
            }
            return qry;
        }
        #endregion
    }
}