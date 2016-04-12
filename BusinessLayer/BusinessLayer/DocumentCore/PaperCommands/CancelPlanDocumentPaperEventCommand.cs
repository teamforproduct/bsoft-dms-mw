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
    public class CancelPlanDocumentPaperEventCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public CancelPlanDocumentPaperEventCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private List<int> Model
        {
            get
            {
                if (!(_param is List<int>))
                {
                    throw new WrongParameterTypeError();
                }
                return (List<int>)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Papers.Where(
                    x => x.IsInWork &&
                        x.LastPaperEvent.SourcePositionId == positionId &&
                        x.LastPaperEvent.PaperRecieveDate == null && x.LastPaperEvent.PaperSendDate == null && x.LastPaperEvent.PaperPlanDate != null)
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
            _document = _operationDb.EventDocumentPaperPrepare(_context, Model, true);
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
            foreach (var paper in _document.Papers)
            {
                paper.LastPaperEvent.ParentEventId = null;
                paper.LastPaperEvent.SendListId = null;
                paper.LastPaperEvent.PaperListId = null;
                paper.LastPaperEvent.PaperPlanDate = null;
                paper.LastPaperEvent.PaperPlanAgentId = null;
                CommonDocumentUtilities.SetLastChange(_context, paper.LastPaperEvent);
                paper.LastPaperEvent.Date = paper.LastPaperEvent.LastChangeDate;
                paper.LastPaperEventId = paper.PreLastPaperEventId;
                CommonDocumentUtilities.SetLastChange(_context, paper);
            }
            _operationDb.CancelPlanDocumentPaperEvent(_context, _document.Papers);
            return null;
        }

    }
}