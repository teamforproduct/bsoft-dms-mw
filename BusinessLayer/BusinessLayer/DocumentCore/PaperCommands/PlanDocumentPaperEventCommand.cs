using System.Collections.Generic;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.PaperCommands
{
    public class PlanDocumentPaperEventCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        //private InternalDocumentPaper _paper;

        public PlanDocumentPaperEventCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private List<PlanMovementPaper> Model
        {
            get
            {
                if (!(_param is List<PlanMovementPaper>))
                {
                    throw new WrongParameterTypeError();
                }
                return (List<PlanMovementPaper>)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Papers.Where(
                    x => x.IsInWork &&
                        x.LastPaperEvent.SourcePositionId == positionId &&
                        x.LastPaperEvent.PaperRecieveDate != null )
                        .Select(x => new InternalActionRecord
                        {
                            PaperId = x.Id,
                        });
            if (!_actionRecords.Any())
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.PlanDocumentPaperEventPrepare(_context, Model.Select(x=>x.Id).ToList());
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            foreach (var paper in _document.Papers)
            {
                if (paper?.LastPaperEvent?.SourcePositionId == null
                    || !CanBeDisplayed(paper.LastPaperEvent.SourcePositionId.Value)
                    )
                {
                    throw new CouldNotPerformOperationWithPaper();
                }
                _context.SetCurrentPosition(paper.LastPaperEvent.SourcePositionId);
                _admin.VerifyAccess(_context, CommandType);
            }
            return true;
        }

        public override object Execute()
        {
            
            foreach (var model in Model)
            {
                var paper = _document.Papers.FirstOrDefault(x => x.Id == model.Id);
                if (paper != null)
                {
                    paper.LastPaperEventId = null;
                    paper.LastPaperEvent = CommonDocumentUtilities.GetNewDocumentPaperEvent(_context, paper.Id,
                        EnumEventTypes.MoveDocumentPaper, model.Description, model.TargetPositionId, null, paper.LastPaperEvent.SourcePositionId, null, true, false);
                    CommonDocumentUtilities.SetLastChange(_context, paper);
                }
            }
            _operationDb.PlanDocumentPaperEvent(_context, _document.Papers);
            return _document.Id;
        }

    }
}