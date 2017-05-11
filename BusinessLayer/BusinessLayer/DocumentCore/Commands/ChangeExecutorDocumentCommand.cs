using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DocumentCore.Commands
{
    public class ChangeExecutorDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public ChangeExecutorDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
        }

        private ChangeExecutor Model
        {
            get
            {
                if (!(_param is ChangeExecutor))
                {
                    throw new WrongParameterTypeError();
                }
                return (ChangeExecutor)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            if (_document.ExecutorPositionId != positionId || (_document.IsRegistered.HasValue && _document.IsRegistered.Value)
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            _document = _documentDb.ChangeExecutorDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminProc.VerifyAccess(_context, CommandType);
            if (Model.PositionId == _context.CurrentPositionId)
            {
                throw new CouldNotChangeAttributeLaunchPlan();
            }
            if ((_document.RestrictedSendLists?.Any() ?? false)
                && !_document.RestrictedSendLists.Select(x => x.PositionId).Contains(Model.PositionId)
                )
            {
                throw new DocumentSendListNotFoundInDocumentRestrictedSendList();
            }

            if (Model.PaperEvents != null && Model.PaperEvents.Any())
            {
                _adminProc.VerifyAccess(_context, EnumDocumentActions.PlanDocumentPaperEvent);
                _document.Papers = _operationDb.PlanDocumentPaperEventPrepare(_context, Model.PaperEvents.Select(x => x.Id).ToList()).Papers;
                if (_document.Papers.Any(x => x.LastPaperEvent.TargetPositionId == null || x.LastPaperEvent.TargetPositionId.Value != _document.ExecutorPositionId
                                            || !x.IsInWork || x.LastPaperEvent.PaperRecieveDate == null))
                {
                    throw new CouldNotPerformOperationWithPaper();
                }
            }
            var positionExecutor = CommonDocumentUtilities.GetExecutorAgentIdByPositionId(_context, Model.PositionId);
            if (positionExecutor?.ExecutorAgentId.HasValue ?? false)
            {
                _document.ExecutorPositionExecutorAgentId = positionExecutor.ExecutorAgentId.Value;
                _document.ExecutorPositionExecutorTypeId = positionExecutor.ExecutorTypeId;
            }
            else
            {
                throw new ExecutorAgentForPositionIsNotDefined();
            }

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.ExecutorPositionId = Model.PositionId;
            _document.AccessLevel = Model.AccessLevel;

            var evAcceesses = (Model.TargetCopyAccessGroups?.Where(x => x.AccessType == EnumEventAccessTypes.TargetCopy) ?? new List<AccessGroup>())
                .Concat(new List<AccessGroup> { new AccessGroup { AccessType = EnumEventAccessTypes.Target, AccessGroupType = EnumEventAccessGroupTypes.Position, RecordId = Model.PositionId } })
                .ToList();
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, (int)EnumEntytiTypes.Document, Model.DocumentId, EnumEventTypes.ChangeExecutor, Model.EventDate, Model.Description, null, null, null, 
                Model.PositionId, accessGroups: evAcceesses);

            _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, (int)EnumEntytiTypes.Document, Model.AccessLevel, _document.Events.First().Accesses);

            if (Model.PaperEvents?.Any() ?? false)
            {
                foreach (var model in Model.PaperEvents)
                {
                    var paper = _document.Papers.FirstOrDefault(x => x.Id == model.Id);
                    if (paper != null)
                    {
                        paper.LastPaperEventId = null;
                        paper.LastPaperEvent = CommonDocumentUtilities.GetNewDocumentPaperEvent(_context, (int)EnumEntytiTypes.Document,
                            paper.DocumentId, paper.Id,
                            EnumEventTypes.MoveDocumentPaper, model.Description, Model.PositionId, null,
                            paper.LastPaperEvent.SourcePositionId, null, true, false);
                        CommonDocumentUtilities.SetLastChange(_context, paper);
                    }
                }
            }
            if (_document.DocumentFiles?.Any() ?? false)
            {
                //CommonDocumentUtilities.SetLastChange(_context, _document.DocumentFiles);
                ((List<InternalDocumentFile>)_document.DocumentFiles).ForEach(x=>
                {
                    x.ExecutorPositionId = _document.ExecutorPositionId;
                    x.ExecutorPositionExecutorAgentId = _document.ExecutorPositionExecutorAgentId;
                    x.ExecutorPositionExecutorTypeId = _document.ExecutorPositionExecutorTypeId;
                });
            }
            if (_document.Tasks?.Any() ?? false)
            {
                CommonDocumentUtilities.SetLastChange(_context, _document.Tasks);
                ((List<InternalDocumentTask>)_document.Tasks).ForEach(x =>
                {
                    x.PositionId = _document.ExecutorPositionId;
                    x.PositionExecutorAgentId = _document.ExecutorPositionExecutorAgentId;
                    x.PositionExecutorTypeId = _document.ExecutorPositionExecutorTypeId;
                });
            }
            _documentDb.ChangeExecutorDocument(_context, _document);

            return Model.DocumentId;
        }

    }
}