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
    public class RecieveDocumentPaperEventCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentPaper _paper;

        public RecieveDocumentPaperEventCommand(IDocumentOperationsDbProcess operationDb)
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
                        x.LastPaperEvent.TargetPositionId == positionId &&
                        x.LastPaperEvent.PaperRecieveDate == null && x.LastPaperEvent.PaperPlanDate != null)
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
            _document = _operationDb.EventDocumentPaperPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _paper = _document.Papers.First();
            if (_paper?.LastPaperEvent?.TargetPositionId == null
                || !CanBeDisplayed(_paper.LastPaperEvent.TargetPositionId.Value)
                )
            {
                throw new CouldNotPerformOperationWithPaper();
            }
            _context.SetCurrentPosition(_paper.LastPaperEvent.TargetPositionId);
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _paper.LastPaperEvent);
            _paper.LastPaperEvent.Date = _paper.LastPaperEvent.LastChangeDate;
            _paper.LastPaperEvent.PaperRecieveDate = _paper.LastPaperEvent.LastChangeDate;
            _paper.LastPaperEvent.PaperRecieveAgentId = _context.CurrentAgentId;
            _operationDb.RecieveDocumentPaperEvent(_context, _paper);
            return null;
        }

    }
}