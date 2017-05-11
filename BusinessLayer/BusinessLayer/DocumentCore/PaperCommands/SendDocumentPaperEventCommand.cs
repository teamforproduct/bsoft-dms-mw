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
    public class SendDocumentPaperEventCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendDocumentPaperEventCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private PaperList Model
        {
            get
            {
                if (!(_param is PaperList))
                {
                    throw new WrongParameterTypeError();
                }
                return (PaperList)_param ;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            _actionRecords =
                _document.Papers.Where(
                    x => x.IsInWork &&
                        x.LastPaperEvent.SourcePositionId == positionId &&
                        x.LastPaperEvent.PaperRecieveDate == null && x.LastPaperEvent.PaperSendDate == null && x.LastPaperEvent.PaperPlanDate.HasValue)
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
                if (paper?.LastPaperEvent?.SourcePositionId == null
                    || !CanBeDisplayed(paper.LastPaperEvent.SourcePositionId.Value)
                    )
                {
                    throw new CouldNotPerformOperationWithPaper();
                }
                _context.SetCurrentPosition(paper.LastPaperEvent.SourcePositionId);
                _adminProc.VerifyAccess(_context, CommandType);
            }
            return true;
        }

        public override object Execute()
        {
            foreach (var paper in _document.Papers)
            {
                CommonDocumentUtilities.SetLastChange(_context, paper.LastPaperEvent);
                paper.LastPaperEvent.Date = paper.LastPaperEvent.LastChangeDate;
                paper.LastPaperEvent.PaperSendDate = paper.LastPaperEvent.LastChangeDate;
                paper.LastPaperEvent.PaperSendAgentId = _context.CurrentAgentId;
            }
            _operationDb.SendDocumentPaperEvent(_context, _document.Papers);
            
            return null;
        }

    }
}