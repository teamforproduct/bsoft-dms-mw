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
using System.Globalization;
using BL.Database.DBModel.Admin;
using BL.CrossCutting.Helpers;

namespace BL.Database.Common
{
    internal static class CommonQueries
    {
        #region Documents
        public static IQueryable<DBModel.Document.Documents> GetDocumentQuery(IContext context, IQueryable<DocumentAccesses> userAccesses = null, bool? isVerifyExecutorPosition = null, bool isVerifyAccessLevel = true, bool isVerifyIsInWork = false)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.DocumentsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();
            if (!context.IsAdmin)
            {
                if (userAccesses == null)
                {
                    var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                    filterContains = isVerifyAccessLevel
                        ? context.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                        : context.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                    if (isVerifyIsInWork)
                        filterContains = filterContains.And(x => x.IsInWork == true).Expand();
                    if (isVerifyExecutorPosition.HasValue && isVerifyExecutorPosition.Value)
                        filterContains = filterContains.And(x => x.PositionId == x.Document.ExecutorPositionId).Expand();
                    if (isVerifyExecutorPosition.HasValue && isVerifyExecutorPosition.Value)
                        filterContains = filterContains.And(x => x.PositionId == context.CurrentPositionId).Expand();
                    qry = qry.Where(x => x.Accesses.AsQueryable().Where(filterContains).Any());
                }
                else
                    qry = qry.Where(x => userAccesses.Select(a => a.DocumentId).Contains(x.Id));

                //if (isVerifyExecutorPosition)
                //{
                //    var filterExecutorPositionContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                //    filterExecutorPositionContains = context.CurrentPositionsIdList.Aggregate(filterExecutorPositionContains, (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());
                //    qry = qry.Where(filterExecutorPositionContains);
                //}
                //else 
                if (!isVerifyExecutorPosition.HasValue && userAccesses == null)  //доступ к журналам проверяем, если нет ограничений на Accesses
                {
                    var filterPositionsIdList = PredicateBuilder.New<AdminRegistrationJournalPositions>(false);
                    filterPositionsIdList = context.CurrentPositionsIdList.Aggregate(filterPositionsIdList, (current, value) => current.Or(e => e.PositionId == value).Expand());
                    if (dbContext.AdminRegistrationJournalPositionsSet
                        .Where(filterPositionsIdList).Where(x => x.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.View)
                        .Select(x => x.RegJournalId).Any())
                    {
                        var qryRJA = dbContext.DocumentsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable()
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

        public static IQueryable<DBModel.Document.Documents> GetDocumentQuery(IContext context, FilterDocument filter, bool isVerifyAccessLevel = true)
        {
            var dbContext = context.DbContext as DmsContext;
            IQueryable<DocumentAccesses> acc = null;

            #region Filter access
            if (filter != null && (filter.IsInWork.HasValue || filter.IsFavourite.HasValue || filter.AccessLevelId?.Count() > 0))
            {
                acc = GetDocumentAccessesQuery(context, new FilterDocumentAccess
                {
                    IsInWork = filter?.IsInWork,
                    IsFavourite = filter?.IsFavourite,
                    AccessLevelId = filter?.AccessLevelId,
                    AccessPositionId = filter?.AccessPositionId,
                }, false, true, isVerifyAccessLevel);
            }
            #endregion Filter access

            var qry = CommonQueries.GetDocumentQuery(context, acc);

            if (filter == null || !filter.IsIgnoreRegistered)
            {
                qry = qry.Where(x => x.IsRegistered.HasValue);
            }

            if (filter != null)
            {
                #region Base

                if (filter.AllLinkedDocuments && filter.DocumentId?.Count() == 1)
                {
                    filter.DocumentId = GetLinkedDocumentIds(context, filter.DocumentId.First()).ToList();
                }

                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsDocumentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(true);
                    filterContains = filter.NotContainsDocumentId.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }
                // Исключение списка ИД процессов
                if (filter.NotContainsLinkId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(true);
                    filterContains = filter.NotContainsLinkId.Aggregate(filterContains,
                        (current, value) => current.And(e => e.LinkId != value).Expand());
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
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
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
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = filter.DocumentTypeId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TemplateDocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = filter.TemplateDocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TemplateDocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentDirectionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = filter.DocumentDirectionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentDirectionId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (!String.IsNullOrEmpty(filter.DocumentSubject))
                {
                    qry = qry.Where(x => x.Description.Contains(filter.DocumentSubject));
                }

                if (filter.RegistrationJournalId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = filter.RegistrationJournalId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RegistrationJournalId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ExecutorPositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = filter.ExecutorPositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ExecutorPositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = filter.ExecutorPositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ExecutorDepartmentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = filter.ExecutorDepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.SenderAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
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
                    var filterContains = PredicateBuilder.New<DBModel.Document.Documents>(false);
                    filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.SimultaneousAccessPositionId?.Count() > 0)
                {
                    foreach (var id in filter.SimultaneousAccessPositionId)
                    {
                        qry = qry.Where(x => x.Accesses.Any(y => y.PositionId == id));
                    }
                }
                #endregion Base

                #region Subscription
                if ((filter.SubscriptionPositionId?.Count() > 0) ||
                    (filter.SubscriptionPositionExecutorAgentId?.Count() > 0) ||
                    (filter.SubscriptionDepartmentId?.Count() > 0))
                {
                    var filterContains = PredicateBuilder.New<DocumentSubscriptions>(false);
                    filterContains = new List<int> { (int)EnumSubscriptionStates.Sign, (int)EnumSubscriptionStates.Visa, (int)EnumSubscriptionStates.Аgreement, (int)EnumSubscriptionStates.Аpproval }
                        .Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SubscriptionStateId == value).Expand());

                    if (filter.SubscriptionPositionId?.Count() > 0)
                    {
                        var filterContainsSubscriptionPositionId = PredicateBuilder.New<DocumentSubscriptions>(false);
                        filterContainsSubscriptionPositionId = filter.SubscriptionPositionId.Aggregate(filterContainsSubscriptionPositionId,
                            (current, value) => current.Or(e => e.DoneEvent.SourcePositionId == value).Expand());

                        qry = qry.Where(x =>
                                    x.Subscriptions.AsQueryable().Where(filterContains)
                                        .Any(filterContainsSubscriptionPositionId));
                    }

                    if (filter.SubscriptionPositionExecutorAgentId?.Count() > 0)
                    {
                        var filterContainsSubscriptionPositionExecutorAgentId = PredicateBuilder.New<DocumentSubscriptions>(false);
                        filterContainsSubscriptionPositionExecutorAgentId = filter.SubscriptionPositionExecutorAgentId.Aggregate(filterContainsSubscriptionPositionExecutorAgentId,
                            (current, value) => current.Or(e => e.DoneEvent.SourcePositionExecutorAgentId == value).Expand());

                        qry = qry.Where(x =>
                                    x.Subscriptions.AsQueryable().Where(filterContains)
                                        .Any(filterContainsSubscriptionPositionExecutorAgentId));
                    }

                    if (filter.SubscriptionDepartmentId?.Count() > 0)
                    {
                        var filterContainsSubscriptionDepartmentId = PredicateBuilder.New<DocumentSubscriptions>(false);
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
                    var filterContains = PredicateBuilder.New<DocumentTasks>(false);
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
                    //var filterContainsPosition = PredicateBuilder.New<DocumentTags>(false);
                    //filterContainsPosition = context.CurrentPositionsIdList.Aggregate(filterContainsPosition,
                    //    (current, value) => current.Or(e => !e.Tag.PositionId.HasValue || e.Tag.PositionId == value).Expand());

                    if (filter.TagId?.Count() > 0)
                    {
                        //var filterContains = PredicateBuilder.New<DocumentTags>(true);
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
                        var qryTmp = dbContext.PropertyValuesSet.Where(x => x.PropertyLink.Property.ClientId == context.CurrentClientId)
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
        public static IQueryable<DocumentEvents> GetDocumentEventQuery(IContext context, FilterDocumentEvent filter)
        {
            var qrys = GetDocumentEventQueryWithoutUnion(context, filter);
            var res = qrys.First();
            foreach (var qry in qrys.Skip(1).ToList())
            {
                res = res.Concat(qry);
            }
            return res;
        }

        public static IQueryable<DocumentEvents> GetEventsNativelyQuery(IContext context, FilterDocumentEventNatively filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.DocumentEventsSet.AsQueryable();

            if (filter.Date?.HasValue == true)
            {
                qry = qry.Where(x => x.Date >= filter.Date.DateBeg & x.Date <= filter.Date.DateEnd);
            }

            if (filter.ReadDate?.HasValue == true)
            {
                qry = qry.Where(x => x.ReadAgentId.HasValue && x.ReadDate.HasValue && x.ReadDate >= filter.ReadDate.DateBeg && x.ReadDate <= filter.ReadDate.DateEnd);
            }

            if (filter.SourcePositionIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                filterContains = filter.SourcePositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourcePositionId == value).Expand());
                qry = qry.Where(filterContains);
            }

            if (filter.SourcePositionExecutorAgentIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                filterContains = filter.SourcePositionExecutorAgentIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourcePositionExecutorAgentId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.SourceAgentIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                filterContains = filter.SourceAgentIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.SourceAgentId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.TargetPositionIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                filterContains = filter.TargetPositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());
                qry = qry.Where(filterContains);
            }

            if (filter.TargetPositionExecutorAgentIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                filterContains = filter.TargetPositionExecutorAgentIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.TargetPositionExecutorAgentId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.ReadAgentIDs?.Count() > 0)
            {
                var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                filterContains = filter.ReadAgentIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.ReadAgentId == value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;

        }
        public static List<IQueryable<DocumentEvents>> GetDocumentEventQueryWithoutUnion(IContext context, FilterDocumentEvent filter)
        {
            var dbContext = context.DbContext as DmsContext;

            var qry = dbContext.DocumentEventsSet.AsQueryable();

            qry = qry.Where(x => x.ClientId == context.CurrentClientId);

            if (filter != null)
            {
                if (filter.EventId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.EventId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AllLinkedDocuments && filter.DocumentId?.Count() == 1)
                {
                    filter.DocumentId = GetLinkedDocumentIds(context, filter.DocumentId.First()).ToList();
                }

                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsNew.HasValue)
                {
                    if (filter.IsNew.Value)
                    {
                        var filteTargetPositionIdContains = PredicateBuilder.New<DocumentEvents>(false);
                        filteTargetPositionIdContains = context.CurrentPositionsAccessLevel.Aggregate(filteTargetPositionIdContains,
                            (current, value) => current.Or(e => e.TargetPositionId == value.Key && e.Document.Accesses.Any(x => x.PositionId == value.Key && x.AccessLevelId >= value.Value)).Expand());
                        qry = qry.Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId).Where(filteTargetPositionIdContains);
                    }
                    //else
                    //{
                    //    qry = qry.Where(x => x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId);
                    //}
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
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.TypeId.Select(x => (int)x).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.EventTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ImportanceEventTypeId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
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
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.PositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value || e.TargetPositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.SourcePositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.SourcePositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.TargetPositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.TargetPositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.PositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.PositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionExecutorAgentId == value || e.TargetPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.SourcePositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.SourcePositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetPositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.TargetPositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.TargetAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetAgentId == value || e.TargetPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.SourceAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.SourceAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourceAgentId == value || e.SourcePositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.AgentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
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
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.DepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePosition.DepartmentId == value || e.TargetPosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.SourceDepartmentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.SourceDepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetDepartmentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentEvents>(false);
                    filterContains = filter.DepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            //TODO Что то придумать с union
            var res = new List<IQueryable<DocumentEvents>>();

            if (!context.IsAdmin)
            {
                var filterPositionContains = PredicateBuilder.New<DocumentEvents>(false);
                filterPositionContains = context.CurrentPositionsAccessLevel.Aggregate(filterPositionContains,
                    (current, value) => current.Or(e => (e.TargetPositionId == value.Key || e.SourcePositionId == value.Key)
                    && e.Document.Accesses.Any(x => x.PositionId == value.Key && x.AccessLevelId >= value.Value)).Expand());
                res.Add(qry.Where(filterPositionContains));
            }
            else
            {
                res.Add(qry);
            }

            return res;
        }

        #endregion

        #region Files
        public static IQueryable<DocumentFiles> GetDocumentFileQuery(IContext context, FilterDocumentFile filter, bool isVerifyAccessLevel = true)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.DocumentFilesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (!context.IsAdmin)
            {
                var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterContains = isVerifyAccessLevel
                    ? context.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                    : context.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter.FileId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentFiles>(false);
                    filterContains = filter.FileId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AllLinkedDocuments && filter.DocumentId?.Count() == 1)
                {
                    filter.DocumentId = GetLinkedDocumentIds(context, filter.DocumentId.First()).ToList();
                }

                if (filter.DocumentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentFiles>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.OrderInDocument?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentFiles>(false);
                    filterContains = filter.OrderInDocument.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OrderNumber == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.Types?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentFiles>(false);
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
                    var filterContains = PredicateBuilder.New<DocumentFiles>(false);
                    filterContains = filter.AgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.LastChangeUserId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.IsMyFiles ?? false)
                {
                    var filterContains = PredicateBuilder.New<DocumentFiles>(false);
                    filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public static IEnumerable<InternalDocumentAttachedFile> GetInternalDocumentFiles(IContext context, int documentId)
        {
            var sq = GetDocumentFileQuery(context, new FilterDocumentFile { DocumentId = new List<int> { documentId } });

            return
                sq.Select(x => new InternalDocumentAttachedFile
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
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
        public static IQueryable<DocumentWaits> GetDocumentWaitQuery(IContext context, FilterDocumentWait filter)
        {
            var qrys = GetDocumentWaitQueryWithoutUnion(context, filter);
            var res = qrys.First();
            foreach (var qry in qrys.Skip(1).ToList())
            {
                res = res.Concat(qry);
            }
            return res;
        }

        public static List<IQueryable<DocumentWaits>> GetDocumentWaitQueryWithoutUnion(IContext context, FilterDocumentWait filter)
        {
            var dbContext = context.DbContext as DmsContext;

            var qry = dbContext.DocumentWaitsSet.AsQueryable();

            qry = qry.Where(x => x.ClientId == context.CurrentClientId);

            //if (!(filter?.IsOpened.HasValue??false) && !(filter?.DocumentId?.Any()??false))
            //{
            //    qry = qry.Where(x => !x.OffEventId.HasValue);
            //}

            if (filter != null)
            {
                if (filter.AllLinkedDocuments && filter.DocumentId?.Count() == 1)
                {
                    filter.DocumentId = GetLinkedDocumentIds(context, filter.DocumentId.First()).ToList();
                }

                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.OnEventId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                    filterContains = filter.OnEventId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OnEventId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.OffEventId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                    filterContains = filter.OffEventId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OffEventId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsOpened.HasValue)
                {
                    //qry = qry.Where(x => !x.ReadDate.HasValue && x.TargetPositionId.HasValue && x.TargetPositionId != x.SourcePositionId).Where(filteTargetPositionIdrContains);
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
                        qry = qry.Where(x => x.DueDate.HasValue && x.DueDate.Value < (x.OffEvent != null ? x.OffEvent.Date : DateTime.UtcNow));
                    }
                    else
                    {
                        qry = qry.Where(x => !(x.DueDate.HasValue && x.DueDate.Value < (x.OffEvent != null ? x.OffEvent.Date : DateTime.UtcNow)));
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
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                        filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value && e.OnEvent.SourcePositionId != value).Expand());

                        qry = qry.Where(x => !x.OffEventId.HasValue && (x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForExecution || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForExecutionChange || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecutionChange))
                                .Where(filterContains);
                    }

                    if (filter.ControlToMePositionId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                        filterContains = filter.ControlToMePositionId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.SourcePositionId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlToMePositionExecutorAgentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                        filterContains = filter.ControlToMePositionExecutorAgentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.SourcePositionExecutorAgentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlToMeDepartmentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
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
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                        filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPositionId != value && e.OnEvent.SourcePositionId == value).Expand());

                        qry = qry.Where(x => !x.OffEventId.HasValue && (x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForExecution || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForExecutionChange || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution || x.OnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecutionChange))
                                .Where(filterContains);
                    }

                    if (filter.ControlFromMePositionId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                        filterContains = filter.ControlFromMePositionId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlFromMePositionExecutorAgentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                        filterContains = filter.ControlFromMePositionExecutorAgentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPositionExecutorAgentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlFromMeDepartmentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                        filterContains = filter.ControlFromMeDepartmentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetPosition.DepartmentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ControlFromMeAgentId?.Count() > 0)
                    {
                        var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                        filterContains = filter.ControlFromMeAgentId.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.OnEvent.TargetAgentId == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                }
                #endregion

                #region Самоконтроль ControlOn

                if (filter.IsSelfControl ?? false)
                {
                    var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                    filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value && e.OnEvent.SourcePositionId == value).Expand());

                    qry = qry.Where(x => !x.OffEventId.HasValue && x.OnEvent.EventTypeId == (int)EnumEventTypes.ControlOn)
                            .Where(filterContains);
                }
                #endregion

                #region Поступившие на визирование SendForVisaing, SendForАgreement, SendForАpproval, SendForSigning
                if (filter.IsVisaingToMe ?? false)
                {
                    var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                    filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
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
                    var filterContains = PredicateBuilder.New<DocumentWaits>(false);
                    filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
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

            if (context != null && !context.IsAdmin)
            {
                //var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                //filterContains = isVerifyAccessLevel
                //    ? context.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                //    : context.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                //qry = qry.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));

                var filterOnEventPositionsContains = PredicateBuilder.New<DocumentWaits>(false);
                filterOnEventPositionsContains = context.CurrentPositionsAccessLevel.Aggregate(filterOnEventPositionsContains,
                    (current, value) => current.Or(e => (e.OnEvent.TargetPositionId == value.Key || e.OnEvent.SourcePositionId == value.Key)
                                                && e.Document.Accesses.Any(x => x.PositionId == value.Key && x.AccessLevelId >= value.Value)).Expand());

                if (filter?.IsMyControl ?? false)
                {
                    res.Add(qry.Where(filterOnEventPositionsContains));
                }
                else
                {
                    var filterOnEventTaskAccessesContains = PredicateBuilder.New<DocumentTaskAccesses>(false);
                    filterOnEventTaskAccessesContains = context.CurrentPositionsIdList.Aggregate(filterOnEventTaskAccessesContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());
                    res.Add(qry.Where(filterOnEventPositionsContains));
                }
            }
            else
            {
                res.Add(qry);
            }

            return res;
        }

        #endregion

        #region Accesses
        public static IQueryable<DocumentAccesses> GetDocumentAccessesQuery(IContext context, FilterDocumentAccess filter, bool isAll = false, bool isAddClientFilter = true, bool isVerifyAccessLevel = false)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.DocumentAccessesSet.AsQueryable();
            if (isAddClientFilter)
            {
                qry = qry.Where(x => x.ClientId == context.CurrentClientId);
            }
            if (!isAll && !context.IsAdmin)
            {
                var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterContains = isVerifyAccessLevel
                    ? context.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                    : context.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(filterContains);
            }
            if (filter != null)
            {
                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());
                    qry = qry.Where(filterContains);
                }

                if (filter.AccessLevelId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                    filterContains = filter.AccessLevelId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AccessLevelId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsInWork.HasValue)
                {
                    qry = qry.Where(x => x.IsInWork == filter.IsInWork);
                }
                if (filter.IsFavourite.HasValue)
                {
                    qry = qry.Where(x => x.IsFavourite == filter.IsFavourite);
                }
                if (filter.AccessPositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                    filterContains = filter.AccessPositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());
                    qry = qry.Where(filterContains);
                }
            }
            return qry;
        }

        public static IEnumerable<DocumentAccesses> GetDbDocumentAccesses(IContext context, IEnumerable<InternalDocumentAccess> docAccesses, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            if (docAccesses == null || !docAccesses.Any()) return null;
            var accPositions = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId).Select(x => x.PositionId);
            return docAccesses.Where(x => !accPositions.Contains(x.PositionId)).Select(ModelConverter.GetDbDocumentAccess);
        }

        public static IQueryable<DocumentAccesses> GetDocumentAccessesesQry(IContext context, int documentId, bool isVerifyAccessLevel = false)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.DocumentId == documentId);
            if (context != null && !context.IsAdmin)
            {
                var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterContains = isVerifyAccessLevel
                    ? context.CurrentPositionsAccessLevel.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value.Key && e.AccessLevelId >= value.Value).Expand())
                    : context.CurrentPositionsIdList.Aggregate(filterContains, (current, value) => current.Or(e => e.PositionId == value).Expand());

                //filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                //    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(filterContains);
            }
            return qry;
        }

        public static IEnumerable<InternalDocumentAccess> GetInternalDocumentAccesses(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            return dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.DocumentId == documentId).Select(acc => new InternalDocumentAccess
                    {
                        Id = acc.Id,
                        DocumentId = acc.DocumentId,
                        PositionId = acc.PositionId,
                        AgentId = acc.AgentId,
                    }).ToList();
        }
        #endregion

        #region Positions
        public static IEnumerable<InternalPositionInfo> GetInternalPositionsInfo(IContext context, List<int> positionIds)
        {
            var dbContext = context.DbContext as DmsContext;
            var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
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
        public static IEnumerable<FrontDocumentTask> GetDocumentTasks(IContext context, FilterDocumentTask filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            var tasksDb = dbContext.DocumentTasksSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (!context.IsAdmin)
            {
                var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                tasksDb = tasksDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentTasks>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    tasksDb = tasksDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentTasks>(false);
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

                //FactResponsibleExecutorPositionName = x.SendListDb.TargetPosition.Name,
                //FactResponsibleExecutorPositionExecutorAgentName = x.SendListDb.TargetPositionExecutorAgent.Name,

                //PlanResponsibleExecutorPositionName = x.Event.TargetPosition.Name,
                //PlanResponsibleExecutorPositionExecutorAgentName = x.Event.TargetPositionExecutorAgent.Name,
            }).ToList();

            var eventFilterContains = PredicateBuilder.New<DocumentEvents>(false);
            eventFilterContains = tasks.Select(x => x.Id).Aggregate(eventFilterContains, (current, value) => current.Or(e => e.TaskId == value).Expand());

            var taskFactRespEx = dbContext.DocumentEventsSet.Where(eventFilterContains).Where(x => x.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution || x.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecutionChange)
                    .Where(x => x.OnWait.Any(y => !y.OffEventId.HasValue))
                    .Select(x => new FrontDocumentTask
                    {
                        Id = x.TaskId.Value,
                        IsFactExecutor = true,
                        ExecutorSendType = EnumSendTypes.SendForResponsibleExecution,
                        ExecutorType = "##l@TaskExecutor:ResponsibleExecutor@l##",
                        ResponsibleExecutorPositionName = x.TargetPosition.Name,
                        ResponsibleExecutorPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : null),
                    }).ToList();

            var taskFactContr = dbContext.DocumentEventsSet.Where(eventFilterContains).Where(x => x.EventTypeId == (int)EnumEventTypes.SendForControl)
                .Select(x => new FrontDocumentTask
                {
                    Id = x.TaskId.Value,
                    IsFactExecutor = true,
                    ExecutorSendType = EnumSendTypes.SendForControl,
                    ExecutorType = "##l@TaskExecutor:Controler@l##",
                    ResponsibleExecutorPositionName = x.TargetPosition.Name,
                    ResponsibleExecutorPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : null),
                }).ToList();


            var sendListFilterContains = PredicateBuilder.New<DocumentSendLists>(false);
            sendListFilterContains = tasks.Select(x => x.Id).Aggregate(sendListFilterContains, (current, value) => current.Or(e => e.TaskId == value).Expand());

            var taskPlanRespEx = dbContext.DocumentSendListsSet.Where(sendListFilterContains).Where(x => x.SendTypeId == (int)EnumSendTypes.SendForResponsibleExecution)
                                .Select(x => new FrontDocumentTask
                                {
                                    Id = x.TaskId.Value,
                                    IsFactExecutor = false,
                                    ExecutorSendType = EnumSendTypes.SendForResponsibleExecution,
                                    ExecutorType = "##l@TaskExecutor:ResponsibleExecutor@l##",
                                    ResponsibleExecutorPositionName = x.TargetPosition.Name,
                                    ResponsibleExecutorPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : null),
                                }).ToList();

            var taskPlanContr = dbContext.DocumentSendListsSet.Where(sendListFilterContains).Where(x => x.SendTypeId == (int)EnumSendTypes.SendForControl)
                .Select(x => new FrontDocumentTask
                {
                    Id = x.TaskId.Value,
                    IsFactExecutor = false,
                    ExecutorSendType = EnumSendTypes.SendForControl,
                    ExecutorType = "##l@TaskExecutor:Controler@l##",
                    ResponsibleExecutorPositionName = x.TargetPosition.Name,
                    ResponsibleExecutorPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name + (x.TargetPosition.ExecutorType.Suffix != null ? " (" + x.TargetPosition.ExecutorType.Suffix + ")" : null),
                }).ToList();

            tasks.ForEach(x =>
            {
                FrontDocumentTask taskExecutor = taskFactRespEx.OrderByDescending(y => y.Id).FirstOrDefault(y => y.Id == x.Id);
                if (taskExecutor == null) taskExecutor = taskFactContr.OrderByDescending(y => y.Id).FirstOrDefault(y => y.Id == x.Id);
                if (taskExecutor == null) taskExecutor = taskPlanRespEx.OrderByDescending(y => y.Id).FirstOrDefault(y => y.Id == x.Id);
                if (taskExecutor == null) taskExecutor = taskPlanContr.OrderByDescending(y => y.Id).FirstOrDefault(y => y.Id == x.Id);
                if (taskExecutor != null)
                {
                    x.IsFactExecutor = taskExecutor.IsFactExecutor;
                    x.ExecutorSendType = taskExecutor.ExecutorSendType;
                    x.ExecutorType = taskExecutor.ExecutorType;
                    x.ResponsibleExecutorPositionName = taskExecutor.ResponsibleExecutorPositionName;
                    x.ResponsibleExecutorPositionExecutorAgentName = taskExecutor.ResponsibleExecutorPositionExecutorAgentName;
                }
            });

            tasks.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));

            return tasks;

        }
        #endregion

        #region Subscriptions
        public static IQueryable<DocumentSubscriptions> GetDocumentSubscriptionsQuery(IContext context, FilterDocumentSubscription filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();
            if (!context.IsAdmin)
            {
                var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                subscriptionsDb = subscriptionsDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter.DocumentId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentSubscriptions>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
                if (filter.SubscriptionStates?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentSubscriptions>(false);
                    filterContains = filter.SubscriptionStates.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumSubscriptionStates)e.SubscriptionStateId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
            }

            return subscriptionsDb;

        }
        public static IEnumerable<InternalDocumentSubscription> GetInternalDocumentSubscriptions(IContext context, FilterDocumentSubscription filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var subscriptionsDb = dbContext.DocumentSubscriptionsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId.Any())
                {
                    var filterContains = PredicateBuilder.New<DocumentSubscriptions>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
                if (filter.SubscriptionStates?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentSubscriptions>(false);
                    filterContains = filter.SubscriptionStates.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumSubscriptionStates)e.SubscriptionStateId == value).Expand());

                    subscriptionsDb = subscriptionsDb.Where(filterContains);
                }
            }

            var subscriptions = subscriptionsDb.Select(x => new InternalDocumentSubscription
            {
                Id = x.Id,
                ClientId = x.ClientId,
                EntityTypeId = x.EntityTypeId,
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
        public static IQueryable<DocumentTags> GetDocumentTagsQuery(IContext context, FilterDocumentTag filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.DocumentTagsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentTags>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.CurrentPositionsId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentTags>(false);
                    filterContains = filter.CurrentPositionsId.Aggregate(filterContains,
                        (current, value) => current.Or(e => !e.Tag.PositionId.HasValue || e.Tag.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }
            return qry;

        }
        #endregion

        #region PropertyValues
        public static IQueryable<PropertyValues> GetPropertyValuesQuery(IContext context, FilterPropertyValue filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var itemsDb = dbContext.PropertyValuesSet.Where(x => x.PropertyLink.Property.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.Object?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<PropertyValues>(false);
                    filterContains = filter.Object.Select(x => (int)x).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PropertyLink.ObjectId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }

                if (filter.RecordId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<PropertyValues>(false);
                    filterContains = filter.RecordId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RecordId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
            }

            return itemsDb;

        }

        public static IEnumerable<FrontPropertyValue> GetPropertyValues(IContext context, FilterPropertyValue filter)
        {
            var dbContext = context.DbContext as DmsContext;

            var itemsDb = GetPropertyValuesQuery(context, filter);

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

        public static IEnumerable<InternalPropertyValue> GetInternalPropertyValues(IContext context, FilterPropertyValue filter)
        {
            var itemsDb = GetPropertyValuesQuery(context, filter);

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

        public static void DeletePropertyValues(IContext context, FilterPropertyValue filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var itemsDb = GetPropertyValuesQuery(context, filter);
            dbContext.PropertyValuesSet.RemoveRange(itemsDb);
        }
        public static void ModifyPropertyValues(IContext context, InternalPropertyValues model)
        {
            var dbContext = context.DbContext as DmsContext;
            var propertyValues = dbContext.PropertyValuesSet
                .Where(x => x.PropertyLink.Property.ClientId == context.CurrentClientId)
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
                dbContext.SafeAttach(item);
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
            foreach (var item in groupJoinItems.Where(x => !x.values.Any()).Select(x => x.propertyValueId))
            {
                var itemAtt = dbContext.PropertyValuesSet.Attach(new PropertyValues { Id = item });
                dbContext.Entry(itemAtt).State = EntityState.Deleted;
            }
            #endregion
        }
        #endregion

        #region WorkGroups
        public static IQueryable<DocumentAccesses> GetDocumentWorkGroupQuery(IContext context, FilterDictionaryPosition filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                    filterContains = filter.DocumentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;

        }
        #endregion

        #region LinkedDocuments
        public static IEnumerable<int> GetLinkedDocumentIds(IContext context, int documentId)
        {
            var dbContext = context.DbContext as DmsContext;
            var docLinkId = CommonQueries.GetDocumentQuery(context).Where(y => y.LinkId.HasValue && y.Id == documentId).Select(y => y.LinkId).AsQueryable();
            var docIds = CommonQueries.GetDocumentQuery(context).Where(x => x.LinkId.HasValue && docLinkId.Contains(x.LinkId)).Select(x => x.Id).ToList();
            if (!docIds.Any())
                docIds = new List<int> { documentId };
            return docIds;
        }
        #endregion

        #region SendLists
        private static IQueryable<DocumentSendLists> GetDocumentSendListQuery(IContext context, FilterDocumentSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var sendListDb = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentSendLists>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentSendLists>(false);
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }

            }

            return sendListDb;
        }

        public static IEnumerable<FrontDocumentSendList> GetDocumentSendList(IContext context, FilterDocumentSendList filter)
        {
            var sendListDb = GetDocumentSendListQuery(context, filter);

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
                AccessLevel = (EnumAccessLevels)y.AccessLevelId,
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
        public static IEnumerable<InternalDocumentSendList> GetInternalDocumentSendList(IContext context, FilterDocumentSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;

            var sendListDb = dbContext.DocumentSendListsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentSendLists>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentSendLists>(false);
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }

            }

            return sendListDb.Select(y => new InternalDocumentSendList
            {
                Id = y.Id,
                ClientId = y.ClientId,
                EntityTypeId = y.EntityTypeId,
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
        public static IQueryable<DocumentRestrictedSendLists> GetDocumentRestrictedSendListQuery(IContext context, FilterDocumentRestrictedSendList filter)
        {
            var dbContext = context.DbContext as DmsContext;

            var sendListDb = dbContext.DocumentRestrictedSendListsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentRestrictedSendLists>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentRestrictedSendLists>(false);
                    filterContains = filter.Id.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    sendListDb = sendListDb.Where(filterContains);
                }

            }
            return sendListDb;
        }
        #endregion

        #region FullText
        public static void AddFullTextCacheInfo(IContext context, int objectId, EnumObjects objType, EnumOperationType operationType)
        {
            var dbContext = context.DbContext as DmsContext;
            var cacheInfo = new FullTextIndexCash
            {
                ObjectId = objectId,
                ObjectType = (int)objType,
                OperationType = (int)operationType,
                ClientId = context.CurrentClientId
            };


            dbContext.FullTextIndexCashSet.Add(cacheInfo);

            dbContext.SaveChanges();

        }

        public static void AddFullTextCacheInfo(IContext context, List<int> objectId, EnumObjects objType, EnumOperationType operationType)
        {
            var dbContext = context.DbContext as DmsContext;
            if (objectId == null || !objectId.Any()) return;
            var cashInfos = objectId.Select(x =>
                new FullTextIndexCash
                {
                    ObjectId = x,
                    ObjectType = (int)objType,
                    OperationType = (int)operationType,
                    ClientId = context.CurrentClientId
                }).ToList();

            dbContext.FullTextIndexCashSet.AddRange(cashInfos);

            dbContext.SaveChanges();
        }
        #endregion

        #region Papers
        public static IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext context, FilterDocumentPaper filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            var itemsDb = dbContext.DocumentPapersSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (!context.IsAdmin)
            {
                var filterContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                itemsDb = itemsDb.Where(x => x.Document.Accesses.AsQueryable().Any(filterContains));
            }

            if (filter != null)
            {
                if (filter?.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentPapers>(false);
                    filterContains = filter.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    itemsDb = itemsDb.Where(filterContains);
                }
                if (filter?.Id?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentPapers>(false);
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
                PaperPlanDate = x.LastPaperEvent.PaperPlanDate,
                PaperSendDate = x.LastPaperEvent.PaperSendDate,
                PaperRecieveDate = x.LastPaperEvent.PaperRecieveDate,


            }).ToList();

            items.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));

            return items;
        }
        #endregion

        #region PaperLists
        public static IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext context, FilterDocumentPaperList filter, UIPaging paging)
        {
            var dbContext = context.DbContext as DmsContext;
            var itemsDb = dbContext.DocumentPaperListsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (filter?.PaperListId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<DocumentPaperLists>(false);
                    filterContains = filter.PaperListId.Aggregate(filterContains,
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
                    return new List<FrontDocumentPaperList>();
                }


                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    itemsDb = itemsDb
                        .Skip(() => skip).Take(() => take);
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

        public static void SetRegistrationFullNumber(FrontDocument item, bool isClearFields = true)
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

        public static void SetRegistrationFullNumber(FrontRegistrationFullNumber item)
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
            item.RegistrationNumber = null;
            item.RegistrationNumberPrefix = null;
            item.RegistrationNumberSuffix = null;
        }
        #endregion

        #region Hash
        public static InternalDocument GetDocumentHash(IContext context, int documentId, bool isUseInternalSign, bool isUseCertificateSign, InternalDocumentSubscription newSubscription, string serverMapPath, bool isAddSubscription = false, bool isFull = false, bool isContinueIfEmptySubscriptions = false)
        {
            var dbContext = context.DbContext as DmsContext;

            var subscriptionStates = new List<EnumSubscriptionStates> {
                        EnumSubscriptionStates.Sign,
                        EnumSubscriptionStates.Visa,
                        EnumSubscriptionStates.Аgreement,
                        EnumSubscriptionStates.Аpproval
                        };

            List<InternalDocumentSubscription> subscriptions = CommonQueries.GetInternalDocumentSubscriptions(context,
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

            InternalDocument document = CommonQueries.GetDocumentDigitalSignaturePrepare(context, documentId, subscriptionStates);

            subscriptions = document.Subscriptions.ToList();

            var IsFilesIncorrect = false;

            if (isFull || isAddSubscription)
            {
                var fs = DmsResolver.Current.Get<IFileStore>();
                foreach (var file in document.DocumentFiles)
                {
                    if (!fs.IsFileCorrect(context, file))
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
                            FileLogger.AppendTextToFile(DateTime.Now + " GetDocumentHash GetDocumentCertificateSign ", @"C:\TEMPLOGS\sign.log");
                            document.CertificateSign = CommonQueries.GetDocumentCertificateSign(context, document, newSubscription.CertificateId.Value, newSubscription.CertificatePassword, serverMapPath);
                        }
                        else
                            throw new SigningTypeNotAllowed();
                    }
                    break;
            }

            bool IsVioalted = IsFilesIncorrect;

            if (subscriptions.Any())
            {
                foreach (var subscription in subscriptions)
                {
                    if (IsFilesIncorrect ||
                        (!VerifyDocumentHash(subscription.Hash, document) || ((isFull || isAddSubscription) && !VerifyDocumentHash(subscription.FullHash, document, true)) ||
                        //TODO is internal sign
                        (subscription.SigningType == EnumSigningTypes.InternalSign && !VerifyDocumentInternalSign(subscription.InternalSign, document)) ||
                        //TODO is certificate sign
                        (subscription.SigningType == EnumSigningTypes.CertificateSign && !VerifyDocumentCertificateSign(context, subscription.CertificateSign, document, serverMapPath))
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
                        dbContext.SafeAttach(subscriptionDb);
                        var entry = dbContext.Entry(subscriptionDb);
                        entry.Property(x => x.SubscriptionStateId).IsModified = true;
                        entry.Property(x => x.LastChangeUserId).IsModified = true;
                        entry.Property(x => x.LastChangeDate).IsModified = true;

                        var sendList = dbContext.DocumentSendListsSet
                            .Where(x => x.ClientId == context.CurrentClientId)
                            .FirstOrDefault(x => x.StartEventId == subscription.SendEventId && x.IsInitial);

                        if (sendList != null)
                        {
                            sendList.StartEventId = null;
                            sendList.CloseEventId = null;
                            sendList.LastChangeUserId = context.CurrentAgentId;
                            sendList.LastChangeDate = DateTime.UtcNow;
                        }

                        dbContext.SaveChanges();

                        //TODO проверить поля
                        //var eventDb = new DocumentEvents
                        //{
                        //    DocumentId = document.Id,
                        //    EventTypeId = (int)EnumEventTypes.LaunchPlan,
                        //    SourceAgentId = context.CurrentAgentId,
                        //    SourcePositionId = context.CurrentPositionId,
                        //    //SourcePositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, sourcePositionId ?? context.CurrentPositionId),
                        //    TargetPositionId = context.CurrentPositionId,
                        //    //TargetPositionExecutorAgentId = GetExecutorAgentIdByPositionId(context, targetPositionId ?? context.CurrentPositionId),
                        //    TargetAgentId = context.CurrentAgentId,
                        //    LastChangeUserId = context.CurrentAgentId,
                        //    LastChangeDate = DateTime.UtcNow,
                        //    Date = DateTime.UtcNow,
                        //    CreateDate = DateTime.UtcNow,
                        //};

                        //dbContext.DocumentEventsSet.Add(eventDb);
                    }
                }
            }
            //TODO is certificate sign
            if ((isUseCertificateSign && (newSubscription?.SigningType == EnumSigningTypes.CertificateSign && isAddSubscription)) && !IsVioalted)
            {
                subscriptions = subscriptions.Where(x => subscriptionStates.Contains(x.SubscriptionStates)).ToList();
                if (isAddSubscription)
                {
                    var sub = dbContext.DictionaryPositionsSet.Where(x => x.Id == context.CurrentPositionId)
                        .Select(x => new InternalDocumentSubscription { DoneEventSourcePositionName = x.Name, DoneEventSourcePositionExecutorAgentName = x.ExecutorAgent.Name }).FirstOrDefault();
                    if (sub != null)
                    {
                        sub.Id = newSubscription != null ? newSubscription.Id : 0;
                        sub.DocumentId = documentId;
                        subscriptions.Add(sub);
                    }
                }
                document.Subscriptions = subscriptions;
                FileLogger.AppendTextToFile(DateTime.Now + " GetDocumentHash CertificateSignPdfFileIdentity ", @"C:\TEMPLOGS\sign.log");

                document.CertificateSignPdfFileIdentity = CommonQueries.GetDocumentCertificateSignPdf(context, document, newSubscription?.CertificateId, newSubscription?.CertificatePassword, serverMapPath);
            }

            if (IsFilesIncorrect && isAddSubscription)
            {
                throw new DocumentFileWasChangedExternally();
            }
            FileLogger.AppendTextToFile(DateTime.Now + " GetDocumentHash end ", @"C:\TEMPLOGS\sign.log");

            return document;
        }

        public static InternalDocument GetDocumentHashPrepare(IContext context, int documentId)
        {
            var doc = CommonQueries.GetDocumentQuery(context).Where(x => x.Id == documentId)
                .Select(x => new InternalDocument
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    DocumentTypeId = x.DocumentTypeId,
                    Description = x.Description
                }).FirstOrDefault();

            if (doc == null)
            {
                throw new Model.Exception.DocumentNotFoundOrUserHasNoAccess();
            }

            doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(context, documentId).Where(x => x.Type != EnumFileTypes.SubscribePdf).ToList();

            doc.SendLists = CommonQueries.GetInternalDocumentSendList(context, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

            return doc;
        }

        public static InternalDocument GetDocumentDigitalSignaturePrepare(IContext context, int documentId, List<EnumSubscriptionStates> subscriptionStates)
        {
            var doc = CommonQueries.GetDocumentQuery(context).Where(x => x.Id == documentId)
                .Select(x => new InternalDocument
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    DocumentTypeId = x.DocumentTypeId,
                    ExecutorPositionId = x.ExecutorPositionId,
                    DocumentTypeName = x.DocumentType.Name,
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

            doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(context, documentId).Where(x => x.Type != EnumFileTypes.SubscribePdf).ToList();

            doc.SendLists = CommonQueries.GetInternalDocumentSendList(context, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

            var maxDateTime = DateTime.UtcNow.AddYears(50);

            doc.Waits = CommonQueries.GetDocumentWaitQuery(context, new FilterDocumentWait { DocumentId = new List<int> { doc.Id } })
                .Select(x => new InternalDocumentWait
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
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

            doc.Subscriptions = CommonQueries.GetDocumentSubscriptionsQuery(context, new FilterDocumentSubscription
            {
                DocumentId = new List<int> { doc.Id },
                SubscriptionStates = subscriptionStates
            })
                .Select(x => new InternalDocumentSubscription
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
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

        public static string GetDocumentCertificateSign(IContext context, InternalDocument doc, int certificateId, string certificatePassword, string serverMapPath)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, true);

            string sign = DmsResolver.Current.Get<IEncryptionDbProcess>().GetCertificateSign(context, certificateId, certificatePassword, stringDocument, serverMapPath);

            return sign;
        }

        public static FrontReport GetDocumentCertificateSignPdf(IContext context, InternalDocument doc)
        {
            //TODO PDF Can we reuse pdf?? 
            var fileStore = DmsResolver.Current.Get<IFileStore>();
            // такое не делают на уровне БД! Это должно происходит на уровне логики! SZ
            var pdf = DmsResolver.Current.Get<DmsReport>().ReportExportToStream(doc, fileStore.GetFullTemplateReportFilePath(context, EnumReportTypes.DocumentForDigitalSignature));

            using (PdfReader reader = new PdfReader(pdf.FileContent))
            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfStamper stamper = new PdfStamper(reader, ms))
                {
                    foreach (var file in doc.DocumentFiles.Where(x => x.IsMainVersion && x.Type != EnumFileTypes.SubscribePdf))
                    {
                        var fileBytes = fileStore.GetFile(context, new InternalDocumentAttachedFile
                        {
                            ClientId = file.ClientId,
                            EntityTypeId = file.EntityTypeId,
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

        public static FilterDocumentFileIdentity GetDocumentCertificateSignPdf(IContext context, InternalDocument doc, int? certificateId, string certificatePassword, string serverMapPath)
        {
            var dbContext = context.DbContext as DmsContext;
            FileLogger.AppendTextToFile(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " GetDocumentCertificateSignPdf begin ", @"C:\TEMPLOGS\sign.log");

            var fileStore = DmsResolver.Current.Get<IFileStore>();
            var pdf = GetDocumentCertificateSignPdf(context, doc);

            if (certificateId.HasValue)
            {
                pdf.FileContent = DmsResolver.Current.Get<IEncryptionDbProcess>().GetCertificateSignPdf(context, certificateId.Value, certificatePassword, pdf.FileContent, serverMapPath);
            }

            var positionId = (int)EnumSystemPositions.AdminPosition;
            try
            {
                positionId = context.CurrentPositionId;
            }
            catch { }

            var executorPositionExecutorAgentId = dbContext.DictionaryPositionsSet.Where(x => x.Id == positionId).Select(x => x.ExecutorAgentId).FirstOrDefault().GetValueOrDefault();
            if (executorPositionExecutorAgentId == 0)
                executorPositionExecutorAgentId = context.CurrentAgentId;

            var att = new InternalDocumentAttachedFile
            {
                DocumentId = doc.Id,
                ClientId = doc.ClientId,
                EntityTypeId = doc.EntityTypeId,
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

            var ordInDoc = operationDb.CheckFileForDocument(context, att.DocumentId, att.Name, att.Extension);

            FileLogger.AppendTextToFile(DateTime.Now + " GetDocumentCertificateSignPdf CheckFileForDocument " + ordInDoc.ToString(), @"C:\TEMPLOGS\sign.log");

            if (ordInDoc == -1)
            {
                att.Version = 1;
                att.OrderInDocument = operationDb.GetNextFileOrderNumber(context, att.DocumentId);
                FileLogger.AppendTextToFile(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " GetDocumentCertificateSignPdf GetNextFileOrderNumber ", @"C:\TEMPLOGS\sign.log");
            }
            else
            {
                att.Version = operationDb.GetFileNextVersion(context, att.DocumentId, ordInDoc);
                FileLogger.AppendTextToFile(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " GetDocumentCertificateSignPdf GetFileNextVersion ", @"C:\TEMPLOGS\sign.log");

                att.OrderInDocument = ordInDoc;
            }

            att.LastChangeDate = DateTime.UtcNow;
            att.LastChangeUserId = context.CurrentAgentId;

            fileStore.SaveFile(context, att);
            FileLogger.AppendTextToFile(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " GetDocumentCertificateSignPdf SaveFile ", @"C:\TEMPLOGS\sign.log");

            operationDb.AddNewFileOrVersion(context, att);
            FileLogger.AppendTextToFile(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " GetDocumentCertificateSignPdf AddNewFileOrVersion ", @"C:\TEMPLOGS\sign.log");

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

        public static bool VerifyDocumentCertificateSign(IContext context, string sign, InternalDocument doc, string serverMapPath)
        {
            string stringDocument = GetStringDocumentForDocumentHash(doc, true);

            var res = DmsResolver.Current.Get<IEncryptionDbProcess>().VerifyCertificateSign(context, stringDocument, sign, serverMapPath);

            return res;
        }
        #endregion

        #region TaskAccesses
        public static void ModifyDocumentTaskAccesses(IContext context, int documentId, int? taskId = null)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry0 = dbContext.DocumentEventsSet.Where(x => x.ClientId == context.CurrentClientId)
                .Where(x => x.DocumentId == documentId);
            if (taskId.HasValue)
                qry0 = qry0.Where(x => x.TaskId == taskId);
            else
                qry0 = qry0.Where(x => x.TaskId.HasValue);
            var qry1 = qry0.GroupBy(x => new { x.TaskId, x.SourcePositionId, x.TargetPositionId })
                .Select(x => new { x.Key.TaskId, x.Key.SourcePositionId, x.Key.TargetPositionId }).ToList();
            var qry2 = qry1.GroupBy(x => new { x.TaskId, x.SourcePositionId }).Where(x => x.Key.SourcePositionId.HasValue)
                .Select(x => new { x.Key.TaskId, PositionId = x.Key.SourcePositionId }).ToList();
            var qry3 = qry1.GroupBy(x => new { x.TaskId, x.TargetPositionId }).Where(x => x.Key.TargetPositionId.HasValue)
                .Select(x => new { x.Key.TaskId, PositionId = x.Key.TargetPositionId }).ToList();
            var taNew = qry2.Union(qry3).GroupBy(x => new { x.TaskId, x.PositionId })
                .Select(x => new InternalDocumentTaskAccess { TaskId = x.Key.TaskId.Value, PositionId = x.Key.PositionId.Value }).ToList();

            var taOld = dbContext.DocumentTaskAccessesSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Task.DocumentId == documentId)
                .Select(x => new InternalDocumentTaskAccess { Id = x.Id, TaskId = x.TaskId, PositionId = x.PositionId }).ToList();

            var delId = taOld.GroupJoin(taNew
                , ta1 => new { ta1.TaskId, ta1.PositionId }
                , ta2 => new { ta2.TaskId, ta2.PositionId }
                , (ta1, ta2) => new { ta1.Id, ta2 }).Where(x => x.ta2.Count() == 0).Select(x => x.Id).ToList();

            var insTA = taNew.GroupJoin(taOld
                , ta1 => new { ta1.TaskId, ta1.PositionId }
                , ta2 => new { ta2.TaskId, ta2.PositionId }
                , (ta1, ta2) => new { ta1, ta2 }).Where(x => x.ta2.Count() == 0).Select(x => x.ta1).ToList();

            {
                var filterContains = PredicateBuilder.New<DocumentTaskAccesses>(false);
                filterContains = delId.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.ClientId == context.CurrentClientId).Where(filterContains));
            }
            //TODO DELETE or CLIENT
            dbContext.DocumentTaskAccessesSet.AddRange(insTA.Select(x => new DocumentTaskAccesses { TaskId = x.TaskId, PositionId = x.PositionId }));

        }

        public static List<int> GetEventsSourceTarget(InternalDocumentEvent events)
        {
            return events != null ? GetEventsSourceTarget(new List<InternalDocumentEvent> { events }) : null;
        }
        public static List<int> GetEventsSourceTarget(List<InternalDocumentEvent> events)
        {
            if (!(events?.Any() ?? false)) return null;
            var res = events.Where(x => x.SourcePositionId.HasValue).Select(x => x.SourcePositionId.Value).
                Concat(events.Where(x => x.TargetPositionId.HasValue).Select(x => x.TargetPositionId.Value)).Distinct().ToList();
            return res;
        }
        public static void ModifyDocumentAccessesStatistics(IContext context, int documentId, int positionId)
        {
            ModifyDocumentAccessesStatistics(context, documentId, new List<int> { positionId });
        }
        public static void ModifyDocumentAccessesStatistics(IContext context, int? documentId = null, List<int> positionId = null)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.DocumentAccessesSet.Where(x => x.ClientId == context.CurrentClientId);
            if (documentId.HasValue)
            {
                qry = qry.Where(x => x.DocumentId == documentId);
            }
            if (positionId?.Count() > 0)
            {
                var filterAccessPositionContains = PredicateBuilder.New<DocumentAccesses>(false);
                filterAccessPositionContains = positionId.Aggregate(filterAccessPositionContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(filterAccessPositionContains);
            }
            var qryStat = qry.Select(x => new
            {
                Access = x,
                EventCounts = x.Document.Events.GroupBy(y => true)
                    .Select(y => new
                    {
                        CountNewEvents = y.Count(z => !z.ReadDate.HasValue && z.TargetPositionId.HasValue
                                && z.TargetPositionId != z.SourcePositionId && z.TargetPositionId == x.PositionId),
                    }).FirstOrDefault(),
                WaitCounts = x.Document.Waits.GroupBy(y => true)
                    .Select(y => new
                    {
                        CountWaits = y.Count(z => !z.OffEventId.HasValue
                                && (z.OnEvent.SourcePositionId == x.PositionId || z.OnEvent.TargetPositionId == x.PositionId)),
                        OverDueCountWaits = y.Count(z => !z.OffEventId.HasValue && z.DueDate.HasValue && z.DueDate.Value < DateTime.UtcNow
                                && (z.OnEvent.SourcePositionId == x.PositionId || z.OnEvent.TargetPositionId == x.PositionId)),
                        MinDueDate = y.Where(z => !z.OffEventId.HasValue && z.DueDate.HasValue
                                && (z.OnEvent.SourcePositionId == x.PositionId || z.OnEvent.TargetPositionId == x.PositionId)).Min(z => z.DueDate),
                    }).FirstOrDefault(),
            }
            )
            .Where(x => ((x.Access.CountNewEvents ?? 0) != ((int?)x.EventCounts.CountNewEvents ?? 0)) ||
                        ((x.Access.CountWaits ?? 0) != ((int?)x.WaitCounts.CountWaits ?? 0)) ||
                        ((x.Access.OverDueCountWaits ?? 0) != ((int?)x.WaitCounts.OverDueCountWaits ?? 0)) ||
                        ((x.Access.MinDueDate) != x.WaitCounts.MinDueDate)
            );
            var stat = qryStat.ToList();
            stat.ForEach(x =>
            {
                x.Access.CountNewEvents = x.EventCounts?.CountNewEvents != 0 ? x.EventCounts?.CountNewEvents : (int?)null;
                x.Access.CountWaits = x.WaitCounts?.CountWaits != 0 ? x.WaitCounts?.CountWaits : (int?)null;
                x.Access.OverDueCountWaits = x.WaitCounts?.OverDueCountWaits != 0 ? x.WaitCounts?.OverDueCountWaits : (int?)null;
                x.Access.MinDueDate = x.WaitCounts?.MinDueDate;
            });
            dbContext.SaveChanges();
        }
        #endregion

        #region Certificates
        public static IQueryable<EncryptionCertificates> GetCertificatesQuery(IContext context, FilterEncryptionCertificate filter)
        {
            var dbContext = context.DbContext as DmsContext;
            var qry = dbContext.EncryptionCertificatesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();
            if (!context.IsAdmin)
            {
                qry = qry.Where(x => x.AgentId == context.CurrentAgentId);
            }

            if (filter != null)
            {
                if (filter.CertificateId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.New<EncryptionCertificates>(false);
                    filterContains = filter.CertificateId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AgentId?.Count() > 0 && context.IsAdmin)
                {
                    var filterContains = PredicateBuilder.New<EncryptionCertificates>(false);
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