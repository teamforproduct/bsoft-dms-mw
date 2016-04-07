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
    public class RecieveDocumentPaperEventCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public RecieveDocumentPaperEventCommand(IDocumentOperationsDbProcess operationDb)
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
            foreach (var paper in _document.Papers)
            {
                if (paper?.LastPaperEvent?.TargetPositionId == null
                    || !CanBeDisplayed(paper.LastPaperEvent.TargetPositionId.Value)
                    )
                {
                    throw new CouldNotPerformOperationWithPaper();
                }
                _context.SetCurrentPosition(paper.LastPaperEvent.TargetPositionId);
                _admin.VerifyAccess(_context, CommandType);
            }
            return true;
        }

        public override object Execute()
        {
            foreach (var paper in _document.Papers)
            {
                CommonDocumentUtilities.SetLastChange(_context, paper.LastPaperEvent);
                paper.LastPaperEvent.Date = paper.LastPaperEvent.LastChangeDate;
                paper.LastPaperEvent.PaperRecieveDate = paper.LastPaperEvent.LastChangeDate;
                paper.LastPaperEvent.PaperRecieveAgentId = _context.CurrentAgentId;
            }
            _operationDb.RecieveDocumentPaperEvent(_context, _document.Papers);
            return null;
        }

    }
}