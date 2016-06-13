using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Database.DBModel.Document;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.InternalModel;
using BL.Model.DocumentCore.ReportModel;
using LinqKit;
using System.Data.Entity;

namespace BL.Database.Documents
{
    internal class DocumentsDbProcess : CoreDb.CoreDb, IDocumentsDbProcess
    {
        public DocumentsDbProcess()
        {
        }

        public void GetCountDocuments(IContext ctx, LicenceInfo licence)
        {
            if (licence==null)
            {
                throw new LicenceError();
            }

            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).AsQueryable();

                licence.CountDocument = qry.Count();
                if (licence.CountDocument > 0)
                    licence.DateFirstDocument = qry.OrderBy(x => x.CreateDate).Select(x => x.CreateDate).FirstOrDefault();
                else
                    licence.DateFirstDocument = DateTime.MinValue;
            }
        }
        public IEnumerable<FrontDocument> GetDocuments(IContext ctx, FilterDocument filters, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            {

                var acc = CommonQueries.GetDocumentAccesses(ctx, dbContext);
                if (filters.IsInWork.HasValue)
                {
                    acc = acc.Where(x => x.IsInWork == filters.IsInWork);
                }
                if (filters.AccessLevelId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<FrontDocumentAccess>();
                    filterContains = filters.AccessLevelId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AccessLevelId == value).Expand());

                    acc = acc.Where(filterContains);
                }
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx, acc);
                qry = qry.Where(x => x.IsRegistered.HasValue);
                #region DocumentsSetFilter

                #region Base

                if (filters.DocumentFromDate.HasValue)
                {
                    qry = qry.Where(x => (x.RegistrationDate ?? x.CreateDate) >= filters.DocumentFromDate.Value);
                }

                if (filters.DocumentToDate.HasValue)
                {
                    qry = qry.Where(x => (x.RegistrationDate ?? x.CreateDate) <= filters.DocumentToDate.Value);
                }

                if (filters.CreateFromDate.HasValue)
                {
                    qry = qry.Where(x => x.CreateDate >= filters.CreateFromDate.Value);
                }

                if (filters.CreateToDate.HasValue)
                {
                    qry = qry.Where(x => x.CreateDate <= filters.CreateToDate.Value);
                }

                if (filters.RegistrationFromDate.HasValue)
                {
                    qry = qry.Where(x => x.RegistrationDate >= filters.RegistrationFromDate.Value);
                }

                if (filters.RegistrationToDate.HasValue)
                {
                    qry = qry.Where(x => x.RegistrationDate <= filters.RegistrationToDate.Value);
                }

                if (filters.SenderFromDate.HasValue)
                {
                    qry = qry.Where(x => x.SenderDate >= filters.SenderFromDate.Value);
                }

                if (filters.SenderToDate.HasValue)
                {
                    qry = qry.Where(x => x.SenderDate <= filters.SenderToDate.Value);
                }

                if (!String.IsNullOrEmpty(filters.Description))
                {
                    qry = qry.Where(x => x.Description.Contains(filters.Description));
                }

                if (!String.IsNullOrEmpty(filters.RegistrationNumber))
                {
                    qry =
                        qry.Where(
                            x =>
                                (
                                x.RegistrationNumber.HasValue
                                ? x.RegistrationNumberPrefix + x.RegistrationNumber.ToString() + x.RegistrationNumberSuffix
                                : "#" + x.Id.ToString()
                                )
                                    .Contains(filters.RegistrationNumber));
                }

                if (!String.IsNullOrEmpty(filters.Addressee))
                {
                    qry = qry.Where(x => x.Addressee.Contains(filters.Addressee));
                }

                if (filters.SenderAgentPersonId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.SenderAgentPersonId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SenderAgentPersonId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filters.SenderNumber))
                {
                    qry = qry.Where(x => x.SenderNumber.Contains(filters.SenderNumber));
                }

                if (filters.DocumentTypeId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.DocumentTypeId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TemplateDocument.DocumentTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.DocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.DocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.TemplateDocumentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.TemplateDocumentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TemplateDocumentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.DocumentDirectionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.DocumentDirectionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TemplateDocument.DocumentDirectionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.DocumentSubjectId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.DocumentSubjectId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentSubjectId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.RegistrationJournalId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.RegistrationJournalId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RegistrationJournalId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.ExecutorPositionId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.ExecutorPositionId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                //TODO Проверить фильтры
                if (filters.ExecutorPositionExecutorAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.ExecutorPositionExecutorAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionExecutorAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.ExecutorDepartmentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.ExecutorDepartmentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPosition.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filters.SenderAgentId?.Count() > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = filters.SenderAgentId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SenderAgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filters.IsRegistered.HasValue)
                {
                    qry = qry.Where(x => x.IsRegistered == filters.IsRegistered.Value);
                }

                if (filters.IsInMyControl ?? false)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.Documents>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ExecutorPositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                #endregion Base

                #region Subscription
                if ((filters.SubscriptionPositionId?.Count() > 0) ||
                    (filters.SubscriptionPositionExecutorAgentId?.Count() > 0) ||
                    (filters.SubscriptionrDepartmentId?.Count() > 0))
                {
                    //TODO Contains
                    var qryTmp = from Doc in qry
                                 join ds in dbContext.DocumentSubscriptionsSet on Doc.Id equals ds.DocumentId
                                 join de in dbContext.DocumentEventsSet on ds.DoneEventId equals de.Id
                                 join dpos in dbContext.DictionaryPositionsSet on de.SourcePositionId equals dpos.Id into dpos
                                 from dp in dpos.DefaultIfEmpty()
                                 where ds.DoneEventId.HasValue && ds.SubscriptionStateId == (int)EnumSubscriptionStates.Sign || ds.SubscriptionStateId == (int)EnumSubscriptionStates.Visa || ds.SubscriptionStateId == (int)EnumSubscriptionStates.Аgreement || ds.SubscriptionStateId == (int)EnumSubscriptionStates.Аpproval
                                 select new { Doc, ds, de, dp };

                    if (filters.SubscriptionPositionId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x => x.de.SourcePositionId.HasValue && filters.SubscriptionPositionId.Contains(x.de.SourcePositionId.Value));
                    }

                    if (filters.SubscriptionPositionExecutorAgentId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x => x.de.SourcePositionExecutorAgentId.HasValue && filters.SubscriptionPositionExecutorAgentId.Contains(x.de.SourcePositionExecutorAgentId.Value));
                    }

                    if (filters.SubscriptionrDepartmentId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x => x.de.SourcePositionId.HasValue && filters.SubscriptionrDepartmentId.Contains(x.dp.DepartmentId));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Subscription

                #region Event
                if (filters.EventIsNew.HasValue
                    || filters.EventFromDate.HasValue
                    || filters.EventToDate.HasValue
                    || (filters.EventTypeId?.Count() > 0)
                    || (filters.EventImportanceEventTypeId?.Count() > 0)
                    || !string.IsNullOrEmpty(filters.EventDescription)
                    || (filters.EventPositionId?.Count() > 0)
                    || (filters.EventPositionExecutorAgentId?.Count() > 0)
                    || (filters.EventAgentId?.Count() > 0))
                {
                    var qryTmp = from Doc in qry
                                 join de in dbContext.DocumentEventsSet on Doc.Id equals de.DocumentId
                                 join det in dbContext.DictionaryEventTypesSet on de.EventTypeId equals det.Id
                                 join SourcePos in dbContext.DictionaryPositionsSet on de.SourcePositionId equals SourcePos.Id into SourcePos
                                 from DSourcePos in SourcePos.DefaultIfEmpty()
                                 join TargetPos in dbContext.DictionaryPositionsSet on de.TargetPositionId equals TargetPos.Id into TargetPos
                                 from DTargetPos in TargetPos.DefaultIfEmpty()
                                 select new { Doc, de, det, DSourcePos, DTargetPos };
                    if (filters.EventIsNew.HasValue)
                    {
                        if (filters.EventIsNew.Value)
                        {
                            qryTmp = qryTmp.Where(x => !x.de.ReadDate.HasValue);
                        }
                        else
                        {
                            qryTmp = qryTmp.Where(x => x.de.ReadDate.HasValue);
                        }
                    }

                    if (filters.EventFromDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.de.CreateDate >= filters.EventFromDate.Value);
                    }

                    if (filters.EventToDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.de.CreateDate <= filters.EventToDate.Value);
                    }

                    if (filters.EventTypeId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x => filters.EventTypeId.Contains(x.de.EventTypeId));
                    }

                    if (filters.EventImportanceEventTypeId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x => filters.EventImportanceEventTypeId.Contains(x.det.ImportanceEventTypeId));
                    }

                    if (!string.IsNullOrEmpty(filters.EventDescription))
                    {
                        qryTmp = qryTmp.Where(x => x.de.Description.Contains(filters.EventDescription));
                    }

                    if (filters.EventPositionId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x =>
                            (x.de.SourcePositionId.HasValue && filters.EventPositionId.Contains(x.de.SourcePositionId.Value))
                            || (x.de.TargetPositionId.HasValue && filters.EventPositionId.Contains(x.de.TargetPositionId.Value)));
                    }

                    if (filters.EventPositionExecutorAgentId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x =>
                            (x.de.SourcePositionExecutorAgentId.HasValue && filters.EventPositionExecutorAgentId.Contains(x.de.SourcePositionExecutorAgentId.Value))
                            || (x.de.TargetPositionExecutorAgentId.HasValue && filters.EventPositionExecutorAgentId.Contains(x.de.TargetPositionExecutorAgentId.Value)));
                    }

                    if (filters.EventDepartmentId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x =>
                            (x.de.SourcePositionId.HasValue && filters.EventDepartmentId.Contains(x.DSourcePos.DepartmentId))
                            || (x.de.TargetPositionId.HasValue && filters.EventDepartmentId.Contains(x.DTargetPos.DepartmentId)));
                    }

                    if (filters.EventAgentId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x =>
                            (x.de.TargetAgentId.HasValue && filters.EventAgentId.Contains(x.de.SourceAgentId.Value))
                            || (x.de.TargetAgentId.HasValue && filters.EventAgentId.Contains(x.de.TargetAgentId.Value)));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Event

                #region Task
                if ((filters.TaskId?.Count() > 0) ||
                    !string.IsNullOrEmpty(filters.TaskDescription))
                {
                    var qryTmp = from Doc in qry
                                 join dt in dbContext.DocumentTasksSet on Doc.Id equals dt.DocumentId
                                 select new { Doc, dt };

                    if (filters.TaskId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x => filters.TaskId.Contains(x.dt.Id));
                    }

                    if (!string.IsNullOrEmpty(filters.TaskDescription))
                    {
                        qryTmp = qryTmp.Where(x => x.dt.Task.Contains(filters.TaskDescription));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Task

                #region Tag
                if ((filters.TagId?.Count() > 0) ||
                    !string.IsNullOrEmpty(filters.TagDescription))
                {
                    //TODO Contains
                    var qryTmp = from Doc in qry
                                 join DocTag in dbContext.DocumentTagsSet on Doc.Id equals DocTag.DocumentId
                                 join DicTag in dbContext.DictionaryTagsSet on DocTag.TagId equals DicTag.Id
                                 where !DicTag.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(DicTag.PositionId ?? 0)
                                 select new { Doc, DocTag, DicTag };

                    if (filters.TagId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x => filters.TagId.Contains(x.DocTag.TagId));
                    }

                    if (!string.IsNullOrEmpty(filters.TagDescription))
                    {
                        qryTmp = qryTmp.Where(x => x.DicTag.Name.Contains(filters.TagDescription));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Tag

                //TODO Перепроверить
                #region Wait
                if (filters.WaitDueDateFromDate.HasValue
                    || filters.WaitDueDateToDate.HasValue
                    || filters.WaitCreateFromDate.HasValue
                    || filters.WaitCreateToDate.HasValue)
                {
                    var qryTmp = from Doc in qry
                                 join Wait in dbContext.DocumentWaitsSet on Doc.Id equals Wait.DocumentId
                                 join WaitOnEvent in dbContext.DocumentEventsSet on Wait.OnEventId equals WaitOnEvent.Id
                                 join WaitOnEventSourcePos in dbContext.DictionaryPositionsSet on WaitOnEvent.SourcePositionId equals WaitOnEventSourcePos.Id into WaitOnEventSourcePos
                                 from DWaitOnEventSourcePos in WaitOnEventSourcePos.DefaultIfEmpty()
                                 join WaitOnEventTargetPos in dbContext.DictionaryPositionsSet on WaitOnEvent.TargetPositionId equals WaitOnEventTargetPos.Id into WaitOnEventTargetPos
                                 from DWaitOnEventTargetPos in WaitOnEventTargetPos.DefaultIfEmpty()
                                 select new { Doc, Wait, WaitOnEvent, DWaitOnEventSourcePos, DWaitOnEventTargetPos };

                    if (filters.WaitDueDateFromDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.Wait.DueDate >= filters.WaitDueDateFromDate.Value);
                    }

                    if (filters.WaitDueDateToDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.Wait.DueDate <= filters.WaitDueDateToDate.Value);
                    }

                    if (filters.WaitCreateFromDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.WaitOnEvent.Date >= filters.WaitCreateFromDate.Value);
                    }

                    if (filters.WaitCreateToDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.WaitOnEvent.Date <= filters.WaitCreateToDate.Value);
                    }

                    #region Вид контроля

                    if ((filters.WaitControlToMePositionId?.Count() > 0)
                        || (filters.WaitControlToMePositionExecutorAgentId?.Count() > 0)
                        || (filters.WaitControlToMeDepartmentId?.Count() > 0)

                        || (filters.WaitControlFromMePositionId?.Count() > 0)
                        || (filters.WaitControlFromMePositionExecutorAgentId?.Count() > 0)
                        || (filters.WaitControlFromMeDepartmentId?.Count() > 0)
                        || (filters.WaitControlFromMeAgentId?.Count() > 0)

                        || (filters.WaitIsSelfControl ?? false)
                        || (filters.WaitIsVisaingToMe ?? false)
                        || (filters.WaitIsVisaingFromMe ?? false)
                        || (filters.WaitIsMarkExecution ?? false)
                        )
                    {
                        bool isFirstUnion = true;
                        var qryTmpUnion = qryTmp;

                        #region Чужой контроль исполнения  SendForExecution, SendForResponsibleExecution
                        if ((filters.WaitControlToMePositionId?.Count() > 0)
                            || (filters.WaitControlToMePositionExecutorAgentId?.Count() > 0)
                            || (filters.WaitControlToMeDepartmentId?.Count() > 0))
                        {
                            //TODO Contains
                            var qryTmpControl = qryTmp.Where(x =>
                                (x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForExecution
                                || x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution)
                                && !x.Wait.OffEventId.HasValue)
                                 .Where(x =>
                                     ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.TargetPositionId ?? 0)
                                     && !ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.SourcePositionId ?? 0));

                            if ((filters.WaitControlToMePositionId?.Count() > 0))
                            {
                                //TODO Contains
                                qryTmpControl = qryTmpControl.Where(x => x.WaitOnEvent.SourcePositionId.HasValue && filters.WaitControlToMePositionId.Contains(x.WaitOnEvent.SourcePositionId.Value));
                            }

                            if ((filters.WaitControlToMePositionExecutorAgentId?.Count() > 0))
                            {
                                //TODO Contains
                                qryTmpControl = qryTmpControl.Where(x => x.WaitOnEvent.SourcePositionExecutorAgentId.HasValue && filters.WaitControlToMePositionExecutorAgentId.Contains(x.WaitOnEvent.SourcePositionExecutorAgentId.Value));
                            }

                            if ((filters.WaitControlToMeDepartmentId?.Count() > 0))
                            {
                                //TODO Contains
                                qryTmpControl = qryTmpControl.Where(x => x.WaitOnEvent.SourcePositionId.HasValue
                                    && filters.WaitControlToMeDepartmentId.Contains(x.DWaitOnEventSourcePos.DepartmentId));
                            }


                            if (isFirstUnion)
                            {
                                isFirstUnion = false;
                                qryTmpUnion = qryTmpControl;
                            }
                            else
                            {
                                qryTmpUnion = qryTmpUnion.Union(qryTmpControl);
                            }
                        }

                        #endregion Чужой контроль исполнения  SendForExecution, SendForResponsibleExecution

                        #region Собственный контроль исполнения SendForExecution, SendForResponsibleExecution

                        if ((filters.WaitControlFromMePositionId?.Count() > 0)
                            || (filters.WaitControlFromMePositionExecutorAgentId?.Count() > 0)
                            || (filters.WaitControlFromMeDepartmentId?.Count() > 0)
                            || (filters.WaitControlFromMeAgentId?.Count() > 0))
                        {
                            //TODO Contains
                            var qryTmpControl = qryTmp.Where(x =>
                                (x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForExecution
                                || x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForResponsibleExecution)
                                && !x.Wait.OffEventId.HasValue)
                                 .Where(x =>
                                     !ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.TargetPositionId ?? 0)
                                     && ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.SourcePositionId ?? 0));

                            if ((filters.WaitControlFromMePositionId?.Count() > 0))
                            {
                                //TODO Contains
                                qryTmpControl = qryTmpControl.Where(x => x.WaitOnEvent.TargetPositionId.HasValue && filters.WaitControlFromMePositionId.Contains(x.WaitOnEvent.TargetPositionId.Value));
                            }

                            if ((filters.WaitControlFromMePositionExecutorAgentId?.Count() > 0))
                            {
                                //TODO Contains
                                qryTmpControl = qryTmpControl.Where(x => x.WaitOnEvent.TargetPositionExecutorAgentId.HasValue
                                    && filters.WaitControlFromMePositionExecutorAgentId.Contains(x.WaitOnEvent.TargetPositionExecutorAgentId.Value));
                            }

                            if ((filters.WaitControlFromMeDepartmentId?.Count() > 0))
                            {
                                //TODO Contains
                                qryTmpControl = qryTmpControl.Where(x => x.WaitOnEvent.TargetPositionId.HasValue
                                    && filters.WaitControlFromMeDepartmentId.Contains(x.DWaitOnEventTargetPos.DepartmentId));
                            }

                            if ((filters.WaitControlFromMeAgentId?.Count() > 0))
                            {
                                //TODO Contains
                                qryTmpControl = qryTmpControl.Where(x => x.WaitOnEvent.TargetAgentId.HasValue
                                    && filters.WaitControlFromMeAgentId.Contains(x.WaitOnEvent.TargetAgentId.Value));
                            }


                            if (isFirstUnion)
                            {
                                isFirstUnion = false;
                                qryTmpUnion = qryTmpControl;
                            }
                            else
                            {
                                qryTmpUnion = qryTmpUnion.Union(qryTmpControl);
                            }
                        }
                        #endregion Собственный контроль исполнения SendForExecution, SendForResponsibleExecution

                        #region Самоконтроль ControlOn

                        if (filters.WaitIsSelfControl ?? false)
                        {
                            //TODO Contains
                            var qryTmpControl = qryTmp.Where(x =>
                                x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.ControlOn
                                && !x.Wait.OffEventId.HasValue)
                                 .Where(x =>
                                     ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.TargetPositionId ?? 0)
                                     && ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.SourcePositionId ?? 0));

                            if (isFirstUnion)
                            {
                                isFirstUnion = false;
                                qryTmpUnion = qryTmpControl;
                            }
                            else
                            {
                                qryTmpUnion = qryTmpUnion.Union(qryTmpControl);
                            }
                        }
                        #endregion Самоконтроль ControlOn

                        #region Поступившие на визирование SendForVisaing, SendForАgreement, SendForАpproval, SendForSigning

                        if (filters.WaitIsVisaingToMe ?? false)
                        {
                            //TODO Contains
                            var qryTmpControl = qryTmp.Where(x =>
                                (x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForVisaing
                                || x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForАgreement
                                || x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForАpproval
                                || x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForSigning)
                                && !x.Wait.OffEventId.HasValue)
                                 .Where(x =>
                                     ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.TargetPositionId ?? 0)
                                     && !ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.SourcePositionId ?? 0));

                            if (isFirstUnion)
                            {
                                isFirstUnion = false;
                                qryTmpUnion = qryTmpControl;
                            }
                            else
                            {
                                qryTmpUnion = qryTmpUnion.Union(qryTmpControl);
                            }
                        }
                        #endregion Поступившие на визирование SendForVisaing, SendForАgreement, SendForАpproval, SendForSigning

                        #region Отправленные на визирование SendForVisaing, SendForАgreement, SendForАpproval, SendForSigning

                        if (filters.WaitIsVisaingFromMe ?? false)
                        {
                            //TODO Contains
                            var qryTmpControl = qryTmp.Where(x =>
                                (x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForVisaing
                                || x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForАgreement
                                || x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForАpproval
                                || x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.SendForSigning)
                                && !x.Wait.OffEventId.HasValue)
                                 .Where(x =>
                                     !ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.TargetPositionId ?? 0)
                                     && ctx.CurrentPositionsIdList.Contains(x.WaitOnEvent.SourcePositionId ?? 0));

                            if (isFirstUnion)
                            {
                                isFirstUnion = false;
                                qryTmpUnion = qryTmpControl;
                            }
                            else
                            {
                                qryTmpUnion = qryTmpUnion.Union(qryTmpControl);
                            }
                        }
                        #endregion Отправленные на визирование SendForVisaing, SendForАgreement, SendForАpproval, SendForSigning

                        #region Отчеты о выполнении MarkExecution

                        if (filters.WaitIsMarkExecution ?? false)
                        {
                            var qryTmpControl = qryTmp.Where(x =>
                                x.WaitOnEvent.EventTypeId == (int)EnumEventTypes.MarkExecution
                                && !x.Wait.OffEventId.HasValue);

                            if (isFirstUnion)
                            {
                                isFirstUnion = false;
                                qryTmpUnion = qryTmpControl;
                            }
                            else
                            {
                                qryTmpUnion = qryTmpUnion.Union(qryTmpControl);
                            }
                        }
                        #endregion Отчеты о выполнении MarkExecution

                        qryTmp = qryTmpUnion;
                    }
                    #endregion Вид контроля

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion Wait

                #region File
                if (!string.IsNullOrEmpty(filters.FileName)
                    || !string.IsNullOrEmpty(filters.FileExtension)
                    || filters.FileSizeFrom.HasValue
                    || filters.FileSizeTo.HasValue
                    || filters.FileCreateFromDate.HasValue
                    || filters.FileCreateToDate.HasValue
                    || (filters.FileAgentId?.Count() > 0))
                {
                    var qryTmp = (from Doc in qry
                                  join DocFile in dbContext.DocumentFilesSet on Doc.Id equals DocFile.DocumentId
                                  select new { Doc, DocFile })
                                 .GroupBy(g => new { g.Doc, g.DocFile.OrderNumber })
                                 .Select(x => new { Doc = x.Key.Doc, DocFile = x.OrderByDescending(f => f.DocFile.Version).First().DocFile });

                    if (!string.IsNullOrEmpty(filters.FileName))
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.Name.Contains(filters.FileName));
                    }

                    if (!string.IsNullOrEmpty(filters.FileExtension))
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.Extension.Contains(filters.FileExtension));
                    }
                    if (filters.FileSizeFrom.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.FileSize >= filters.FileSizeFrom);
                    }
                    if (filters.FileSizeTo.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.FileSize <= filters.FileSizeTo);
                    }
                    if (filters.FileCreateFromDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.Date >= filters.FileCreateFromDate.Value);
                    }
                    if (filters.FileCreateToDate.HasValue)
                    {
                        qryTmp = qryTmp.Where(x => x.DocFile.Date <= filters.FileCreateToDate);
                    }
                    if (filters.FileAgentId?.Count() > 0)
                    {
                        //TODO Contains
                        qryTmp = qryTmp.Where(x => filters.FileAgentId.Contains(x.DocFile.LastChangeUserId));
                    }

                    qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                }
                #endregion File

                #region Property
                if (filters.FilterProperties?.Count() > 0)
                {
                    foreach (var filterProperty in filters.FilterProperties)
                    {
                        var qryTmp = from Doc in qry
                                     join pv in dbContext.PropertyValuesSet on new { RecordId = Doc.Id, PropertyLinkId = filterProperty.PropertyLinkId }
                                        equals new { RecordId = pv.RecordId, PropertyLinkId = pv.PropertyLinkId }
                                     where pv.PropertyLink.Property.ClientId == ctx.CurrentClientId
                                     select new { Doc, pv };
                        switch (filterProperty.ValueType)
                        {
                            case EnumValueTypes.Text:
                                qryTmp = qryTmp.Where(x => x.pv.ValueString.Contains(filterProperty.Text));
                                break;
                            case EnumValueTypes.Number:
                                if (filterProperty.NumberFrom.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.NumberFrom <= x.pv.ValueNumeric);
                                }
                                if (filterProperty.NumberTo.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.NumberTo >= x.pv.ValueNumeric);
                                }
                                break;
                            case EnumValueTypes.Date:
                                if (filterProperty.DateFrom.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.DateFrom <= x.pv.ValueDate);
                                }
                                if (filterProperty.DateTo.HasValue)
                                {
                                    qryTmp = qryTmp.Where(x => filterProperty.DateTo >= x.pv.ValueDate);
                                }
                                break;
                            case EnumValueTypes.Api:
                                if (!(filterProperty.Ids?.Count() > 0))
                                {
                                    filterProperty.Ids = new List<int>();
                                }
                                //TODO Contains
                                var ids = filterProperty.Ids.Select(y => (double?)y).ToList();
                                qryTmp = qryTmp.Where(x => ids.Contains(x.pv.ValueNumeric));
                                break;
                        }
                        qry = qryTmp.GroupBy(x => x.Doc).Select(x => x.Key);
                    }
                }

                #endregion Property

                #endregion DocumentsSetFilter

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.OrderByDescending(x => x.CreateDate)
                            .Skip(() => skip).Take(() => take);
                    }
                }

                #region Counter
                var filterOnEventPositionsContains = PredicateBuilder.False<DocumentWaits>();
                filterOnEventPositionsContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventPositionsContains,
                    (current, value) => current.Or(e => e.OnEvent.TargetPositionId == value || e.OnEvent.SourcePositionId == value).Expand());

                var filterOnEventTaskAccessesContains = PredicateBuilder.False<DocumentTaskAccesses>();
                filterOnEventTaskAccessesContains = ctx.CurrentPositionsIdList.Aggregate(filterOnEventTaskAccessesContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                var filterNewEventContains = PredicateBuilder.False<DocumentEvents>();
                filterNewEventContains = ctx.CurrentPositionsIdList.Aggregate(filterNewEventContains,
                    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());
                #endregion Counter

                var res = qry.Select(doc => new FrontDocument
                {
                    Id = doc.Id,
                    DocumentDirectionName = doc.TemplateDocument.DocumentDirection.Name,
                    DocumentTypeName = doc.TemplateDocument.DocumentType.Name,

                    RegistrationNumber = doc.RegistrationNumber,
                    RegistrationNumberPrefix = doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = doc.RegistrationNumberSuffix,

                    DocumentDate = doc.RegistrationDate ?? doc.CreateDate,
                    IsRegistered = doc.IsRegistered,
                    IsLaunchPlan = doc.IsLaunchPlan,
                    Description = doc.Description,
                    ExecutorPositionExecutorAgentName = doc.ExecutorPositionExecutorAgent.Name,
                    ExecutorPositionName = doc.ExecutorPosition.Name,

                    WaitCount = doc.Waits.AsQueryable().Where(x => !x.OnEvent.IsAvailableWithinTask).Where(filterOnEventPositionsContains)
                        .Concat(doc.Waits.AsQueryable().Where(x => x.OnEvent.IsAvailableWithinTask && x.OnEvent.TaskId.HasValue &&
                            x.OnEvent.Task.TaskAccesses.AsQueryable().Any(filterOnEventTaskAccessesContains)))
                            .GroupBy(x => x.DocumentId)
                            .Select(x => new UICounters { Counter1 = x.Count(), Counter2 = x.Count(s => s.DueDate.HasValue && s.DueDate.Value < DateTime.Now) })
                            .FirstOrDefault(),

                    NewEventCount = doc.Events.AsQueryable().Where(filterNewEventContains).Count(x => !x.ReadDate.HasValue && x.TargetPositionId != x.SourcePositionId),

                    AttachedFilesCount = doc.Files.GroupBy(g => g.OrderNumber).Count(),

                    LinkedDocumentsCount = doc.Links
                    .GroupBy(x => x.LinkId)
                    .Select(x => x.Count())
                    .Select(x => x < 2 ? 0 : x - 1).FirstOrDefault()

                });

                var docs = res.ToList();

                docs.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

                {
                    var filterContains = PredicateBuilder.False<FrontDocumentAccess>();
                    filterContains = docs.Select(x=>x.Id).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    var accs = acc.Where(filterContains).ToList();

                    foreach (var doc in docs)
                    {
                        doc.Accesses = accs.Where(x => x.DocumentId == doc.Id).ToList();
                        doc.IsFavourite = doc.Accesses.Any(x => x.IsFavourite);
                        doc.IsInWork = doc.Accesses.Any(x => x.IsInWork);
                        //doc.AccessLevel = doc.Accesses.Max(x => x.AccessLevel);
                        //doc.AccessLevelName = doc.Accesses.FirstOrDefault(x => x.AccessLevel == doc.AccessLevel).AccessLevelName;
                    }
                }

                docs.GroupJoin(CommonQueries.GetDocumentTags(dbContext, ctx, new FilterDocumentTag { DocumentId = docs.Select(x => x.Id).ToList(), CurrentPositionsId = ctx.CurrentPositionsIdList }),
                    d => d.Id,
                    t => t.DocumentId,
                    (d, t) => d.DocumentTags = t);

                return docs;
            }
        }

        public FrontDocument GetDocument(IContext ctx, int documentId, FilterDocumentById filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId);

                var accs = CommonQueries.GetDocumentAccesses(ctx, dbContext).Where(x => x.DocumentId == documentId).ToList();

                var res = qry.Select(doc => new FrontDocument
                {
                    Id = doc.Id,
                    DocumentDirection = (EnumDocumentDirections)doc.TemplateDocument.DocumentDirectionId,
                    DocumentDirectionName = doc.TemplateDocument.DocumentDirection.Name,
                    DocumentTypeName = doc.TemplateDocument.DocumentType.Name,

                    DocumentDate = doc.RegistrationDate ?? doc.CreateDate,
                    IsRegistered = doc.IsRegistered,
                    Description = doc.Description,
                    ExecutorPositionExecutorAgentName = doc.ExecutorPositionExecutorAgent.Name,
                    ExecutorPositionName = doc.ExecutorPosition.Name,
                    LinkedDocumentsCount = 0, //TODO

                    TemplateDocumentId = doc.TemplateDocumentId,
                    DocumentSubjectId = doc.DocumentSubjectId,
                    DocumentSubjectName = doc.DocumentSubject.Name,

                    RegistrationJournalId = doc.RegistrationJournalId,
                    RegistrationJournalName = doc.RegistrationJournal.Name,
                    RegistrationNumber = doc.RegistrationNumber,
                    RegistrationNumberPrefix = doc.RegistrationNumberPrefix,
                    RegistrationNumberSuffix = doc.RegistrationNumberSuffix,
                    RegistrationDate = doc.RegistrationDate,

                    ExecutorPositionId = doc.ExecutorPositionId,
                    ExecutorPositionExecutorNowAgentName = doc.ExecutorPosition.ExecutorAgent.Name,
                    ExecutorPositionAgentPhoneNumber = "(888)888-88-88", //TODO

                    SenderAgentId = doc.SenderAgentId,
                    SenderAgentName = doc.SenderAgent.Name,
                    SenderAgentPersonId = doc.SenderAgentPersonId,
                    SenderAgentPersonName = doc.SenderAgentPerson.Agent.Name,
                    SenderNumber = doc.SenderNumber,
                    SenderDate = doc.SenderDate,
                    Addressee = doc.Addressee,

                    IsLaunchPlan = doc.IsLaunchPlan,
                    TemplateDocumentName = doc.TemplateDocument.Name,
                    IsHard = doc.TemplateDocument.IsHard,
                    LinkId = doc.LinkId,

                }).FirstOrDefault();

                if (res == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }

                var accByExecutorPosition = accs.Where(x => x.PositionId == res.ExecutorPositionId).FirstOrDefault();
                if (accByExecutorPosition != null)
                {
                    res.AccessLevelId = accByExecutorPosition.AccessLevelId;
                    res.AccessLevelName = accByExecutorPosition.AccessLevelName;
                }
                res.IsFavourite = accs.Any(x => x.IsFavourite);
                res.IsInWork = accs.Any(x => x.IsInWork);
                res.Accesses = accs;

                CommonQueries.ChangeRegistrationFullNumber(res, false);

                var docIds = new List<int> { res.Id };

                if (res.LinkId.HasValue)
                {
                    res.LinkedDocuments = CommonQueries.GetLinkedDocuments(ctx, dbContext, res.LinkId.Value);
                    var linkedDocumentsCount = res.LinkedDocuments.Count();
                    res.LinkedDocumentsCount = linkedDocumentsCount < 2 ? 0 : linkedDocumentsCount - 1;

                    if ((filter?.DocumentsIdForAIP != null) && (filter.DocumentsIdForAIP.Any()))
                    {
                        docIds = filter?.DocumentsIdForAIP;
                    }
                }

                res.SendLists = CommonQueries.GetDocumentSendList(dbContext, ctx, new FilterDocumentSendList { DocumentId = new List<int> { documentId } });

                res.SendListStageMax = (res.SendLists == null) || !res.SendLists.Any() ? 0 : res.SendLists.Max(x => x.Stage);

                res.RestrictedSendLists = CommonQueries.GetDocumentRestrictedSendList(dbContext, ctx, new FilterDocumentRestrictedSendList { DocumentId = new List<int> { documentId } });

                res.DocumentTags = CommonQueries.GetDocumentTags(dbContext, ctx, new FilterDocumentTag { DocumentId = docIds, CurrentPositionsId = ctx.CurrentPositionsIdList });

                res.DocumentWorkGroup = CommonQueries.GetDocumentWorkGroup(dbContext, ctx, new FilterDictionaryPosition { DocumentIDs = docIds });

                res.Properties = CommonQueries.GetPropertyValues(dbContext, ctx, new FilterPropertyValue { RecordId = new List<int> { documentId }, Object = new List<EnumObjects> { EnumObjects.Documents } });

                return res;
            }
        }

        public InternalDocument ReportRegistrationCardDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId);

                var doc = qry.Select(x => new InternalDocument
                {
                    Id = x.Id,
                    DocumentDirection = (EnumDocumentDirections)x.TemplateDocument.DocumentDirectionId,
                    IsRegistered = x.IsRegistered,
                    ExecutorPositionId = x.ExecutorPositionId,
                }).FirstOrDefault();

                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }

                return doc;
            }
        }

        public ReportDocument ReportRegistrationCardDocument(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Id == documentId);

                var doc = qry.FirstOrDefault();
                if (doc == null)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                var accs =
                    CommonQueries.GetDocumentAccesses(ctx, dbContext).Where(x => x.DocumentId == doc.Id).ToList();

                var res = new ReportDocument
                {
                    Id = doc.Id,
                    DocumentTypeName = doc.TemplateDocument.DocumentType.Name,
                    ExecutorPositionName = doc.ExecutorPosition.Name,
                    Addressee = doc.Addressee,
                    Description = doc.Description,
                    SenderAgentName = doc.SenderAgent.Name,
                    SenderAgentPersonName = doc.SenderAgentPerson.Agent.Name,
                };

                var docIds = new List<int> { res.Id };

                res.DocumentWaits = CommonQueries.GetDocumentWaitsQuery(dbContext, ctx, res.Id)
                    .Select(x => new ReportDocumentWait
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        CreateDate = x.OnEvent.Date,
                        TargetPositionName = x.OnEvent.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.OnEvent.TargetPositionExecutorAgent.Name,
                        SourcePositionName = x.OnEvent.SourcePosition.Name,
                        SourcePositionExecutorAgentName = x.OnEvent.SourcePositionExecutorAgent.Name,
                        DueDate = x.DueDate,
                        IsClosed = x.OffEventId != null,
                        ResultTypeName = x.ResultType.Name,
                        AttentionDate = x.AttentionDate,
                        OnEventTypeName = x.OnEvent.EventType.Name,
                        OffEventDate = x.OffEventId.HasValue ? x.OffEvent.CreateDate : (DateTime?)null
                    }).ToList();

                res.DocumentSubscriptions = CommonQueries.GetDocumentSubscriptionsQuery(dbContext, new FilterDocumentSubscription { DocumentId = new List<int> { res.Id }, SubscriptionStates = new List<EnumSubscriptionStates> { EnumSubscriptionStates.Sign } }, ctx)
                    .Select(x => new ReportDocumentSubscription
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        SubscriptionStatesId = x.SubscriptionStateId,
                        SubscriptionStatesName = x.SubscriptionState.Name,
                        DoneEventSourcePositionName = x.DoneEventId.HasValue ? x.DoneEvent.SourcePosition.Name : string.Empty,
                        DoneEventSourcePositionExecutorAgentName = x.DoneEventId.HasValue ? x.DoneEvent.SourcePositionExecutorAgent.Name : string.Empty
                    }).ToList();

                return res;
            }
        }

        //public InternalDocument ReportTransmissionDocumentPaperEventPrepare(IContext ctx, int documentId)
        //{
        //    using (var dbContext = new DmsContext(ctx))
        //    {
        //        var qry = CommonQueries.GetDocumentQuery(dbContext, ctx).Where(x => x.Doc.Id == documentId);

        //        var doc = qry.Select(x => new InternalDocument
        //        {
        //            Id = x.Doc.Id,
        //            ExecutorPositionId = x.Doc.ExecutorPositionId,
        //        }).FirstOrDefault();

        //        if (doc == null)
        //        {
        //            throw new DocumentNotFoundOrUserHasNoAccess();
        //        }

        //        return doc;
        //    }
        //}

        public List<ReportDocument> ReportRegisterTransmissionDocuments(IContext ctx, int paperListId)
        {
            using (var dbContext = new DmsContext(ctx))
            {

                var qry = dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.PaperListId == paperListId);

                var res = qry.GroupBy(x => x.Document)
                    .Select(x => x.Key)
                    .Select(x => new ReportDocument
                    {
                        Id = x.Id,
                        RegistrationNumber = x.RegistrationNumber,
                        RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                        Description = x.Description
                        //DocumentTypeName = x.DocTypeName,
                        //ExecutorPositionName = x.ExecutorPosName,
                        //Addressee = x.Doc.Addressee,
                        //SenderAgentName = doc.SenderAgentname,
                        //SenderAgentPersonName = doc.SenderPersonName,
                    }).ToList();

                res.ForEach(x => CommonQueries.ChangeRegistrationFullNumber(x));

                var events = qry.Select(x => new ReportDocumentEvent
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    SourcePositionName = x.SourcePosition.Name,
                    SourcePositionExecutorAgentName = x.SourcePositionExecutorAgent.Name,
                    TargetPositionName = x.TargetPosition.Name,
                    TargetPositionExecutorAgentName = x.TargetPositionExecutorAgent.Name,
                    PaperId = x.PaperId,
                    Paper = !x.PaperId.HasValue
                        ? null
                        : new ReportDocumentPaper
                        {
                            Id = x.Paper.Id,
                            DocumentId = x.Paper.DocumentId,
                            Name = x.Paper.Name,
                            Description = x.Paper.Description
                        }
                }).ToList();

                res = res.GroupJoin(events, o => o.Id, i => i.DocumentId, (o, i) => { o.DocumentEvents = i.ToList(); return o; }).ToList();

                return res;
            }
        }

        public InternalDocument AddDocumentPrepare(IContext context, int templateDocumentId)
        {
            using (var dbContext = new DmsContext(context))
            {

                var doc = dbContext.TemplateDocumentsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == templateDocumentId)
                    .Select(x => new InternalDocument
                    {
                        TemplateDocumentId = x.Id,
                        DocumentSubjectId = x.DocumentSubjectId,
                        Description = x.Description,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        Addressee = x.Addressee,
                        DocumentTypeId = x.DocumentTypeId,
                        DocumentDirection = (EnumDocumentDirections)x.DocumentDirectionId,
                        RegistrationJournalId = x.RegistrationJournalId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                doc.Tasks = dbContext.TemplateDocumentTasksSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentTask()
                    {
                        Name = y.Task,
                        Description = y.Description,
                        PositionId = y.PositionId ?? 0,
                    }).ToList();

                doc.RestrictedSendLists = dbContext.TemplateDocumentRestrictedSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentRestrictedSendList()
                    {
                        PositionId = y.PositionId,
                        AccessLevel = (EnumDocumentAccesses)y.AccessLevelId
                    }).ToList();

                doc.SendLists = dbContext.TemplateDocumentSendListsSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(y => y.DocumentId == templateDocumentId)
                    .Select(y => new InternalDocumentSendList()
                    {
                        SendType = (EnumSendTypes)y.SendTypeId,
                        SourcePositionId = y.SourcePositionId ?? 0,
                        TargetPositionId = y.TargetPositionId,
                        TargetAgentId = y.TargetAgentId,
                        TaskName = y.Task.Task,
                        IsAvailableWithinTask = y.IsAvailableWithinTask,
                        IsWorkGroup = y.IsWorkGroup,
                        IsAddControl = y.IsAddControl,
                        SelfDueDate = y.SelfDueDate,
                        SelfDueDay = y.SelfDueDay,
                        SelfAttentionDate = y.SelfAttentionDate,
                        Description = y.Description,
                        Stage = y.Stage,
                        DueDay = y.DueDay,
                        AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                    }).ToList();

                doc.DocumentFiles = dbContext.TemplateDocumentFilesSet.Where(x => x.Document.ClientId == context.CurrentClientId).Where(x => x.DocumentId == templateDocumentId).Select(x => new InternalDocumentAttachedFile
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    Extension = x.Extention,
                    Name = x.Name,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                    OrderInDocument = x.OrderNumber,
                    IsAdditional = x.IsAdditional,
                    Hash = x.Hash
                }).ToList();

                doc.Properties = CommonQueries.GetInternalPropertyValues(dbContext, context, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.TemplateDocument }, RecordId = new List<int> { templateDocumentId } }).ToList();

                return doc;
            }
        }

        public InternalDocument CopyDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        TemplateDocumentId = x.TemplateDocumentId,
                        DocumentSubjectId = x.DocumentSubjectId,
                        Description = x.Description,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        Addressee = x.Addressee,

                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                doc.AccessLevel = (EnumDocumentAccesses)CommonQueries.GetDocumentAccessesesQry(dbContext, documentId, ctx).Max(x => x.AccessLevelId);
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId)
                        .Select(x => new InternalDocumentTask
                        {
                            Name = x.Task,
                            Description = x.Description,
                            PositionId = x.PositionId,
                        }
                        ).ToList();
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId && x.IsInitial)
                        .Select(y => new InternalDocumentSendList
                        {
                            Stage = y.Stage,
                            SendType = (EnumSendTypes)y.SendTypeId,
                            SourcePositionId = y.SourcePositionId,
                            TargetPositionId = y.TargetPositionId,
                            TargetAgentId = y.TargetAgentId,
                            TaskName = y.Task.Task,
                            IsAvailableWithinTask = y.IsAvailableWithinTask,
                            IsWorkGroup = y.IsWorkGroup,
                            IsAddControl = y.IsAddControl,
                            SelfDueDate = y.SelfDueDate,
                            SelfDueDay = y.SelfDueDay,
                            SelfAttentionDate = y.SelfAttentionDate,
                            Description = y.Description,
                            DueDate = y.DueDate,
                            DueDay = y.DueDay,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                            IsInitial = y.IsInitial,
                        }).ToList();
                doc.RestrictedSendLists = dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId)
                        .Select(y => new InternalDocumentRestrictedSendList
                        {
                            PositionId = y.PositionId,
                            AccessLevel = (EnumDocumentAccesses)y.AccessLevelId,
                        }).ToList();
                doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, documentId);

                doc.Properties = CommonQueries.GetInternalPropertyValues(dbContext, ctx, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.Documents }, RecordId = new List<int> { documentId } }).ToList();

                return doc;
            }
        }

        public void AddDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var doc = ModelConverter.GetDbDocument(document);

                    if (document.Accesses != null && document.Accesses.Any())
                    {
                        doc.Accesses = ModelConverter.GetDbDocumentAccesses(document.Accesses).ToList();
                    }

                    if (document.Events != null && document.Events.Any())
                    {
                        doc.Events = ModelConverter.GetDbDocumentEvents(document.Events).ToList();
                    }

                    if (document.RestrictedSendLists != null && document.RestrictedSendLists.Any())
                    {
                        doc.RestrictedSendLists = ModelConverter.GetDbDocumentRestrictedSendLists(document.RestrictedSendLists).ToList();
                    }

                    if (document.DocumentFiles != null && document.DocumentFiles.Any())
                    {
                        doc.Files = ModelConverter.GetDbDocumentFiles(document.DocumentFiles).ToList();
                    }

                    if (document.Tasks?.Any(x => x.Id == 0) ?? false)
                    {
                        doc.Tasks = ModelConverter.GetDbDocumentTasks(document.Tasks.Where(x => x.Id == 0)).ToList();
                    }

                    dbContext.DocumentsSet.Add(doc);
                    dbContext.SaveChanges();

                    if (document.SendLists != null && document.SendLists.Any())
                    {
                        var sendLists = document.SendLists.ToList();
                        sendLists.ForEach(x =>
                        {
                            x.DocumentId = doc.Id;
                            var taskId = doc.Tasks.Where(y => y.Task == x.TaskName).Select(y => y.Id).FirstOrDefault();
                            x.TaskId = (taskId == 0 ? null : (int?)taskId);
                        });
                        var sendListsDb = ModelConverter.GetDbDocumentSendLists(sendLists).ToList();
                        dbContext.DocumentSendListsSet.AddRange(sendListsDb);
                        dbContext.SaveChanges();
                    }

                    if (document.Properties?.Any() ?? false)
                    {
                        document.Properties.ToList().ForEach(x => { x.RecordId = doc.Id; });
                        var propertyValues = ModelConverter.GetDbPropertyValue(document.Properties).ToList();
                        dbContext.PropertyValuesSet.AddRange(propertyValues);
                        dbContext.SaveChanges();
                    }

                    document.Id = doc.Id;

                    //TODO we schould check if it needed or not? 
                    if (document.DocumentFiles != null)
                        foreach (var fl in document.DocumentFiles)
                        {
                            fl.DocumentId = doc.Id;
                        }

                    CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumObjects.Documents, EnumOperationType.AddNew);
                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public InternalDocument ModifyDocumentPrepare(IContext ctx, ModifyDocument model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == model.Id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        TemplateDocumentId = x.TemplateDocumentId,
                        IsHard = x.TemplateDocument.IsHard,
                        DocumentDirection = (EnumDocumentDirections)x.TemplateDocument.DocumentDirectionId,
                        IsRegistered = x.IsRegistered,
                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.Accesses = dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.Id && x.PositionId == doc.ExecutorPositionId && x.AccessLevelId != model.AccessLevelId)
                    .Select(x => new InternalDocumentAccess
                    {
                        Id = x.Id,
                        AccessLevel = (EnumDocumentAccesses)x.AccessLevelId
                    }).ToList();
                return doc;
            }
        }

        public void ModifyDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = new DBModel.Document.Documents
                {
                    Id = document.Id,
                    DocumentSubjectId = document.DocumentSubjectId,
                    Description = document.Description,
                    SenderAgentId = document.SenderAgentId,
                    SenderAgentPersonId = document.SenderAgentPersonId,
                    SenderNumber = document.SenderNumber,
                    SenderDate = document.SenderDate,
                    Addressee = document.Addressee,
                    LastChangeDate = document.LastChangeDate,
                    LastChangeUserId = document.LastChangeUserId,
                    IsRegistered = document.IsRegistered,
                };
                dbContext.DocumentsSet.Attach(doc);
                var entry = dbContext.Entry(doc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.DocumentSubjectId).IsModified = true;
                entry.Property(x => x.Description).IsModified = true;
                entry.Property(x => x.SenderAgentId).IsModified = true;
                entry.Property(x => x.SenderAgentPersonId).IsModified = true;
                entry.Property(x => x.SenderNumber).IsModified = true;
                entry.Property(x => x.SenderDate).IsModified = true;
                entry.Property(x => x.Addressee).IsModified = true;
                entry.Property(x => x.IsRegistered).IsModified = true;

                var docAccess = document.Accesses.FirstOrDefault();
                if (docAccess != null)
                {
                    var acc = new DocumentAccesses
                    {
                        Id = docAccess.Id,
                        AccessLevelId = (int)docAccess.AccessLevel,
                        LastChangeDate = docAccess.LastChangeDate,
                        LastChangeUserId = docAccess.LastChangeUserId
                    };
                    dbContext.DocumentAccessesSet.Attach(acc);
                    var entryAcc = dbContext.Entry(acc);
                    entryAcc.Property(x => x.LastChangeDate).IsModified = true;
                    entryAcc.Property(x => x.LastChangeUserId).IsModified = true;
                    entryAcc.Property(x => x.AccessLevelId).IsModified = true;
                }

                if (document.Properties != null)
                {
                    CommonQueries.ModifyPropertyValues(dbContext, ctx, new InternalPropertyValues { Object = EnumObjects.Documents, RecordId = document.Id, PropertyValues = document.Properties });
                }

                CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumObjects.Documents, EnumOperationType.Update);
                CommonQueries.GetDocumentHash(dbContext, ctx, document.Id, false, false);
                dbContext.SaveChanges();

            }
        }

        public InternalDocument DeleteDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsRegistered = x.IsRegistered,
                        LinkId = x.LinkId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        WaitsCount = x.Waits.Count,
                        SubscriptionsCount = x.Subscriptions.Count,
                    }).FirstOrDefault();

                if (doc == null) return null;


                doc.DocumentFiles = CommonQueries.GetInternalDocumentFiles(ctx, dbContext, doc.Id);

                return doc;
            }
        }

        public void DeleteDocument(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {

                    //ADD OTHER TABLES!!!!
                    dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id).ToList()  //TODO OPTIMIZE
                    .ForEach(x =>
                    {
                        x.LastPaperEventId = null;
                    });

                    //dbContext.DocumentWaitsSet.RemoveRange(dbContext.DocumentWaitsSet.Where(x => x.DocumentId == id));
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.SaveChanges();
                    dbContext.DocumentAccessesSet.RemoveRange(dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentFilesSet.RemoveRange(dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentRestrictedSendListsSet.RemoveRange(dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentSendListsSet.RemoveRange(dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));
                    dbContext.DocumentTasksSet.RemoveRange(dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));

                    dbContext.DocumentPapersSet.RemoveRange(dbContext.DocumentPapersSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == id));

                    CommonQueries.DeletePropertyValues(dbContext, ctx, new FilterPropertyValue { Object = new List<EnumObjects> { EnumObjects.Documents }, RecordId = new List<int> { id } });

                    dbContext.DocumentsSet.RemoveRange(dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Id == id));

                    CommonQueries.AddFullTextCashInfo(dbContext, id, EnumObjects.Documents, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        public InternalDocument RegisterDocumentPrepare(IContext context, RegisterDocumentBase model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        DocumentSubjectId = x.DocumentSubjectId,
                        Description = x.Description,
                        IsRegistered = x.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                        SenderAgentId = x.SenderAgentId,
                        SenderAgentPersonId = x.SenderAgentPersonId,
                        SenderNumber = x.SenderNumber,
                        SenderDate = x.SenderDate,
                        Addressee = x.Addressee,
                        LinkId = x.LinkId,

                        DocumentTypeId = x.TemplateDocument.DocumentTypeId,
                        DocumentDirection = (EnumDocumentDirections)x.TemplateDocument.DocumentDirectionId,
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }
                var regJournal = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == model.RegistrationJournalId)
                    .Select(x => new { x.Id, x.NumerationPrefixFormula, x.PrefixFormula, x.SuffixFormula }).FirstOrDefault();

                if (regJournal != null)
                {
                    doc.RegistrationJournalId = regJournal.Id;
                    doc.NumerationPrefixFormula = regJournal.NumerationPrefixFormula;
                    doc.RegistrationJournalPrefixFormula = regJournal.PrefixFormula;
                    doc.RegistrationJournalSuffixFormula = regJournal.SuffixFormula;
                }
                else
                {
                    doc.RegistrationJournalId = null;
                }
                return doc;
            }
        }

        public InternalDocumnRegistration RegisterModelDocumentPrepare(IContext context, RegisterDocumentBase model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, context)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new
                    {
                        DocumentId = x.Id,
                        LinkId = x.LinkId,
                        SenderAgentId = x.SenderAgentId,
                        ExecutorPositionDepartmentCode = x.ExecutorPosition.Department.Code,
                        SubscriptionsPositionDepartmentCode = x.Subscriptions
                                        .Where(y => y.SubscriptionStateId == (int)EnumSubscriptionStates.Sign)
                                        .OrderBy(y => y.LastChangeDate).Take(1)
                                        .Select(y => y.DoneEvent.SourcePosition.Department.Code).FirstOrDefault(),
                        DocumentSendListLastAgentExternalFirstSymbolName = x.SendLists
                                        .Where(y => y.SendTypeId == (int)EnumSendTypes.SendForInformationExternal)
                                        .OrderByDescending(y => y.LastChangeDate).Take(1)
                                        .Select(y => y.TargetAgent.Name).FirstOrDefault()
                    }).FirstOrDefault();

                if (doc == null)
                {
                    return null;
                }

                var res = new InternalDocumnRegistration
                {
                    ExecutorPositionDepartmentCode = doc.ExecutorPositionDepartmentCode,
                    RegistrationDate = model.RegistrationDate,
                    SubscriptionsPositionDepartmentCode = doc.SubscriptionsPositionDepartmentCode,
                    DocumentSendListLastAgentExternalFirstSymbolName = doc.DocumentSendListLastAgentExternalFirstSymbolName
                };

                if (!string.IsNullOrEmpty(res.DocumentSendListLastAgentExternalFirstSymbolName))
                    res.DocumentSendListLastAgentExternalFirstSymbolName = res.DocumentSendListLastAgentExternalFirstSymbolName.Substring(0, 1);

                //TODO ??? если doc.LinkId==null || doc.SenderAgentId ==null
                res.OrdinalNumberDocumentLinkForCorrespondent = dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == context.CurrentClientId)
                        .Where(x => x.LinkId == doc.LinkId && x.SenderAgentId == doc.SenderAgentId && x.IsRegistered == true)
                        .Count() + 1;

                var regJournal = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == model.RegistrationJournalId)
                    .Select(x => new { x.Id, x.NumerationPrefixFormula, x.PrefixFormula, x.SuffixFormula, x.Index, RegistrationJournalDepartmentCode = x.Department.Code }).FirstOrDefault();

                if (regJournal != null)
                {
                    res.RegistrationJournalId = regJournal.Id;
                    res.RegistrationJournalIndex = regJournal.Index;
                    res.RegistrationJournalDepartmentCode = regJournal.RegistrationJournalDepartmentCode;
                }
                else
                {
                    res.RegistrationJournalId = null;
                }

                var initiativeDoc = dbContext.DocumentLinksSet.Where(x => x.Document.TemplateDocument.ClientId == context.CurrentClientId)
                    .Where(x => x.DocumentId == doc.DocumentId)
                    .OrderBy(x => x.LastChangeDate)
                    .Select(x => x.ParentDocument)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsRegistered = x.IsRegistered,
                        RegistrationNumber = x.RegistrationNumber,
                        RegistrationNumberPrefix = x.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.RegistrationNumberSuffix,
                        SenderNumber = x.SenderNumber
                    })
                    .FirstOrDefault();

                if (initiativeDoc != null)
                {
                    res.InitiativeRegistrationFullNumber = CommonQueries.GetRegistrationFullNumber(initiativeDoc);
                    res.InitiativeRegistrationNumberPrefix = initiativeDoc.RegistrationNumberPrefix;
                    res.InitiativeRegistrationNumberSuffix = initiativeDoc.RegistrationNumberSuffix;
                    res.InitiativeRegistrationNumber = initiativeDoc.RegistrationNumber;
                    res.InitiativeRegistrationSenderNumber = initiativeDoc.SenderNumber;
                }

                res.CurrentPositionDepartmentCode = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Where(x => x.Id == model.CurrentPositionId)
                    .Select(x => x.Department.Code).FirstOrDefault();

                return res;
            }
        }

        public void RegisterDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = new DBModel.Document.Documents
                {
                    Id = document.Id,
                    IsRegistered = document.IsRegistered,
                    RegistrationJournalId = document.RegistrationJournalId,
                    NumerationPrefixFormula = document.NumerationPrefixFormula,
                    RegistrationNumber = document.RegistrationNumber,
                    RegistrationNumberSuffix = document.RegistrationNumberSuffix,
                    RegistrationNumberPrefix = document.RegistrationNumberPrefix,
                    RegistrationDate = document.RegistrationDate,
                    LastChangeDate = document.LastChangeDate,
                    LastChangeUserId = document.LastChangeUserId
                };
                dbContext.DocumentsSet.Attach(doc);
                var entry = dbContext.Entry(doc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsRegistered).IsModified = true;
                entry.Property(x => x.RegistrationJournalId).IsModified = true;
                entry.Property(x => x.NumerationPrefixFormula).IsModified = true;
                entry.Property(x => x.RegistrationNumber).IsModified = true;
                entry.Property(x => x.RegistrationNumberSuffix).IsModified = true;
                entry.Property(x => x.RegistrationNumberPrefix).IsModified = true;
                entry.Property(x => x.RegistrationDate).IsModified = true;

                if (document.IsRegistered ?? false)
                {
                    if (document.Events != null && document.Events.Any(x => x.Id == 0))
                    {
                        doc.Events = ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList();
                        document.Events = null; //евент добавляем один раз
                    }

                }
                else
                {
                    dbContext.DocumentEventsSet.RemoveRange(dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == document.Id && x.EventTypeId == (int)EnumEventTypes.Registered));
                }

                CommonQueries.AddFullTextCashInfo(dbContext, document.Id, EnumObjects.Documents, EnumOperationType.Update);
                dbContext.SaveChanges();
            }
        }

        public void GetNextDocumentRegistrationNumber(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //get next number
                var maxNumber = (from docreg in dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                                 where docreg.RegistrationJournalId == document.RegistrationJournalId
                                       && docreg.NumerationPrefixFormula == document.NumerationPrefixFormula
                                       && docreg.Id != document.Id
                                 select docreg.RegistrationNumber).Max();
                document.RegistrationNumber = (maxNumber ?? 0) + 1;
            }
        }

        public bool VerifyDocumentRegistrationNumber(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return !dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                                .Any(x => x.RegistrationJournalId == document.RegistrationJournalId
                                         && x.NumerationPrefixFormula == document.NumerationPrefixFormula
                                         && x.RegistrationNumber == document.RegistrationNumber
                                         && x.Id != document.Id
                    );
            }
        }

        public InternalDocument ChangeExecutorDocumentPrepare(IContext ctx, ChangeExecutor model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.ExecutorPositionId == doc.ExecutorPositionId && !x.IsAdditional)
                    .Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.Id,
                    }).ToList();
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == doc.ExecutorPositionId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
                    }).ToList();
                return doc;
            }
        }

        public InternalDocument ChangePositionDocumentPrepare(IContext ctx, ChangePosition model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == model.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.ExecutorPositionId == doc.ExecutorPositionId && !x.IsAdditional)
                    .Select(x => new InternalDocumentAttachedFile
                    {
                        Id = x.Id,
                    }).ToList();
                doc.Tasks = dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                    .Where(x => x.DocumentId == model.DocumentId && x.PositionId == doc.ExecutorPositionId)
                    .Select(x => new InternalDocumentTask
                    {
                        Id = x.Id,
                    }).ToList();
                return doc;
            }
        }


        public void ChangeExecutorDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                using (
                    var transaction = new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var doc = new DBModel.Document.Documents
                    {
                        Id = document.Id,
                        ExecutorPositionId = document.ExecutorPositionId,
                        ExecutorPositionExecutorAgentId = document.ExecutorPositionExecutorAgentId,
                        LastChangeDate = document.LastChangeDate,
                        LastChangeUserId = document.LastChangeUserId
                    };
                    dbContext.DocumentsSet.Attach(doc);
                    var entry = dbContext.Entry(doc);
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    entry.Property(x => x.ExecutorPositionId).IsModified = true;
                    entry.Property(x => x.ExecutorPositionExecutorAgentId).IsModified = true;

                    if (document.Events != null && document.Events.Any(x => x.Id == 0))
                    {
                        doc.Events = ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList();
                    }

                    //TODO При получении документа возвращаеться только один Accesses
                    if (document.Accesses != null && document.Accesses.Any())
                    {
                        //TODO Не сохраняеться через свойства
                        //doc.Accesses = CommonQueries.GetDbDocumentAccesses(dbContext, document.Accesses, doc.Id).ToList();
                        dbContext.DocumentAccessesSet.AddRange(
                            CommonQueries.GetDbDocumentAccesses(dbContext, ctx, document.Accesses, doc.Id).ToList());
                    }
                    dbContext.SaveChanges();

                    if (document.Papers != null && document.Papers.Any(x => !x.LastPaperEventId.HasValue && x.LastPaperEvent != null))
                    {
                        foreach (
                            var paper in
                                document.Papers.Where(x => !x.LastPaperEventId.HasValue && x.LastPaperEvent != null).ToList())
                        {
                            var paperEventDb = ModelConverter.GetDbDocumentEvent(paper.LastPaperEvent);
                            dbContext.DocumentEventsSet.Add(paperEventDb);
                            dbContext.SaveChanges();
                            paper.LastPaperEventId = paperEventDb.Id;
                            var paperDb = ModelConverter.GetDbDocumentPaper(paper);
                            dbContext.DocumentPapersSet.Attach(paperDb);
                            var entryPaper = dbContext.Entry(paperDb);
                            entryPaper.Property(e => e.LastPaperEventId).IsModified = true;
                            entryPaper.Property(e => e.LastChangeUserId).IsModified = true;
                            entryPaper.Property(e => e.LastChangeDate).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    if (document.DocumentFiles?.Any() ?? false)
                    {
                        foreach (var fileDb in document.DocumentFiles.Select(ModelConverter.GetDbDocumentFile))
                        {
                            dbContext.DocumentFilesSet.Attach(fileDb);
                            var entryFile = dbContext.Entry(fileDb);
                            entryFile.Property(e => e.ExecutorPositionId).IsModified = true;
                            entryFile.Property(e => e.ExecutorPositionExecutorAgentId).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    if (document.Tasks?.Any() ?? false)
                    {
                        foreach (var taskDb in document.Tasks.Select(ModelConverter.GetDbDocumentTask))
                        {
                            dbContext.DocumentTasksSet.Attach(taskDb);
                            var entryTask = dbContext.Entry(taskDb);
                            entryTask.Property(e => e.PositionId).IsModified = true;
                            entryTask.Property(e => e.PositionExecutorAgentId).IsModified = true;
                            entryTask.Property(e => e.LastChangeUserId).IsModified = true;
                            entryTask.Property(e => e.LastChangeDate).IsModified = true;
                            dbContext.SaveChanges();
                        }
                    }
                    transaction.Complete();


                }
            }
        }

        public void ChangePositionDocument(IContext ctx, ChangePosition model, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //using (
                //    var transaction = new TransactionScope(TransactionScopeOption.Required,
                //        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    dbContext.DocumentsSet.Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Id == model.DocumentId && x.ExecutorPositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.ExecutorPositionId = model.NewPositionId;
                        });
                    dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.ExecutorPositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.ExecutorPositionId = model.NewPositionId;
                        });
                    dbContext.DocumentTasksSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.PositionId = model.NewPositionId;
                        });
                    dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.SourcePositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.SourcePositionId = model.NewPositionId;
                        });
                    dbContext.DocumentEventsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.TargetPositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.TargetPositionId = model.NewPositionId;
                        });
                    dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.SourcePositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.SourcePositionId = model.NewPositionId;
                        });
                    dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.TargetPositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.TargetPositionId = model.NewPositionId;
                        });
                    dbContext.DocumentRestrictedSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.PositionId = model.NewPositionId;
                        });
                    dbContext.DocumentAccessesSet.RemoveRange(dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.NewPositionId));
                    dbContext.DocumentAccessesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.PositionId = model.NewPositionId;
                        });
                    dbContext.DocumentTaskAccessesSet.RemoveRange(dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Task.DocumentId == model.DocumentId && x.PositionId == model.NewPositionId));
                    dbContext.DocumentTaskAccessesSet.Where(x => x.Task.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.Task.DocumentId == model.DocumentId && x.PositionId == model.OldPositionId).ToList()
                        .ForEach(x =>
                        {
                            x.PositionId = model.NewPositionId;
                        });

                    if (document.Events != null && document.Events.Any(x => x.Id == 0))
                    {
                        dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList());
                    }

                    dbContext.SaveChanges();

                    //transaction.Complete();


                }
            }
        }

        public InternalDocument ChangeIsLaunchPlanDocumentPrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, true)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsLaunchPlan = x.IsLaunchPlan
                    }).FirstOrDefault();
                if (doc == null) return null;
                doc.SendLists = dbContext.DocumentSendListsSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                                    .Where(x => x.DocumentId == documentId)
                                    .Select(x => new InternalDocumentSendList
                                    {
                                        Id = x.Id,
                                    }
                                    ).ToList();
                return doc;
            }
        }

        public void ChangeIsLaunchPlanDocument(IContext ctx, InternalDocument document)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = new DBModel.Document.Documents
                {
                    Id = document.Id,
                    IsLaunchPlan = document.IsLaunchPlan,
                    LastChangeDate = document.LastChangeDate,
                    LastChangeUserId = document.LastChangeUserId
                };
                dbContext.DocumentsSet.Attach(doc);
                var entry = dbContext.Entry(doc);
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.IsLaunchPlan).IsModified = true;
                if (document.Events != null && document.Events.Any(x => x.Id == 0))
                {
                    doc.Events = ModelConverter.GetDbDocumentEvents(document.Events.Where(x => x.Id == 0)).ToList();
                }
                dbContext.SaveChanges();
            }
        }

        public InternalDocument GetBlankInternalDocumentById(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId
                    }).FirstOrDefault();

                return doc;
            }
        }


        #region DocumentPapers

        public IEnumerable<FrontDocumentPaper> GetDocumentPapers(IContext ctx, FilterDocumentPaper filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentPapers(dbContext, ctx, filter, paging);
            }
        }

        public FrontDocumentPaper GetDocumentPaper(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentPapers(dbContext, ctx, new FilterDocumentPaper { Id = new List<int> { id } }, null).FirstOrDefault();
            }
        }
        #endregion DocumentPapers   

        #region DocumentPaperLists

        public IEnumerable<FrontDocumentPaperList> GetDocumentPaperLists(IContext ctx, FilterDocumentPaperList filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentPaperLists(dbContext, ctx, filter);
            }
        }

        public FrontDocumentPaperList GetDocumentPaperList(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentPaperLists(dbContext, ctx, new FilterDocumentPaperList { PaperListId = new List<int> { id } }).FirstOrDefault();
            }
        }
        #endregion DocumentPaperLists   
    }
}