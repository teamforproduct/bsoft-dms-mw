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
    public class CancelPlanDocumentPaperEventCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentPaper _paper;

        public CancelPlanDocumentPaperEventCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Papers.Where(
                    x => x.IsInWork &&
                        x.LastPaperEvent.SourcePositionId == positionId &&
                        x.LastPaperEvent.PaperRecieveDate == null && x.LastPaperEvent.SendDate == null)
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
            _document = _operationDb.EventDocumentPaperPrepare(_context, Model,true);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _paper = _document.Papers.First();
            if (_paper?.LastPaperEvent?.SourcePositionId == null
                || !CanBeDisplayed(_paper.LastPaperEvent.SourcePositionId.Value)
                )
            {
                throw new CouldNotPerformOperationWithPaper();
            }
            _context.SetCurrentPosition(_paper.LastPaperEvent.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _paper.LastPaperEvent.ParentEventId = null;
            _paper.LastPaperEvent.SendListId = null;
            _paper.LastPaperEvent.PaperPlanDate = null;
            _paper.LastPaperEvent.PaperPlanAgentId = null;
            CommonDocumentUtilities.SetLastChange(_context, _paper.LastPaperEvent);
            _paper.LastPaperEvent.Date = _paper.LastPaperEvent.LastChangeDate;

            _paper.LastPaperEventId = _paper.PreLastPaperEventId;
            CommonDocumentUtilities.SetLastChange(_context, _paper);
            _operationDb.CancelPlanDocumentPaperEvent(_context, _paper);
            return null;
        }

    }
}